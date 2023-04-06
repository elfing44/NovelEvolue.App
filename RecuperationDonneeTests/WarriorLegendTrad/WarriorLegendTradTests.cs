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
            List<string> listeLienErreur = new();
            IEnumerable<Novel> listeNovel = new WarriorLegendTrad().RecuperationListeNovel();
            Assert.AreEqual(11, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new WarriorLegendTrad().RecuperationListeChapitre(novel.LientHtmlSommaire);
                //listeLienErreur.Add(listechapitre.Count() + " - " + novel.LientHtmlSommaire);
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

        [TestMethod]
        public void RecuperationListeChapitreTestReincarnated()
        {
            IEnumerable<Chapitre> listechapitre = new WarriorLegendTrad().RecuperationListeChapitre("https://warriorlegendtrad.fr/2021/10/17/i-reincarnated-for-nothing/");
            Assert.AreEqual(130, listechapitre.Count());
        }

        [TestMethod]
        public void RecuperationListeChapitreTestTaming()
        {

            IEnumerable<Chapitre> listechapitre = new WarriorLegendTrad().RecuperationListeChapitre("https://warriorlegendtrad.fr/2021/11/30/taming-master/");
            Assert.AreEqual(35, listechapitre.Count());
        }

        [TestMethod]
        public void RecuperationListeChapitreTestPoisson()
        {

            IEnumerable<Chapitre> listechapitre = new WarriorLegendTrad().RecuperationListeChapitre("https://warriorlegendtrad.fr/2022/07/12/jetais-un-petit-poisson-quand-je-me-suis-reincarne-mais-il-semble-que-je-puisse-devenir-un-dragon-alors-je-ferai-de-mon-mieux/");
            Assert.AreEqual(25, listechapitre.Count());
        }

        [TestMethod]
        public void RecupererInformationNovelTest()
        {
            List<string> listeInfos = new();
            IEnumerable<Novel> listeNovel = new WarriorLegendTrad().RecuperationListeNovel();
            foreach (Novel novel in listeNovel)
            {
                InformationNovel infos = new WarriorLegendTrad().RecupererInformationNovel(novel.LientHtmlSommaire);

                Assert.IsFalse(string.IsNullOrEmpty(infos.Auteur), novel.LientHtmlSommaire);
                Assert.IsFalse(string.IsNullOrEmpty(infos.LienImage), novel.LientHtmlSommaire);
                // Il n'y a pas de traducteur sur la fiche
                //Assert.IsFalse(string.IsNullOrEmpty(infos.TraducteurFR), novel.LientHtmlSommaire);
                Assert.IsFalse(string.IsNullOrEmpty(infos.Resume), novel.LientHtmlSommaire);


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
    }
}
