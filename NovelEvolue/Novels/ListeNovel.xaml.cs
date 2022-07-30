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
		IEnumerable<Novel> listeNovel = DonnerSite().RecuperationListeNovel();
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
}