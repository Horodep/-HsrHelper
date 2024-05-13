namespace HsrHelper
{
    public class Character
    {
        public string name { get; private set; }
        public string body { get; private set; }
        public string feet { get; private set; }
        public string rope { get; private set; }
        public string sphere { get; private set; }
        public string comments { get; private set; }
        public string substats { get; private set; }
        public bool enabled { get; set; }

        public Character(string name, string body, string feet, string rope, string sphere, string comments, string substats)
        {
            this.name = name;
            this.body = body;
            this.feet = feet;
            this.rope = rope;
            this.sphere = sphere;
            this.comments = comments;
            this.substats = substats;
            this.enabled = true;
        }
    }
}