using CollectionManager.Data_Access;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using NuGet.Packaging.Signing;

namespace CollectionManager.Controllers
{
    public class SearchController : Controller
    {
        private readonly CollectionMangerDbContext _context;

        public SearchController(CollectionMangerDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new SearchResultModel());
            }

            _context.UpdateItemSearchVectors();

            var words = query.Split(' ');
            var queryString = string.Join(" | ", words.Select(w => $"{w}:*"));

            var fullTextQuery = NpgsqlTsQuery.Parse(queryString);

            var matchedItems = _context.items
                 .Where(i => i.search_vector != null && (i.search_vector.Matches(fullTextQuery) ))
                 .Select(i => new MatchedItemModel
                 {
                     Id = i.Id,
                     Name = i.Name,
                     CollectionId = i.CollectionId,
                 })
                 .ToList();


            var matchedCollections = _context.collections
                .Where(c => c.search_vector != null && c.search_vector.Matches(fullTextQuery))
                .Select(c => new MatchedCollectionModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToList();

            var viewModel = new SearchResultModel
            {
                Items = matchedItems,
                Collections = matchedCollections
            };

            return View(viewModel);
        }

    }
}
