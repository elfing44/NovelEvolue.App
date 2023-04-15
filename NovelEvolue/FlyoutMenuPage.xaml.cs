using System.Collections.ObjectModel;
using NovelEvolue.Entite;
using RecuperationDonnee;

namespace NovelEvolue;

public partial class FlyoutMenuPage : ContentPage
{
    public FlyoutMenuPage()
    {
        InitializeComponent();
        AlimentaterListeSite();
    }

    private void AlimentaterListeSite()
    {
        ObservableCollection<FlyoutPageItem> listeSite = new()
            {
                new FlyoutPageItem() { Title = "Xiaowaz" , SiteType = SiteEnum.Xiaowaz, IconSource="xiaowaz.png", Action = ActionEnum.OuvrirListeNovel },
                new FlyoutPageItem() { Title = "Novel de Glace", SiteType = SiteEnum.NovelDeGlace, IconSource="novelglace.png", Action = ActionEnum.OuvrirListeNovel  },
                new FlyoutPageItem() { Title = "Chireads", SiteType = SiteEnum.Chireads, IconSource="chireads.png", Action = ActionEnum.OuvrirListeNovel  },
                new FlyoutPageItem() { Title = "Harken Eliwood", SiteType = SiteEnum.HarkenEliwwoof, IconSource="harkeneliwood.png", Action = ActionEnum.OuvrirListeNovel  },
                new FlyoutPageItem() { Title = "WuxiaLNScantrad", SiteType = SiteEnum.WuxiaLNScantrad, IconSource="wuxialn.png", Action = ActionEnum.OuvrirListeNovel  },
                new FlyoutPageItem() { Title = "Warrior Legend Trad", SiteType = SiteEnum.WarriorLegendTrad,IconSource="warirorlegendtrad.png", Action = ActionEnum.OuvrirListeNovel  },
                new FlyoutPageItem() { Title = "Exporter", IconSource = "export.png", Action = ActionEnum.ExporterBaseDonner }
            };
        collectionView.ItemsSource = listeSite;
    }
}