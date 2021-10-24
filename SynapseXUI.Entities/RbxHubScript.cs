using Newtonsoft.Json;
using System.ComponentModel;

namespace SynapseXUI.Entities
{
    public class RbxHubScript : INotifyPropertyChanged
    {
        private bool display;

        public bool Display
        {
            get => display;
            set
            {
                display = value;
                OnPropertyChanged(nameof(Display));
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public RbxHubScript()
        {
            Display = true;
        }
    }

    public class FeaturedImage
    {
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }
    }
}
