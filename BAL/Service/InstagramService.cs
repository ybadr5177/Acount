using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BAL.Service
{
	public class InstagramService
	{
		private readonly HttpClient _httpClient;

		public InstagramService()
		{
			_httpClient = new HttpClient();
		}

		public async Task<List<RssItem>> GetInstagramImagesAsync(string rssUrl)
		{

			List<RssItem> items = new List<RssItem>();

			try
			{
				using (HttpClient client = new HttpClient())
				{
					var response = await client.GetStringAsync(rssUrl);
					using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(response)))
					{
						SyndicationFeed feed = SyndicationFeed.Load(reader);

						items = feed.Items.Select(item => new RssItem
						{
							ImageUrl = ExtractImageUrl(item.Summary?.Text)
						}).Where(x => !string.IsNullOrEmpty(x.ImageUrl)).ToList();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ خطأ: {ex.Message}");
			}

			return items;
		}
		public class RssItem
		{
			public string ImageUrl { get; set; }
		}
		private string ExtractImageUrl(string description)
		{
			if (string.IsNullOrEmpty(description)) return "";

			var match = Regex.Match(description, "<img[^>]+src=\"([^\"]+)\"");
			return match.Success ? match.Groups[1].Value : "";
		}
	}
}
