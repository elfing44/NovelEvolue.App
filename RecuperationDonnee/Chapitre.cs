namespace RecuperationDonnee
{
    /// <summary>
    /// C'est la définition du chapitre par son libellé et son lien internte
    /// </summary>
    public class Chapitre
    {
        /// <summary>
        /// Lien du chapitre
        /// </summary>
        public string LientHtml { get; set; }

        /// <summary>
        /// Libellé du chapitre
        /// </summary>
        public string Libelle { get; set; }


        public string Texte { get; set; }


        public bool Estlu { get; set; }
    }
}
