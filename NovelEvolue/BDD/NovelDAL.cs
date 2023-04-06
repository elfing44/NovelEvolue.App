using RecuperationDonnee;
using SQLite;

namespace NovelEvolue.BDD
{
    public class NovelDAL
    {
        readonly SQLiteConnection _database;

        public NovelDAL(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<NovelBDD>();
            _database.CreateTable<ChapitreBDD>();
        }

        public void SauverNovel(Novel novel, SiteEnum siteEnum)
        {
            //Transformation de  Novel en novel BDD
            NovelBDD novelBDD = new() { LientHtmlSommaire = novel.LientHtmlSommaire, Titre = novel.Titre, Site = siteEnum };
            NovelBDD novelBDDPrecedent = _database.Table<NovelBDD>().ToList().FirstOrDefault(x => novel.LientHtmlSommaire == x.LientHtmlSommaire);
            //Récupération du Guid si existe déja en BDD ou en met un nouveau
            novelBDD.NombreChapitre = novelBDDPrecedent?.NombreChapitre ?? 0;
            novelBDD.NombreChapitreLu = novelBDDPrecedent?.NombreChapitreLu ?? 0;

            //Transformation de la liste des chapitres en chapitre BDD
            List<ChapitreBDD> listeChapitreBDD = new();
            if (novel.ListeChapitre != null)
            {
                novelBDD.NombreChapitre = novel.ListeChapitre.Count;
                foreach (var chapitre in novel.ListeChapitre)
                {
                    //Récupération du texte qui est déjà en base
                    ChapitreBDD chapitreEnBase = _database.Table<ChapitreBDD>().FirstOrDefault(x => x.NovelLientHtmlSommaire == novelBDD.LientHtmlSommaire && x.LientHtml == chapitre.LientHtml);
                    if (chapitreEnBase == null)
                    {
                        listeChapitreBDD.Add(new ChapitreBDD() { Libelle = chapitre.Libelle, LientHtml = chapitre.LientHtml, NovelLientHtmlSommaire = novelBDD.LientHtmlSommaire });
                    }
                    else
                    {
                        listeChapitreBDD.Add(new ChapitreBDD() { Libelle = chapitre.Libelle, LientHtml = chapitre.LientHtml, NovelLientHtmlSommaire = novelBDD.LientHtmlSommaire, Texte = chapitreEnBase.Texte, EstLu = chapitreEnBase.EstLu });
                    }
                }
                novelBDD.NombreChapitreLu = listeChapitreBDD.Count(x => x.EstLu);
            }

            if (_database.Table<NovelBDD>().FirstOrDefault(x => x.LientHtmlSommaire == novelBDD.LientHtmlSommaire) == null)
            {
                _database.Insert(novelBDD);
            }
            else
            {
                _database.Update(novelBDD);
            }

            if (novel.ListeChapitre != null)
            {
                _database.Table<ChapitreBDD>().Delete(x => x.NovelLientHtmlSommaire == novelBDD.LientHtmlSommaire);
                _database.InsertAll(listeChapitreBDD);
            }
        }

        public void SupprimerNovel(List<Novel> listeNovel, SiteEnum site)
        {
            List<Novel> listeNovelEnBase = ChargerListeNovel(site);
            foreach (Novel novelEnBase in listeNovelEnBase)
            {
                if (!listeNovel.Any(x => x.Titre.Equals(novelEnBase.Titre)))
                {
                    NovelBDD novelBdd = _database.Table<NovelBDD>().FirstOrDefault(x => x.LientHtmlSommaire == novelEnBase.LientHtmlSommaire);
                    _database.Delete<NovelBDD>(novelBdd.LientHtmlSommaire);
                    _database.Table<ChapitreBDD>().Delete(x => x.NovelLientHtmlSommaire == novelBdd.LientHtmlSommaire);
                }
            }

        }

        public void UpdateChapitre(ChapitreBDD chapitre)
        {
            _database.Update(chapitre);
            // On relance la sauvegarde du novel pour mettre a jour le nombre de chapitre lu
            NovelBDD novelBdd = _database.Table<NovelBDD>().FirstOrDefault(x => x.LientHtmlSommaire == chapitre.NovelLientHtmlSommaire);
            SauverNovel(ChargerNovel(chapitre.NovelLientHtmlSommaire), novelBdd.Site);
        }

        /// <summary>
        /// Charge un novel par son id
        /// </summary>
        /// <param name="id">id du novel</param>
        /// <returns>novel</returns>
        public Novel ChargerNovel(string id)
        {
            NovelBDD novelBdd = _database.Table<NovelBDD>().FirstOrDefault(x => x.LientHtmlSommaire == id);
            if (novelBdd == null)
                return null;
            List<ChapitreBDD> listeChapitreBDD = _database.Table<ChapitreBDD>().Where(x => x.NovelLientHtmlSommaire == novelBdd.LientHtmlSommaire).ToList();

            // transformation du novel BDD en novel

            Novel novel = NovelDAL.TransformerNovelBDDEnNovel(novelBdd);
            List<RecuperationDonnee.Chapitre> listeChapitre = new();
            foreach (var chapitreBDD in listeChapitreBDD)
            {
                listeChapitre.Add(new RecuperationDonnee.Chapitre() { Libelle = chapitreBDD.Libelle, LientHtml = chapitreBDD.LientHtml, Texte = chapitreBDD.Texte, Estlu = chapitreBDD.EstLu });
            }
            novel.ListeChapitre = listeChapitre;

            return novel;
        }

        public List<Novel> ChargerListeNovel(SiteEnum site)
        {
            IEnumerable<NovelBDD> listeNovelBdd = _database.Table<NovelBDD>().Where(x => x.Site == site);

            return listeNovelBdd.Select(x => TransformerNovelBDDEnNovel(x)).ToList();
        }

        private static Novel TransformerNovelBDDEnNovel(NovelBDD novel)
        {
            return new Novel() { LientHtmlSommaire = novel.LientHtmlSommaire, Titre = novel.Titre, NombreChapitre = novel.NombreChapitre, NombreChapitreLu = novel.NombreChapitreLu };
        }
    }
}
