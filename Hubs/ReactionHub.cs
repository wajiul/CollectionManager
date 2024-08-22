using AutoMapper;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.SignalR;

namespace CollectionManager.Hubs
{
    public class ReactionHub: Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReactionHub(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddLike(LikeModel like)
        {
            try
            {
                var liked = _unitOfWork.Item.IsUserLikedAsync(like.ItemId, like.UserId);
                if(liked)
                {
                    return;
                }
                var likeEntity = _mapper.Map<Like>(like);
                await _unitOfWork.Item.AddLikeAsync(likeEntity);
                await _unitOfWork.Save();
            }
            catch(Exception ex)
            {
                throw;
            }
            int likeCount= await _unitOfWork.Item.GetTotalLikeOfItemAsync(like.ItemId);
            await Clients.All.SendAsync("GetItemLikeCount", likeCount);
        }

        public async Task AddComment(CommentModel comment)
        {
            var commentEntity = _mapper.Map<Comment>(comment);
            commentEntity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Item.AddCommentAsync(commentEntity);
            await _unitOfWork.Save();

            var newComment = await _unitOfWork.Item.GetCommentAsync(commentEntity.Id);
            int commentCnt = await _unitOfWork.Item.GetTotalCommentOfItemAsync(comment.ItemId);
            await Clients.All.SendAsync("GetComment", newComment, commentCnt);
        }

    }
}
