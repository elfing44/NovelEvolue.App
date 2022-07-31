using NovelEvolue.BDD;

namespace NovelEvolue;
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppFlyout();
    }

    static NovelDAL _database;

    public static NovelDAL Database
    {
        get
        {
            if (_database == null)
            {
                _database = new NovelDAL(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Novel.db3"));
            }
            return _database;
        }
    }
}
