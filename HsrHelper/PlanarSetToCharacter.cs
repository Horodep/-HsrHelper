namespace HsrHelper
{
    public class PlanarSetToCharacter
    {
        public Character character { get; private set; }
        public string tier { get; private set; }
        public PlanarSet planar { get; private set; }
        public double value { get; private set; }
        public string comment { get; private set; }
        public string notes { get; private set; }
        public bool flag { get; set; }

        public PlanarSetToCharacter(Character character, string tier, PlanarSet planar, double value, string comment, string notes)
        {
            this.character = character;
            this.tier = tier;
            this.planar = planar;
            this.value = value;
            this.comment = comment;
            this.notes = notes;
        }
    }
}