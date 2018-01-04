using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Bitlush.LinqToHtml;
using System.Linq;

namespace MoobSharp.LOM
{
    public class Idrac6 : ILom
    {
        public string st1 = "";
        public string authHash;

        HttpClientHandler handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseCookies = true,
            AllowAutoRedirect = true
        };

        public Idrac6(string user, string pass, string hostname, bool secure) : base ( user, pass, hostname, secure)
        {
            session = new HttpClient(handler);
            Name = "Dell iDrac 6";
        }

        public override bool Detect()
        {
            return GetPage("login.html").Contains("Integrated Dell Remote Access Controller 6");
        }

        public override void JNLP()
        {
            if (!Authenticated) return;
            var infos = GetInfos(new string[] { "sysDesc" });
            var title = Hostname + ", " + infos["sysDesc"] + ", User:" + Username;
            var viewer = Get("viewer.jnlp(" + Hostname + "@0@" + title + "@" + getUnixTimestamp() * 1000 + (st1 == "" ? "" : "@ST1=" + st1 ) + ")");
            if (viewer.IsSuccessStatusCode)
            {
                RunJnlp(Hostname + ".jnlp", GetContent(viewer));
            }
        }

        public override void Authenticate()
        {
            if (!Get("start.html").IsSuccessStatusCode)
                return;
            var response = Post("data/login", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("user", Username),
                new KeyValuePair<string, string>("password", Password)
            }));
            if (response.IsSuccessStatusCode && GetContent(response).Contains("<authResult>0</authResult>"))
            {
                HDocument document = HDocument.Parse(GetContent(response));
                Authenticated = true;
                var forwardurl = document.Descendants("forwardUrl").First().Value;
                try
                {
                    authHash = forwardurl.Split('?')[1];
                    st1 = authHash.Split(',')[0].Replace("ST1=", "");
                }
                catch
                {
                }
            }
        }

        public override void Logout()
        {
            if (!Authenticated) return;
            Get("/data/logout");
        }

        public override void SetPxeBoot()
        {
            if (!Authenticated) return;
            var response = Post("data?set=vmBootOnce:1,firstBootDevice:1");
        }
    }
}
