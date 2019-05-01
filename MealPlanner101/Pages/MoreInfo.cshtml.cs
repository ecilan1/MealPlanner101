using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace MealPlanner101.Pages
{
    public class MoreInfoModel : PageModel
    {
        string url_for_uTube_api = "https://www.googleapis.com/youtube/v3/search?part=snippet&q=mannan&key=AIzaSyBYO-mAglB8uQUInLmHz8x3J0ecRLfccVI";
        public string Message { get; set; } = "";
        public bool isChecked { get; set; }
        public JObject jObj { get; set; }
        public JObject jObj_recipe { get; set; }
        [BindProperty]
        public List<int> AreChecked { get; set; }
        [BindProperty(SupportsGet = true)]
        public Int32 Num { get; set; }
        public void OnGet()
        {

            string filepath = "json1.json";
            string filepath_recipe = "json2.json";
            string result = string.Empty;
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                jObj = JObject.Parse(json);
            }
            using (StreamReader r = new StreamReader(filepath_recipe))
            {
                var json = r.ReadToEnd();
                jObj_recipe = JObject.Parse(json);
            }
            if (Num == null)
            {
                Num = 0;
            }
            else if (Num >= jObj_recipe["hits"].Count())
            {
                Num = 0;
            }
            Message = Num.ToString();
        }

        public void OnPost()
        {
            string filepath = "json1.json";
            string filepath_recipe = "json.json";
            string result = string.Empty;
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                jObj = JObject.Parse(json);
            }
            using (StreamReader r = new StreamReader(filepath_recipe))
            {
                var json = r.ReadToEnd();
                jObj_recipe = JObject.Parse(json);
            }

            filepath = "json.json";

            JArray ingredients;
            JObject jsonData;
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                jsonData = JObject.Parse(json);

                ingredients = JArray.FromObject(jsonData["hits"][0]["recipe"]["ingredients"]);
            }

            ArrayList keywords = new ArrayList();
            for (int i = 0; i < AreChecked.Count(); ++i)
            {
                keywords.Add(ingredients[(int)AreChecked[i] - 1]["text"].ToString());
            }

            string link = GetAddToCartLink((string[])keywords.ToArray(typeof(string)));

            ViewData["AddToCart"] += "<script>window.open(\"" + link + "\", \"addtocart\");</script>";

        }



        public static string GetAddToCartLink(string[] keywords)
        {

            string link = "https://www.amazon.com/gp/aws/cart/add.html?";// + ASIN + "&Quantity.1=2";

            for (int i = 0; i < keywords.Length; i++)
            {

                string query = String.Empty;
                for (int j = 0; j < keywords[i].Length; j++)
                {
                    if (keywords[i][j] == ' ')
                    {
                        query += '+';
                    }
                    else
                    {
                        query += keywords[i][j];
                    }
                }

                query += "+site:amazon.com";

                string uri = "https://api.cognitive.microsoft.com/bing/v7.0/search?q=" + query + "&count=10&offset=0&mkt=en-us&safesearch=Moderate";

                try
                {

                    var request = System.Net.WebRequest.Create(uri);

                    if (request == null)
                    {
                        Console.WriteLine("Request is NULL.");
                        continue;
                    }

                    // Send request.
                    request.Method = "GET";
                    request.ContentType = "application/json";

                    request.Headers.Add("Host", "api.cognitive.microsoft.com");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", "0b62faa2edbf431cb87c66b1af5c9909");

                    string jsonResponse;
                    using (System.IO.Stream s = request.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            jsonResponse = sr.ReadToEnd();
                        }
                    }

                    // Get the first URL from the results that matches the appropriate product page syntax.
                    JObject jObj = JObject.Parse(jsonResponse);
                    JArray JArr = JArray.Parse(jObj["webPages"]["value"].ToString());

                    string URL = "";

                    Regex regex = new Regex(@"http(s?)\:\/\/www\.amazon\.com\/.+\/dp\/.+");
                    for (int j = 0; j < 10; j++)
                    {

                        if (regex.IsMatch(JArr[j]["url"].ToString()))
                        {
                            URL = JArr[j]["url"].ToString();
                            break;
                        }
                    }


                    if (URL == "")
                    {
                        Console.WriteLine("Did not find URL.");
                        continue;
                    }

                    // Get ASIN
                    string ASIN = "";
                    Boolean foundDp = false;
                    for (int j = 4; j < URL.Length; j++)
                    {
                        if (URL.Substring(j - 4, 4) == "/dp/")
                        {
                            foundDp = true;
                        }
                        if (foundDp)
                        {
                            ASIN += URL[j];
                        }
                    }

                    link += "ASIN." + (i + 1) + "=" + ASIN + "&Quantity." + (i + 1) + "=2";
                    if (i != keywords.Length - 1)
                    {
                        link += "&";
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occured calculating link for ingredient {0}:\n{1}", i, e.Message);
                    continue;
                }
            }

            return link;

        }

    }

}