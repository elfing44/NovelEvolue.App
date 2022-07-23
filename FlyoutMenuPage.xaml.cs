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
        ObservableCollection<FlyoutPageItem> listeSite = new ObservableCollection<FlyoutPageItem>()
            {
                new FlyoutPageItem() { Title = "Xiaowaz" , SiteType = SiteEnum.Xiaowaz},
                new FlyoutPageItem() { Title = "Novel de Glace", SiteType = SiteEnum.NovelDeGlace },
                new FlyoutPageItem() { Title = "Chireads", SiteType = SiteEnum.Chireads },
                new FlyoutPageItem() { Title = "Harken Eliwood", SiteType = SiteEnum.HarkenEliwwoof },
                new FlyoutPageItem() { Title = "WuxiaLNScantrad", SiteType = SiteEnum.WuxiaLNScantrad }
            };
        collectionView.ItemsSource = listeSite;
    }
}