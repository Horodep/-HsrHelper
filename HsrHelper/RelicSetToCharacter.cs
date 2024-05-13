namespace HsrHelper
{
    public class RelicSetToCharacter
    {
        public Character character { get; private set; }
        public string tier { get; private set; }
        public RelicSet relic { get; private set; }
        public RelicSet relic_2 { get; private set; }
        public double value { get; private set; }
        public string comment { get; private set; }
        public string notes { get; private set; }
        public bool flag { get; set; }

        public RelicSetToCharacter(Character character, string tier, RelicSet relic, RelicSet relic_2, double value, string comment, string notes)
        {
            this.character = character;
            this.tier = tier;
            this.relic = relic;
            this.relic_2 = relic_2;
            this.value = value;
            this.comment = comment;
            this.notes = notes;
        }
    }
}