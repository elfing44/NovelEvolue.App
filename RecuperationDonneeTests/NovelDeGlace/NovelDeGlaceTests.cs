﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecuperationDonnee.NovelDeGlace.Tests
{
    [TestClass]
    public class NovelDeGlaceTests
    {
        [TestMethod()]
        public void RecuperationListeNovelTest()
        {
            IEnumerable<Novel> listeNovel = new NovelDeGlace().RecuperationListeNovel();
            Assert.AreEqual(72, listeNovel.Count());
        }

        [TestMethod()]
        public void RecuperationListeChapitreTest()
        {
            IEnumerable<Chapitre> listeChapitre = new NovelDeGlace().RecuperationListeChapitre(@"https://noveldeglace.com/roman/genjitsushugisha-no-oukokukaizouki/");
            Assert.AreEqual(235, listeChapitre.Count());
        }
    }
}