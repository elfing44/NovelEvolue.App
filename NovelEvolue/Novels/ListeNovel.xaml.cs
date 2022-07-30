using System.Collections.ObjectModel;
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

	public ListeNovel(SiteEnum site)
	{
		InitializeComponent();
		_site = site;
		AlimenterListeNovel();
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