using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Bitlush.LinqToHtml;
using System.Linq;

namespace MoobSharp.LOM
{
    public class Idrac7_8 : ILom
    {
        public string st2;
        public string authHash;

        CookieContainer cookies = new CookieContainer();

        HttpClientHandler handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseCookies = true,
            AllowAutoRedirect = true
        };

        public Idrac7_8(string user, string pass, string hostname, bool secure) : base(user, pass, hostname, secure)
        {
            Name = "Dell iDrac 7-8";
            handler.CookieContainer = cookies;
            session = new HttpClient(handler);
            session.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.7; rv:5.0.1) Gecko/20100101 Firefox/5.0.1");
            session.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8,sv;q=0.6");
            session.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,sdch");
            session.Timeout = new TimeSpan(0, 0, 30);
        }

        public override bool Detect()
        {
            var response = GetPage("locale/locale_en.json");
            return (response.Contains("Integrated Dell Remote Access Controller 8") || response.Contains("iDRAC8") || response.Contains("Integrated Dell Remote Access Controller 7") || response.Contains("iDRAC7"));
        }

        public override void JNLP()
        {
            if (!Authenticated) return;
            var infos = GetInfos(new string[] { "sysDesc" });
            var title = Hostname + ", " + infos["sysDesc"] + ", User:" + Username;
            var viewer = Get("viewer.jnlp(" + Hostname + "@0@" + title + "@" + getUnixTimestamp() * 1000 + "@" + authHash + ")");
            if (viewer.IsSuccessStatusCode)
            {
                RunJnlp(Hostname + ".jnlp", GetContent(viewer));
            }
        }

        public override void Authenticate()
        {
            var authContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("user", Username),
                new KeyValuePair<string, string>("password", Password)
            });
            var authResponse = Post("data/login", authContent);

            if (authResponse.IsSuccessStatusCode && GetContent(authResponse).Contains("<authResult>0</authResult>"))
            {
                try
                {
                    HDocument document = HDocument.Parse(GetContent(authResponse));
                    var forwardurl = document.Descendants("forwardurl").First().Value;
                    authHash = forwardurl.Split('?')[1];
                    st2 = authHash.Split(',')[1].Replace("ST2=", "");
                    session.DefaultRequestHeaders.Add("ST2", st2);
                    Authenticated = true;
                }
                catch
                {
                    Authenticated = false;
                }
            }
        }

        public override void Logout()
        {
            if (!Authenticated) return;
            var logout = Get("/data/logout");
        }

        public override void SetPxeBoot()
        {
            if (!Authenticated) return;
            var response = Post("data?set=vmBootOnce:1,firstBootDevice:1");
        }

    }
}
