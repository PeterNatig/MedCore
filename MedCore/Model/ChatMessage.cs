using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedCore.Model
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChatSessionId { get; set; }

        [ForeignKey("ChatSessionId")]
        public ChatSession Session { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "user"; // "user" or "assistant"

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
