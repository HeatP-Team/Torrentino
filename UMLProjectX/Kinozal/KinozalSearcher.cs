﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace UMLProjectX.Kinozal
{
    public static class KinozalSearcher
    {
        private const string SearchUrl = "https://kinozal-tv.appspot.com/browse.php?s=";
        private const string MainUrl = "https://kinozal-tv.appspot.com/";
        private static readonly HtmlWeb Web = new();

        public static List<string> GetLinks(string filmName)
        {
            var res = GetInfos(filmName);

            return new List<string> {"111", "222", "333"};
        }

        private static IList<FilmInfo> GetInfos(string filmName)
        {
            var url = SearchUrl + filmName;

            var doc = GetDoc(url);

            return doc.DocumentNode.CssSelect("tr.bg > .nam > a")
                //.AsParallel()
                .Select(node => GetFilmInfo(MainUrl + node.Attributes["href"].Value))
                .ToArray();
        }
        private static FilmInfo GetFilmInfo(string filmUrl)
        {
            filmUrl = filmUrl.Remove(filmUrl.IndexOf("amp;", StringComparison.InvariantCultureIgnoreCase), 4);

            var page = GetDoc(filmUrl).DocumentNode;

            var res = page.CssSelect("h1");

            var fullName = page.CssSelect("h1 > a").First().InnerText.Split(" / ");

            //TODO: cringeParse

            var fullInfo = page.CssSelect("#tabs > b")
                .First().InnerText.Split("<br>")
                .Select(a => a.Split("\n")[1])
                .ToArray();

            return new FilmInfo
            {
                Name = fullName[0],
                Year = fullName[2],
                Quality = fullInfo[0],
                Size = fullInfo[3],
                Link = filmUrl,
                Poster = page.CssSelect("img.p200").First().Attributes["src"].Value
            };
        }

        private static HtmlDocument GetDoc(string url)
        {
            string html;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc;
        }
    }
}