using System.Collections.ObjectModel;
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
                new FlyoutPageItem() { Title = "Xiaowaz" , SiteType = SiteEnum.Xiaowaz, IconSource="xiaowaz.png"},
                new FlyoutPageItem() { Title = "Novel de Glace", SiteType = SiteEnum.NovelDeGlace, IconSource="novelglace.png" },
                new FlyoutPageItem() { Title = "Chireads", SiteType = SiteEnum.Chireads, IconSource="chireads.png" },
                new FlyoutPageItem() { Title = "Harken Eliwood", SiteType = SiteEnum.HarkenEliwwoof, IconSource="harkeneliwood.png" },
                new FlyoutPageItem() { Title = "WuxiaLNScantrad", SiteType = SiteEnum.WuxiaLNScantrad, IconSource="wuxialn.png" },
                new FlyoutPageItem() { Title = "Warrior Legend Trad", SiteType = SiteEnum.WarriorLegendTrad,IconSource="warirorlegendtrad.png" }
            };
        collectionView.ItemsSource = listeSite;
    }
}