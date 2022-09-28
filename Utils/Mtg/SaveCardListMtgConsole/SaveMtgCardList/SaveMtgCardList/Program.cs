using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace SaveMtgCardList
{
    class Program
    {

        //Url API
        static string apiUrl = "https://api.scryfall.com/cards/search?order=cmc&q=a";
        //static string apiUrl = "https://api.scryfall.com/cards/search?order=cmc&q=brainstorm";    //Usata per Test


        //Dove verrà salvata la risposta dell'API elaborata
        static List<Dictionary<string, object>> apiResList = new List<Dictionary<string, object>>();

        static void Main(string[] args)
        {
            Console.Title = "Console Mtg API";
            Console.WriteLine("Start API Request");

            Stopwatch checkTime = new Stopwatch();

            checkTime.Start();

            //Chiamata API
            callApi();

            //Creo metodo per salvare i dati su MongoDb

            checkTime.Stop();

            Console.WriteLine("End API Request");
        }

        static void callApi()
        {
            //Stringa Json dalla chiamata Api
            string jsonString = "";


            //Chiamata Api
            Uri address = new Uri(apiUrl);
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "text/json";

            try
            {
                //Salvataggio su "jsonString" della risposta
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    jsonString = reader.ReadToEnd();
                }

                //Conversione da Json String a Json
                var allJsonContent = JObject.Parse(jsonString);

                if (allJsonContent.ContainsKey("next_page"))
                {
                    Console.WriteLine($"[{DateTime.Now}] Yes");

                    //Nuova API da chiamare
                    apiUrl = allJsonContent["next_page"].ToString();

                    //Tempo di attesa per l'API
                    Thread.Sleep(200);

                    //RIchiama l'API
                    callApi();
                }
                else
                {

                    Console.WriteLine($"[{DateTime.Now}] Nope (Lista creata correttamente)");

                    // Per Test
                    var test = allJsonContent["data"].Where(p => p["name"].ToString().ToLower().Contains("brainstorm")).ToList();
                }

                /*

                //Estrazione dei dati
                apiResList = allJsonContent["data"].Select(s => new Dictionary<string, object> {
                            { "name", s["name"].ToString() },
                            { "type", s["type"].ToString()},
                            { "race", s["race"].ToString()},
                            { "desc", s["desc"].ToString()},
                            { "attribute", s.ToObject<JObject>().ContainsKey("attribute") == true && s["atk"].ToString() != "" ? s["attribute"].ToString() : "Null" },
                            { "atk", s.ToObject<JObject>().ContainsKey("atk") == true && s["atk"].ToString() != "" ? int.Parse(s["atk"].ToString()) : 0},
                            { "def", s.ToObject<JObject>().ContainsKey("def") == true && s["def"].ToString() != "" ? int.Parse(s["def"].ToString()) : 0},
                            { "level", s.ToObject<JObject>().ContainsKey("level") == true && s["level"].ToString() != "" ? int.Parse(s["level"].ToString()) : 0},
                            { "linkval", s.ToObject<JObject>().ContainsKey("linkval") == true && s["linkval"].ToString() != "" ? int.Parse(s["linkval"].ToString()) : 0},
                            { "imageId", s["card_images"][0]["id"].ToString()},
                            { "price", s["card_prices"][0]["cardmarket_price"].ToString()},
                            { "sets", s.ToObject<JObject>().ContainsKey("card_sets") == true ? s["card_sets"].Select(p => new Dictionary<string, object> {
                                { "set", p["set_code"].ToString() }
                            }).ToList() : new List<Dictionary<string, object>>()}
                                })
                                .ToList();

                */



            }
            catch (Exception err)
            {
                string Message = $"{err.StackTrace}\n{err.Message}";
                Console.WriteLine(Message);
            }
        }
    }
}
