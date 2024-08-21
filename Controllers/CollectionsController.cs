using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.AspNetCore.Identity;
using CollectionManager.Enums;
using System.Security.Claims;
using CollectionManager.Data_Access.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Authorization;

namespace CollectionManager.Controllers
{
    [Route("collections")]
    public class CollectionsController : Controller
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ItemRepository _itemRepository;

        public CollectionsController(  CollectionRepository collectionRepository, UserManager<User> userManager, IMapper mapper, ItemRepository itemRepository)
        {
            _collectionRepository = collectionRepository;
            _userManager = userManager;
            _mapper = mapper;
            _itemRepository = itemRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{id}/items")]
        public IActionResult Collection(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var exist = _collectionRepository.IsCollectionExist(id.Value);

            if (!exist)
            {
                return NotFound();
            }

            return View(id.Value);
        }

        [HttpGet("{collectionId}/items/{id}")]
        public IActionResult Items(int? Id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId;
            return View(Id);
        }

        [HttpPost("items/like")]
        [Authorize]
        public async Task<IActionResult> Like([FromBody] LikeModel like)
        {
            var liked = _itemRepository.IsUserLikedAsync(like.ItemId, like.UserId);
            if (liked)
            {
                return BadRequest(new { Message = "Already liked" });
            }

            var likeEntity = _mapper.Map<Like>(like);

            await _itemRepository.AddLikeAsync(likeEntity);
            await _itemRepository.SaveAsync();

            return Ok(new { Message = "Like added successfully" });
        }

        [HttpGet("/items/{itemId}/isliked")]
        public IActionResult IsUserLiked(int? itemId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null || itemId == null)
            {
                return NotFound();
            }

            var isLiked = _itemRepository.IsUserLikedAsync(itemId.Value, userId);
            return Ok(isLiked);
        }

        [HttpPost("items/comment")]
        [Authorize]
        public async Task<IActionResult> Comment([FromBody] CommentModel comment)
        {
            var commentEntity = _mapper.Map<Comment>(comment);
            commentEntity.CreatedAt = DateTime.UtcNow;

            try
            {
                await _itemRepository.AddCommentAsync(commentEntity);
                await _itemRepository.SaveAsync();

            }
            catch (Exception)
            {
                throw;
            }
            return Ok(await _itemRepository.GetCommentAsync(commentEntity.Id));
        }

        


    }
}
