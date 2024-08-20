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
        private readonly CollectionMangerDbContext _context;
        private readonly ItemRepository _itemRepository;

        public SearchController(CollectionMangerDbContext context, ItemRepository itemRepository)
        {
            _context = context;
            _itemRepository = itemRepository;
        }
        [HttpGet("")]
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
                     CollectionName = i.Collection.Name,
                     LikeCount = i.Likes.Count,
                     CommentCount = i.Comments.Count
                 })
                 .ToList();


            var matchedCollections = _context.collections
                .Where(c => c.search_vector != null && c.search_vector.Matches(fullTextQuery))
                .Select(c => new MatchedCollectionModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ItemCount = c.Items.Count
                })
                .ToList();

            var viewModel = new SearchResultModel
            {
                Items = matchedItems,
                Collections = matchedCollections
            };

            return View(viewModel);
        }

        [HttpGet("tag/{tagId}")]
        public async Task<IActionResult> SearchByTag(int tagId)
        {
            var items = await _itemRepository.GetItemsByTagAsync(tagId);
            return View(items);
        }
    }
}
