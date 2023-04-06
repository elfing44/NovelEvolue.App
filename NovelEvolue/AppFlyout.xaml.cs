using NovelEvolue.Novels;

namespace NovelEvolue;

public partial class AppFlyout : FlyoutPage
{
    public AppFlyout()
    {
        InitializeComponent();
        flyoutPage.collectionView.SelectionChanged += OnSelectionChanged;

        Detail = new NavigationPage(new ListeNovel(RecuperationDonnee.SiteEnum.Xiaowaz, "Xiaowaz"));
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            IsPresented = false;
        }
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is FlyoutPageItem item)
        {
            Detail = new NavigationPage(new ListeNovel(item.SiteType, item.Title));
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                IsPresented = false;
            }
        }
    }
}