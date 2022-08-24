using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace RecuperationDonnee.WuxiaLNScantrad
{
    public class WuxiaLNScantrad : ISite
    {
        public string LienRecuperationNovel => @"https://wuxialnscantrad.wordpress.com";

        public SiteEnum siteEnum => SiteEnum.WuxiaLNScantrad;

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
                    if (!balise.OuterHtml.Contains("<div") && !balise.OuterHtml.Contains("<script"))
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
                var test = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']/div/ul/li/a");
                if (test == null)
                {
                    test = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']/ul/li/a");
                }

                var contenantListeChapitre = test.Where(x => x.GetAttributeValue("Href", string.Empty).Length > 0 && !x.InnerText.Contains("https://") && !x.InnerText.Contains("http://"));
                // group by pour enlever les doublons
                return contenantListeChapitre.Select(x => new Chapitre() { Libelle = System.Net.WebUtility.HtmlDecode(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) }).GroupBy(x => x.LientHtml).Select(x => x.First());
            }
        }

        public IEnumerable<Novel> RecuperationListeNovel()
        {
            using (WebClient client = new WebClient())
            {
                var htmlSite = client.DownloadString(LienRecuperationNovel);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);
                var entetePage = doc.GetElementbyId("primary-menu");
                // Récupération des séries
                var elementOngletSerie = entetePage.Descendants().Where(x => x.GetAttributeValue("id", string.Empty) == "menu-item-2210");
                var elementDansOngletSerie = elementOngletSerie.Select(x => x.Element("ul")).SelectMany(x => x.ChildNodes);
                var listeSerie = elementDansOngletSerie.Select(x => x.Element("a")).Where(x => x != null);
                return listeSerie.Select(x => new Novel() { LientHtmlSommaire = x.GetAttributeValue("href", string.Empty), Titre = x.InnerText });
            }
        }

        public InformationNovel RecupererInformationNovel(string lienPageIntroduction)
        {
            InformationNovel informationNovel = new InformationNovel();

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                var htmlSite = client.DownloadString(lienPageIntroduction);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);

                var listeBaliseDivTexte = doc.GetElementbyId("primary").SelectNodes("//div[@class='entry-content']");
                string elementContenantSynopsis = string.Join(Environment.NewLine, listeBaliseDivTexte.Select(x => WebUtility.HtmlDecode(x.InnerText)));
                var image = doc.GetElementbyId("primary").SelectNodes("//div[@class='entry-content']/p/strong/img");

                if (image != null)
                {
                    informationNovel.LienImage = image.Select(i => i.GetAttributeValue("src", string.Empty)).FirstOrDefault();
                }
                else
                {
                    image = doc.GetElementbyId("primary").SelectNodes("//div[@class='entry-content']/p/img");
                    if (image != null)
                    {
                        informationNovel.LienImage = image.Select(i => i.GetAttributeValue("src", string.Empty)).FirstOrDefault();
                    }
                }
                Regex regexAuteur = new Regex(@"Auteur\(s\):(.*?)\n");
                informationNovel.Auteur = regexAuteur.Match(elementContenantSynopsis).Groups[1].Value.Trim(' ');
                Regex regexResume = new Regex(@"Synopsis :([\s\S]*)Chapitres disponibles", RegexOptions.IgnoreCase);
                informationNovel.Resume = regexResume.Match(elementContenantSynopsis).Groups[1].Value;
                if (string.IsNullOrEmpty(informationNovel.Resume))
                {
                    regexResume = new Regex(@"Sypnopsis([\s\S]*)Sypnopsis officiel", RegexOptions.IgnoreCase);
                    informationNovel.Resume = regexResume.Match(elementContenantSynopsis).Groups[1].Value;
                }
                if (string.IsNullOrEmpty(informationNovel.Resume))
                {
                    regexResume = new Regex(@"Synopsis([\s\S]*)Chapitres disponibles", RegexOptions.IgnoreCase);
                    informationNovel.Resume = regexResume.Match(elementContenantSynopsis).Groups[1].Value;
                }
            }

            return informationNovel;
        }
    }
}
