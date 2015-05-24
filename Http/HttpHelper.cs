using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Akumu.Antigate;
using Awesomium.Core;
using HtmlAgilityPack;
using xNet.Net;
using HttpRequest = xNet.Net.HttpRequest;

namespace TimepadEvents.Http
{
	internal static class HttpHelper
	{
		private static readonly WebView webView = StaticSettings.View;
		private static FormSettings fs;
		private static bool checker;
		private static bool status;
		public static async Task<DataItem> PostFeedback(string link, FormSettings formSettings)
		{
			var wc = new WebClient();
			fs = formSettings;
			var resp=string.Empty;// = Encoding.UTF8.GetString(await wc.DownloadDataTaskAsync(link));

			var cookieDictionary = new CookieDictionary();
			using (var req = new HttpRequest())
			{
				req.Cookies = cookieDictionary;
				resp = req.Get(link).ToString();
			}

			var doc = new HtmlDocument();
			doc.LoadHtml(resp);

			//var challengeTemp =
			//	doc.DocumentNode.Descendants("script")
			//		.First(
			//			x =>
			//				x.Attributes.Contains("src") &&
			//				x.Attributes["src"].Value.Contains(@"https://www.google.com/recaptcha/api/challenge"))
			//		.GetAttributeValue("src", string.Empty);
			//var cht = await wc.DownloadStringTaskAsync(challengeTemp);
			//var captchaChallange = Regex.Match(cht, @"(?<=challenge : ').+(?=')").Value;
			//var captchaLink = @"https://www.google.com/recaptcha/api/image?c=" + captchaChallange;
			//captchaChallange =
			//	Regex.Match(
			//		await
			//			wc.DownloadStringTaskAsync(@"https://www.google.com/recaptcha/api/reload?c=03AHJ_VutJhHfBg6gB4XxgfBHq4m5E9J5xvD9z59xKH4VMPUlWBm5bSww7eH2hRogexhWrA33m__P06KFjP0LwaAPgmXqVMfX5Ewak-axbK9ckGkhXgaOVvyZdY1ZQ7vUzf8-e03cKXwBNI6dBI2ztrryDwsR-ufq7SmclkZzAICEpWxS6GX3rs0yhmOQfAb3o-HtSPmeyCt0xIIiFb9x5vph6D0T4XQIlELylC-BEp-tScQqab9Lp-np6uGj_ZkQZEQIkTgTOzuEMdE9LFwavHrn4QEe8b2J8ag&k=6Lc6wAkAAAAAAMMZ_jRO1azUYH7SIZPtMnh9ND0k&reason=i&type=image&lang=ru&th=,MsOwvw5gnA004j62tp-Sv7u46dvwAAAAU6AAAAAH2ADTscw4pnNenPC5msU6ISfR9YC-tNVZikttNLQFnt2JFyx_qFOWUIts_oaxTNiWZFaQKhn7fLYRjPXVr0_3j77dmnWrUqZ6faM1GXDB1MOOvYrEEW4t17P-VcMJQiCdis1KDGBk4aGluILatpkwN7B30gJ-Kx-3w6id9ONf_nuj8D1JMwZOXPRkw1RuCmYeyjfo8oFzeB18K1Q1ymHt81Rz4j3mRAEJ-PpRWUSG_IE7XbzqZ4umE8QWzTUt6SOk67t8LKl_EwnSscZUw8j68B1YvAwu1g"),
			//		@"(?<=reload\(').+(?=', 'image')").Value;

			var ID = Regex.Match(link, @"\d+$").Value;

			var name=string.Empty;
			try
			{
				name = doc.DocumentNode.Descendants("h1").First(x => x.Attributes["class"].Value == @"event-page-h1").InnerText;
			}
			catch
			{
				//name =
				//	doc.DocumentNode.Descendants("span")
				//		.First(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "summary")
				//		.InnerText;
			}

			if (name == string.Empty)
			{
				name = Regex.Match(doc.DocumentNode.Descendants("title").First().InnerText, @"^.+(?= \/)").Value;
				var fbLink = Regex.Match(link, @".+(?=\/event\/)").Value +
				             Regex.Match(doc.DocumentNode.InnerHtml, @"\/\w+\/feedback\/\d+\/").Value;

				var nvc = new NameValueCollection
				{
					{"name",formSettings.Name},
					{"mail",formSettings.Email},
					{"text",formSettings.Message}
				};

				using (var req = new HttpRequest())
				{
					req.Cookies = cookieDictionary;

					req.AddHeader(HttpHeader.Referer, link);
					req.AddHeader(HttpHeader.UserAgent, xNet.Net.HttpHelper.ChromeUserAgent());

					req.AddField("name", formSettings.Name);
					req.AddField("mail", formSettings.Email);
					req.AddField(formSettings.Message);

					var response = req.Post(fbLink).ToString();
					status = true;
				}
				//wc.Headers.Add(HttpRequestHeader.Cookie,
				//	"TPSESSID=8bf52dd5f06c38c0887e3eafd693b9a56484e48378d125df7424f0495e664eb54c12a5e58192543c;" +
				//	"_timepad_front_session_store=b0VCbitIN0RSYmNwdllMUU5SYmQ1YkFkTUpubzJrRnlBemExOG9xNFVaSTN6YmlpMnJWbDRaaXNxa3BnSkphU0FTN0FFaVBLTnY3MHRJaUF3Wm8vcSthV3pyZGVEbzZGSVRCK0d5RlhOYjdLMXhNOWFlR0dSYWFZWldpSVpGQ0c2M2VtTjNHNyt2Tkc1cEhNRmlPSGZnPT0tLW1HbGRqVUVpcnpNWkVJa1FSUG13aHc9PQ%3D%3D--30d27ec27f4b869b82f1ff161541ad6b6ed3c698;" +
				//	"_tus=2acebcc4c49dbf66a2ec631cbf412688;");

				//var query = string.Join("&",
				//	nvc.AllKeys.Where(key => !string.IsNullOrWhiteSpace(nvc[key]))
				//		.Select(
				//			key =>
				//				string.Join("&",
				//					nvc.GetValues(key)
				//						.Select(val => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(val))))));

				//var jsonResponse = await wc.UploadStringTaskAsync(fbLink, query);
				//status = jsonResponse.Contains("result");
					//jsonResponse != null && jsonResponse.result != null && jsonResponse.result == "ok";
			}
			else
			{
				webView.LoadingFrameComplete += View_LoadingFrameComplete;
				webView.Source = new Uri(link);

				await Task.Run(() =>
				{
					while (!checker)
					{

					}
				});
				checker = false;
			}

			//var antiCap = new AntiCaptcha(formSettings.AntigateKey)
			//{
			//	CheckDelay = 5000,
			//	CheckRetryCount = 30,
			//	SlotRetry = 5,
			//	SlotRetryDelay = 1000
			//};

			//var captchaAnswer = antiCap.GetAnswer(await wc.DownloadDataTaskAsync(captchaLink));

			//var nvc = new NameValueCollection
			//{
			//	{"fb_fromname",formSettings.Name},
			//	{"fb_from",formSettings.Email},
			//	{"fb_message",formSettings.Message},
			//	{"recaptcha_challenge_field",captchaChallange},
			//	{"recaptcha_response_field",captchaAnswer}
			//};
			//var query = string.Join("&",
			//	nvc.AllKeys.Where(key => !string.IsNullOrWhiteSpace(nvc[key]))
			//		.Select(
			//			key =>
			//				string.Join("&",
			//					nvc.GetValues(key)
			//						.Select(val => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(val))))));

			//var feedbackLink = link.Insert(link.IndexOf(@"event/") + @"event/".Length, @"feedback/");

			//dynamic jsonResponse = await wc.UploadStringTaskAsync(feedbackLink, query);
			//var status = jsonResponse != null && jsonResponse.result != null && jsonResponse.result == "ok";


			return new DataItem
			{
				Date = DateTime.Now,
				ID = ID,
				Name = name,
				Status = status
			};
		}

