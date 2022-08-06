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
        this.Resources.Add("tailleecran", DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
        InitializeComponent();
        _site = site;
        _novel = novel;
        AlimenterListeNovel(site, novel);
        Title = novel.Titre;
        ListeChapitre.ItemSelected += ListeChapitre_ItemSelected;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        this.Resources["tailleecran"] = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        base.OnSizeAllocated(width, height);
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
                App.Database.UpdateChapitre(new BDD.ChapitreBDD() { Libelle = chapitre.Libelle, LientHtml = chapitre.LienHtml, Texte = chapitre.Texte, EstLu = chapitre.EstLu, NovelLientHtmlSommaire = _novel.LienHtml });
            }

            Navigation.PushModalAsync(new NavigationPage(new VisualisateurChapitre(chapitre.Texte, ListeChapitreView, index, _site, _novel, chapitre.Libelle)));
            ListeChapitre.SelectedItem = null;
        }
    }

    public void AlimenterListeNovel(ISite site, NovelView novelView)
    {
        Novel novel = App.Database.ChargerNovel(novelView.LienHtml);
        if (novel.ListeChapitre == null || !novel.ListeChapitre.Any())
        {
            novel.ListeChapitre = site.RecuperationListeChapitre(novelView.LienHtml).ToList();
            App.Database.SauverNovel(novel, _site.siteEnum);
        }
        // Pour mettre a jour les novels déja récupérer avant la maj
        else if (novel.NombreChapitre == 0)
        {
            App.Database.SauverNovel(novel, _site.siteEnum);
        }

        int i = 0;
        ListeChapitreView = new ObservableCollection<ChapitreView>(novel.ListeChapitre.Select(x =>
        new ChapitreView()
        {
            Libelle = x.Libelle,
            LienHtml = x.LientHtml,
            Texte = x.Texte,
            Numero = i++,
            EstLu = x.Estlu,
            NovelLienHtmlSommaire = novel.LientHtmlSommaire
        }));
        novelView.NombreChapitre = novel.ListeChapitre.Count;
        novelView.NombreChapitreLu = novel.ListeChapitre.Count(x => x.Estlu);

        BindingContext = ListeChapitreView;
    }

    private void ToolbarItem_TelechargerTout(object sender, System.EventArgs e)
    {
        foreach (ChapitreView chapitre in ListeChapitreView)
        {
            if (string.IsNullOrEmpty(chapitre.Texte))
            {
                chapitre.Texte = _site.RecuperationChapitre(chapitre.LienHtml, true);
                App.Database.UpdateChapitre(new BDD.ChapitreBDD() { Libelle = chapitre.Libelle, LientHtml = chapitre.LienHtml, Texte = chapitre.Texte, NovelLientHtmlSommaire = _novel.LienHtml });
            }
        }
    }

    private void ToolbarItem_Actualiser(object sender, System.EventArgs e)
    {
        ListeChapitre.IsRefreshing = true;
        Novel novel = App.Database.ChargerNovel(_novel.LienHtml);
        novel.ListeChapitre = _site.RecuperationListeChapitre(_novel.LienHtml).ToList();
        App.Database.SauverNovel(novel, _site.siteEnum);
        ListeChapitreView.Clear();
        int i = 0;
        novel = App.Database.ChargerNovel(_novel.LienHtml);
        foreach (RecuperationDonnee.Chapitre c in novel.ListeChapitre)
        {
            ListeChapitreView.Add(new ChapitreView()
            {
                Libelle = c.Libelle,
                LienHtml = c.LientHtml,
                Texte = c.Texte,
                Numero = i++,
                EstLu = c.Estlu,
                NovelLienHtmlSommaire = novel.LientHtmlSommaire
            });
        }

        if (_triDescendant)
            BindingContext = ListeChapitreView.OrderByDescending(x => x.Numero);
        else
            BindingContext = ListeChapitreView.OrderBy(x => x.Numero);

        ListeChapitre.IsRefreshing = false;
    }

    private void ToolbarItem_Trie(object sender, System.EventArgs e)
    {
        _triDescendant = !_triDescendant;
        if (_triDescendant)
            BindingContext = ListeChapitreView.OrderByDescending(x => x.Numero);
        else
            BindingContext = ListeChapitreView.OrderBy(x => x.Numero);
    }
}