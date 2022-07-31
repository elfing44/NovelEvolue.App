using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace NovelEvolue.BDD
{
    public class ChapitreBDD
    {
        [ForeignKey(nameof(NovelBDD))]
        public string NovelLientHtmlSommaire { get; set; }

        /// <summary>
        /// Lien du chapitre
        /// </summary>
        [PrimaryKey]
        public string LientHtml { get; set; }

        /// <summary>
        /// Libellé du chapitre
        /// </summary>
        public string Libelle { get; set; }

        public string Texte { get; set; }

        public bool EstLu { get; set; }
    }
}
