using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_DES_project4.Data;
using S_DES_project4.Helpers;
using S_DES_project4.Models;

namespace S_DES_project4.Controllers
{
    public class MessagesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public MessagesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }


        [HttpGet]
        public async Task<IActionResult> LoadMessages(int friendId, string keyInput)
        {
            var currentUserId = _userManager.GetUserId(User);

            var friend = await _context.Friends.FindAsync(friendId);
            if (friend == null)
                return BadRequest("Friend not found");

            // Validate key
            //if (friend.KeyValue != keyInput)
            //    return BadRequest("Invalid key");

            // Fetch all messages for this FriendId (both directions are included automatically)
            var messages = await _context.Messages
                .Where(m => m.FriendId == friendId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            if (friend.KeyValue != keyInput || friend.KeyValue == null)
            {
                var Encrypted = messages.Select(m => new
                {
                    Text = m.CipherText,
                    IsMine = m.SenderId == currentUserId
                }).ToList();

                return Json(Encrypted);
            }

            // Decrypt messages
            var decrypted = messages.Select(m => new
            {
                Text = SDESHelper.Decrypt(m.CipherText, keyInput),
                IsMine = m.SenderId == currentUserId
            }).ToList();

            return Json(decrypted);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Make sure friendId is valid
            var friend = await _context.Friends.FirstOrDefaultAsync(f => f.Id == request.FriendId);
            if (friend == null) return BadRequest("Friend not found");

            // ensure current user is part of the chat
            if (friend.UserAId != currentUserId && friend.UserBId != currentUserId)
                return BadRequest("You are not part of this chat");

            // get key from Friends table
            //string key = friend.KeyValue;

            //var friend = await _context.Friends.FindAsync(request.FriendId);
            string key = friend.KeyValue; // 10-bit key
            string encryptedBinary = SDESHelper.Encrypt(request.Message, key);

            // encrypt the message
            var encryptedText = SDESHelper.Encrypt(request.Message, key);

            var msg = new Message
            {
                FriendId = request.FriendId,
                SenderId = currentUserId,
                ReceiverId = friend.UserAId == currentUserId ? friend.UserBId : friend.UserAId,
                CipherText = encryptedText,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Message sent" });
        }

    }
}