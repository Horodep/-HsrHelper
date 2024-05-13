using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace HsrHelper
{
    public static class WebLoader
    {
        private static int loaded = 0;
        private static int total = 0;

        public static void LoadData(Action<int> updateProgressBar)
        {
            if (!Directory.Exists("./data")) Directory.CreateDirectory("./data");
            if (!Directory.Exists("./images")) Directory.CreateDirectory("./images");

            LoadWebJson("https://www.prydwen.gg/page-data/star-rail/guides/relic-sets/page-data.json", "./data/relic-sets.json");
            LoadWebJson("https://www.prydwen.gg/page-data/star-rail/tier-list/page-data.json", "./data/tier-list.json");

            using StreamReader file1 = File.OpenText("./data/tier-list.json");
            using JsonTextReader reader1 = new JsonTextReader(file1);
            JObject tierListObj = (JObject)JToken.ReadFrom(reader1);
            JArray tierListArray = (JArray)tierListObj["result"]["data"]["allCharacters"]["nodes"];

            using StreamReader file2 = File.OpenText("./data/relic-sets.json");
            using JsonTextReader reader2 = new JsonTextReader(file2);
            JObject relicsObj = (JObject)JToken.ReadFrom(reader2);
            JArray relicsArray = (JArray)relicsObj["result"]["data"]["allCharacters"]["nodes"];

            total = tierListArray.Count + relicsArray.Count;

            foreach (var character in tierListArray)
            {
                LoadCharacterJson((string)character["slug"]);

                loaded++;
                updateProgressBar(100 * loaded / total);
            }

            foreach (var relics in relicsArray)
            {
                var id = (string)relics["relicId"];
                var uri = (string)relics["image"]["localFile"]["childImageSharp"]["gatsbyImageData"]["images"]["fallback"]["src"];

                LoadWebJson("https://www.prydwen.gg" + uri, "./images/" + id + ".png");

                loaded++;
                updateProgressBar(100 * loaded / total);
            }
        }

        private static void LoadCharacterJson(string characterName)
        {
            LoadWebJson("https://www.prydwen.gg/page-data/star-rail/characters/" + characterName + "/page-data.json", "./data/" + characterName + ".json");
        }

        private static void LoadWebJson(string url, string localPath)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, localPath);
            }

        }
    }
}