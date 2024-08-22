using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class CollectionsViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public CollectionsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? userId, bool displayAction = false, bool displayAuthor = true, bool isAdmin = false)
        {

            ViewData["Action"] = displayAction;
            ViewData["DisplayAuthor"] = displayAuthor;
            ViewData["IsAdmin"] = isAdmin;

            if (userId == null)
            {
                var collections = await _unitOfWork.Collection.GetCollections();
                return View(collections);
            }
            var userCollections = await _unitOfWork.Collection.GetUserCollectionsAsync(userId);
            return View(userCollections);
        }
       
    }
}
