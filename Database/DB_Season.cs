namespace R6Downloader.CLI.Database;

public struct DB_Season
{
    [LiteDB.BsonId]
    public int Id { get; set; }
    public string Name { get; set; }
    public string DateRelease { get; set; }
    public int Year { get; set; }
    public int Season { get; set; }
    public string FolderName { get; set; }
    public string CrackName { get; set; }
}
