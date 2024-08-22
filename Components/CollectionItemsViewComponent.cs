using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CollectionManager.Components
{
    public class CollectionItemsViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public CollectionItemsViewComponent(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(int collectionId, bool displayAction = false, bool isAdmin = false)
        {
            var collectionItems = await _unitOfWork.Collection.GetCollectionWithItemsReactionCount(collectionId);
            ViewData["Action"] = displayAction;
            ViewData["IsAdmin"] = isAdmin;
            return View(collectionItems);
        }
    }
}
