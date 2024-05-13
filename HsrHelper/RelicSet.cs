using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;

namespace HsrHelper
{
    public class RelicSet
    {
        public string name { get; private set; }
        public int id { get; private set; }
        public string bonus2 { get; private set; }
        public string bonus4 { get; private set; }
        public string imageUri => "images/" + id + ".png";
        public BitmapImage image { get; private set; }

        public RelicSet(string name, int id, string bonus2, string bonus4)
        {
            this.name = name;
            this.id = id;
            this.bonus2 = "";
            this.bonus4 = "";

            if (id == -1)
                return;

            image = new BitmapImage();

            try
            {
                image.BeginInit();
                image.UriSource = new Uri(imageUri, UriKind.Relative);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
            }
            catch { }

            JObject b2 = JObject.Parse(bonus2.Replace("\\\"", "\""));
            JArray b2arr = (JArray)b2["content"][0]["content"];
            foreach (var item in b2arr)
                this.bonus2 += (string)item["value"];

            JObject b4 = JObject.Parse(bonus4.Replace("\\\"", "\""));
            JArray b4arr = (JArray)b4["content"][0]["content"];
            foreach (var item in b4arr)
                this.bonus4 += (string)item["value"];
        }
    }
}