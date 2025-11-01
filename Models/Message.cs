using Microsoft.AspNetCore.Identity;

namespace S_DES_project4.Models
{
    public class Message
    {
        public int Id { get; set; }

        // Sender
        public string SenderId { get; set; }
        public IdentityUser Sender { get; set; }

        // Receiver
        public string ReceiverId { get; set; }
        public IdentityUser Receiver { get; set; }

        // Encrypted message text
        public string CipherText { get; set; }

        // Foreign key to Friend to use the key value for S-DES encryption
        public int FriendId { get; set; }
        public Friend Friend { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
