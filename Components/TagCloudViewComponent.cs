using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class TagCloudViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagCloudViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tags = await _unitOfWork.Item.GetTagsAsync();
            return View(tags);
        }
    }
}
