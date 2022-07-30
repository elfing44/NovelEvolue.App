using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecuperationDonnee.Xiaowaz.Tests
{
    [TestClass]
    public class XiaowazTests
    {
        [TestMethod]
        public void RecuperationListeChapitreTestMonster()
        {
            IEnumerable<Chapitre> listeChapitre = new Xiaowaz().RecuperationListeChapitre(@"https://xiaowaz.fr/series-en-cours/a-monster-who-levels-up/");
            Assert.AreEqual(175, listeChapitre.Count());
        }

        [TestMethod]
        public void RecuperationListeChapitreTestBattle()
        {
            IEnumerable<Chapitre> listeChapitre = new Xiaowaz().RecuperationListeChapitre(@"https://xiaowaz.fr/series-en-cours/battle-through-the-heavens/");
            Assert.AreEqual(616, listeChapitre.Count());
        }

        [TestMethod]
        public void RecuperationListeChapitreTestEveryone()
        {
            IEnumerable<Chapitre> listeChapitre = new Xiaowaz().RecuperationListeChapitre(@"https://xiaowaz.fr/series-en-cours/everyone-else-is-a-returnee/");
            Assert.AreEqual(124, listeChapitre.Count());
        }

        [TestMethod]
        public void RecuperationListeChapitreTestIC()
        {
            IEnumerable<Chapitre> listeChapitre = new Xiaowaz().RecuperationListeChapitre(@"https://xiaowaz.fr/series-en-cours/infinite-competitive-dungeon-society/");
            Assert.AreEqual(353, listeChapitre.Count());
        }

        [TestMethod]
        public void RecuperationListeChapitreTestFuyao()
        {
            IEnumerable<Chapitre> listeChapitre = new Xiaowaz().RecuperationListeChapitre(@"https://xiaowaz.fr/series-en-cours/la-legende-de-fuyao/");
            Assert.AreEqual(14, listeChapitre.Count());
        }

        [TestMethod]
        public void RecuperationListeNovelTest()
        {
            IEnumerable<Novel> listeNovel = new Xiaowaz().RecuperationListeNovel();
            Assert.AreEqual(21, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                new Xiaowaz().RecuperationListeChapitre(novel.LientHtmlSommaire);
            }
        }

        [TestMethod]
        public void RecupererInformationNovelTest()
        {
            InformationNovel info = new Xiaowaz().RecupererInformationNovel(@"https://xiaowaz.fr/series-en-cours/a-monster-who-levels-up/");
        }
    }
}