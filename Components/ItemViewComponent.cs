using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectionManager.Components
{
    public class ItemViewComponent: ViewComponent
    {
        private readonly ItemRepository _itemRepository;

        public ItemViewComponent(ItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id, bool canReact = false)
        {
            var item = await _itemRepository.GetItemWithReactionsAsync(Id);
            ViewData["canReact"] = canReact;
            return View(item);
        }
    }
}
