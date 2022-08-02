using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecuperationDonnee.Chireads.Tests
{
    [TestClass()]
    public class ChireadsTests
    {
        [TestMethod]
        public void RecuperationListeNovelTest()
        {
            List<string> listeLienErreur = new List<string>();
            IEnumerable<Novel> listeNovel = new Chireads().RecuperationListeNovel();
            Assert.AreEqual(81, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new Chireads().RecuperationListeChapitre(novel.LientHtmlSommaire);
                foreach (Chapitre chapitre in listechapitre)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(chapitre.Libelle));
                    try
                    {

                        new Chireads().RecuperationChapitre(chapitre.LientHtml, true);
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

        [TestMethod()]
        public void RecuperationListeChapitreTest()
        {
            IEnumerable<Chapitre> listeChapitre = new Chireads().RecuperationListeChapitre(@"https://chireads.com/category/translatedtales/dragon-marked-war-god/");
            Assert.AreEqual(195, listeChapitre.Count());
        }
    }
}