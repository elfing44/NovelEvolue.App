﻿using System.Collections.Generic;
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
            IEnumerable<Novel> listeNovel = new Chireads().RecuperationListeNovel();
            Assert.AreEqual(81, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new Chireads().RecuperationListeChapitre(novel.LientHtmlSommaire);
                foreach (Chapitre chapitre in listechapitre)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(chapitre.Libelle));
                }
            }
        }

        [TestMethod()]
        public void RecuperationListeChapitreTest()
        {
            IEnumerable<Chapitre> listeChapitre = new Chireads().RecuperationListeChapitre(@"https://chireads.com/category/translatedtales/dragon-marked-war-god/");
            Assert.AreEqual(194, listeChapitre.Count());
        }
    }
}