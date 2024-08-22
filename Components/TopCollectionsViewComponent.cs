using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class TopCollectionsViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public TopCollectionsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var collections = await _unitOfWork.Collection.GetTopLargestCollectionsAsync();
            return View(collections);
        }
    }
}
