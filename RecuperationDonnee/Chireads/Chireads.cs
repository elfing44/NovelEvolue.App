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

        public List<string> listeLienARetirer = new List<string>()
        {
            "https://chireads.com/translatedtales/le-monde-des-mages/passage-a-1-chap-jour-jusqua-nouvel-ordre-et-bonus-sur-tipeee/2021/05/21/",
            "https://chireads.com/translatedtales/la-renaissance-dun-maitre-demoniaque/en-raison-dun-probleme-technique-deathmanorangel-ne-pourra-pas-publier-de-chapitre-cette-semaine-nous-nous-excusons-pour-la-gene-occasionnee/2020/09/15/",
            "https://chireads.com/translatedtales/le-rythme-de-publication-de-la-renaissance-du-plus-puissant-des-dieux-epeistes-passe-a-3-chapitres-par-semaine-a-partir-de-lundi-prochain-20-janvier-bonne-lecture-a-tous/2020/01/18/",
            "https://chireads.com/translatedtales/et-une-nouvelle-semaine-de-folie-avec-zhang-xuan-et-sa-voie-celeste/2019/06/19/",
            "https://chireads.com/translatedtales/cadeau-de-noel-pour-la-voie-celeste-3-chapitres-par-jour-a-partir-de-demain-et-ce-jusqua-lundi-prochain-joyeux-noel-a-tous/2019/12/23/",
            "https://chireads.com/translatedtales/le-rythme-de-publication-de-la-voie-celeste-passe-a-1-chapitre-par-jour-a-partir-de-ce-samedi-11-janvier-bonne-lecture-a-tous/2020/01/09/",
            "https://chireads.com/translatedtales/martial-god-asura/bonjour-a-tous-merci-pour-votre-soutien-jusquici-sur-ce-novel-jusqua-fin-aout-le-rythme-de-parution-sera-legerement-ralenti-en-passant-a-1-chapitre-par-semaine-tout-redeviendra/2020/07/23/",
            "https://chireads.com/translatedtales/joyaux-celestes-heavenly-jewel-change/nouveaux-chapitres-egalement-dispo-sur-tipeee-merci-pour-votre-participation-indispensable-a-la-continuite-du-projet/2021/06/23/",
            "https://chireads.com/translatedtales/lavatar-du-roi-expert-omniclasses-the-kings-avatar/bonjour-a-tous-en-raison-dune-baisse-de-rythme-de-geo-durant-le-mois-daout-lavatar-du-roi-ne-sera-publie-que-les-samedi-a-17h-un-chapitre-par-semaine-ce-mois-puis-repris/2020/08/10/",
            "https://chireads.com/translatedtales/je-scellerai-les-cieux/chapitre-104-un-grand-vent-seleve-le-rokh-etend-ses-ailes-2/2020/04/10/",
            "https://chireads.com/translatedtales/battle-through-the-heavens/le-rythme-de-publication-de-bataille-a-travers-les-cieux-passe-a-3-chapitres-par-semaine-les-jours-de-publication-sont-les-lundi-mercredi-et-vendredi-a-partir-de-ce-vendredi-3-avr/2020/04/02/",
            "https://chireads.com/translatedtales/exceptionnellement-un-seul-chapitre-par-jour-ce-week-end-merci-pour-votre-comprehension/2019/06/14/",
            "https://chireads.com/translatedtales/chapitres-bonus-sur-tipeee-2/2020/02/25/",
            "https://chireads.com/translatedtales/liberez-la-sorciere-release-that-witch/4-chapitres-supplementaires-sont-de-nouveau-disponibles-sur-tipeee-en-avant-premiere-bonne-lecture-a-tous/2020/09/29/",
            "https://chireads.com/translatedtales/carte-du-monde-de-roland/2019/04/02/",
        };


        public string RecuperationChapitre(string lienChapitre, bool html)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienChapitre);
            doc.LoadHtml(doc.Text.Replace("</i> <i>", " "));
            doc.LoadHtml(doc.Text.Replace("</strong> <strong>", " "));

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
                return contenantListeChapitre.Select(x =>
                new Chapitre()
                {
                    LientHtml = x.GetAttributeValue("Href", string.Empty),
                    Libelle = System.Net.WebUtility.HtmlDecode(x.GetAttributeValue("title", string.Empty))
                }).Where(x => !listeLienARetirer.Contains(x.LientHtml));
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
            doc.LoadHtml(doc.Text.Replace("</i> <i>", " "));
            var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//div[@class='inform-inform-txt']/div[@class='inform-txt-show font-color-black6']");

            List<string> listeParagraphe = new List<string>();

            foreach (var balise in listeBaliseDivTexte.SelectMany(x => x.ChildNodes).Where(x => !string.IsNullOrEmpty(x.InnerHtml)))
            {
                listeParagraphe.Add(balise.InnerHtml);
            }

            informationNovel.Resume = string.Join(Environment.NewLine, listeParagraphe).Replace("<br>", Environment.NewLine).Trim(Environment.NewLine.ToArray());
            var image = doc.GetElementbyId("content").SelectNodes("//div[@class='inform-product']/img");
            Regex regexAuteur = new Regex("Auteur : (.*?)&nbsp;");
            informationNovel.Auteur = regexAuteur.Match(doc.Text).Groups[1].Value;

            Regex regexTraducteur = new Regex("Fantrad : (.*?)&nbsp;");
            informationNovel.TraducteurFR = regexTraducteur.Match(doc.Text).Groups[1].Value;
            if (string.IsNullOrEmpty(informationNovel.TraducteurFR))
            {
                regexTraducteur = new Regex("Traducteur : (.*?)&nbsp;");
                informationNovel.TraducteurFR = regexTraducteur.Match(doc.Text).Groups[1].Value;
            }
            informationNovel.LienImage = image.Select(i => i.GetAttributeValue("src", string.Empty)).FirstOrDefault();
            return informationNovel;
        }
    }
}