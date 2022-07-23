using RecuperationDonnee;

namespace NovelEvolue.Novel;

public partial class ListeNovel : ContentPage
{
	SiteEnum _site;

	public ListeNovel(SiteEnum site)
	{
		InitializeComponent();
		_site = site;
	}

	public void AlimenterListeNovel()
	{

	}
}