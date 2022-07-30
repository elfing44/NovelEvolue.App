using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace RecuperationDonnee.HarkenEliwood
{
    public class HarkenEliwood : ISite
    {
        public string LienRecuperationNovel => @"https://harkeneliwood.wordpress.com/projets/";

        public SiteEnum siteEnum { get => SiteEnum.HarkenEliwwoof; }

        public string RecuperationChapitre(string lienChapitre, bool html)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                var htmlSite = client.DownloadString(lienChapitre);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);
                var listeBaliseDivTitre = doc.GetElementbyId("content").SelectNodes("//h1[@class='entry-title']");
                var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']");

                List<string> listeParagraphe = new List<string>();

                if (html)
                {
                    listeParagraphe.Add("<html>");
                    listeParagraphe.Add("<body>");
                }

                if (html)
                    listeParagraphe.Add(WebUtility.HtmlDecode(listeBaliseDivTitre.First().OuterHtml));
                else
                    listeParagraphe.Add(WebUtility.HtmlDecode(listeBaliseDivTitre.First().InnerText));

                foreach (var balise in listeBaliseDivTexte.SelectMany(x => x.ChildNodes))
                {
                    if ((balise.OuterHtml.Contains("https://") || balise.OuterHtml.Contains("https://")) && !balise.OuterHtml.Contains("<img "))
                        break;
                    if (html)
                    {
                        listeParagraphe.Add(WebUtility.HtmlDecode(balise.OuterHtml));
                    }
                    else
                        listeParagraphe.Add(WebUtility.HtmlDecode(balise.InnerText));
                }

                if (html)
                {
                    listeParagraphe.Add("</body>");
                    listeParagraphe.Add("</html>");
                }

                return string.Join(Environment.NewLine, listeParagraphe);
            }
        }

        public IEnumerable<Chapitre> RecuperationListeChapitre(string lienPagechapitre)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                var htmlSite = client.DownloadString(lienPagechapitre);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);
                var contenantListeChapitre = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']/p/a").Where(x => x.GetAttributeValue("Href", string.Empty).Length > 0 && !x.InnerText.Contains("https://") && !x.InnerText.Contains("http://"));
                return contenantListeChapitre.Select(x => new Chapitre() { Libelle = System.Net.WebUtility.HtmlDecode(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) });
            }
        }

        public IEnumerable<Novel> RecuperationListeNovel()
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                var htmlSite = client.DownloadString(LienRecuperationNovel);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);
                var contenantListeNovel = doc.GetElementbyId("content");
                var listeNovel = contenantListeNovel.SelectNodes("//*[@class='entry-content']").Descendants().Where(x => x.GetAttributeValue("Href", string.Empty).Length > 0 && x.InnerText != "Twitter" && x.InnerText != "Facebook");
                return listeNovel.Select(x => new Novel() { LientHtmlSommaire = x.GetAttributeValue("Href", string.Empty), Titre = System.Net.WebUtility.HtmlDecode(x.InnerText) }).ToList();
            }
        }

        public InformationNovel RecupererInformationNovel(string lienPageIntroduction)
        {
            throw new System.NotImplementedException();
        }
    }
}
