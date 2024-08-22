using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectionManager.Components
{
    public class ItemViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItemViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync(int Id, bool canReact = false)
        {
            var item = await _unitOfWork.Item.GetItemWithReactionsAsync(Id);
            ViewData["canReact"] = canReact;
            return View(item);
        }
    }
}
