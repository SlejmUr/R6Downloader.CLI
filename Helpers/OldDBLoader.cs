using R6Downloader.CLI.Database;

namespace R6Downloader.CLI.Helpers;

internal class OldDBLoader
{
    public static void Load()
    {
        if (File.Exists("ultimatedepot.csv"))
            ultimatedepotLoad();
        if (File.Exists("pick.csv"))
            pickLoad();
    }


    static void ultimatedepotLoad()
    {
        // SELECT depotid, depotdate, manifest, pick_ID FROM ultimateDepot WHERE depotid == 359551 or depotid == 377237 or depotid == 377238 ORDER BY pick_ID
        using var stream = File.OpenText("ultimatedepot.csv");
        while (!stream.EndOfStream)
        {
            string? line = stream.ReadLine();
            if (line == null)
                continue;
            var splitted = line.Split(',');
            var manifest = splitted[2];
            var build = DatabaseManager.GetBuild(x => x.ManifestId == manifest);
            if (!build.HasValue || build.Value.Id == 0)
                DatabaseManager.AddBuild(new DB_Build()
                {
                    AppId = 359550,
                    DepotId = uint.Parse(splitted[0]),
                    DepotDate = DateTime.Parse(splitted[1]).ToString("yyyy-MM-dd"),
                    ManifestId = splitted[2].ToString(),
                    PotentialSeasonId = splitted.Length > 3 ? int.Parse(splitted[3]) : 0
                });
        }
    }

    static void pickLoad()
    {
        using var stream = File.OpenText("pick.csv");
        while (!stream.EndOfStream)
        {
            string? line = stream.ReadLine();
            if (line == null)
                continue;
            var splitted = line.Split(',');
            var folder = splitted[2];
            var season = DatabaseManager.GetSeason(x => x.FolderName == folder);
            if (!season.HasValue || season.Value.Id == 0)
                DatabaseManager.AddSeason(new DB_Season()
                {
                    CrackName = splitted[3],
                    FolderName = splitted[2],
                    DateRelease = DateTime.Parse(splitted[0]).ToString("yyyy-MM-dd"),
                    Name = splitted[1],
                    Season = int.Parse(splitted[2][3].ToString()),
                    Year = int.Parse(splitted[2][1].ToString()),
                });
        }
    }
}
