﻿using System;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CollectionsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{collectionId}/items")]
        public IActionResult Collection(int? collectionId)
        {
            if (collectionId == null)
            {
                return NotFound();
            }
            var exist = _unitOfWork.Collection.IsCollectionExist(collectionId.Value);

            if (!exist)
            {
                return NotFound();
            }

            return View(collectionId.Value);
        }

        [HttpGet("{collectionId}/items/{itemId}")]
        public IActionResult Items(int? itemId, int? collectionId = null)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId;
            ViewData["CollectionId"] = collectionId;
            return View(itemId);
        }

        [HttpPost("items/like")]
        [Authorize]
        public async Task<IActionResult> Like([FromBody] LikeModel like)
        {
            var liked = _unitOfWork.Item.IsUserLikedAsync(like.ItemId, like.UserId);
            if (liked)
            {
                return BadRequest(new { Message = "Already liked" });
            }

            var likeEntity = _mapper.Map<Like>(like);

            await _unitOfWork.Item.AddLikeAsync(likeEntity);
            await _unitOfWork.Save();

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

            var isLiked = _unitOfWork.Item.IsUserLikedAsync(itemId.Value, userId);
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
                await _unitOfWork.Item.AddCommentAsync(commentEntity);
                await _unitOfWork.Save();
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(await _unitOfWork.Item.GetCommentAsync(commentEntity.Id));
        }
    }
}
