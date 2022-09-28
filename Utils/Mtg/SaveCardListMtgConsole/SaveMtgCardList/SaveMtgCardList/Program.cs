using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace SaveMtgCardList
{
    class Program
    {
        //Url API
        static string apiUrl = "https://api.scryfall.com/cards/search?order=cmc&q=e";
        //static string apiUrl = "https://api.scryfall.com/cards/search?order=cmc&q=brainstorm";    //Usata per Test

        //Contatore delle varie pagine dci risposta dell'API
        static int countPage = 0;

        //Numero carte totali, modificato dall'api
        static int totalCards = -1;

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

            //Aumento il counter delle pagine
            countPage++;

            Console.WriteLine($"[{DateTime.Now}] -> {countPage}");

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

                //Salvo il numero totale di carte
                if (totalCards == -1)
                {
                    totalCards = int.Parse(allJsonContent["total_cards"].ToString());

                    Console.WriteLine($"Numero totale di carte: {totalCards}");
                }

                //Salva tutti i dati nel db
                var jsonFromAPI = allJsonContent["data"].Select(s => new Dictionary<string, object> {
                            { "id", s["id"].ToString() },
                            { "name", s["name"].ToString() },
                            { "scryfall_uri", s["scryfall_uri"].ToString()},
                            { "oracle_text", s["oracle_text"].ToString()},
                            { "type_line", s["type_line"].ToString()},
                            { "img", s["image_uris"]["normal"].ToString()},
                            { "mana_cost", s["mana_cost"].ToString()},
                            { "cmc", s["cmc"].ToString()},
                            { "colors", s["colors"].ToString()},
                            { "color_identity", s["color_identity"].ToString()},
                            { "reserved", s["reserved"].ToString()},
                            { "price", s["prices"]["eur"].ToString()},
                            { "legalities", s["legalities"].Select(p => new Dictionary<string, object> {
                                            { "standard", p["standard"].ToString() },
                                            { "future", p["future"].ToString() },
                                            { "historic", p["historic"].ToString() },
                                            { "gladiator", p["gladiator"].ToString() },
                                            { "pioneer", p["pioneer"].ToString() },
                                            { "explorer", p["explorer"].ToString() },
                                            { "modern", p["modern"].ToString() },
                                            { "legacy", p["legacy"].ToString() },
                                            { "pauper", p["pauper"].ToString() },
                                            { "vintage", p["vintage"].ToString() },
                                            { "penny", p["penny"].ToString() },
                                            { "commander", p["commander"].ToString() },
                                            { "brawl", p["brawl"].ToString() },
                                            { "historicbrawl", p["historicbrawl"].ToString() },
                                            { "alchemy", p["alchemy"].ToString() },
                                            { "paupercommander", p["paupercommander"].ToString() },
                                            { "duel", p["duel"].ToString() },
                                            { "oldschool", p["oldschool"].ToString() },
                                            { "premodern", p["premodern"].ToString() },
                                        })
                            .ToList()} }).ToList();

                //Per ogni elemento preso dall'API (lo salvo su MongoDB)
                foreach (var item in jsonFromAPI)
                {
                    try
                    {
                        //Controllo connessione MongoDB
                        if (MongoDB.IsConnected())
                        {
                            //Prende la lista delle Collection e controlla se esiste la collection MtgCards
                            //DA TESTARE
                            var listCollection = MongoDB.Client.GetDatabase("a").ListCollectionNames();
                            //SE NON TROVA NESSUNA CORRISPONDENZA NEL NOME
                            if (listCollection.Where(s => s).Count() == 0)
                            {
                                //CREA LA COLLECTION
                            }

                            //Prendo tutti gli elementi esistenti nella collection
                            var mongoDbCardList = MongoDB.Client
                    .GetDatabase("a")
                    .GetCollection<Dictionary<string, object>>("a")
                   .Find(Builders<Dictionary<string, object>>.Filter.Empty)
                   .ToList();

                            //Controllo se esiste già e se non esiste la inserisco
                            if (mongoDbCardList.Where(s => s["name"].ToString().ToLower().Equals(item["name"].ToString().ToLower())).Count() == 0)
                            {
                                //Nuovo Dict da inserire
                                Dictionary<string, object> dictToInsert = new Dictionary<string, object>();
                                dictToInsert.Add("id", item["id"].ToString());
                                dictToInsert.Add("name", item["name"].ToString());
                                dictToInsert.Add("scryfall_uri", item["scryfall_uri"].ToString());
                                dictToInsert.Add("oracle_text", item["oracle_text"].ToString());
                                dictToInsert.Add("type_line", item["type_line"].ToString());
                                dictToInsert.Add("img", item["img"].ToString());
                                dictToInsert.Add("mana_cost", item["mana_cost"].ToString());
                                dictToInsert.Add("cmc", item["cmc"].ToString());
                                dictToInsert.Add("colors", item["colors"].ToString());
                                dictToInsert.Add("color_identity", item["color_identity"].ToString());
                                dictToInsert.Add("reserved", item["reserved"].ToString());
                                dictToInsert.Add("price", item["price"].ToString());
                                dictToInsert.Add("legalities", item["legalities"].ToString());

                                //InsertOne MongoDb
                                MongoDB.Client.GetDatabase("a")
                                    .GetCollection<Dictionary<string, object>>("a")
                                    .InsertOne(dictToInsert);
                            }

                            /*
                            //NEL CASO DI UPDATE, TENGO PER IL FUTURO TEST
                            if (action == EAction.Update)
                            {
                                //Filtro di ricerca
                                var searchFilter = Builders<Dictionary<string, object>>.Filter.Eq("name", cardNameOld)
                                     & Builders<Dictionary<string, object>>.Filter.Eq("nrCopies", nrCopiesOld);

                                //Update MongoDb
                                MongoDB.Client.GetDatabase("yugiohCardDb").GetCollection<Dictionary<string, object>>(nameCollection + "Wishlist")
                                        .UpdateOne(searchFilter, Builders<Dictionary<string, object>>
                                        .Update.Set("name", cardName).Set("nrCopies", nrCopies));

                            }
                            */
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine($"{item["name"]}\n{err.StackTrace}\n{err.Message}");
                    }
                }

                if (allJsonContent.ContainsKey("next_page"))
                {
                    //Nuova API da chiamare
                    apiUrl = allJsonContent["next_page"].ToString();

                    //Tempo di attesa per l'API (Richiesti 100 ms tra le varie chiamate, considerando il resto del codice potrebbe anche non servire)
                    Thread.Sleep(100);

                    //RIchiama l'API
                    callApi();
                }
                else
                {
                    //Fine inserimento carte
                    Console.WriteLine($"[{DateTime.Now}] -> END (Lista creata/modificata correttamente)");

                }
            }
            catch (Exception err)
            {
                //In caso di errore con API o altro
                string Message = $"{err.StackTrace}\n{err.Message}";
                Console.WriteLine(Message);
            }
        }
    }
}
