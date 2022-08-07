using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace RecuperationDonnee.WarriorLegendTrad
{
    public class WarriorLegendTrad : ISite
    {
        public string LienRecuperationNovel => @"https://warriorlegendtrad.fr/light-novel/";

        public SiteEnum siteEnum => SiteEnum.WarriorLegendTrad;

        public string RecuperationChapitre(string lienChapitre, bool html)
        {
            using (WebClient client = new WebClient())
            {
                var htmlSite = client.DownloadString(lienChapitre);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);
                var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']");

                List<string> listeParagraphe = new List<string>();

                if (html)
                {
                    listeParagraphe.Add("<html>");
                    listeParagraphe.Add("<body>");
                }

                foreach (var balise in listeBaliseDivTexte.SelectMany(x => x.ChildNodes))
                {
                    if (!balise.OuterHtml.Contains("<div") && !balise.OuterHtml.Contains("<script") && !balise.OuterHtml.Contains("<hr class=wp"))
                    {
                        if (html)
                        {
                            listeParagraphe.Add(WebUtility.HtmlDecode(balise.OuterHtml));
                        }
                        else
                        {
                            listeParagraphe.Add(WebUtility.HtmlDecode(balise.InnerText));
                        }
                    }
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
                var htmlSite = client.DownloadString(lienPagechapitre);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);
                var test = doc.GetElementbyId("content").SelectNodes("//div[@class='wp-block-newspack-blocks-homepage-articles is-style-default wpnbha ts-3 is-style-default']/div/article/div/h2/a");
                if (test == null)
                {
                    test = doc.GetElementbyId("content").SelectNodes("//div[@class='wp-block-newspack-blocks-homepage-articles is-style-default wpnbha ts-4 is-style-default']/div/article/div/h2/a");
                }
                var contenantListeChapitre = test.Where(x => x.GetAttributeValue("Href", string.Empty).Length > 0 && !x.InnerText.Contains("https://") && !x.InnerText.Contains("http://"));
                // group by pour enlever les doublons
                return contenantListeChapitre.Select(x => new Chapitre() { Libelle = System.Net.WebUtility.HtmlDecode(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) }).GroupBy(x => x.LientHtml).Select(x => x.First()).Reverse();
            }
        }

        public IEnumerable<Novel> RecuperationListeNovel()
        {
            using (WebClient client = new WebClient())
            {
                var htmlSite = client.DownloadString(LienRecuperationNovel);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);
                var entetePage = doc.GetElementbyId("menu-primary");
                // Récupération des séries
                var elementOngletSerie = entetePage.Descendants().Where(x => x.GetAttributeValue("class", string.Empty) == "sub-menu");
                var listeSerie = elementOngletSerie.Select(x => x.ChildNodes).First().Select(x => x.Element("a")).Where(x => x != null);


                return listeSerie.Select(x => new Novel() { LientHtmlSommaire = x.GetAttributeValue("href", string.Empty), Titre = WebUtility.HtmlDecode(x.InnerText) });
            }
        }

        public InformationNovel RecupererInformationNovel(string lienPageIntroduction)
        {
            throw new NotImplementedException();
        }
    }
}
