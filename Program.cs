if (args.Length == 0)
{
	Console.WriteLine("Usage: export-wiki WIKI_PAGE_URL [OUTPUT_FILE]");
	return;
}

var pageName = args[0].LastIndexOf("/") > 0 ? args[0].Substring(args[0].LastIndexOf("/") + 1) : args[0];

var outputFile = args.Length == 1 ? $"{pageName}.json" : args[1];
Console.WriteLine($"Exporting {pageName} to {outputFile}");

var wikiUrl = $"https://en.wikipedia.org/wiki/Special:Export/{pageName}";
var pageContent = Utils.GetUrlContent(wikiUrl);
Console.WriteLine($"Page content: {pageContent.Length}");

var json = WikiXmlToJson.Convert(pageContent);
Console.WriteLine($"JSON: {json}");

File.WriteAllText(outputFile, json);