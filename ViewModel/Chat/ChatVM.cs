using System.ComponentModel.DataAnnotations;

namespace ViewModel.Chat;

public class ChatMessageVM
{
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
}

public class ChatRequestVM
{
    [Required]
    public string Message { get; set; } = string.Empty;
    public List<ChatMessageVM> History { get; set; } = [];
}

public class ChatResponseVM
{
    public string Reply { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}
