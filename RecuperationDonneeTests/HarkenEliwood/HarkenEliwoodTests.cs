using System;
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
            Assert.AreEqual(11, listeNovel.Count());
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
            new HarkenEliwood().RecuperationChapitre("https://harkeneliwood.wordpress.com/2017/04/17/shikkaku-mon-no-saikyou-kenja-episode-1/", true);
        }


        [TestMethod]
        public void RecupererInformationNovelTest()
        {
            List<string> listeInfos = new();
            IEnumerable<Novel> listeNovel = new HarkenEliwood().RecuperationListeNovel();
            foreach (Novel novel in listeNovel)
            {
                InformationNovel infos = new HarkenEliwood().RecupererInformationNovel(novel.LientHtmlSommaire);

                Assert.IsFalse(string.IsNullOrEmpty(infos.Auteur), novel.LientHtmlSommaire);
                if (novel.LientHtmlSommaire != "https://harkeneliwood.wordpress.com/kidnappe-par-les-dieux/")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(infos.LienImage), novel.LientHtmlSommaire);
                }
                // Il n'y a pas de traducteur sur la fiche
                //Assert.IsFalse(string.IsNullOrEmpty(infos.TraducteurFR), novel.LientHtmlSommaire);
                Assert.IsFalse(string.IsNullOrEmpty(infos.Resume), novel.LientHtmlSommaire);


                //Permet l'affichage des infos de tous les novels
                /* listeInfos.Add(string.Format("Lien du sommaire : {0}", novel.LientHtmlSommaire));
                 listeInfos.Add(string.Format("Auteur : {0}", infos.Auteur));
                 listeInfos.Add(string.Format("LienImage : {0}", infos.LienImage));
                 listeInfos.Add(string.Format("Traducteur Français : {0}", infos.TraducteurFR));
                 listeInfos.Add(string.Format("Résume : {0}", infos.Resume));
                 listeInfos.Add(string.Empty);
                 listeInfos.Add(string.Empty);*/
            }
            if (listeInfos.Any())
            {
                Assert.Fail(string.Join(Environment.NewLine, listeInfos));
            }
        }
    }
}