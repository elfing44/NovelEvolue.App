using System.Collections.ObjectModel;
using NovelEvolue.Chapitre;
using RecuperationDonnee;
using RecuperationDonnee.Chireads;
using RecuperationDonnee.HarkenEliwood;
using RecuperationDonnee.NovelDeGlace;
using RecuperationDonnee.PizzaTranslation;
using RecuperationDonnee.WarriorLegendTrad;
using RecuperationDonnee.WuxiaLNScantrad;
using RecuperationDonnee.Xiaowaz;

namespace NovelEvolue.Novels;

public partial class ListeNovel : ContentPage
{
    private readonly SiteEnum _site;

    public ListeNovel(SiteEnum site, string title)
    {
        this.Resources.Add("tailleecran", DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
        InitializeComponent();
        _site = site;
        AlimenterListeNovel();
        ListeNovelView.ItemSelected += ListeNovelView_ItemSelected;
        Title = title;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        this.Resources["tailleecran"] = Math.Round(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density, 0);
        base.OnSizeAllocated(Math.Round(width, 0), Math.Round(height, 0));
    }

    private void ListeNovelView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            Navigation.PushModalAsync(new NavigationPage(new SaisieListeChapitre(DonnerSite(), (NovelView)e.SelectedItem)));
            ListeNovelView.SelectedItem = null;
        }
    }

    public void AlimenterListeNovel()
    {
        ISite site = DonnerSite();

        IEnumerable<Novel> listeNovel = App.Database.ChargerListeNovel(site.SiteEnum);
        if (listeNovel == null || !listeNovel.Any())
        {
            listeNovel = site.RecuperationListeNovel();
            foreach (Novel novel in listeNovel)
            {
                App.Database.SauverNovel(novel, site.SiteEnum);
            }
            // pour avoir les ids a jour des novels
            listeNovel = App.Database.ChargerListeNovel(site.SiteEnum);
        }
        ObservableCollection<NovelView> listeNovelView = new(listeNovel.Select(x => TransformerNovelEnNovelView(x)).OrderBy(x => x.Titre));
        ListeNovelView.ItemsSource = listeNovelView;
    }

    private static NovelView TransformerNovelEnNovelView(Novel novel)
    {
        return new NovelView()
        {
            LienHtml = novel.LientHtmlSommaire,
            Titre = novel.Titre,
            NombreChapitre = novel.NombreChapitre,
            NombreChapitreLu = novel.NombreChapitreLu
        };
    }

    private ISite DonnerSite()
    {
        ISite site = null;
        switch (_site)
        {
            case SiteEnum.Xiaowaz:
                site = new Xiaowaz();
                break;
            case SiteEnum.HarkenEliwwoof:
                site = new HarkenEliwood();
                break;
            case SiteEnum.Chireads:
                site = new Chireads();
                break;
            case SiteEnum.WuxiaLNScantrad:
                site = new WuxiaLNScantrad();
                break;
            case SiteEnum.NovelDeGlace:
                site = new NovelDeGlace();
                break;
            case SiteEnum.WarriorLegendTrad:
                site = new WarriorLegendTrad();
                break;
            case SiteEnum.PizzaTranslaion:
                site = new PizzaTranslation();
                break;
        }
        return site;
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        ListeNovelView.IsRefreshing = true;
        ISite site = DonnerSite();

        IEnumerable<Novel> listeNovel = site.RecuperationListeNovel();
        foreach (Novel novel in listeNovel)
        {
            App.Database.SauverNovel(novel, site.SiteEnum);
        }

        App.Database.SupprimerNovel(listeNovel.ToList(), site.SiteEnum);

        listeNovel = App.Database.ChargerListeNovel(site.SiteEnum);
        ObservableCollection<NovelView> listeNovelView = new(listeNovel.Select(x => ListeNovel.TransformerNovelEnNovelView(x)).OrderBy(x => x.Titre));
        ListeNovelView.ItemsSource = listeNovelView;
        ListeNovelView.IsRefreshing = false;
    }
}