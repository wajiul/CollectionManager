using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class TopCollectionsViewComponent: ViewComponent
    {
        private readonly CollectionRepository _collectionRepository;

        public TopCollectionsViewComponent(CollectionRepository collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var collections = await _collectionRepository.GetTopLargestCollectionsAsync();
            return View(collections);
        }
    }
}
