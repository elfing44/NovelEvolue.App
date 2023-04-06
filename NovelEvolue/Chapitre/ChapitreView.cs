using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NovelEvolue.Chapitre
{
    public class ChapitreView : INotifyPropertyChanged
    {
        public ChapitreView()
        {
            PassageEnLuCommand = new Command(() =>
            {
                EstLu = !EstLu;
                App.Database.UpdateChapitre(new BDD.ChapitreBDD() { Libelle = Libelle, LientHtml = LienHtml, Texte = Texte, EstLu = EstLu, NovelLientHtmlSommaire = NovelLienHtmlSommaire });
            });
        }

        /// <summary>
        /// Libellé du chapitre
        /// </summary>
        public string Libelle { get; set; }

        public string LienHtml { get; set; }

        string _text;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Texte
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                NotifyPropertyChanged(nameof(NEstTelecharger));
            }
        }

        public int Numero { get; set; }

        bool _estlu;
        public bool EstLu
        {
            get
            {
                return _estlu;
            }
            set
            {
                _estlu = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(EtatLu));
            }
        }

        public string EtatLu
        {
            get
            {
                string text;
                if (EstLu)
                {
                    text = "Lu";
                }
                else
                {
                    text = "Non lu";
                }
                return text;
            }
        }


        public bool NEstTelecharger
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_text);
            }
        }

        public ICommand PassageEnLuCommand { get; set; }
        public string NovelLienHtmlSommaire { get; internal set; }
    }
}
