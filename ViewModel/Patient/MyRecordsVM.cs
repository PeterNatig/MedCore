namespace ViewModel.Patient;

public class MyRecordsVM
{
    public IReadOnlyList<MyRecordItemVM> Records { get; set; } = [];
}

public class MyRecordItemVM
{
    public string AppointmentId { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public IReadOnlyList<PrescriptionItemVM> Prescriptions { get; set; } = [];
}

public class PrescriptionItemVM
{
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
}

public class PatientPrescriptionsVM
{
    public IReadOnlyList<PrescriptionDetailVM> Prescriptions { get; set; } = [];
}

public class PrescriptionDetailVM
{
    public string PrescriptionId { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public DateTime PrescribedDate { get; set; }
}
