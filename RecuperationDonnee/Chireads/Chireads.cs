using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace RecuperationDonnee.Chireads
{
    /// <summary>
    /// méthode ^pir me site chireads 
    /// </summary>
    public class Chireads : ISite
    {
        public string LienRecuperationNovel => @"https://chireads.com/category/translatedtales/page/{0}";
        public string LienRecuperationNovelOriginal => @"https://chireads.com/category/original/";

        public SiteEnum siteEnum { get => SiteEnum.Chireads; }


        public string RecuperationChapitre(string lienChapitre, bool html)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienChapitre);
            doc.LoadHtml(doc.Text.Replace("</i> <i>", ""));
            var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//*[@id='content']/p");
            var listeBaliseDivTitre = doc.GetElementbyId("content").SelectNodes("//div[@class='font-color-black3 article-title']");

            List<string> listeParagraphe = new List<string>();

            if (html)
            {
                listeParagraphe.Add("<html>");
                listeParagraphe.Add("<body>");
            }

            if (html)
                listeParagraphe.Add(listeBaliseDivTitre.First().OuterHtml);
            else
                listeParagraphe.Add(listeBaliseDivTitre.First().InnerText);

            foreach (var balise in listeBaliseDivTexte.SelectMany(x => x.ChildNodes))
            {
                if (!balise.OuterHtml.Contains("<strong class=\"ql-author-") && !balise.OuterHtml.Contains("<a class=\"ql-link ql-author-"))
                {
                    if (html)
                    {
                        Regex rg = new Regex("style=\"(.*)\"");
                        listeParagraphe.Add("<p>" + rg.Replace(balise.OuterHtml, "") + "</p>");
                    }
                    else
                        listeParagraphe.Add(balise.InnerText);
                }
            }

            if (html)
            {
                listeParagraphe.Add("</body>");
                listeParagraphe.Add("</html>");
            }

            return string.Join(Environment.NewLine, listeParagraphe);
        }

        public IEnumerable<Chapitre> RecuperationListeChapitre(string lienPagechapitre)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienPagechapitre);
            try
            {
                var contenantListeChapitre = doc.GetElementbyId("content").SelectNodes("//div[@id='content']/ul/li/a[@class='font-color-black']");
                return contenantListeChapitre.Select(x => new Chapitre() { LientHtml = x.GetAttributeValue("Href", string.Empty), Libelle = System.Net.WebUtility.HtmlDecode(x.GetAttributeValue("title", string.Empty)) }).ToList();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
            }
#pragma warning restore CA1031 // Do not catch general exception types
            return new List<Chapitre>();
        }

        public IEnumerable<Novel> RecuperationListeNovel()
        {
            List<Novel> listeNovel = new List<Novel>();
            listeNovel.AddRange(RecuperationListeNovelPage(string.Format(CultureInfo.InvariantCulture, LienRecuperationNovel, "1")));
            listeNovel.AddRange(RecuperationListeNovelPage(string.Format(CultureInfo.InvariantCulture, LienRecuperationNovel, "2")));
            listeNovel.AddRange(RecuperationListeNovelPage(LienRecuperationNovelOriginal));
            return listeNovel;
        }

        private static IEnumerable<Novel> RecuperationListeNovelPage(string page)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(page);
            var contenantListeNovel = doc.GetElementbyId("content");
            var listeNovel = contenantListeNovel.SelectNodes("//*[@class='font-color-black3']").Descendants().Where(x => x.GetAttributeValue("Href", string.Empty).Length > 0);
            return listeNovel.Select(x => new Novel() { LientHtmlSommaire = x.GetAttributeValue("Href", string.Empty), Titre = System.Net.WebUtility.HtmlDecode(x.GetAttributeValue("title", string.Empty)) }).ToList();
        }

        public InformationNovel RecupererInformationNovel(string lienPageIntroduction)
        {
            InformationNovel informationNovel = new InformationNovel();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienPageIntroduction);
            doc.LoadHtml(doc.Text.Replace("</i> <i>", ""));
            var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//div[@class='inform-inform-txt']/div[@class='inform-txt-show font-color-black6']");

            List<string> listeParagraphe = new List<string>();

            listeParagraphe.Add("<html>");
            listeParagraphe.Add("<body>");



            foreach (var balise in listeBaliseDivTexte.SelectMany(x => x.ChildNodes))
            {

                listeParagraphe.Add(balise.OuterHtml);

            }

            listeParagraphe.Add("</body>");
            listeParagraphe.Add("</html>");

            informationNovel.Resume = string.Join(Environment.NewLine, listeParagraphe);
            var image = doc.GetElementbyId("content").SelectNodes("//div[@class='inform-product']/img");

            informationNovel.LienImage = image.Select(i => i.GetAttributeValue("src", string.Empty)).FirstOrDefault();
            return informationNovel;
        }
    }
}