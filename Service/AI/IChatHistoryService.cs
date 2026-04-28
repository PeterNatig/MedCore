using ViewModel.Chat;

namespace Service.AI;

public interface IChatHistoryService
{
    Task<List<ChatSessionVM>> GetSessionsAsync(string userEmail);
    Task<List<ChatMessageVM>> GetSessionMessagesAsync(int sessionId, string userEmail);
    Task<int> CreateSessionAsync(string userEmail, string title);
    Task AddMessageAsync(int sessionId, string role, string content);
}

public class ChatSessionVM
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime UpdatedAt { get; set; }
}