		private static async void View_LoadingFrameComplete(object sender, FrameEventArgs e)
		{
			try
			{
				webView.LoadingFrameComplete -= View_LoadingFrameComplete;
				await Task.Run(() => Task.Delay(1000));

				//var doc = webView.ExecuteJavascriptWithResult(@"document.documentElement.innerHTML").ToString();
				var captchaImg =
					webView.ExecuteJavascriptWithResult(@"document.getElementById('recaptcha_challenge_image').getAttribute('src')")
						.ToString();
				var antiCap = new AntiCaptcha("3a2aa3c836b5b514e0cc3bfadeaecc22")
				{
					CheckDelay = 5000,
					CheckRetryCount = 30,
					SlotRetry = 5,
					SlotRetryDelay = 1000
				};

				var captchaAnswer = string.Empty;
				await
					Task.Run(
						async () => { captchaAnswer = antiCap.GetAnswer(await new WebClient().DownloadDataTaskAsync(captchaImg)); });

				webView.ExecuteJavascript(string.Format(@"
				document.getElementById('fb_fromname').value='{0}';
				document.getElementById('fb_from').value='{1}';
				document.getElementById('fb_message').value='{2}';
				document.getElementById('recaptcha_response_field').value='{3}';
				document.getElementsByClassName('btn btn-success')[0].click();"
					, fs.Name, fs.Email, fs.Message, captchaAnswer));

				status = true;
                checker = true;				
			}
			catch (Exception ex)
			{
				checker = true;
				status = false;
			}
		}

		public static async Task<List<string>> GetEventsLinks(string link, int count)
		{
			var temp = new List<string>();
			var counter = 1;

			do
			{
				var resp = Encoding.UTF8.GetString(await new WebClient().DownloadDataTaskAsync(link + counter++));
				if(!resp.Contains("Подробности и регистрация"))
					break;

				temp.AddRange(await GetLinksFromPage(resp));

			} while (temp.Count < count);

			return temp.GetRange(0, temp.Count < count ? temp.Count : count);
		}

		private static async Task<List<string>> GetLinksFromPage(string html)
		{
			return await Task.Run(() =>
			{
				var doc = new HtmlDocument();
				doc.LoadHtml(html);

				return
					doc.DocumentNode.Descendants("a")
						.Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "btn")
						.Select(x => x.Attributes["href"].Value)
						.Distinct()
						.ToList();
			});
		}
	}
}