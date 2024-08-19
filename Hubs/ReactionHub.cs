using AutoMapper;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.SignalR;

namespace CollectionManager.Hubs
{
    public class ReactionHub: Hub
    {
        private readonly ItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public ReactionHub(ItemRepository itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task AddLike(LikeModel like)
        {
            try
            {
                var liked = _itemRepository.IsUserLikedAsync(like.ItemId, like.UserId);
                if(liked)
                {
                    return;
                }
                var likeEntity = _mapper.Map<Like>(like);
                await _itemRepository.AddLikeAsync(likeEntity);
                await _itemRepository.SaveAsync();
            }
            catch(Exception ex)
            {
                throw;
            }
            int likeCount= await _itemRepository.GetTotalLikeOfItemAsync(like.ItemId);
            await Clients.All.SendAsync("GetItemLikeCount", likeCount);
        }

        public async Task AddComment(CommentModel comment)
        {
            var commentEntity = _mapper.Map<Comment>(comment);
            commentEntity.CreatedAt = DateTime.UtcNow;

            await _itemRepository.AddCommentAsync(commentEntity);
            await _itemRepository.SaveAsync();

            var newComment = await _itemRepository.GetCommentAsync(commentEntity.Id);
            int commentCnt = await _itemRepository.GetTotalCommentOfItemAsync(comment.ItemId);
            await Clients.All.SendAsync("GetComment", newComment, commentCnt);
        }

    }
}
