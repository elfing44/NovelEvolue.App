using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace RecuperationDonnee.NovelDeGlace
{
    /// <summary>
    /// Récupération des infos du site novel de glace
    /// </summary>
    public class NovelDeGlace : ISite
    {
        public string LienRecuperationNovel => @"https://noveldeglace.com/roman/";
        public SiteEnum siteEnum { get => SiteEnum.NovelDeGlace; }

        public string RecuperationChapitre(string lienChapitre, bool html)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienChapitre);
            var listeBaliseDivTome = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content-chapitre chapter-inner chapter-content']/div[@class='content-tome']");
            if (listeBaliseDivTome == null)
            {
                listeBaliseDivTome = doc.GetElementbyId("content").SelectNodes("//div[@class='entry-content-chapitre chapter-inner chapter-content']");
            }
            IEnumerable<HtmlNode> listebalise = listeBaliseDivTome.SelectMany(x => x.ChildNodes);

            List<string> listeParagraphe = new List<string>();

            if (html)
            {
                listeParagraphe.Add("<html>");
                listeParagraphe.Add("<body>");
            }

            bool estAPrendre = false;

            foreach (var balise in listebalise)
            {
                if (balise.Name == "h2")
                    estAPrendre = true;
                if (estAPrendre)
                {
                    if (html)
                        listeParagraphe.Add(balise.OuterHtml);
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
            var contenantListeChapitre = doc.GetElementbyId("wrapper");
            var listeChapitre = contenantListeChapitre.SelectNodes("//div[@class='su-column-inner su-clearfix']/ul/li[@class='chpt']").Nodes().Where(x => x.GetAttributeValue("Href", string.Empty) != string.Empty);
            return listeChapitre.Select(x => new Chapitre() { Libelle = System.Net.WebUtility.HtmlDecode(x.InnerText), LientHtml = x.GetAttributeValue("Href", string.Empty) }).ToList();
        }

        public IEnumerable<Novel> RecuperationListeNovel()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(LienRecuperationNovel);
            var contenantListeNovel = doc.GetElementbyId("content");
            var listeNovel = contenantListeNovel.SelectNodes("//article/div[@class='entry-summary']/div[@class='romans']/div[@class='info-romans']/h2[@class='entry-title']").Descendants().Where(x => x.GetAttributeValue("Href", string.Empty) != string.Empty);
            return listeNovel.Select(x => new Novel() { LientHtmlSommaire = x.GetAttributeValue("Href", string.Empty), Titre = System.Net.WebUtility.HtmlDecode(x.InnerText) }).ToList();
        }

        public InformationNovel RecupererInformationNovel(string lienPageIntroduction)
        {
            InformationNovel informationNovel = new InformationNovel();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(lienPageIntroduction);
            var listeBaliseDivTexte = doc.GetElementbyId("content").SelectNodes("//div[@class='content']");
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
            var image = doc.GetElementbyId("content").SelectNodes("//div[@class='su-column-inner su-clearfix']/img");

            informationNovel.LienImage = image.Select(i => i.GetAttributeValue("src", string.Empty)).FirstOrDefault();
            return informationNovel;
        }
    }
}
