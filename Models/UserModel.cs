namespace CollectionManager.Models
{
    public class UserModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }

    public class UserWithSelectFlagModel : UserModel
    {
        public bool IsSelected { get; set; } = false;
    }
}
