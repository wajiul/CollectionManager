namespace CollectionManager.Models
{
    public class CollectionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<ItemViewModel> Items { get; set; } = new List<ItemViewModel>();
    }
}
