using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MealPlanner101.Pages
{
    public class RecipesModel : PageModel
    {
        public String Message { get; set; } = "Page model in C#";
        [BindProperty(SupportsGet = true)]
        public string T { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        public void OnGet()
        {
            Message += "Hello There mate";
        }
        public void OnPost()
        {
            var recipe = Request.Form["Recipe"];
            System.Net.WebClient client = new System.Net.WebClient();
            //https://api.edamam.com/search?q=chicken&app_id=4c09efeb&app_key=370e1f80e62548d4f94d4a5c40e0eed3&from=0&to=3&calories=591-722&health=alcohol-free
            //string downloadString = client.DownloadString("https://api.edamam.com/search?q=chicken&app_id=4c09efeb&app_key=370e1f80e62548d4f94d4a5c40e0eed3&from=0&to=3&calories=591-722&health=alcohol-free");
            //dynamic stuff1 = Newtonsoft.Json.JsonConvert.DeserializeObject(downloadString);
            //Message = (String)stuff1;
            string filepath = "json.json";
            string filepath_write = "json2.json";
            string result = string.Empty;
            JObject jObj;
            using (StreamReader r = new StreamReader(filepath))
            {
                var json = r.ReadToEnd();
                jObj = JObject.Parse(json);
                
                result = jObj["hits"][0]["recipe"].ToString();
            }
            using (StreamWriter file = new StreamWriter(filepath_write))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                jObj.WriteTo(writer);
            }
            Message = result;
            ViewData["confirmation"] = "<div class=\"container\">";
            bool b = false;
            for (int i = 0; i < jObj["hits"].Count(); ++i)
            {
                string dietLabels = "";
                for(int j = 0; j<jObj["hits"][i]["recipe"]["dietLabels"].Count(); ++j)
                {
                    dietLabels += jObj["hits"][i]["recipe"]["dietLabels"][j];
                    dietLabels += " ";
                }
                //dietLabels += "</p>";
                string healthLabels = "";
                for (int j = 0; j < jObj["hits"][i]["recipe"]["healthLabels"].Count(); ++j)
                {
                    healthLabels += jObj["hits"][i]["recipe"]["healthLabels"][j];
                    healthLabels += " ";
                }
                if (b)
                {
                    ViewData["confirmation"] += "<div class=\"card bg-info col-sm-push-1 col-sm-5\">" +
                            "<div class=\"row no-gutters\">" +
                                "<div class=\"col-md-6\">" +
                                    "<img src=\"" + jObj["hits"][i]["recipe"]["image"] + "\" class=\"card-img col-md-12\" alt=\"Food pic\" style=\"max-height:184px\">" +
                                "</div>" +
                                "<div class=\"col-md-6\">" +
                                    "<div class=\"card-body\">" +
                                        "<h2 class=\"card-title\">" + jObj["hits"][i]["recipe"]["label"] + "</h2>" +
                                        "<p class=\"card-text\">"+"<p><h4>Diet Labels:</h4> " +dietLabels + "</p><h4>Health Labels:</h4> " + healthLabels + "</p>" +
                                        "<div class=\"row\">" +
                                        "<div style=\"padding-left:20px\">" +
                                            "<a href=\"/MoreInfo?num=" + i + "\" class=\"btn btn-primary\">More Info</a>" +
                                            "<a href = \"" + jObj["hits"][i]["recipe"]["url"] + "\" target = \"_blank\" \" class=\"btn btn-primary\">Recipe</a>" +
                                        "</div>" +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                            "</div>" +
                        "</div>" + "</div>" + "&nbsp";
                }
                else
                {
                    ViewData["confirmation"] += "<div class=\"row \">" +
                        "<div class=\"card bg-info col-sm-5\">" +
                            "<div class=\"row no-gutters\">" +
                                "<div class=\"col-md-6\">" +
                                    "<img src=\"" + jObj["hits"][i]["recipe"]["image"] + "\" class=\"card-img col-md-12\" alt=\"Food pic\" style=\"max-height:184px\">" +
                                "</div>" +
                                "<div class=\"col-md-6\">" +
                                    "<div class=\"card-body\">" +
                                        "<h2 class=\"card-title\">" + jObj["hits"][i]["recipe"]["label"] + "</h2>" +
                                        "<p class=\"card-text\">" + "<p><h4>Diet Labels:</h4> " + dietLabels + "</p><h4>Health Labels:</h4> " + healthLabels + "</p>" +

                                        "<div class=\"row\">" +
                                        "<div style=\"padding-left:20px\">" +
                                            "<a href=\"/MoreInfo?num=" + i + "\" class=\"btn btn-primary\">More Info</a>" +
                                            "<a href = \"" + jObj["hits"][i]["recipe"]["url"] + "\" target = \"_blank\" \" class=\"btn btn-primary\">Recipe</a>" +
                                        "</div>" +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                            "</div>" +
                        "</div>";
                }
                b = !b;
            }
            ViewData["confirmation"] += "</div>";
            //ViewData["Recipe Display"] = 
        }
    }
}