using System;
using System.Collections.Generic;

namespace RecuperationDonnee
{
    /// <summary>
    /// Définition d'un novel
    /// </summary>
    public class Novel
    {
        public Guid? Id;

        /// <summary>
        /// Lien du sommaire
        /// </summary>
        public string LientHtmlSommaire { get; set; }

        /// <summary>
        /// Titre du novel
        /// </summary>
        public string Titre { get; set; }


        /// <summary>
        /// Liste des chapitres du novel
        /// </summary>
        public List<Chapitre> ListeChapitre { get; set; }

        /// <summary>
        /// Nombre de chapitre du novel
        /// </summary>
        public int NombreChapitre { get; set; }

        /// <summary>
        /// Nombre de chapitre lu du novel
        /// </summary>
        public int NombreChapitreLu { get; set; }
    }
}
