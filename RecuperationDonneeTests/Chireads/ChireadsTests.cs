using System;
using System.Collections.Generic;
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
            List<string> listeLienErreur = new List<string>();
            IEnumerable<Novel> listeNovel = new Chireads().RecuperationListeNovel();
            Assert.AreEqual(81, listeNovel.Count());
            foreach (Novel novel in listeNovel)
            {
                IEnumerable<Chapitre> listechapitre = new Chireads().RecuperationListeChapitre(novel.LientHtmlSommaire);
                foreach (Chapitre chapitre in listechapitre)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(chapitre.Libelle));
                    try
                    {

                        new Chireads().RecuperationChapitre(chapitre.LientHtml, true);
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

        [TestMethod()]
        public void RecuperationListeChapitreTest()
        {
            IEnumerable<Chapitre> listeChapitre = new Chireads().RecuperationListeChapitre(@"https://chireads.com/category/translatedtales/dragon-marked-war-god/");
            Assert.AreEqual(195, listeChapitre.Count());
        }

        [TestMethod]
        public void RecupererInformationNovelTest()
        {
            InformationNovel infos = new Chireads().RecupererInformationNovel("https://chireads.com/category/translatedtales/la-voie-celeste/");

            Assert.AreEqual("横扫天涯", infos.Auteur);
            Assert.AreEqual("", infos.TraducteurFR);
            Assert.AreEqual("https://chireads.com/wp-content/uploads/2020/04/天道图书馆-1.jpg", infos.LienImage);
            Assert.AreEqual(@"Venu d’une autre époque, Zhang Xuan, un simple bibliothécaire, se retrouve dans la peau d’un professeur dépressif dont la réputation reste à faire. 
Sa mission serait compromise si, à son arrivée dans ce monde, il n'avait reçu le don de faire apparaître dans son esprit une incroyable bibliothèque.
 
En effet, tout ce qu’il rencontre, fut-ce un être humain, un animal ou un objet, est immédiatement recensé dans un livre qui lui en indique tous les défauts et les points faibles. 
Grâce à ce don, il va pouvoir guider au mieux ses élèves et faire de simples étudiants les plus grands experts au monde.

Light novel Library of Heaven’s Path en français /Traduction de Library of Heaven’s Path en Français / Library of Heaven’s Path Fr ", infos.Resume);
        }
    }
}