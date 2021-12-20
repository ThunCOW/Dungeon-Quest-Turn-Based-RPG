namespace DungeonCrawler.CharacterStats
{
    public enum StatModType{
        flat = 100,
        percentAdd = 200,
        percentMult = 300,
    }

    public class StatModifier
    {
        public readonly float value;
        public readonly StatModType type;
        public readonly int order;
        public readonly object source;

        public StatModifier(float value, StatModType type, int order, object source){
            this.value = value;
            this.type = type;
            this.order = order;
            this.source = source;
        }

        // value and type required, order and source optional
        public StatModifier(float value, StatModType type) : this(value, type, (int)type, null){}

        public StatModifier(float value, StatModType type, int order) : this(value, type, (int)type, null){}

        public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source){}
    }
}
