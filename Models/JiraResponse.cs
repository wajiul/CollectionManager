namespace CollectionManager.Models
{
    public class JiraResponse
    {
        public List<Issue> Issues { get; set; } = new List<Issue>();
    }

    public class Issue
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public Fields Fields { get; set; }
    }

    public class Fields
    {
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public string Summary { get; set; }
    }

    public class Status
    {
        public string Name { get; set; }
    }

    public class Priority
    {
        public string Name { get; set; }
    }

}
