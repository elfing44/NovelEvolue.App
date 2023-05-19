using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecuperationDonnee.PizzaTranslation.Tests
{
    [TestClass()]
    public class PizzaTranslationTests
    {
        [TestMethod()]
        public void RecuperationListeChapitreTest()
        {
            IEnumerable<Chapitre> listechapitre = new PizzaTranslation().RecuperationListeChapitre("http://piza-translation.e-monsite.com/blog/female-soldier-the-female-soldier-has-been-told-to-infiltrate-the-imperial-magic-academy/");
            Assert.AreEqual(93, listechapitre.Count(), string.Join(Environment.NewLine, listechapitre.Select(x => x.LientHtml + " " + x.Libelle)));
        }

        [TestMethod()]
        public void RecuperationListeNovelTest()
        {
            List<string> listeLienErreur = new();
            IEnumerable<Novel> listeNovel = new PizzaTranslation().RecuperationListeNovel();
            Assert.AreEqual(3, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new PizzaTranslation().RecuperationListeChapitre(novel.LientHtmlSommaire);
                //listeLienErreur.Add(listechapitre.Count() + " - " + novel.LientHtmlSommaire);
                foreach (Chapitre chapitre in listechapitre)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(chapitre.Libelle));
                    try
                    {

                        Assert.IsFalse(string.IsNullOrEmpty(new PizzaTranslation().RecuperationChapitre(chapitre.LientHtml, true)));
                    }
                    catch
                    {
                        listeLienErreur.Add(chapitre.LientHtml);
                    }
                }
            }

            if (listeLienErreur.Any())
            {
                Assert.Fail(string.Join(Environment.NewLine, listeLienErreur));
            }
        }
    }
}