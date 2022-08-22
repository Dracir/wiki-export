
using System.Text;
using System.Text.RegularExpressions;

//TODO parse
//*The[[Normalcy bias]], a form of [[cognitive dissonance]], is the refusal to plan for, or react to, a disaster which has never happened before.

// Assumes a format like this:
// == HEADER 2 ==
// === HEADER 3===
// * [[TITLE]], DESCRIPTION
// * [[TITLE1]] or [[TITLE2]]. DESCRIPTION
// * [[TITLE]], DESCRIPTION
// * [[TITLE]], DESCRIPTION
// === HEADER 3===
// * [[TITLE]], DESCRIPTION
// * [[TITLE]], DESCRIPTION
// * [[TITLE]], DESCRIPTION
public static class WikiXmlToJson
{
	private static readonly string H2Marker = "==";
	private static readonly string H3Marker = "===";
	private static readonly string LIMarker = "* ";


	public static string Convert(string xml)
	{
		var json = new StringBuilder();
		var currentLevel = 1;
		foreach (var line in xml.Split('\n'))
		{
			if (line.StartsWith(H3Marker))
			{
				AddTitleToBuilder(json, currentLevel, H3Marker, 3, line);
				currentLevel = 3;
			}
			else if (line.StartsWith(H2Marker))
			{
				AddTitleToBuilder(json, currentLevel, H2Marker, 2, line);
				currentLevel = 2;
			}
			else if (line.StartsWith(LIMarker))
			{
				AddLIToBuilder(json, currentLevel, line);
				currentLevel = 2;
			}
			else
			{
				// json.AppendLine(line);
			}
		}

		CloseLevelTo(json, currentLevel, 1);

		return json.ToString();
	}


	private static void AddLIToBuilder(StringBuilder json, int currentLevel, string line)
	{
		var doubleTitleWithOr = Regex.Match(line, @"\[\[(.*)\]\] or \[\[(.*)\]\].(.*)");
		if (doubleTitleWithOr.Success)
		{
			var title = $"{doubleTitleWithOr.Groups[1].Value} or {doubleTitleWithOr.Groups[1].Value}";
			AddLIToBuilder(json, title, doubleTitleWithOr.Groups[3].Value, currentLevel);
			return;
		}

		var simpleTitleWithDescription = Regex.Match(line, @"\[\[(.*)\]\],(.*)");
		if (simpleTitleWithDescription.Success)
		{
			AddLIToBuilder(json, simpleTitleWithDescription.Groups[1].Value, simpleTitleWithDescription.Groups[2].Value, currentLevel);
			return;
		}
		else
		{
			Console.WriteLine($"Nothing found for {line}");
		}
	}

	private static void AddLIToBuilder(StringBuilder json, string title, string descriptions, int currentLevel)
	{
		json.Append(new String('\t', currentLevel));
		json.Append($"{{");
		json.Append(new String('\t', currentLevel + 1));
		json.Append($"Title: {title},");
		json.Append(new String('\t', currentLevel + 1));
		json.Append($"Descriptions: \"{descriptions}\"");
		json.Append(new String('\t', currentLevel));
		json.Append($"}}");
	}

	private static void AddTitleToBuilder(StringBuilder json, int currentlevel, string headerMarker, int headerLevel, string lineText)
	{
		var title = lineText.Trim(headerMarker[0]);
		CloseLevelTo(json, currentlevel, headerLevel);

		json.Append(new String('\t', headerLevel));
		json.Append($"{title}:{{\n");
	}

	private static void CloseLevelTo(StringBuilder json, int currentlevel, int level)
	{
		for (int i = currentlevel; i >= level; i--) // Close all levels above
			json.Append(new String('\t', i) + "},\n");
	}
}