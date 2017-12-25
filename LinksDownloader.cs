using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinksDownloader
{
    class LinksDownloader
    {
        static void Main(string[] args)
        {
            var url = Console.ReadLine();
            var urlPattern = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w\.\-\(\)\=]*)*\/?$";
            if(!Regex.IsMatch(url, urlPattern))
            {
                Console.WriteLine("this is not url");
                return;
            }
            var links = new List<Tuple<string, Task<int>>>();
            var hrefPattern = "href\\s*=\\s*(?:[\"'])(http[^\"']*|[^\"']*\\.html?|[^\"']*\\.php\\??)(?:[\"'])";
            var source = new WebClient().DownloadString(url);
            var match = Regex.Match(source, hrefPattern, 
                        RegexOptions.IgnoreCase | RegexOptions.Compiled, 
                        TimeSpan.FromSeconds(1));
            while (match.Success)
            {
                var link = match.Groups[1].Value;
                if (!link.StartsWith("http") || !link.StartsWith("https"))
                {
                    link = url.Split('?')[0] + link;
                }
                match = match.NextMatch();
                Task<int> linkLengthTask = GetLengthAsync(link);
                links.Add(new Tuple<string, Task<int>>(link, linkLengthTask));
            }
            foreach (var link in links)
            {
                Console.WriteLine("{0} - {1}", link.Item1, link.Item2.Result);
            }
        }

        async static Task<int> GetLengthAsync(string link)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var data = await webClient.DownloadStringTaskAsync(link);
                    return data.Length;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}