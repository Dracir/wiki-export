using System.Net;

public static class Utils
{
	public static string GetUrlContent(string url)
	{
		HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
		var rep = myReq.GetResponse();
		var reader = new StreamReader(rep.GetResponseStream());
		var content = reader.ReadToEnd();
		return content;
	}

}