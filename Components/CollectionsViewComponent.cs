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
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var collectionEntities = await _collectionRepository.GetCollections();
            var collectionModels = new List<CollectionModel>();
            foreach (var collection in collectionEntities)
            {
                collectionModels.Add(new CollectionModel
                {
                    Id = collection.Id,
                    Name = collection.Name,
                    Description = collection.Description,
                    Category = collection.Category,
                    ImageUrl = collection.ImageUrl,
                    UserId = collection.UserId
                });
            }
            return View(collectionModels);
        }
       
    }
}
