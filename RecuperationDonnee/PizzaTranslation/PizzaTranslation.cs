using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RecuperationDonnee.PizzaTranslation
{
    public class PizzaTranslation : ISite
    {
        public string LienRecuperationNovel => "http://piza-translation.e-monsite.com";

        public List<string> listeLienARetirer = new()
        {
            // il y a 2 versions
            "http://piza-translation.e-monsite.com/blog/female-soldier-the-female-soldier-has-been-told-to-infiltrate-the-imperial-magic-academy/female-soldier-ch-82-festival-de-l-ecole-partie-6.html",
        };


        public SiteEnum SiteEnum => SiteEnum.PizzaTranslaion;

        public string RecuperationChapitre(string lienChapitre, bool html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(RecuperationText(lienChapitre).Result);
            var contenantListeP = doc.GetElementbyId("main").SelectNodes("//div[@class='content']/p");

            List<string> listeParagraphe = new();

            if (html)
            {
                listeParagraphe.Add("<html>");
                listeParagraphe.Add("<body>");
            }

            foreach (var balise in contenantListeP)
            {
                if (!balise.OuterHtml.Contains("style=\"margin-bottom:9px\""))
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
            return string.Join(Environment.NewLine, listeParagraphe).Trim();
        }

        public IEnumerable<Chapitre> RecuperationListeChapitre(string lienPagechapitre)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(RecuperationText(lienPagechapitre).Result);
            var contenantListeChapitre = doc.GetElementbyId("wrapper").SelectNodes("//p[@class='media-heading']/a");
            return contenantListeChapitre.Select(x =>
                           new Chapitre()
                           {
                               LientHtml = x.GetAttributeValue("Href", string.Empty),
                               Libelle = x.InnerText.Trim()
                           }).Reverse().Where(x => !listeLienARetirer.Contains(x.LientHtml));
        }

        public IEnumerable<Novel> RecuperationListeNovel()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(RecuperationText(LienRecuperationNovel).GetAwaiter().GetResult());
            var contenantListeNovel = doc.GetElementbyId("widget-1").SelectNodes("//div[@id='widget-1']/div/div/a");
            foreach (var span in contenantListeNovel.Descendants("span").ToList())
            {
                span.Remove();
            }
            return contenantListeNovel.Select(x =>
               new Novel()
               {
                   LientHtmlSommaire = x.GetAttributeValue("Href", string.Empty),
                   Titre = x.InnerText.Replace("\n", string.Empty).Trim()
               });
        }

        private async Task<string> RecuperationText(string lien)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(lien, HttpCompletionOption.ResponseContentRead, new CancellationToken(false)).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync();
        }

        public InformationNovel RecupererInformationNovel(string lienPageIntroduction)
        {
            throw new NotImplementedException();
        }
    }
}
