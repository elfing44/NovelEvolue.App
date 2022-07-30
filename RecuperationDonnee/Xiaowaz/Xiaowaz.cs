using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace RecuperationDonnee.Xiaowaz
{
    public class Xiaowaz : ISite
    {
        private const string _idElementListe = "lcp_instance_0";

        private const string _classeSerie = "page_item page-item-866 page_item_has_children";

        private const string _classeCreation = "page_item page-item-13731 page_item_has_children";

        private const string _classeAbandonner = "page_item page-item-2504 page_item_has_children";

        public string LienRecuperationNovel => @"https://xiaowaz.fr/";

        public SiteEnum siteEnum { get => SiteEnum.Xiaowaz; }

        private const string _debutIdSpan = "more-";

        /// <summary>
        /// Retoune la liste de chaptitre sur la page de lien
        /// </summary>
        /// <param name="lienPagechapitre">lien ou les chapitres sont référencé</param>
        /// <returns>liste de chapitre</returns>
        public IEnumerable<Chapitre> RecuperationListeChapitre(string lienPagechapitre)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienPagechapitre);
            // Récupération de l'élément qui a pour id : lcp_instance_0 qui contient la liste de tous les chapitres
            List<Chapitre> listeChapitre = new List<Chapitre>();

            var listeNodesChapitre = doc.GetElementbyId(_idElementListe);
            if (listeNodesChapitre != null)
            {
                listeChapitre.AddRange(listeNodesChapitre.ChildNodes.Select(x => x.Element("a")).Select(x => new Chapitre() { Libelle = RemplacerEspaceIncecable(x.GetAttributeValue("title", string.Empty)), LientHtml = x.GetAttributeValue("Href", string.Empty) }));
            }
            else
            {
                var test = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']/p/a");

                if (test != null)
                {
                    listeChapitre.AddRange(test.Where(x => x.GetAttributeValue("Href", string.Empty) != string.Empty && x.GetAttributeValue("rel", string.Empty) == string.Empty).Select(x => new Chapitre() { Libelle = RemplacerEspaceIncecable(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) }));
                    if (!listeChapitre.Any())
                    {
                        listeChapitre.AddRange(test.Where(x => x.GetAttributeValue("Href", string.Empty) != string.Empty).Select(x => new Chapitre() { Libelle = RemplacerEspaceIncecable(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) }));
                    }
                }

                if (!listeChapitre.Any() && doc.GetElementbyId("content").ChildNodes.Elements("div").Select(x => x.Elements("p")).LastOrDefault() != null)
                {
                    listeChapitre.AddRange(doc.GetElementbyId("content").ChildNodes.Elements("div").Select(x => x.Elements("p")).Last().SelectMany(x => x.ChildNodes).Where(x => x.GetAttributeValue("Href", string.Empty) != string.Empty && x.GetAttributeValue("rel", string.Empty) == string.Empty).Select(x => new Chapitre() { Libelle = RemplacerEspaceIncecable(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) }));
                }
            }

            if (!listeChapitre.Any() && doc.GetElementbyId("content").ChildNodes.Elements("div").Select(x => x.Elements("p")).LastOrDefault() != null)
            {
                listeChapitre = doc.GetElementbyId("content").ChildNodes.Elements("div").Select(x => x.Elements("p")).Last().SelectMany(x => x.ChildNodes).Where(x => x.GetAttributeValue("Href", string.Empty) != string.Empty).Select(x => new Chapitre() { Libelle = RemplacerEspaceIncecable(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) }).ToList();
            }

            listeChapitre.AddRange(RecuperationListeChapitreNonSommaire(listeChapitre.LastOrDefault()));

            return listeChapitre;
        }

        /// <summary>
        /// Récupére les chapitres qui ne sont pas encore dans le somaire
        /// </summary>
        /// <param name="dernierChapitre">dernier chapitre du sommaire</param>
        /// <returns>liste de chapitre qui ne sont pas dans le sommaire</returns>
        private static List<Chapitre> RecuperationListeChapitreNonSommaire(Chapitre dernierChapitre)
        {
            HtmlWeb web = new HtmlWeb();
            bool existeUnChapitreSuivant = true;
            List<Chapitre> listeChapitre = new List<Chapitre>();
            if (dernierChapitre != null)
            {
                string lienChapitreSUivant = dernierChapitre.LientHtml;
                while (existeUnChapitreSuivant)
                {
                    HtmlDocument doc = web.Load(lienChapitreSUivant);
                    var elementNavigationSuivant = doc.GetElementbyId("content").SelectNodes("//*[@class='wp-post-navigation-next']");
                    if (elementNavigationSuivant != null && elementNavigationSuivant.First() != null && elementNavigationSuivant.First().Element("a") != null)
                    {
                        lienChapitreSUivant = elementNavigationSuivant.First().Element("a").GetAttributeValue("Href", string.Empty);
                        if (lienChapitreSUivant != string.Empty)
                        {
                            listeChapitre.Add(new Chapitre() { LientHtml = lienChapitreSUivant, Libelle = MiseEnFormeTexteChapitre(elementNavigationSuivant.First().InnerText) });
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

        /// <summary>
        /// Retourne la liste de novel du site internet
        /// </summary>
        /// <param name="lienPageInternet">lien du site internet</param>
        /// <returns>liste de novel</returns>
        public IEnumerable<Novel> RecuperationListeNovel()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(LienRecuperationNovel);
            var entetePage = doc.GetElementbyId("primary-menu");
            // Récupération des séries
            var elementOngletSerie = entetePage.Descendants().Where(x => x.GetAttributeValue("class", string.Empty) == _classeSerie || x.GetAttributeValue("class", string.Empty) == _classeCreation || x.GetAttributeValue("class", string.Empty) == _classeAbandonner);
            var elementDansOngletSerie = elementOngletSerie.Select(x => x.Element("ul")).SelectMany(x => x.ChildNodes);
            var listeSerie = elementDansOngletSerie.Select(x => x.Element("a")).Where(x => x != null);
            return listeSerie.Select(x => new Novel() { LientHtmlSommaire = x.GetAttributeValue("href", string.Empty), Titre = RemplacerEspaceIncecable(x.InnerText) });
        }

        private static string RemplacerEspaceIncecable(string text)
        {
            return text.Replace("&nbsp;", " ");
        }

        private static string MiseEnFormeTexteChapitre(string text)
        {
            return text.Replace(@"\t;", "").Replace(@"\n", "").Trim();
        }

        public string RecuperationChapitre(string lienChapitre, bool html)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienChapitre);
            var listeBaliseP = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']/p").Where(x => x.GetAttributeValue("class", string.Empty) != "post-meta");

            // le more-9 correpond au chapitre 1 de TDG je part du principe qu'il n'y a pas d'autre chapitre avec la même ID 
            // sur le chapitre 1 de TDG le span et au début du chapitre et pas au dessus du chapitre
            bool contientUnSpanMore = listeBaliseP.Where(x => x.FirstChild.Id.Contains(_debutIdSpan)).Any() && !listeBaliseP.Where(x => x.FirstChild.Id.Equals(_debutIdSpan + "9")).Any();

            List<string> listeParagraphe = new List<string>();

            if (html)
            {
                listeParagraphe.Add("<html>");
                listeParagraphe.Add("<body>");
            }

            //Récuperer le titre caché :
            var baliseSpanTitre = doc.GetElementbyId("content").SelectNodes("//div[@id='content']/div[@class='entry-content']/div[@class='entry-content-inner']/span[@class='blur_click']");

            if (baliseSpanTitre != null && baliseSpanTitre.Any())
            {
                if (html)
                    listeParagraphe.Add("<h1>" + baliseSpanTitre[0].InnerText + "</h1>");
                else
                    listeParagraphe.Add(baliseSpanTitre[0].InnerText);
            }


            bool prendreElement = !contientUnSpanMore;
            foreach (var baliseP in listeBaliseP)
            {
                if (baliseP.FirstChild.Id.Contains(_debutIdSpan))
                    prendreElement = true;
                if (prendreElement && !baliseP.InnerText.Equals("&nbsp;") && !string.IsNullOrWhiteSpace(baliseP.InnerText))
                {
                    if (html)
                        listeParagraphe.Add(baliseP.OuterHtml);
                    else
                        listeParagraphe.Add(baliseP.InnerText);
                }
            }

            if (html)
            {
                listeParagraphe.Add("</body>");
                listeParagraphe.Add("</html>");
            }

            return string.Join(Environment.NewLine, listeParagraphe);
        }

        public InformationNovel RecupererInformationNovel(string lienPageIntroduction)
        {
            InformationNovel informationNovel = new InformationNovel();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienPageIntroduction);
            var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']");
            List<string> listeParagraphe = new List<string>();

            listeParagraphe.Add("<html>");
            listeParagraphe.Add("<body>");


            foreach (var balise in listeBaliseDivTexte.SelectMany(x => x.ChildNodes))
            {
                if (balise.OuterHtml.StartsWith("<p"))
                {
                    if (balise.OuterHtml.StartsWith("<p><a href="))
                        break;
                    if (!balise.OuterHtml.Contains("<img "))
                        listeParagraphe.Add(balise.OuterHtml);
                }

            }

            listeParagraphe.Add("</body>");
            listeParagraphe.Add("</html>");

            informationNovel.Resume = string.Join(Environment.NewLine, listeParagraphe);
            var image = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']/p/img");
            if (image == null)
                image = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content']/h4/img");
            informationNovel.LienImage = image.Select(i => i.GetAttributeValue("src", string.Empty)).FirstOrDefault();
            return informationNovel;
        }
    }
}
