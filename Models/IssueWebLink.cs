using Newtonsoft.Json;

namespace CollectionManager.Models
{
    public class IssueWebLink
    {
        [JsonProperty("object")]
        public WebLinkObject WebLinkObject { get; set; }
    }

    public class WebLinkObject
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
