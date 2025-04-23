using System.IO;
using System.IO.Compression;

namespace R6Downloader.CLI.Helpers;

public static class Download
{
    public static void DownloadDepotDownloader()
    {
        if (!Directory.Exists("DepotDownloader"))
        {
            //Download
            string Download = "https://github.com/SteamRE/DepotDownloader/releases/download/DepotDownloader_3.2.0/DepotDownloader-framework.zip";
            string extractPath = "DepotDownloader";
            var myWebClient = new HttpClient();
            var stream = myWebClient.GetStreamAsync(Download).Result;
            if (stream == null)
                return;
            //Extract
            ZipFile.ExtractToDirectory(stream, extractPath);
        }
    }

    public static void DownloadFileList(string filename)
    {
        if (!File.Exists(filename))
        {
            //Download
            Console.WriteLine(filename);
            string Download = $"https://github.com/SlejmUr/R6-AIOTool-Csharp/raw/new-downloader/{filename.Replace(Path.PathSeparator, '/')}";
            var myWebClient = new HttpClient();
            var bytes = myWebClient.GetByteArrayAsync(Download).Result;
            if (bytes == null)
                return;
            //Extract
            var dir = Path.GetDirectoryName(filename);
            if (dir != null)
                Directory.CreateDirectory(dir);
            File.WriteAllBytes(filename, bytes);
        }
    }

    public static void DownloadFile(string filename, string link)
    {
        if (!File.Exists(filename))
        {
            //Download
            var myWebClient = new HttpClient();
            var stream = myWebClient.GetStreamAsync(link).Result;
            if (stream == null)
                return;
            using (var fileStream = File.Create(filename))
            {
                stream.CopyTo(fileStream);
            }
        }
    }

    public static void DownloadZip(string filename, string link)
    {
        if (!Directory.Exists(filename))
        {
            //Download
            var myWebClient = new HttpClient();
            var stream = myWebClient.GetStreamAsync(link).Result;
            if (stream == null)
                return;
            //Extract
            ZipFile.ExtractToDirectory(stream, filename, true);
        }
    }
}
