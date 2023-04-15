using NovelEvolue.Entite;
using RecuperationDonnee;

namespace NovelEvolue;

public class FlyoutPageItem
{
    public string Title { get; set; }
    public string IconSource { get; set; }

    public SiteEnum? SiteType { get; set; }

    /// <summary>
    /// Action a effectuer
    /// </summary>
    public ActionEnum Action { get; set; }
}
