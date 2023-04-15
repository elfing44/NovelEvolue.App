using CommunityToolkit.Maui.Storage;
using NovelEvolue.Entite;
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
            if (item.Action == ActionEnum.OuvrirListeNovel)
            {
                Detail = new NavigationPage(new ListeNovel(item.SiteType.Value, item.Title));
            }
            else if (item.Action == ActionEnum.ExporterBaseDonner)
            {
                ExporterBaseDonner();
            }

            // On déselectionne l'élement
            flyoutPage.collectionView.SelectedItem = null;
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // on ferme le menu 
                IsPresented = false;
            }
        }
    }

    /// <summary>
    /// Exporte le fichier de la base de donnée
    /// </summary>
    private async void ExporterBaseDonner()
    {
        // On ferme la BDD avant l'export
        App.Database.Close();
        using (Stream fileStream = File.OpenRead(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Novel.db3")))
        {
            // Appel de la méthode SaveAsync pour envoyer le fichier
            FileSaverResult result = await FileSaver.Default.SaveAsync(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "*.db3", fileStream, CancellationToken.None);
        }
    }
}