using MedCore.Data;
using MedCore.Model;
using Microsoft.EntityFrameworkCore;
using ViewModel.Chat;

namespace Service.AI;

public class ChatHistoryService : IChatHistoryService
{
    private readonly MedCoreDbContext _context;

    public ChatHistoryService(MedCoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChatSessionVM>> GetSessionsAsync(string userEmail)
    {
        return await _context.ChatSessions
            .Where(s => s.UserEmail == userEmail)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => new ChatSessionVM
            {
                Id = s.Id,
                Title = s.Title,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<List<ChatMessageVM>> GetSessionMessagesAsync(int sessionId, string userEmail)
    {
        var session = await _context.ChatSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserEmail == userEmail);

        if (session == null) return new List<ChatMessageVM>();

        return session.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessageVM
            {
                Role = m.Role,
                Content = m.Content
            })
            .ToList();
    }

    public async Task<int> CreateSessionAsync(string userEmail, string title)
    {
        var session = new ChatSession
        {
            UserEmail = userEmail,
            Title = string.IsNullOrWhiteSpace(title) ? "New Conversation" : title,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ChatSessions.Add(session);
        await _context.SaveChangesAsync();
        return session.Id;
    }

    public async Task AddMessageAsync(int sessionId, string role, string content)
    {
        var session = await _context.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            session.UpdatedAt = DateTime.UtcNow;
            var message = new ChatMessage
            {
                ChatSessionId = sessionId,
                Role = role,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}
