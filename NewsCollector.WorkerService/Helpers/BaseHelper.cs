using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace NewsCollector.WorkerService.Helpers
{
    public class BaseHelper
    {
        public string GetNewsContent(string html)
        {
            var newsContent = "";

            var doc = new HtmlDocument();
            doc.LoadHtml(HtmlEntity.DeEntitize(html));

            var nodes = doc.DocumentNode.SelectNodes("//p");
            if (nodes.Count > 0)
            {
                foreach (var node in nodes)
                {
                    var dem = node.InnerText;
                    newsContent += dem;
                }
            }
            return newsContent;

        }
        public string Decompress(byte[] zip)
        {
            var str = "";
            try
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(zip), CompressionMode.Decompress))
                {
                    const int size = 16384;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        str = System.Text.Encoding.Default.GetString(memory.ToArray());
                        return str;
                    }
                }
            }
            catch
            {
                return str;
            }
        }

        public XmlDocument GetRssXml(string link)
        {
            XmlDocument xml = new XmlDocument();
            Uri url = new Uri(link);
            WebClient client = new WebClient();
            string html = client.DownloadString(url);
            xml.LoadXml(html);
            return xml;
        }

        public string ClearHtmlTags(string node)
        {
            node = Regex.Replace(node, "<[^>]*>", string.Empty);
            return node;
        }
    }
}
