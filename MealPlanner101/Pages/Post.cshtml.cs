using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

//using Microsoft.Azure.CognitiveServices.Search.WebSearch;
//using Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;

using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;

using Cloudmersive.APIClient.NET.ImageRecognition.Api;
using Cloudmersive.APIClient.NET.ImageRecognition.Client;
using Cloudmersive.APIClient.NET.ImageRecognition.Model;

namespace MealPlanner101.Pages
{
    public class PostModel : PageModel
    {
        public void OnGet()
        {
        }

        public void OnPost()
        {
            ViewData["test"] = "";
            string url = Request.Form["user_in"];
            string twitter_button = GetTwitterButton(url);
            ViewData["test"] = twitter_button;

        }

        public static string GetTwitterButton(string url)
        {
            // Credentials:
            var IMGUR_ID = "12115820ce1d5b6";
            var IMGUR_SECRET = "dc71545b6d4f107e03d47b9dcab2268070e1658d";

            var CLOUDMERSIVE_ID = "28e63794-ef8a-4616-80bb-26fdd3709a19";

            // Download file:
            string imagesDir = "wwwroot/Images/";

            if (System.IO.File.Exists(imagesDir + "image.jpeg")) System.IO.File.Delete(imagesDir + "image.jpeg");

            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(new Uri(url), imagesDir + "image.jpeg");
            }

            // Imgur API:
            var client = new ImgurClient(IMGUR_ID, IMGUR_SECRET);
            var endpoint = new ImageEndpoint(client);

            IImage imgur_image;
            using (var fs = new FileStream(imagesDir + "image.jpeg", FileMode.Open))
            {
                imgur_image = Task.Run(async () => { return await endpoint.UploadImageStreamAsync(fs); }).Result;
            }


            var imgur_link = imgur_image.Link;
            Console.WriteLine("Imgur link: " + imgur_link);

            // Cloudmersive API:

            Configuration.Default.AddApiKey("Apikey", CLOUDMERSIVE_ID);
            var cm_API = new RecognizeApi();

            string description = "";
            try
            {
                description = cm_API.RecognizeDescribe(new System.IO.FileStream(imagesDir + "image.jpeg", System.IO.FileMode.Open)).BestOutcome.Description;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                description = "something the Cloudmersive API could not identify.";
                //return "";
            }
            Console.WriteLine("DESCRIPTION: " + description);

            // Twitter button:
            string tweet_body = "Here is a picture of " + description + " Posted through MealPlanner101: " + imgur_link;
            string tweet_body_url = "";
            foreach (var tweet_char in tweet_body)
            {
                if (tweet_char == ' ')
                {
                    tweet_body_url += "%20";
                }
                else
                {
                    tweet_body_url += tweet_char;
                }
            }

            string tweet_button =

            "<a class = \"twitter-share-button\" href = https://twitter.com/intent/tweet?text=" + tweet_body_url + ">Tweet</a>";

            Console.WriteLine("Tweet HTML: " + tweet_button);

            return tweet_button;

        }

    }
}