using System.Text.Json.Serialization;

namespace NewsCollector.Core.Models
{
    public class User
    {      
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}