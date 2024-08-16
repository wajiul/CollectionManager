using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;

namespace CollectionManager.Controllers
{
    public class ItemsController : Controller
    {
        private readonly CollectionMangerDbContext _context;

        public ItemsController(CollectionMangerDbContext context)
        {
            _context = context;
        }
        

        [HttpPost]
        public async Task<IActionResult> Like([FromBody] LikeModel like)
        {
            var existing = await _context.likes.FirstOrDefaultAsync(l => l.UserId == like.UserId && l.ItemId == like.ItemId);
            if(existing != null)
            {
                return BadRequest(new { Message = "Already liked" });
            }
            var likeEntity = new Like
            {
                UserId = like.UserId,
                ItemId = like.ItemId
            };

            await _context.likes.AddAsync(likeEntity);
            await _context.SaveChangesAsync();
            
           return Ok(new { Message = "Like added successfully"});
        }

        [HttpPost]
        public async Task<IActionResult> Comment([FromBody] CommentModel comment)
        {
            
            var commentEntity = new Comment
            {
                Text = comment.Text,
                UserId = comment.UserId,
                ItemId = comment.ItemId,
                CreatedAt = DateTime.Now
            };
            await _context.comments.AddAsync(commentEntity);
            await _context.SaveChangesAsync();  
            return Ok(new { Message = "Comment added successfully" });
        }

    }
}
