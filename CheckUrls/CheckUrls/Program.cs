using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace MyApp
{
class HtmlParser
{
    private readonly Uri m_uri;
    private SortedSet<string> m_links = new SortedSet<string>();
    public HtmlParser(Uri uri)
    {
        m_uri = uri;
    }
    private void AddLinkAndFindLinksOnIt(Uri? uri)
    {
        if (uri == null || m_uri.Host != uri.Host)
        {
            return;
        }

        string absoluteUri = uri.AbsoluteUri;
        if (absoluteUri.Contains('#'))
        {
            absoluteUri = absoluteUri.Substring(0, absoluteUri.LastIndexOf('#'));
        }

        if (m_links.Add(absoluteUri))
        {
            ParsePageAndFindLinks(uri);
        }
    }

    public SortedSet<string> ParsePageAndFindLinks(Uri pageUri)
    {

        HtmlWeb htmlWeb = new HtmlWeb();
        HtmlDocument doc = htmlWeb.Load(pageUri);
        var paths = doc.DocumentNode.Descendants("a")
                        .Select(a => a.GetAttributeValue("href", null))
                        .Where(path => !String.IsNullOrEmpty(path));

        foreach (var path in paths)
        {
                    Uri.TryCreate(pageUri, path, out Uri? uriWithLocalPath);
                    AddLinkAndFindLinksOnIt(uriWithLocalPath);
        }
        return m_links;
    }
}

internal class Program
{

    static Uri? GetUriFromArgs(string[] args)
    {
        if (args.Length != 1)
        {
                    Console.WriteLine("incorrect arguments. Correct: CheckUrls.exe <uri>");
                    return null;
        }

        if (!Uri.TryCreate(args[0], UriKind.Absolute, out Uri? uri))
        {
                    Console.WriteLine("input is not correct uri");
                    return null;
        }
        return uri;
    }

    static void AppendInfoWithDateTime(StreamWriter streamWriter, string info, string dateTimeFormat = "g")
    {
        streamWriter.WriteLine();
        streamWriter.WriteLine(info);
        streamWriter.WriteLine(String.Format("Executed - {0}", DateTime.Now.ToString(dateTimeFormat)));
    }

    static async Task CheckAllLinksOnPage(Uri pageUri)
    {
        HttpClient client = new();
        HtmlParser htmlParser = new HtmlParser(pageUri);

        var links = htmlParser.ParsePageAndFindLinks(pageUri);

        using StreamWriter successLinksFile = new("valid links.TXT");
        using StreamWriter errorLinksFile = new("error links.TXT");

        int successCounter = 0, errorCounter = 0;

        foreach (var link in links)
        {

                    HttpResponseMessage webResponse = await client.GetAsync(link);
                    int statusCode = (int)webResponse.StatusCode;
                    if (statusCode >= 200 && statusCode <= 299)
                    {
                        successLinksFile.WriteLine(String.Format("{0} - {1}", link, statusCode));
                        successCounter++;
                    }
                    else
                    {
                        errorLinksFile.WriteLine(String.Format("{0} - {1}", link, statusCode));
                        errorCounter++;
                    }
        }
        AppendInfoWithDateTime(successLinksFile, String.Format("{0} links found", successCounter));
        AppendInfoWithDateTime(errorLinksFile, String.Format("{0} links found", errorCounter));
    }

    static async Task Main(string[] args)
    {
        var uri = GetUriFromArgs(args);
        if (uri == null)
        {
                    return;
        }
        await CheckAllLinksOnPage(uri);
    }
}
}
