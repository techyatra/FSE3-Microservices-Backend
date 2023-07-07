using Newtonsoft.Json;

namespace ToDoListQuery.Model
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "totalEffortRequired")]
        public int TotalEffortRequired { get; set; }
    }
}
