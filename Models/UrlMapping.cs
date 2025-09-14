
using System.ComponentModel.DataAnnotations;

namespace TinyUrlApi
{

    public class UrlMapping
    {
        [Key]
        public int Code { get; set; }
        public string shortURL { get; set; } 
        public string originalURL { get; set; } 
        public int totalClicks { get; set; }
        public bool isPrivate { get; set; }
    }
}