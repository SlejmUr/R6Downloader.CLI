using R6Downloader.CLI.Database;
using R6Downloader.CLI.Helpers;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Readers;
using Spectre.Console;
using System.Diagnostics;

namespace R6Downloader.CLI;

internal class Program
{
    static string SteamName = string.Empty;
    static bool ShowInfo;
    static bool UseCompressed;
    static string CrackType = "Zero";
    static void Main(string[] args)
    {
        Download.DownloadFile("builds.db", "https://raw.githubusercontent.com/SlejmUr/R6-AIOTool-Csharp/refs/heads/main/builds.db");
        OldDBLoader.Load();
        Download.DownloadDepotDownloader();
        AnsiConsole.WriteLine("Welcome to R6Downloader!");

        SteamName = ConfigINI.Read("config.ini", "User", "SteamName");
        if (string.IsNullOrEmpty(SteamName))
            SteamName = AnsiConsole.Prompt(
            new TextPrompt<string>("What's your steam user name (Login Name)?"));
        ConfigINI.Write("config.ini", "User", "SteamName", SteamName);

        ShowInfo = AnsiConsole.Prompt(
        new TextPrompt<bool>("You want to show infos for season or only download?")
        .AddChoice(true)
        .AddChoice(false)
        .DefaultValue(false)
        .WithConverter(choice => choice ? "y" : "n"));

        if (!ConfigINI.Exists("config.ini", "User", "UseCompressed"))
            UseCompressed = AnsiConsole.Prompt(
            new TextPrompt<bool>("You want to download faster (can cause issues)?")
            .AddChoice(true)
            .AddChoice(false)
            .DefaultValue(false)
            .WithConverter(choice => choice ? "y" : "n"));
        else
            UseCompressed = ConfigINI.Read<bool>("config.ini", "User", "UseCompressed");
        ConfigINI.Write("config.ini", "User", "UseCompressed", UseCompressed);

        if (ConfigINI.Exists("config.ini", "User", "CrackType"))
            CrackType = ConfigINI.Read("config.ini", "User", "CrackType");
        else
            CrackType = AnsiConsole.Prompt(
           new TextPrompt<string>("What crack type you want to use? (For easier select Zer0)")
           .AddChoice("Zero")
           .AddChoice("Slejm")
           .DefaultValue("Zero"));
        ConfigINI.Write("config.ini", "User", "CrackType", CrackType);

        bool confirmation = AnsiConsole.Prompt(
        new TextPrompt<bool>("You want to select season by year?")
        .AddChoice(true)
        .AddChoice(false)
        .DefaultValue(true)
        .WithConverter(choice => choice ? "y" : "n"));

        if (confirmation)
            SelectByYear();
        else
            SelectNormal();
    }

    static void SelectByYear()
    {
        if (DatabaseManager.GetSeasons().Select(s => s.Year).OrderBy(x => x).Count() == 0)
            return;
        var lastYear = DatabaseManager.GetSeasons().Select(s => s.Year).OrderBy(x=>x).Last();

        var year = AnsiConsole.Prompt(
            new TextPrompt<int>($"What year? (Max {lastYear})")
            .DefaultValue(1)
            .Validate((n) => n <= lastYear && n > 1));
        var seasons = DatabaseManager.GetSeasons(x => x.Year == year);

        var selectedSeason = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select your season")
        .PageSize(10)
        .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
        .AddChoices(seasons.Select(x=>x.Name)));

        AnsiConsole.WriteLine($"You selected {selectedSeason}");

