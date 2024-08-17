using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class RecentlyAddedItemsViewComponent: ViewComponent
    {
        private readonly ItemRepository _itemRepository;

        public RecentlyAddedItemsViewComponent(ItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _itemRepository.GetRecentlyAddedItemAsync();
            return View(items);
        }

    }
}
