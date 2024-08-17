using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class TagCloudViewComponent: ViewComponent
    {
        private readonly ItemRepository _itemRepository;

        public TagCloudViewComponent(ItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tags = await _itemRepository.GetTagCloudAsync();
            return View(tags);
        }
    }
}
