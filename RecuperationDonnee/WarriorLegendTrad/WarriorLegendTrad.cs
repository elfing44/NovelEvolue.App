using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
                var premierElement = doc.GetElementbyId("content").SelectNodes("//div/div/article/div/h2/a | //div/div/article/div/h3/a");

                var contenantListeChapitre = premierElement.Where(x => x.GetAttributeValue("Href", string.Empty).Length > 0 && !x.InnerText.Contains("https://") && !x.InnerText.Contains("http://")).ToList();

                // group by pour enlever les doublons
                List<Chapitre> listeChapitre = contenantListeChapitre.Select(x => new Chapitre() { Libelle = System.Net.WebUtility.HtmlDecode(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) }).
                    GroupBy(x => x.LientHtml).Select(x => x.First()).ToList();
                //Détermination sens 
                if (!EstPremierChapitre(listeChapitre.First()))
                {
                    listeChapitre.Reverse();
                }

                listeChapitre.AddRange(RecuperationListeChapitreNonSommaire(listeChapitre.Last(), listeChapitre.Count));
                return listeChapitre;
            }
        }

        /// <summary>
        /// Détermine si le chapitre est le premier chapitre
        /// </summary>
        /// <param name="chapitre">chapitre a testé</param>
        /// <returns>true si premier chapitre false sinon</returns>
        private static bool EstPremierChapitre(Chapitre chapitre)
        {
            bool existeUnChapitrePrecedent = false;

            using (WebClient client = new WebClient())
            {
                var htmlSite = client.DownloadString(chapitre.LientHtml);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);

                var elementNavigationSuivant = doc.GetElementbyId("content").SelectNodes("//*[@class='wp-block-button__link wp-element-button']");
                existeUnChapitrePrecedent = elementNavigationSuivant != null && elementNavigationSuivant.First() != null && elementNavigationSuivant.First().GetAttributeValue("Href", string.Empty) != string.Empty;

            }
            return !existeUnChapitrePrecedent;
        }


        /// <summary>
        /// Récupére les chapitres qui ne sont pas encore dans le somaire
        /// </summary>
        /// <param name="dernierChapitre">dernier chapitre du sommaire</param>
        /// <returns>liste de chapitre qui ne sont pas dans le sommaire</returns>
        private static List<Chapitre> RecuperationListeChapitreNonSommaire(Chapitre dernierChapitre, int nombre)
        {
            List<Chapitre> listeChapitre = new List<Chapitre>();
            bool existeUnChapitreSuivant = true;

            using (WebClient client = new WebClient())
            {
                string lienChapitreSUivant = dernierChapitre.LientHtml;
                while (existeUnChapitreSuivant)
                {
                    var htmlSite = client.DownloadString(lienChapitreSUivant);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlSite);

                    var elementNavigationSuivant = doc.GetElementbyId("content").SelectNodes("//*[@class='wp-block-button__link wp-element-button']");
                    if (elementNavigationSuivant != null && elementNavigationSuivant.Last() != null)
                    {
                        lienChapitreSUivant = elementNavigationSuivant.Last().GetAttributeValue("Href", string.Empty);
                        if (lienChapitreSUivant != string.Empty)
                        {
                            listeChapitre.Add(new Chapitre() { LientHtml = lienChapitreSUivant, Libelle = string.Format("Chapitre {0}", nombre++) });
                        }
                        else
                            existeUnChapitreSuivant = false;
                    }
                    else
                        existeUnChapitreSuivant = false;
                }


            }
            return listeChapitre;
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
            InformationNovel informationNovel = new InformationNovel();

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;

                var htmlSite = client.DownloadString(lienPageIntroduction);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlSite);
                var image = doc.GetElementbyId("content").SelectNodes("//div[@class='site-content']/section/main/article/figure/img");



                var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']");
                string elementContenantSynopsis = string.Join(Environment.NewLine, listeBaliseDivTexte.Select(x => WebUtility.HtmlDecode(x.InnerText)));

                Regex regexResume = new Regex(@"Synopsis :([\s\S]*)index chapitre :", RegexOptions.IgnoreCase);

                informationNovel.Auteur = WebUtility.HtmlDecode(RecupererAuteur(doc)).Trim(' ');
                informationNovel.LienImage = image.Select(i => i.GetAttributeValue("src", string.Empty)).FirstOrDefault();
                informationNovel.Resume = regexResume.Match(elementContenantSynopsis).Groups[1].Value.Trim(Environment.NewLine.ToArray())
                    .Replace("\n\n", Environment.NewLine)
                    .Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }
            return informationNovel;
        }

        private static string RecupererAuteur(HtmlDocument doc)
        {
            Regex regexAuteur = new Regex(@"<p><strong>Auteur&nbsp;:</strong> <a(.*?)></a><a(.*?)>(.*?)</a></p>");
            if (regexAuteur.Match(doc.Text).Groups.Count == 4 && !string.IsNullOrEmpty(regexAuteur.Match(doc.Text).Groups[3].Value))
            {
                return regexAuteur.Match(doc.Text).Groups[3].Value.Trim(' ');
            }
            regexAuteur = new Regex(@"<p><strong>Auteur&nbsp;:</strong> <a(.*?)>(.*?)</a></p>");
            if (regexAuteur.Match(doc.Text).Groups.Count == 3 && !string.IsNullOrEmpty(regexAuteur.Match(doc.Text).Groups[2].Value))
            {
                return regexAuteur.Match(doc.Text).Groups[2].Value;
            }

            regexAuteur = new Regex(@"<p><strong>Auteur&nbsp;:</strong>(.*?)</p>");
            if (!string.IsNullOrEmpty(regexAuteur.Match(doc.Text).Groups[1].Value))
            {
                return regexAuteur.Match(doc.Text).Groups[1].Value;
            }

            return string.Empty;
        }
    }
}
