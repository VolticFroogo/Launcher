using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Launcher
{
    class Network
    {
        public static Message Get()
        {
            Console.WriteLine("Contacting server for any new information.");

            // Create a string to store received data.
            string html = string.Empty;

            // Create a string to store the URL.
            string url;

            // If the version is in the config:
            if (Config.I.Version != string.Empty)
                // Send the version in the URL.
                url = string.Format("https://{0}/api/program?id={1}&version={2}", Config.I.Domain, Config.I.ID, Config.I.Version);
            // Otherwise:
            else
                // Send initial in the URL.
                url = string.Format("https://{0}/api/program?id={1}&initial=t", Config.I.Domain, Config.I.ID);

            // Create an HTTP request.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            // Get response and read data.
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            // Deserialize JSON data into the message.
            var msg = JsonConvert.DeserializeObject<Message>(html);

            return msg;
        }
    }
}