        Selected(selectedSeason, seasons);
    }

    static void SelectNormal()
    {
        var seasons = DatabaseManager.GetSeasons();
        var selectedSeason = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select your season")
        .PageSize(10)
        .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
        .AddChoices(seasons.Select(x => x.Name)));

        AnsiConsole.WriteLine($"You selected {selectedSeason}");

        Selected(selectedSeason, seasons);
    }

    static void Selected(string name, List<DB_Season> seasons)
    {
        var selectedSeason = seasons.Find(x=>x.Name == name);
        var builds = DatabaseManager.GetBuilds(x=>x.PotentialSeasonId == selectedSeason.Id);
        if (builds.Count < 3)
        {
            builds = DatabaseManager.GetBuilds(x => x.DepotDate == selectedSeason.DateRelease);
        }
        if (builds.Count < 3)
        {
            AnsiConsole.WriteLine("[red]Please report this![/]");
            return;
        }

        if (ShowInfo)
        {
            AnsiConsole.WriteLine("TODO! Implement showing season info.");
            return;
        }

        List<Task> tasks = [];
        foreach (var build in builds)
        {
            if (UseCompressed)
                Download.DownloadFileList(Path.Combine("FileLists", $"{build.DepotId}_{selectedSeason.FolderName}.txt"));
            tasks.Add(DownloadSeason(selectedSeason, build, UseCompressed));
        }
        Task.WaitAll(tasks);
        if (!Directory.Exists(selectedSeason.FolderName))
        {
            AnsiConsole.WriteLine("[red]Folder hasnt been created, are you sure the download was success?[/]");
            return;
        }
        ReplaceCrack(selectedSeason);
    }

    static Task DownloadSeason(DB_Season season, DB_Build build, bool compressed)
    {
        string compstr = string.Empty;
        if (compressed)
        {
            compstr = $"-filelist {Path.Combine("FileLists",$"{build.DepotId}_{season.FolderName}.txt")}";
        }
        int downloads = 30;
        string dbBuild = string.Empty;
#if DEBUG
        dbBuild = "-manifest-only";
#endif
        Process process = new()
        {
            StartInfo = new()
            {

                FileName = "dotnet",
                Arguments = $"{Path.Combine("DepotDownloader", "DepotDownloader.dll")} -app {build.AppId} -depot {build.DepotId} -manifest {build.ManifestId} -username {SteamName} -remember-password -dir \"{season.FolderName}\" {compstr} -validate -max-downloads {downloads} {dbBuild}",
            }
        };
        process.Start();
        process.WaitForExit();
        if (compressed)
        {
            File.Delete(Path.Combine("FileLists", $"{build.DepotId}_{season.FolderName}.txt"));
        }
        return Task.CompletedTask;
    }

    static void ReplaceCrack(DB_Season season)
    {
        switch (CrackType.ToLower())
        {
            case "zero":
                ReplaceCrack_Zero(season);
                break;
            case "slejm":
                ReplaceCrack_Slejm(season);
                break;
            default:
                break;
        }
    }

    static void ReplaceCrack_Zero(DB_Season season)
    {
        if (!Directory.Exists("HeliosLoader"))
        {
            Download.DownloadFile("HeliosLoader.7z", "https://cdn.discordapp.com/attachments/1335739761670754395/1364600357237293208/HeliosLoader.7z?ex=680a429e&is=6808f11e&hm=12a9f2efb415931d5623238e930f27b3e7513f992630ba3bac78a20cef32679b&");
            var szip = SevenZipArchive.Open("HeliosLoader.7z");
            Directory.CreateDirectory("HeliosLoader");
            szip.ExtractAllEntries().WriteAllToDirectory("HeliosLoader");
        }
        foreach (var file in Directory.GetFiles("HeliosLoader"))
        {
            string filename = file.Contains(Path.DirectorySeparatorChar) ? file.Split(Path.DirectorySeparatorChar).Last() : file;
            File.Copy(file, Path.Combine(season.FolderName, filename), true);
        }

    }

    static void ReplaceCrack_Slejm(DB_Season season)
    {
        Download.DownloadZip("Plazas.zip", "https://raw.githubusercontent.com/SlejmUr/Manifest_Tool_TB/refs/heads/main/Plazas.zip");
        var crack = Path.Combine("Plazas", season.CrackName);
        if (!Directory.Exists(crack))
            throw new Exception("Crack folder not exists!");
        foreach (var file in Directory.GetFiles(crack))
        {
            string filename = file.Contains(Path.DirectorySeparatorChar) ? file.Split(Path.DirectorySeparatorChar).Last() : file;
            File.Copy(file, Path.Combine(season.FolderName, filename), true);
        }
    }
}
