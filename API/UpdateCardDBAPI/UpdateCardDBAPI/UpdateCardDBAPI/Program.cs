using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace UpdateCardDBAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Variabile usata per testare il tempo di esecuzione della richiesta
            Stopwatch getTimeWork = new Stopwatch();

            string mtgUrlAPI = "";
            string yugiohUrlAPI = "";
            string pokemonUrlAPI = "";


            getTimeWork.Restart();

            Console.WriteLine("Request Card");

            if (mtgUrlAPI != "")
            {
                //Stringa Json dalla chiamata Api
                string mtgJsonAPI = "";

                mtgJsonAPI = GetAPIRequest(mtgUrlAPI);

                if(mtgJsonAPI != "")
                {

                }

                //Conversione da Json String a Json
                var allJsonContent = JObject.Parse(mtgJsonAPI);
            }

            if (yugiohUrlAPI != "")
            {
                //Stringa Json dalla chiamata Api
                string yugiohJsonAPI = "";

                yugiohJsonAPI = GetAPIRequest(yugiohUrlAPI);

                if (yugiohJsonAPI != "")
                {
                    //Conversione da Json String a Json
                    var allJsonContent = JObject.Parse(yugiohJsonAPI);
                }
            }

            if (pokemonUrlAPI != "")
            {
                //Stringa Json dalla chiamata Api
                string pokemonJsonAPI = "";

                pokemonJsonAPI = GetAPIRequest(pokemonUrlAPI);

                if (pokemonJsonAPI != "")
                {
                    //Conversione da Json String a Json
                    var allJsonContent = JObject.Parse(pokemonJsonAPI);
                }
            }



            getTimeWork.Stop();

            Console.WriteLine(getTimeWork.ElapsedMilliseconds);


        }

        static string GetAPIRequest(string url)
        {
            //Stringa Json dalla chiamata Api
            string jsonStringAPI = "";

            //Chiamata Api
            Uri address = new Uri(url);
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "text/json";

            //Salvataggio su "jsonString" della risposta
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                jsonStringAPI = reader.ReadToEnd();
            }

            return jsonStringAPI;
        }
    }
}
