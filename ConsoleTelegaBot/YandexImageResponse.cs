using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureSearchExample
{
    [Serializable]
    public class YandexImageResponse
    {
        public string Name { get; set; }
        public List<string> ImageLinks { get; set; }
        public YandexImageResponse()
        {
            ImageLinks = new List<string>();
            Name = string.Empty;
        }

        public YandexImageResponse(string _name, List<string> _imageLinks)
        {
            Name = _name;
            ImageLinks = _imageLinks;
        }
    }
}
