using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Components
{
    public class BredcrumbViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<LinkModel> Links = new List<LinkModel>();

        public BredcrumbViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var userId = RouteData.Values["userId"]?.ToString();
            var itemId = RouteData.Values["itemId"]?.ToString();
            var collectionId = RouteData.Values["collectionId"]?.ToString();
            var action = RouteData.Values["action"]?.ToString().ToLower();
            var controller = RouteData.Values["controller"]?.ToString().ToLower();

            string collectionName = "", itemName = "";
            if(!string.IsNullOrEmpty(collectionId))
            {
                collectionName = await _unitOfWork.Collection.GetCollectionNameAsync(int.Parse(collectionId));
            }

            if (!string.IsNullOrEmpty(itemId))
            {
                itemName = await _unitOfWork.Item.GetItemNameAsync(int.Parse(itemId));
            }

            AddBreadcrumbs(controller, action, collectionId, itemId, userId, collectionName, itemName);




            return View(Links);
        }

        private void AddBreadcrumbs(string controller, string action, string collectionId, string itemId, string userId, string collectionName, string itemName)
        {
            var breadcrumbMap = new Dictionary<string , List<LinkModel>>
            {
                {
                    "profilecollectionitems",
                    new List<LinkModel>
                    {
                        new LinkModel { Address = Url.Action("index", "profilecollections"), Name = "Collections" },
                        new LinkModel { Address = Url.Action("index", "profilecollectionitems", new { collectionId }), Name = collectionName },
                        new LinkModel { Address = Url.Action("items", "profilecollectionitems", new { collectionId, itemId }), Name = itemName }
                    }
                },
                {
                    "profilecollections",
                    new List<LinkModel>
                    {
                        new LinkModel { Address = Url.Action("index", "profilecollections"), Name = "Collections" },
                        new LinkModel { Address = Url.Action("index", "profilecollectionitems", new { collectionId }), Name = collectionName }
                    }
                },
                {
                    "manageusercollectionitems",
                    new List<LinkModel>
                    {
                        new LinkModel { Address = Url.Action("index", "manageusercollections", new{ area = "admin", userId}), Name = "Collections" },
                        new LinkModel { Address = Url.Action("index", "manageusercollectionitems", new {area = "admin", collectionId, userId}), Name = collectionName },
                        new LinkModel { Address = Url.Action("items", "manageusercollectionitems", new { area = "admin", collectionId, itemId, userId}), Name = itemName }
                    }
                },
                {
                    "manageusercollections",
                    new List<LinkModel>
                    {
                        new LinkModel { Address = Url.Action("index", "manageusercollections", new { area = "admin", userId }), Name = "Collections" },
                        new LinkModel { Address = Url.Action("index", "manageusercollectionitems", new {area = "admin", collectionId, userId}), Name = collectionName }
                    }
                },
                {
                    "collections",
                    new List<LinkModel>
                    {
                        new LinkModel { Address = Url.Action("index", "collections"), Name = "Collections" },
                        new LinkModel { Address = Url.Action("collection", "collections", new { collectionId }), Name = collectionName },
                        new LinkModel { Address = Url.Action("items", "collections", new { collectionId, itemId }), Name = itemName }
                    }
                },

            };

            if (breadcrumbMap.TryGetValue((controller), out var links))
            {
                Links.AddRange(links);
            }
        }
    }
}
