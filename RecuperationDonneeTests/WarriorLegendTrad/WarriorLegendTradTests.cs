using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecuperationDonnee.WarriorLegendTrad.Tests
{
    [TestClass]
    public class WarriorLegendTradTests
    {
        [TestMethod]
        public void RecuperationListeNovelTest()
        {
            List<string> listeLienErreur = new List<string>();
            IEnumerable<Novel> listeNovel = new WarriorLegendTrad().RecuperationListeNovel();
            Assert.AreEqual(9, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new WarriorLegendTrad().RecuperationListeChapitre(novel.LientHtmlSommaire);
                foreach (Chapitre chapitre in listechapitre)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(chapitre.Libelle));
                    try
                    {

                        Assert.IsFalse(string.IsNullOrEmpty(new WarriorLegendTrad().RecuperationChapitre(chapitre.LientHtml, true)));
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
