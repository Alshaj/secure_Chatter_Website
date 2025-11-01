using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_DES_project4.Data;
using S_DES_project4.Models;

namespace S_DES_project4.Controllers
{
    public class ChatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ChatsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            // Load friends with navigation properties
            var friends = await _context.Friends
                .Include(f => f.UserA)
                .Include(f => f.UserB)
                .Where(f => f.UserAId == currentUserId || f.UserBId == currentUserId)
                .ToListAsync();

            return View(friends); // Pass friends list to the view
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest model)
        {
            // 1. Get current user
            var currentUserId = _userManager.GetUserId(User);

            // 2. Find friend user by email
            var friendUser = await _userManager.FindByEmailAsync(model.Email);
            if (friendUser == null)
                return BadRequest("User not found");

            // 3. Check if friendship already exists
            var existingFriend = await _context.Friends
                .FirstOrDefaultAsync(f =>
                    (f.UserAId == currentUserId && f.UserBId == friendUser.Id) ||
                    (f.UserAId == friendUser.Id && f.UserBId == currentUserId));

            if (existingFriend != null)
                return BadRequest("Friendship already exists");

            // 4. Create friend record with key
            var friendRecord = new Friend
            {
                UserAId = currentUserId,
                UserBId = friendUser.Id,
                KeyValue = model.KeyValue,
                CreatedAt = DateTime.UtcNow
            };

            _context.Friends.Add(friendRecord);
            await _context.SaveChangesAsync();

            // 5. Send email with key to friend
            await _emailSender.SendEmailAsync(friendUser.Email, "New Secure Chat Key",
                $"Hello {friendUser.UserName},\n\n" +
                $"You have a new secure chat with {User.Identity.Name}.\n" +
                $"S-DES Key for encryption: {model.KeyValue}\n\n" +
                $"Keep it safe!");

            return Ok(new { message = "Chat created and key sent via email.", friendId = friendRecord.Id });
        }
    }


    // DTO for AJAX request
    public class CreateChatRequest
    {
        public string Email { get; set; }
        public string KeyValue { get; set; }
    }
}
