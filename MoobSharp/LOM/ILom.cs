using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using Bitlush.LinqToHtml;
using System.Linq;

namespace MoobSharp.LOM
{
    public class ILom
    {
        public ILom(string user, string pass, string hostname, bool secure)
        {
            Username = user;
            Password = pass;
            Hostname = hostname;
            Secure = secure;
            ServicePointManager.ServerCertificateValidationCallback +=
                                   (sender, cert, chain, sslPolicyErrors) => true;
        }

        public string Name { get; set; }
        public List<String> InfoFields = new List<string>();
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Secure { get; set; }
        public bool Authenticated { get; set; } = false;

        string Transport
        {
            get
            {
                return Secure ? "https://" : "http://";
            }
        }
        protected HttpClient session = new HttpClient();


        public virtual void LoadSession()
        {

        }

        public virtual bool Detect()
        {
            return false;
        }

        public virtual void Logout()
        {

        }

        public virtual void JNLP()
        {

        }

        public virtual void Authenticate()
        {

        }

        public virtual void SetPxeBoot()
        {
            
        }

        public string GetPage(string URL)
        {
            return GetContent(session.GetAsync(Transport + Hostname + "/" + URL).Result);
        }

        public HttpResponseMessage Get(string URL)
        {
            return session.GetAsync(Transport + Hostname + "/" + URL).Result;
        }

        public HttpResponseMessage Post(string URL, FormUrlEncodedContent Data)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, Transport + Hostname + "/" + URL)
            {
                Content = Data
            };
            return session.SendAsync(req).Result;
        }

        public HttpResponseMessage Post(string URL)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, Transport + Hostname + "/" + URL);
            return session.SendAsync(req).Result;
        }

        public static string GetContent(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                return responseString;
            }
            return "";
        }

        public double getUnixTimestamp()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static void WriteAscii(string Contents, string FileName)
        {
            Write(FileName, Contents, Encoding.ASCII);
        }


        static void Write(string FileName, string Contents, Encoding enc)
        {
            FileName = string.Format(FileName, DateTime.Now);
            using (StreamWriter outputFile = new StreamWriter(new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite), enc))
            {
                outputFile.Write(Contents);
            }
        }

        public static void RunJnlp(string FileName, string Contents)
        {
            WriteAscii(Contents, FileName);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + FileName;
            ExecuteCommand("javaws " + path);
        }

        // Run a bash command
        public static void ExecuteCommand(string command)
        {
            new System.Threading.Thread(() =>
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();

                while (!proc.StandardOutput.EndOfStream)
                {
                    Console.WriteLine(proc.StandardOutput.ReadLine());
                }
            }).Start();

        }

        public Dictionary<string, string> GetInfos(string[] Keys)
        {
            var toReturn = new Dictionary<string, string>();
            HDocument document = HDocument.Parse(GetContent(Post("data?get=" + String.Join(",", Keys))));
            foreach (var item in Keys)
            {
                try
                {
                    var nodeText = document.Descendants(item).First().Value;
                    toReturn.Add(item, nodeText);
                }
                catch{}
            }
            return toReturn;
        }
    }
}
