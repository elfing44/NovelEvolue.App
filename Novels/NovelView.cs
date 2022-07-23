using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NovelEvolue.Novels
{
    public class NovelView : INotifyPropertyChanged
    {
        /// <summary>
        /// Titre du novel
        /// </summary>
        public string Titre { get; set; }

        public string LienHtml { get; set; }

        private int _nombreChapitre;
        /// <summary>
        /// Nombre de chapitre du novel
        /// </summary>
        public int NombreChapitre
        {
            get
            {
                return _nombreChapitre;
            }
            set
            {
                _nombreChapitre = value;
                NotifyPropertyChanged();
            }
        }

        private int _nombreChapitreLu;
        /// <summary>
        /// Nombre de chapitre lu du novel
        /// </summary>
        public int NombreChapitreLu
        {
            get
            {
                return _nombreChapitreLu;
            }
            set
            {
                _nombreChapitreLu = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand AfficherInformationNovelCommand { get; set; }

        string _image;
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                NotifyPropertyChanged();
            }
        }
    }
}
