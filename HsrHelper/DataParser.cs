using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace HsrHelper
{
    public static class DataParser
    {
        private static Dictionary<string, JObject> charRawData = new Dictionary<string, JObject>();


        public static Dictionary<string, Character> CharList = new Dictionary<string, Character>();

        public static List<RelicSetToCharacter> RelicListToCharacters = new List<RelicSetToCharacter>();

        public static List<PlanarSetToCharacter> PlanarListToCharacters = new List<PlanarSetToCharacter>();

        public static List<RelicSet> RelicListTitles = new List<RelicSet> { new RelicSet("", -1, "", "") };

        public static List<PlanarSet> PlanarListTitles = new List<PlanarSet> { new PlanarSet("", -1, "") };


        public static void Initialize()
        {
            charRawData.Clear();
            CharList.Clear();
            RelicListToCharacters.Clear();
            PlanarListToCharacters.Clear();

            RelicListTitles.Clear();
            RelicListTitles.Add(new RelicSet("", -1, "", ""));
            PlanarListTitles.Clear();
            PlanarListTitles.Add(new PlanarSet("", -1, ""));

            ReadCharacterDataFromFiles();
            ReadAndParceRelicData();
            ParceCharacterData();
        }

        private static void ReadCharacterDataFromFiles()
        {
            if (!File.Exists("./data/tier-list.json"))
                return;

            using StreamReader file = File.OpenText("./data/tier-list.json");
            using JsonTextReader reader = new JsonTextReader(file);
            JObject tierListObj = (JObject)JToken.ReadFrom(reader);
            JArray tierListArray = (JArray)tierListObj["result"]["data"]["allCharacters"]["nodes"];

            foreach (var character in tierListArray)
            {
                using StreamReader characterFile = File.OpenText("./data/" + (string)character["slug"] + ".json");
                using JsonTextReader characterReader = new JsonTextReader(characterFile);
                JObject characterObj = (JObject)JToken.ReadFrom(characterReader);

                charRawData.Add((string)character["slug"], (JObject)characterObj["result"]["data"]["currentUnit"]["nodes"][0]);
            }
        }

        private static void ParceCharacterData()
        {
            foreach (var entry in charRawData)
            {
                if (!entry.Value["buildData"].HasValues) continue;

                JObject build = (JObject)entry.Value["buildData"][0];

                JArray bodyArr = (JArray)build["body"];
                JArray feetArr = (JArray)build["feet"];
                JArray ropeArr = (JArray)build["rope"];
                JArray sphereArr = (JArray)build["sphere"];

                string body = (string)bodyArr[0]["stat"] + (bodyArr.Count > 1 ? " / " + (string)bodyArr[1]["stat"] : "");
                string feet = (string)feetArr[0]["stat"] + (feetArr.Count > 1 ? " / " + (string)feetArr[1]["stat"] : "");
                string rope = (string)ropeArr[0]["stat"] + (ropeArr.Count > 1 ? " / " + (string)ropeArr[1]["stat"] : "");
                string sphere = (string)sphereArr[0]["stat"] + (sphereArr.Count > 1 ? " / " + (string)sphereArr[1]["stat"] : "");

                var character = new Character(
                    (string)entry.Value["name"],
                    body, feet, rope, sphere,
                    (string)build["comments"],
                    (string)build["substats"]);

                character.enabled = Config.ReadINI("Characters", character.name) == "False" ? false : true;

                CharList.Add(character.name, character);

                JArray charRelics = (JArray)build["relics"];

                int index = 1;

                foreach (var relicSet in charRelics)
                {
                    RelicListToCharacters.Add(
                        new RelicSetToCharacter(
                            character,
                            "T" + index++,
                            (string)relicSet["relic"] == "" ? null : RelicListTitles.Find(i => i.name == (string)relicSet["relic"]),
                            (string)relicSet["relic_2"] == "" ? null : RelicListTitles.Find(i => i.name == (string)relicSet["relic_2"]),
                            (string)relicSet["value"] == "" || (string)relicSet["value"] == null ? 0.0 :
                                Double.Parse(((string)relicSet["value"]).Replace("%", "").Replace(".", ",")),
                            (string)relicSet["comment"],
                            (string)relicSet["notes"]
                        ));
                }

                JArray charPlanars = (JArray)build["planars"];

                index = 1;

                foreach (var planarSet in charPlanars)
                {
                    PlanarListToCharacters.Add(
                        new PlanarSetToCharacter(
                            character,
                            "T" + index++,
                            (string)planarSet["planar"] == "" ? null : PlanarListTitles.Find(i => i.name == (string)planarSet["planar"]),
                            (string)planarSet["value"] == "" || (string)planarSet["value"] == null ? 0.0 :
                                Double.Parse(((string)planarSet["value"]).Replace("%", "").Replace(".", ",")),
                            (string)planarSet["comment"],
                            (string)planarSet["notes"]
                        ));
                }
            }
        }

        private static void ReadAndParceRelicData()
        {
            if (!File.Exists("./data/relic-sets.json"))
                return;

            using StreamReader file = File.OpenText("./data/relic-sets.json");
            using JsonTextReader reader = new JsonTextReader(file);
            JObject obj = (JObject)JToken.ReadFrom(reader);
            JArray relicsArray = (JArray)obj["result"]["data"]["allCharacters"]["nodes"];

            foreach (var relics in relicsArray)
            {
                if ((string)relics["type"] == "Relic Set")
                    RelicListTitles.Add(new RelicSet((string)relics["name"], (int)relics["relicId"], (string)relics["bonus2"]["raw"], (string)relics["bonus4"]["raw"]));
                else
                    PlanarListTitles.Add(new PlanarSet((string)relics["name"], (int)relics["relicId"], (string)relics["bonus2"]["raw"]));
            }
        }
    }
}