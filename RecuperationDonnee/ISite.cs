using System.Collections.Generic;

namespace RecuperationDonnee
{
    /// <summary>
    /// interface des méthodes a implémenter pour chaque site
    /// </summary>
    public interface ISite
    {
        /// <summary>
        /// Retourne la liste de novel du site internet
        /// </summary>
        /// <returns>liste de novel</returns>
        IEnumerable<Novel> RecuperationListeNovel();

        /// <summary>
        /// Retoune la liste de chaptitre sur la page de lien
        /// </summary>
        /// <param name="lienPagechapitre">lien ou les chapitres sont référencé</param>
        /// <returns>liste de chapitre</returns>
        IEnumerable<Chapitre> RecuperationListeChapitre(string lienPagechapitre);

        /// <summary>
        /// Lien de récupération des novels
        /// </summary>
        string LienRecuperationNovel { get; }

        /// <summary>
        /// Récupére le texte du chaptitre Html
        /// </summary>
        /// <param name="lienChapitre">lien du chaptre</param>
        /// <param name="html">True si on veut récupérer avec les balises HTLML sinon False</param>
        /// <returns>text du chapitre html sans formatage</returns>
        string RecuperationChapitre(string lienChapitre, bool html);

        /// <summary>
        /// Récupère les informations du novel
        /// </summary>
        /// <param name="lienPageIntroduction"></param>
        /// <returns></returns>
        InformationNovel RecupererInformationNovel(string lienPageIntroduction);

        SiteEnum siteEnum { get; }
    }
}
