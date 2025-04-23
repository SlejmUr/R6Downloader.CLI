using System.Linq.Expressions;
using LiteDB;

namespace R6Downloader.CLI.Database;

public class DatabaseManager
{
    const string DataBaseName = "builds.db";
    const string DB_SeasonName = "Seasons";
    const string DB_BuildName = "Builds";

    public static DB_Season? GetSeason(Expression<Func<DB_Season, bool>> predicate)
    {
        using var database = new LiteDatabase(DataBaseName);
        var SeasonCollection = database.GetCollection<DB_Season>(DB_SeasonName);
        if (SeasonCollection == null)
            return null;
        return SeasonCollection.FindOne(predicate);
    }

    public static List<DB_Season> GetSeasons()
    {
        using var database = new LiteDatabase(DataBaseName);
        var SeasonCollection = database.GetCollection<DB_Season>(DB_SeasonName);
        if (SeasonCollection == null)
            return [];
        return SeasonCollection.FindAll().ToList();
    }

    public static void AddSeason(DB_Season season)
    {
        using var database = new LiteDatabase(DataBaseName);
        var SeasonCollection = database.GetCollection<DB_Season>(DB_SeasonName);
        if (SeasonCollection == null)
            return;
        SeasonCollection.Upsert(season);
    }

    public static List<DB_Season> GetSeasons(Expression<Func<DB_Season, bool>> predicate)
    {
        using var database = new LiteDatabase(DataBaseName);
        var SeasonCollection = database.GetCollection<DB_Season>(DB_SeasonName);
        if (SeasonCollection == null)
            return [];
        return SeasonCollection.Find(predicate).ToList();
    }

    public static DB_Build? GetBuild(Expression<Func<DB_Build, bool>> predicate)
    {
        using var database = new LiteDatabase(DataBaseName);
        var BuildCollection = database.GetCollection<DB_Build>(DB_BuildName);
        if (BuildCollection == null)
            return null;
        return BuildCollection.FindOne(predicate);
    }

    public static List<DB_Build> GetBuilds()
    {
        using var database = new LiteDatabase(DataBaseName);
        var BuildCollection = database.GetCollection<DB_Build>(DB_BuildName);
        if (BuildCollection == null)
            return [];
        return BuildCollection.FindAll().ToList();
    }

    public static void AddBuild(DB_Build build)
    {
        using var database = new LiteDatabase(DataBaseName);
        var BuildCollection = database.GetCollection<DB_Build>(DB_BuildName);
        if (BuildCollection == null)
            return;
        BuildCollection.Upsert(build);
    }

    public static List<DB_Build> GetBuilds(Expression<Func<DB_Build, bool>> predicate)
    {
        using var database = new LiteDatabase(DataBaseName);
        var BuildCollection = database.GetCollection<DB_Build>(DB_BuildName);
        if (BuildCollection == null)
            return [];
        return BuildCollection.Find(predicate).ToList();
    }
}
