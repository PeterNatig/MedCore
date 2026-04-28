using Repository.Workflow;
using Service.Workflow;
using System.Text.RegularExpressions;
using ViewModel.Chat;
using Microsoft.Extensions.Options;

namespace Service.AI;

public interface IChatAgentService
{
    Task<ChatResponseVM> HandlePatientChatAsync(string patientEmail, ChatRequestVM request);
    Task<ChatResponseVM> HandleDoctorChatAsync(string doctorEmail, ChatRequestVM request);
}

public class ChatAgentService : IChatAgentService
{
    private readonly IOllamaService _ollama;
    private readonly IPatientWorkflowRepo _patientRepo;
    private readonly IDoctorWorkflowRepo _doctorRepo;
    private readonly IPatientWorkflowService _patientWorkflow;
    private readonly IDoctorWorkflowService _doctorWorkflow;
    private readonly AiAgentModelsOptions _modelOptions;

    public ChatAgentService(
        IOllamaService ollama,
        IPatientWorkflowRepo patientRepo,
        IDoctorWorkflowRepo doctorRepo,
        IPatientWorkflowService patientWorkflow,
        IDoctorWorkflowService doctorWorkflow,
        IOptions<AiAgentModelsOptions> modelOptions)
    {
        _ollama = ollama;
        _patientRepo = patientRepo;
        _doctorRepo = doctorRepo;
        _patientWorkflow = patientWorkflow;
        _doctorWorkflow = doctorWorkflow;
        _modelOptions = modelOptions?.Value ?? new AiAgentModelsOptions();
    }

