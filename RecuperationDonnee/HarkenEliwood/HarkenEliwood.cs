using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
                return contenantListeChapitre.Where(x => !string.IsNullOrEmpty(System.Net.WebUtility.HtmlDecode(x.InnerText))).Select(x => new Chapitre() { Libelle = System.Net.WebUtility.HtmlDecode(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) });
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
            InformationNovel infos = new InformationNovel();

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                var htmlSite = client.DownloadString(lienPageIntroduction);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);

                var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']");
                string elementContenantSynopsis = string.Join(Environment.NewLine, listeBaliseDivTexte.Select(x => WebUtility.HtmlDecode(x.InnerText)));
                var image = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']/p/img");

                if (image != null)
                {
                    infos.LienImage = image.Select(i => i.GetAttributeValue("src", string.Empty)).FirstOrDefault();
                }


                infos.Resume = RecupererResume(elementContenantSynopsis).Trim(Environment.NewLine.ToArray())
                        .Replace("\n\n", Environment.NewLine)
                        .Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                infos.Auteur = RecupererAuteur(elementContenantSynopsis).Trim(' ');
            }


            return infos;
        }

        private static string RecupererAuteur(string text)
        {
            Regex regexAuteur = new Regex(@"Auteur :(.*?)" + Environment.NewLine);
            if (!string.IsNullOrEmpty(regexAuteur.Match(text).Groups[1].Value))
            {
                return regexAuteur.Match(text).Groups[1].Value;
            }
            regexAuteur = new Regex(@"Auteur :(.*?)\n");
            if (!string.IsNullOrEmpty(regexAuteur.Match(text).Groups[1].Value))
            {
                return regexAuteur.Match(text).Groups[1].Value;
            }

            regexAuteur = new Regex(@"Autheur :(.*?)\n");
            if (!string.IsNullOrEmpty(regexAuteur.Match(text).Groups[1].Value))
            {
                return regexAuteur.Match(text).Groups[1].Value;
            }

            return string.Empty;
        }

        private static string RecupererResume(string text)
        {
            Regex regexResume = new Regex(@"Synopsis :([\s\S]*)Traduction anglaise", RegexOptions.IgnoreCase);
            string resume = regexResume.Match(text).Groups[1].Value;
            if (string.IsNullOrEmpty(resume))
            {
                regexResume = new Regex(@"Synopsis :([\s\S]*)Raw :", RegexOptions.IgnoreCase);
                resume = regexResume.Match(text).Groups[1].Value;
            }

            if (string.IsNullOrEmpty(resume))
            {
                regexResume = new Regex(@"Synopsis 1 :([\s\S]*)Synopsis 2 :([\s\S]*)Raw :", RegexOptions.IgnoreCase);
                resume = regexResume.Match(text).Groups[1].Value + regexResume.Match(text).Groups[2].Value;
            }

            if (string.IsNullOrEmpty(resume))
            {
                regexResume = new Regex(@"Synopsis :([\s\S]*)Prélude", RegexOptions.IgnoreCase);
                resume = regexResume.Match(text).Groups[1].Value;
            }

            if (string.IsNullOrEmpty(resume))
            {
                regexResume = new Regex(@"Synospis :([\s\S]*)Original ", RegexOptions.IgnoreCase);
                resume = regexResume.Match(text).Groups[1].Value;
            }

            regexResume = new Regex(@"([\s\S]*)Raw :", RegexOptions.IgnoreCase);

            if (!string.IsNullOrEmpty(regexResume.Match(resume).Groups[1].Value))
            {
                resume = regexResume.Match(resume).Groups[1].Value;
            }
            return resume;
        }
    }
}
