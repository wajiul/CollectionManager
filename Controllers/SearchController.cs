using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using NuGet.Packaging.Signing;

namespace CollectionManager.Controllers
{
    [Route("search")]
    public class SearchController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new SearchResultModel());
            }

            _unitOfWork.Item.UpdateSearchVector();

            var words = query.Split(' ');
            var queryString = string.Join(" | ", words.Select(w => $"{w}:*"));

            var fullTextQuery = NpgsqlTsQuery.Parse(queryString);

            var searchResult = await _unitOfWork.Collection.GetSearchResult(fullTextQuery);

            return View(searchResult);
        }

        [HttpGet("tag/{tagId}")]
        public async Task<IActionResult> SearchByTag(int tagId)
        {
            var items = await _unitOfWork.Item.GetItemsByTagAsync(tagId);
            return View(items);
        }
    }
}
