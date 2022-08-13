using System;
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
            Assert.AreEqual("MWLU : Prologue", listeChapitre.First().Libelle);
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
            List<string> listeLienErreur = new List<string>();
            IEnumerable<Novel> listeNovel = new Xiaowaz().RecuperationListeNovel();
            Assert.AreEqual(21, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new Xiaowaz().RecuperationListeChapitre(novel.LientHtmlSommaire);
                foreach (Chapitre chapitre in listechapitre)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(chapitre.Libelle));
                    try
                    {

                        new Xiaowaz().RecuperationChapitre(chapitre.LientHtml, true);
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

        [TestMethod]
        public void RecupererInformationNovelTest()
        {
            List<string> listeInfos = new List<string>();
            IEnumerable<Novel> listeNovel = new Xiaowaz().RecuperationListeNovel();
            foreach (Novel novel in listeNovel)
            {
                InformationNovel infos = new Xiaowaz().RecupererInformationNovel(novel.LientHtmlSommaire);

                if (novel.LientHtmlSommaire != "https://xiaowaz.fr/articles/category/douluo-dalu/"
                    && novel.LientHtmlSommaire != "https://xiaowaz.fr/series-abandonnees/la-porte-de-la-chance/")
                {
                    Assert.IsFalse(string.IsNullOrEmpty(infos.Auteur), novel.LientHtmlSommaire);
                    Assert.IsFalse(string.IsNullOrEmpty(infos.LienImage), novel.LientHtmlSommaire);
                    if (novel.LientHtmlSommaire != "https://xiaowaz.fr/series-en-cours/defier-les-neuf-cieux/"
                        && novel.LientHtmlSommaire != "https://xiaowaz.fr/series-en-cours/tales-of-demons-and-gods/"
                        && novel.LientHtmlSommaire != "https://xiaowaz.fr/series-abandonnees/shuras-wrath/"
                        && novel.LientHtmlSommaire != "https://xiaowaz.fr/series-abandonnees/wu-dong-qian-kun/"
                        && !novel.LientHtmlSommaire.Contains("oeuvres-originales"))
                    {

                        Assert.IsFalse(string.IsNullOrEmpty(infos.TraducteurFR), novel.LientHtmlSommaire);
                    }
                    Assert.IsFalse(string.IsNullOrEmpty(infos.Resume), novel.LientHtmlSommaire);
                }

                /*Permet l'affichage des infos de tous les novels
                listeInfos.Add(string.Format("Lien du sommaire : {0}", novel.LientHtmlSommaire));
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

        [TestMethod]
        public void RecuperationChapitreTestEER104()
        {
            new Xiaowaz().RecuperationChapitre("https://xiaowaz.fr/articles/eer-chapitre-104-bonus/", true);
        }
    }
}