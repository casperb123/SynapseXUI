using Newtonsoft.Json;

namespace SynapseXUI.Entities.Scripts
{
    public class RbxHubScript
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("featured_image")]
        public FeaturedImage FeaturedImage { get; set; }
    }

    public class FeaturedImage
    {
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }
    }
}
