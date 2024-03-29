﻿using NovelEvolue.BDD;

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
            _database ??= new NovelDAL(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Novel.db3"));
            // si on a du fermer la base de donnée on ce reconnecte
            if (_database.EstClose)
            {
                _database = new NovelDAL(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Novel.db3"));
            }
            return _database;
        }
    }
}
