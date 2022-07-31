using RecuperationDonnee;
using SQLite;

namespace NovelEvolue.BDD
{
    public class NovelBDD
    {
        /// <summary>
        /// Lien du sommaire
        /// </summary>
        [PrimaryKey]
        public string LientHtmlSommaire { get; set; }

        /// <summary>
        /// Titre du novel
        /// </summary>
        public string Titre { get; set; }

        /// <summary>
        /// site
        /// </summary>
        public SiteEnum Site { get; set; }

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
