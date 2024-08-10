using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class CollectionsViewComponent: ViewComponent
    {
        private readonly CollectionRepository _collectionRepository;

        public CollectionsViewComponent(CollectionRepository collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(string? userId)
        {
            if (userId == null)
            {
                var collections = await _collectionRepository.GetCollections();
                return View(collections);
            }
            var userCollections = await _collectionRepository.GetUserCollectionsAsync(userId);
            return View(userCollections);
        }
       
    }
}
