using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class ItemViewComponent: ViewComponent
    {
        private readonly ItemRepository _itemRepository;

        public ItemViewComponent(ItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id)
        {
            var item = await _itemRepository.GetItemWithReactionsAsync(Id);
            return View(item);
        }
    }
}
