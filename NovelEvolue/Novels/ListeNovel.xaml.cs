using System.Collections.ObjectModel;
using NovelEvolue.Chapitre;
using RecuperationDonnee;
using RecuperationDonnee.Chireads;
using RecuperationDonnee.HarkenEliwood;
using RecuperationDonnee.NovelDeGlace;
using RecuperationDonnee.WuxiaLNScantrad;
using RecuperationDonnee.Xiaowaz;

namespace NovelEvolue.Novels;

public partial class ListeNovel : ContentPage
{
	SiteEnum _site;

	public ListeNovel(SiteEnum site, string title)
	{
		InitializeComponent();
		_site = site;
		AlimenterListeNovel();
		ListeNovelView.ItemSelected += ListeNovelView_ItemSelected;
		Title = title;
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

		IEnumerable<Novel> listeNovel = App.Database.ChargerListeNovel(site.siteEnum);
		if (listeNovel == null || !listeNovel.Any())
		{
			listeNovel = site.RecuperationListeNovel();
			foreach (Novel novel in listeNovel)
			{
				App.Database.SauverNovel(novel, site.siteEnum);
			}
			// pour avoir les ids a jour des novels
			listeNovel = App.Database.ChargerListeNovel(site.siteEnum);
		}
		ObservableCollection<NovelView> listeNovelView = new ObservableCollection<NovelView>(listeNovel.Select(x => TransformerNovelEnNovelView(x)).OrderBy(x => x.Titre));
		ListeNovelView.ItemsSource = listeNovelView;
	}

	private NovelView TransformerNovelEnNovelView(Novel novel)
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
			App.Database.SauverNovel(novel, site.siteEnum);
		}

		App.Database.SupprimerNovel(listeNovel.ToList(), site.siteEnum);

		listeNovel = App.Database.ChargerListeNovel(site.siteEnum);
		ObservableCollection<NovelView> listeNovelView = new ObservableCollection<NovelView>(listeNovel.Select(x => TransformerNovelEnNovelView(x)).OrderBy(x => x.Titre));
		ListeNovelView.ItemsSource = listeNovelView;
		ListeNovelView.IsRefreshing = false;
	}
}