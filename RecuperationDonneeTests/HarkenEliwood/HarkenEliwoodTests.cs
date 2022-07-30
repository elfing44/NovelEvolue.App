using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecuperationDonnee.HarkenEliwood.Tests
{
    [TestClass()]
    public class HarkenEliwoodTests
    {
        [TestMethod]
        public void RecuperationListeNovelTest()
        {
            IEnumerable<Novel> listeNovel = new HarkenEliwood().RecuperationListeNovel();
            Assert.AreEqual(10, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new HarkenEliwood().RecuperationListeChapitre(novel.LientHtmlSommaire);
                foreach (Chapitre chapitre in listechapitre)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(chapitre.Libelle));
                }
            }
        }

        [TestMethod()]
        public void RecuperationListeChapitreTest()
        {
            var chapitre = new HarkenEliwood().RecuperationListeChapitre("https://harkeneliwood.wordpress.com/shikkakumon-no-saikyo-kenja/");
            Assert.AreEqual(354, chapitre.Count());
        }

        [TestMethod()]
        public void RecuperationChapitreTest()
        {
            var chapitre = new HarkenEliwood().RecuperationChapitre("https://harkeneliwood.wordpress.com/2017/04/17/shikkaku-mon-no-saikyou-kenja-episode-1/", true);
        }
    }
}