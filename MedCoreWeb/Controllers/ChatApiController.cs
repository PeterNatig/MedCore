using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedCore.Model;
using Service.AI;
using ViewModel.Chat;

namespace MedCoreWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatApiController : ControllerBase
{
    private readonly IChatAgentService _chatAgent;
    private readonly IChatHistoryService _chatHistory;
    private readonly UserManager<User> _userManager;

    public ChatApiController(IChatAgentService chatAgent, IChatHistoryService chatHistory, UserManager<User> userManager)
    {
        _chatAgent = chatAgent;
        _chatHistory = chatHistory;
        _userManager = userManager;
    }

    [HttpGet("sessions")]
    [Authorize]
    public async Task<IActionResult> GetSessions()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var sessions = await _chatHistory.GetSessionsAsync(user.Email);
        return Ok(sessions);
    }

    [HttpGet("sessions/{id}/messages")]
    [Authorize]
    public async Task<IActionResult> GetSessionMessages(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var messages = await _chatHistory.GetSessionMessagesAsync(id, user.Email);
        return Ok(messages);
    }

    [HttpPost("sessions")]
    [Authorize]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var sessionId = await _chatHistory.CreateSessionAsync(user.Email, request.Title);
        return Ok(new { sessionId });
    }

    [HttpPost("patient")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> PatientChat([FromBody] DbChatRequestVM request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var email = user.Email ?? string.Empty;

        // Ensure session exists
        if (request.SessionId == 0) request.SessionId = await _chatHistory.CreateSessionAsync(email, string.IsNullOrWhiteSpace(request.Title) ? "New Conversation" : request.Title);

        // Save User Message
        await _chatHistory.AddMessageAsync(request.SessionId, "user", request.Message);

        var agentRequest = new ChatRequestVM { Message = request.Message, History = request.History };
        var response = await _chatAgent.HandlePatientChatAsync(email, agentRequest);

        // Save Assistant Message
        if (response.Success)
        {
            await _chatHistory.AddMessageAsync(request.SessionId, "assistant", response.Reply);
            return Ok(new { success = true, reply = response.Reply, sessionId = request.SessionId });
        }
        
        return Ok(response);
    }

    [HttpPost("doctor")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> DoctorChat([FromBody] DbChatRequestVM request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var email = user.Email ?? string.Empty;

        if (request.SessionId == 0) request.SessionId = await _chatHistory.CreateSessionAsync(email, string.IsNullOrWhiteSpace(request.Title) ? "New Conversation" : request.Title);

        // Save User Message
        await _chatHistory.AddMessageAsync(request.SessionId, "user", request.Message);

        var agentRequest = new ChatRequestVM { Message = request.Message, History = request.History };
        var response = await _chatAgent.HandleDoctorChatAsync(email, agentRequest);

        // Save Assistant Message
        if (response.Success)
        {
            await _chatHistory.AddMessageAsync(request.SessionId, "assistant", response.Reply);
            return Ok(new { success = true, reply = response.Reply, sessionId = request.SessionId });
        }

        return Ok(response);
    }
}

public class CreateSessionRequest
{
    public string Title { get; set; }
}

public class DbChatRequestVM
{
    public int SessionId { get; set; }
    public string Message { get; set; }
    public string Title { get; set; }
    public List<ChatMessageVM> History { get; set; } = new();
}
