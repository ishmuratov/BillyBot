using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleTelegaBot
{
    class BashORG
    {
        private List<string> articles;
        int counter;

        public BashORG()
        {
            counter = -1;
            articles = new List<string>();
            GetRandoms();
        }

        private void GetRandoms()
        {
            articles.Clear();
            var request = WebRequest.Create("https://bash.im/random");
            using (var responses = request.GetResponse())
            {
                using (var streams = responses.GetResponseStream())
                using (var readers = new StreamReader(streams))
                {
                    //в переменной html наш сайт
                    string html = readers.ReadToEnd();
                    Match match = Regex.Match(html, @"<div class=""quote__body"">(.+?)<\/div>", RegexOptions.Singleline);
                    while (match.Success)
                    {
                        string answer = match.Value;
                        answer = deleteTags(answer);
                        articles.Add(answer);
                        // Переходим к следующему совпадению
                        match = match.NextMatch();
                    }
                }
            }
        }

        private string deleteTags(string _input)
        {
            string result = _input;
            result = result.Replace("<div class=\"quote__body\">", String.Empty);
            result = result.Replace("</div>", String.Empty);
            result = result.Replace("<br>", "\n");
            result = result.Replace("&quot;", "\"");
            result = result.Replace("&lt;", "<");
            result = result.Replace("&gt;", ">");
            result = Regex.Replace(result, "<[^>]+>", string.Empty);
            result = result.Trim();
            return result;
        }

        public string GetRandom()
        {
            if (counter == articles.Count - 1)
            {
                counter = -1;
                GetRandoms();
            }

            counter++;
            string result = articles[counter];

            if (result != null)
                return result;
            else
                return string.Empty;
        }
    }
}
