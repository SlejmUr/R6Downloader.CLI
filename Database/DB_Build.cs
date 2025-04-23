namespace R6Downloader.CLI.Database;

public struct DB_Build
{
    [LiteDB.BsonId]
    public int Id { get; set; }
    public uint AppId { get; set; }
    public uint DepotId { get; set; }
    public string DepotDate { get; set; }
    public string ManifestId { get; set; }
    public int PotentialSeasonId { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, AppId: {AppId}, DepotId: {DepotId}, DepotDate: {DepotDate}, ManifestId: {ManifestId}, Pot Season: {PotentialSeasonId}";
    }
}
