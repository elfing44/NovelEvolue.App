using System.Collections.ObjectModel;
using NovelEvolue.Novels;
using RecuperationDonnee;

namespace NovelEvolue.Chapitre;

public partial class SaisieListeChapitre : ContentPage
{
    ISite _site;
    NovelView _novel;
    bool _triDescendant = false;

    public ObservableCollection<ChapitreView> ListeChapitreView
    {
        get; private set;
    }

    public SaisieListeChapitre(ISite site, NovelView novel)
    {
        InitializeComponent();
        _site = site;
        _novel = novel;
        AlimenterListeNovel(site, novel);
        Title = novel.Titre;
        ListeChapitre.ItemSelected += ListeChapitre_ItemSelected;
    }

    private void ListeChapitre_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            ChapitreView chapitre = (ChapitreView)e.SelectedItem;
            int index = ListeChapitreView.IndexOf(chapitre);
            if (string.IsNullOrEmpty(chapitre.Texte))
            {
                chapitre.Texte = _site.RecuperationChapitre(chapitre.LienHtml, true);
            }

            Navigation.PushModalAsync(new NavigationPage(new VisualisateurChapitre(chapitre.Texte, ListeChapitreView, index, _site, _novel, chapitre.Libelle)));
            ListeChapitre.SelectedItem = null;
        }
    }

    public void AlimenterListeNovel(ISite site, NovelView novelView)
    {
        Novel novel = new Novel();
        novel.ListeChapitre = site.RecuperationListeChapitre(novelView.LienHtml).ToList();

        int i = 0;
        ListeChapitreView = new ObservableCollection<ChapitreView>(novel.ListeChapitre.Select(x => new ChapitreView() { Libelle = x.Libelle, LienHtml = x.LientHtml, Texte = x.Texte, Numero = i++, EstLu = x.Estlu, }));
        novelView.NombreChapitre = novel.ListeChapitre.Count;
        novelView.NombreChapitreLu = novel.ListeChapitre.Count(x => x.Estlu);

        BindingContext = ListeChapitreView;
    }
}