    public async Task<ChatResponseVM> HandlePatientChatAsync(string patientEmail, ChatRequestVM request)
    {
        try
        {
            var message = request.Message?.Trim() ?? string.Empty;

            // 1. Gather context
            var specialties = await _patientRepo.GetSpecialtiesAsync();
            var contextBuilder = new System.Text.StringBuilder();
            contextBuilder.AppendLine("AVAILABLE SPECIALTIES:");
            foreach (var sp in specialties)
            {
                contextBuilder.AppendLine($"- {sp.Name}: {sp.Description}");
                var doctors = await _patientRepo.GetDoctorsBySpecialtyAsync(sp.Id);
                foreach (var doc in doctors)
                {
                    contextBuilder.AppendLine($"  * Doctor: {doc.FullName} (ID: {doc.Id}) - {doc.ExperienceYears} yrs experience.");
                    var docWithSlots = await _patientRepo.GetDoctorWithAvailableSlotsAsync(doc.Id, ""); // patientId not strictly needed for just listing slots
                    if (docWithSlots != null && docWithSlots.Schedules != null)
                    {
                        var availableSlots = docWithSlots.Schedules.Where(s => !s.IsBooked).ToList();
                        if (availableSlots.Any())
                        {
                            contextBuilder.AppendLine($"    Available Slots (Schedule IDs):");
                            foreach (var slot in availableSlots)
                            {
                                contextBuilder.AppendLine($"      - {slot.StartTime:dd MMM yyyy hh:mm tt} (ID: {slot.Id})");
                            }
                        }
                    }
                }
            }

            var systemPrompt = $@"You are a helpful, professional medical assistant for MedCore, designed exclusively to help patients with health and medical-related needs.

YOUR ROLE AND RESPONSIBILITIES:
1. Answer medical and health-related questions (symptoms, conditions, general health advice, wellness, etc.)
2. Help patients identify the right medical specialty based on their symptoms
3. Recommend doctors and provide information about specialties
4. Assist with booking, viewing, and managing appointments
5. Support patient portal features like accessing records and medical history

IMPORTANT GUIDELINES:
- Use the conversation history and context to understand if the user's request is medical/health-related
- ALWAYS respond helpfully to any medical, health, wellness, or medicine-related questions
- If the user asks about a symptom, condition, treatment, or health concern, provide a supportive and informative response
- For non-medical topics (entertainment, general trivia, personal opinions, non-health advice), politely apologize and redirect to health topics
- Be empathetic and professional when discussing health concerns
- Remember: you can assist with MedCore features like booking appointments, finding doctors, and managing patient records

Food policy:
- If food is asked WITHOUT a medical condition/symptom context, politely decline and redirect to health topics
- If food is asked WITH medical condition/symptom context, provide only recommended meals/foods suitable for that condition (NO recipes, cooking steps, or preparation methods)

APPOINTMENT BOOKING:
If the user wants to book an appointment and you know the Schedule ID, output this EXACT command format on a new line:
[BOOK_APPOINTMENT: <Schedule_ID>]

Here is the current database of specialties, doctors, and available slots:
{contextBuilder}
";

            // 2. Call AI
            var patientModel = GetConfiguredModelOrThrow(_modelOptions.PatientAgentModel, "AI:Models:PatientAgentModel");
            var reply = await _ollama.GetCompletionAsync(patientModel, systemPrompt, request.History, message);

            // 3. Process agent actions
            var bookMatch = Regex.Match(reply, @"\[BOOK_APPOINTMENT:\s*([^\]]+)\]");
            if (bookMatch.Success)
            {
                var scheduleId = bookMatch.Groups[1].Value.Trim();
                var result = await _patientWorkflow.BookAppointmentAsync(patientEmail, scheduleId);

                // Remove the raw command from output and append friendly status
                reply = reply.Replace(bookMatch.Value, "").Trim();
                if (result.Success)
                {
                    reply += "\n\n✅ I have successfully booked that appointment for you! You can view it in your My Records page.";
                }
                else
                {
                    reply += $"\n\n❌ I tried to book that appointment, but it failed: {result.Message}";
                }
            }

            return new ChatResponseVM { Reply = reply };
        }
        catch (Exception ex)
        {
            return new ChatResponseVM { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<ChatResponseVM> HandleDoctorChatAsync(string doctorEmail, ChatRequestVM request)
    {
        try
        {
            var currentTime = DateTime.Now;
            var contextBuilder = new System.Text.StringBuilder();

            // 1. Gather existing schedules
            var schedulesModel = await _doctorWorkflow.GetSchedulesAsync(doctorEmail);
            if (schedulesModel?.Schedules != null && schedulesModel.Schedules.Any())
            {
                contextBuilder.AppendLine("\nYOUR UPCOMING SCHEDULES:");
                foreach (var s in schedulesModel.Schedules.Where(x => x.StartTime >= currentTime).OrderBy(x => x.StartTime))
                {
                    var status = s.IsBooked ? "Booked" : "Available";
                    contextBuilder.AppendLine($"- {s.StartTime:yyyy-MM-dd hh:mm tt} (ID: {s.ScheduleId}) - {status}");
                }
            }
            else
            {
                contextBuilder.AppendLine("\nYou currently have no upcoming slots in your schedule.");
            }

            // 2. Gather Patients & Appointments
            var patientsModel = await _doctorWorkflow.GetPatientsAsync(doctorEmail);
            if (patientsModel?.Patients != null && patientsModel.Patients.Any())
            {
                contextBuilder.AppendLine("\nYOUR APPOINTMENTS:");
                foreach (var a in patientsModel.Patients)
                {
                    contextBuilder.AppendLine($"- Patient: {a.PatientName} | Date: {a.AppointmentDate:yyyy-MM-dd hh:mm tt} | Status: {a.Status} | Appointment ID: {a.AppointmentId}");
                }
            }

            // 3. Gather Medications (for prescribing)
            var consultationModel = await _doctorWorkflow.GetConsultationAsync(doctorEmail);
            if (consultationModel?.Medications != null && consultationModel.Medications.Any())
            {
                contextBuilder.AppendLine("\nAVAILABLE MEDICATIONS DATABASE:");
                foreach (var m in consultationModel.Medications)
                {
                    contextBuilder.AppendLine($"- {m.Name} (Medication ID: {m.Id})");
                }
            }

            var systemPrompt = $@"You are an advanced professional AI assistant for Doctors at MedCore.
The current local date and time is: {currentTime:yyyy-MM-dd hh:mm tt}.

You have full read/write access to the doctor's portal. Here is their current state:
{contextBuilder}

YOUR ROLE:
- Use conversation history and context to understand the doctor's requests
- Assist with medical queries, diagnosis support, drug interactions, and medical information
- Manage the doctor's practice through portal commands
- If a request is unrelated to medical practice or MedCore features, politely decline and redirect

CAPABILITIES & COMMANDS:
You can manage the doctor's practice by outputting exact commands. Output each command on a new line.

1. CREATING SLOTS:
If asked to create slots, FIRST calculate the 30-minute intervals and list them clearly for review. DO NOT issue create commands until the doctor confirms.
*CRITICAL SCHEDULING RULE*: If the doctor provides a large time window (e.g., 6 hours) but only requests a few slots (e.g., 6 slots, which is 3 hours of work), DO NOT schedule them all back-to-back. You MUST intelligently spread the slots out across the time window to include breaks and avoid doctor burnout.
Once confirmed, output: [CREATE_SLOT: <StartTime_ISO8601>]

2. DELETING SLOTS:
To delete an 'Available' slot: [DELETE_SLOT: <Schedule_ID>]

3. CONFIRMING APPOINTMENTS:
If the doctor asks to confirm a 'Pending' appointment, output: [CONFIRM_APPOINTMENT: <Appointment_ID>]

4. REJECTING APPOINTMENTS:
If the doctor asks to reject a 'Pending' appointment, output: [REJECT_APPOINTMENT: <Appointment_ID>]

5. PRESCRIBING MEDICATION:
If the doctor asks to prescribe medication to a 'Confirmed' appointment, output: [PRESCRIBE: <Appointment_ID> | <Medication_ID> | <Dosage> | <Frequency>]
Example: [PRESCRIBE: 5 | 2 | 500mg | Twice daily]

MEDICAL QUERIES:
Since you are speaking to a Doctor, you ARE allowed to answer complex medical queries, assist with diagnosis, discuss drug interactions, and provide medical information. Use context and conversation history to provide relevant assistance.
";

            // 2. Call AI
            var doctorMessage = request.Message?.Trim() ?? string.Empty;
            var doctorModel = GetConfiguredModelOrThrow(_modelOptions.DoctorAgentModel, "AI:Models:DoctorAgentModel");
            var reply = await _ollama.GetCompletionAsync(doctorModel, systemPrompt, request.History, doctorMessage);

            // 3. Process CREATE actions
            var slotMatches = Regex.Matches(reply, @"\[CREATE_SLOT:\s*([^\]]+)\]");
            if (slotMatches.Count > 0)
            {
                int created = 0;
                int failed = 0;
                var failReasons = new List<string>();

                foreach (Match match in slotMatches)
                {
                    if (DateTime.TryParse(match.Groups[1].Value.Trim(), out var startTime))
                    {
                        var endTime = startTime.AddMinutes(30);
                        var success = await _doctorWorkflow.AddScheduleAsync(doctorEmail, startTime, endTime);
                        if (success) created++;
                        else
                        {
                            failed++;
                            failReasons.Add($"- {startTime:hh:mm tt}: overlapping or invalid date");
                        }
                    }
                    else
                    {
                        failed++;
                        failReasons.Add($"- {match.Groups[1].Value}: could not parse date format");
                    }
                }

                reply = Regex.Replace(reply, @"\[CREATE_SLOT:\s*([^\]]+)\]", "").Trim();
                reply += $"\n\n✅ I have successfully created {created} schedule slot(s) for you.";
                if (failed > 0) reply += $"\n⚠️ I could not create {failed} slot(s) for the following reasons:\n" + string.Join("\n", failReasons);
            }

            // 4. Process DELETE actions
            var deleteMatches = Regex.Matches(reply, @"\[DELETE_SLOT:\s*([^\]]+)\]");
            if (deleteMatches.Count > 0)
            {
                int deleted = 0;
                int failed = 0;

                foreach (Match match in deleteMatches)
                {
                    var scheduleId = match.Groups[1].Value.Trim();
                    var success = await _doctorWorkflow.DeleteScheduleAsync(doctorEmail, scheduleId);
                    if (success) deleted++; else failed++;
                }

                reply = Regex.Replace(reply, @"\[DELETE_SLOT:\s*([^\]]+)\]", "").Trim();
                reply += $"\n\n🗑️ I have successfully deleted {deleted} schedule slot(s).";
                if (failed > 0) reply += $"\n⚠️ I could not delete {failed} slot(s) (they may be booked or invalid).";
            }

            // 5. Process CONFIRM actions
            var confirmMatches = Regex.Matches(reply, @"\[CONFIRM_APPOINTMENT:\s*([^\]]+)\]");
            if (confirmMatches.Count > 0)
            {
                int confirmed = 0;
                foreach (Match match in confirmMatches)
                {
                    var appointmentId = match.Groups[1].Value.Trim();
                    var success = await _doctorWorkflow.ConfirmAppointmentAsync(doctorEmail, appointmentId);
                    if (success) confirmed++;
                }
                reply = Regex.Replace(reply, @"\[CONFIRM_APPOINTMENT:\s*([^\]]+)\]", "").Trim();
                reply += $"\n\n✅ I have successfully confirmed {confirmed} appointment(s).";
            }

            // 6. Process REJECT actions
            var rejectMatches = Regex.Matches(reply, @"\[REJECT_APPOINTMENT:\s*([^\]]+)\]");
            if (rejectMatches.Count > 0)
            {
                int rejected = 0;
                foreach (Match match in rejectMatches)
                {
                    var appointmentId = match.Groups[1].Value.Trim();
                    var success = await _doctorWorkflow.RejectAppointmentAsync(doctorEmail, appointmentId);
                    if (success) rejected++;
                }
                reply = Regex.Replace(reply, @"\[REJECT_APPOINTMENT:\s*([^\]]+)\]", "").Trim();
                reply += $"\n\n🛑 I have successfully rejected {rejected} appointment(s).";
            }

            // 7. Process PRESCRIBE actions
            var prescribeMatches = Regex.Matches(reply, @"\[PRESCRIBE:\s*([^|]+)\|([^|]+)\|([^|]+)\|([^\]]+)\]");
            if (prescribeMatches.Count > 0)
            {
                int prescribed = 0;
                foreach (Match match in prescribeMatches)
                {
                    var model = new ViewModel.Doctor.ConsultationVM
                    {
                        AppointmentId = match.Groups[1].Value.Trim(),
                        MedicationId = match.Groups[2].Value.Trim(),
                        Dosage = match.Groups[3].Value.Trim(),
                        Frequency = match.Groups[4].Value.Trim()
                    };

                    if (model.MedicationId != null)
                    {
                        var success = await _doctorWorkflow.AddPrescriptionAsync(doctorEmail, model);
                        if (success) prescribed++;
                    }
                }
                reply = Regex.Replace(reply, @"\[PRESCRIBE:\s*([^|]+)\|([^|]+)\|([^|]+)\|([^\]]+)\]", "").Trim();
                reply += $"\n\n💊 I have successfully added {prescribed} prescription(s).";
            }

            return new ChatResponseVM { Reply = reply };
        }
        catch (Exception ex)
        {
            return new ChatResponseVM { Success = false, ErrorMessage = ex.Message };
        }
    }


    private static string GetConfiguredModelOrThrow(string configuredModel, string configPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredModel))
        {
            return configuredModel;
        }

        throw new InvalidOperationException($"Missing required AI model configuration at '{configPath}'.");
    }
}
