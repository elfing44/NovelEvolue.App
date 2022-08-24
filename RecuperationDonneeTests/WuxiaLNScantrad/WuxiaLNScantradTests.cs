using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecuperationDonnee.WuxiaLNScantrad.Tests
{
    [TestClass]
    public class WuxiaLNScantradTests
    {
        [TestMethod]
        public void RecupererInformationNovelTest()
        {
            List<string> listeInfos = new List<string>();
            IEnumerable<Novel> listeNovel = new WuxiaLNScantrad().RecuperationListeNovel();
            foreach (Novel novel in listeNovel)
            {
                InformationNovel infos = new WuxiaLNScantrad().RecupererInformationNovel(novel.LientHtmlSommaire);

                //Assert.IsFalse(string.IsNullOrEmpty(infos.Auteur), novel.LientHtmlSommaire);
                Assert.IsFalse(string.IsNullOrEmpty(infos.LienImage), novel.LientHtmlSommaire);
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
