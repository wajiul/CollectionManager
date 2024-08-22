using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class RecentlyAddedItemsViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecentlyAddedItemsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _unitOfWork.Item.GetRecentlyAddedItemAsync();
            return View(items);
        }

    }
}
