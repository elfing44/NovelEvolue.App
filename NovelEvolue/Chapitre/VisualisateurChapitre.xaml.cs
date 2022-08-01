using System.Collections.ObjectModel;
using NovelEvolue.Novels;
using RecuperationDonnee;

namespace NovelEvolue.Chapitre;

public partial class VisualisateurChapitre : ContentPage
{
    int _indexElement;
    ObservableCollection<ChapitreView> _listeChapitre;
    ISite _site;
    NovelView _novel;


    public VisualisateurChapitre(string texte, ObservableCollection<ChapitreView> chapitreViews, int indexElement, ISite site, NovelView novel, string titre)
    {
        InitializeComponent();
        Title = titre;
        _listeChapitre = chapitreViews;
        _indexElement = indexElement;
        _site = site;
        _novel = novel;
        if (indexElement == 0)
        {
            BoutonPrecedent.IsEnabled = false;
        }
        else if (indexElement == _listeChapitre.Count - 1)
        {
            BoutonSuivant.IsEnabled = false;
        }

        if (Application.Current.RequestedTheme == AppTheme.Dark)
        {
            // Ajout de l'entete pour ajouter un theme
            texte = texte.Replace("<html>", @"<html>
                                <style>
                                body {
                                background-color: #333;
                                color: #e0e0e0;
                                }
                                </style>");
        }

        web.Source = new HtmlWebViewSource() { Html = texte };
    }

    private void ButtonPrecedent_Clicked(object sender, System.EventArgs e)
    {
        PasserEnLu(_indexElement);
        NaviguerChapitre(_indexElement - 1);
    }

    private void ButtonSuivant_Clicked(object sender, System.EventArgs e)
    {
        PasserEnLu(_indexElement);
        NaviguerChapitre(_indexElement + 1);
    }


    public void NaviguerChapitre(int index)
    {
        ChapitreView chapitre = _listeChapitre[index];
        if (string.IsNullOrEmpty(chapitre.Texte))
        {
            chapitre.Texte = _site.RecuperationChapitre(chapitre.LienHtml, true);
            App.Database.UpdateChapitre(new BDD.ChapitreBDD() { Libelle = chapitre.Libelle, LientHtml = chapitre.LienHtml, Texte = chapitre.Texte, NovelLientHtmlSommaire = _novel.LienHtml });

        }
        Navigation.PopModalAsync();
        Navigation.PushModalAsync(new NavigationPage(new VisualisateurChapitre(chapitre.Texte, _listeChapitre, index, _site, _novel, chapitre.Libelle)));
    }

    public void PasserEnLu(int index)
    {
        ChapitreView chapitre = _listeChapitre[index];
        chapitre.EstLu = true;
        App.Database.UpdateChapitre(new BDD.ChapitreBDD() { Libelle = chapitre.Libelle, LientHtml = chapitre.LienHtml, Texte = chapitre.Texte, NovelLientHtmlSommaire = _novel.LienHtml, EstLu = chapitre.EstLu });
    }
}