using NovelEvolue.Novels;

namespace NovelEvolue;

public partial class AppFlyout : FlyoutPage
{
    public AppFlyout()
    {
        InitializeComponent();
        flyoutPage.collectionView.SelectionChanged += OnSelectionChanged;

        Detail = new NavigationPage(new ListeNovel(RecuperationDonnee.SiteEnum.Xiaowaz, "Xiaowaz"));
        IsPresented = false;
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
        if (item != null)
        {
            Detail = new NavigationPage(new ListeNovel(item.SiteType, item.Title));
            IsPresented = false;
        }
    }
}