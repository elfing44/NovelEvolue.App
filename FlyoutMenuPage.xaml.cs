using System.Collections.ObjectModel;

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
                new FlyoutPageItem() { Title = "Xiaowaz" },
                new FlyoutPageItem() { Title = "Novel de Glace" },
                new FlyoutPageItem() { Title = "Chireads" },
                new FlyoutPageItem() { Title = "Harken Eliwood" },
                new FlyoutPageItem() { Title = "WuxiaLNScantrad" }
            };
        collectionView.ItemsSource = listeSite;
    }
}