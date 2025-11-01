namespace S_DES_project4.Models
{
    using Microsoft.AspNetCore.Identity;

    public class Friend
    {
        public int Id { get; set; }
        public string UserAId { get; set; }
        public IdentityUser UserA { get; set; }

        public string UserBId { get; set; }
        public IdentityUser UserB { get; set; }

        public string KeyValue { get; set; } // 10-bit S-DES key
        public bool IsBlocked { get; set; } = false;
        public bool BlockedByA { get; set; } = false;
        public DateTime? BlockDateByA { get; set; }

        public bool BlockedByB { get; set; } = false;
        public DateTime? BlockDateByB { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


}
