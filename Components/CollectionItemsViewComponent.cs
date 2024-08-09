using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CollectionManager.Components
{
    public class CollectionItemsViewComponent: ViewComponent
    {
        private readonly CollectionRepository _collectionRepository;

        public CollectionItemsViewComponent(CollectionRepository collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int collectionId, bool displayAction = false)
        {
            var collectionItems = await _collectionRepository.GetCollectionWithItemsReactionCount(collectionId);

            ViewData["Action"] = displayAction;
            return View(collectionItems);
        }
    }
}
