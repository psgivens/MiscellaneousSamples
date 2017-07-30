using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CsodRestClients
{
    public static class RestImplementation
    {
        public static TokenSession PostWithKey(Uri uri, string apiKey, string apiSecret, DateTime time)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(uri);

            var hash = new Hashtable();
            hash.Add("x-csod-api-key", apiKey);
            hash.Add("x-csod-date", time.ToString("yyyy-MM-ddTHH:mm:ss.000"));
            request.Method = "POST";

            var stringToSign = ConstructStringToSign(request.Method, hash, uri.AbsolutePath);
            var sig = SignString512(stringToSign, apiSecret);
            hash.Add("x-csod-signature", sig);

            foreach (var item
                in (from DictionaryEntry pair in hash
                    let key = (string)pair.Key
                    orderby key
                    select new
                    {
                        Key = key,
                        Value = (string)pair.Value
                    }))
            {
                request.Headers.Add(item.Key, item.Value);
            }

            request.ContentType = "text/xml";
            request.Timeout = 999999;
            request.ContentLength = 0;

            request.Accept = "text/xml";

            using (var response = request.GetResponse())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            using (var reader = new XmlTextReader(streamReader))
            {
                var currentNode = string.Empty;
                var session = new TokenSession();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            currentNode = reader.Name;
                            break;
                        case XmlNodeType.Text:
                            switch (currentNode)
                            {
                                case "a:Token":
                                    session.Token = reader.Value;
                                    break;
                                case "a:Secret":
                                    session.Secret = reader.Value;
                                    break;
                                case "a:Alias":
                                    session.Alias = reader.Value;
                                    break;
                                case "a:ExpiresOn":
                                    session.ExpiresOn = DateTime.Parse(reader.Value);
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            currentNode = string.Empty;
                            break;
                    }
                }
                return session;
            }
        }

        public static string ConstructStringToSign(string httpMethod, Hashtable headers, string pathAndQuery)
        {
            StringBuilder stringToSign = new StringBuilder();
            var httpVerb = httpMethod.Trim() + "\n";

            var csodHeaders = (from DictionaryEntry pair in headers
                               let key = (string)pair.Key
                               where key.StartsWith("x-csod-") && key != "x-csod-signature"
                               orderby key
                               select string.Format("{0}:{1}\n", key, pair.Value))
                     .Distinct()
                     .Aggregate(string.Empty, (a, l) => a + l);

            //var csodHeaders = headers.Cast<string>().Where(w => w.StartsWith("x-csod-"))
            //                                        .Where(w => w != "x-csod-signature")
            //                                        .Distinct()
            //                                        .OrderBy(s => s)
            //                                        .Aggregate(string.Empty, (a, l) => a + l.ToLower().Trim() + ":" + headers[l].Trim() + "\n");


            stringToSign.Append(httpVerb);
            stringToSign.Append(csodHeaders);
            stringToSign.Append(pathAndQuery);
            return stringToSign.ToString();
        }

        public static string SignString512(string stringToSign, string secretKey)
        {
            byte[] secretkeyBytes = Convert.FromBase64String(secretKey);
            byte[] inputBytes = Encoding.UTF8.GetBytes(stringToSign);
            using (var hmac = new HMACSHA512(secretkeyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                return System.Convert.ToBase64String(hashValue);
            }
        }

    }
}
