using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecuperationDonnee.NovelDeGlace.Tests
{
    [TestClass]
    public class NovelDeGlaceTests
    {
        [TestMethod]
        public void RecuperationListeNovelTest()
        {
            IEnumerable<Novel> listeNovel = new NovelDeGlace().RecuperationListeNovel();
            Assert.AreEqual(73, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new NovelDeGlace().RecuperationListeChapitre(novel.LientHtmlSommaire);
                foreach (Chapitre chapitre in listechapitre)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(chapitre.Libelle));
                }
            }
        }

        [TestMethod()]
        public void RecuperationListeChapitreTest()
        {
            IEnumerable<Chapitre> listeChapitre = new NovelDeGlace().RecuperationListeChapitre(@"https://noveldeglace.com/roman/genjitsushugisha-no-oukokukaizouki/");
            Assert.AreEqual(242, listeChapitre.Count());
        }

        [TestMethod]
        public void RecupererInformationNovelTest()
        {
            //https://noveldeglace.com/roman/monster-no-goshujin-sama-wn/
            List<string> listeInfos = new();
            IEnumerable<Novel> listeNovel = new NovelDeGlace().RecuperationListeNovel();
            foreach (Novel novel in listeNovel)
            {
                if (novel.LientHtmlSommaire != "https://noveldeglace.com/roman/monster-no-goshujin-sama-wn/")
                {
                    try
                    {
                        InformationNovel infos = new NovelDeGlace().RecupererInformationNovel(novel.LientHtmlSommaire);

                        //Assert.IsFalse(string.IsNullOrEmpty(infos.Auteur), novel.LientHtmlSommaire);
                        //Assert.IsFalse(string.IsNullOrEmpty(infos.LienImage), novel.LientHtmlSommaire);
                        // Il n'y a pas de traducteur sur la fiche
                        //Assert.IsFalse(string.IsNullOrEmpty(infos.TraducteurFR), novel.LientHtmlSommaire);
                        //Assert.IsFalse(string.IsNullOrEmpty(infos.Resume), novel.LientHtmlSommaire);


                        //Permet l'affichage des infos de tous les novels
                        listeInfos.Add(string.Format("Lien du sommaire : {0}", novel.LientHtmlSommaire));
                        listeInfos.Add(string.Format("Auteur : {0}", infos.Auteur));
                        listeInfos.Add(string.Format("LienImage : {0}", infos.LienImage));
                        listeInfos.Add(string.Format("Traducteur Français : {0}", infos.TraducteurFR));
                        listeInfos.Add(string.Format("Résume : {0}", infos.Resume));
                        listeInfos.Add(string.Empty);
                        listeInfos.Add(string.Empty);
                    }
                    catch (Exception e)
                    {
                        listeInfos.Add(string.Format("Lien du sommaire : {0}", novel.LientHtmlSommaire));
                        listeInfos.Add(e.Message);
                        listeInfos.Add(string.Empty);
                        listeInfos.Add(string.Empty);
                    }
                }
            }
            if (listeInfos.Any())
            {
                Assert.Fail(string.Join(Environment.NewLine, listeInfos));
            }
        }
    }
}