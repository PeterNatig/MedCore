using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedCore.Model
{
    public class ChatSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = "New Conversation";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
