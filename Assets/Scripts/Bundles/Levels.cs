using System.Collections.Generic;

namespace MyGame.Bundles
{
    public sealed class Levels
    {
        public List<Level> levels;
    }

    public sealed class Level
    {
        public int id;
        public string type;
        public int price;
        public int version;
        public TypeLevelStatus typeLevelStatus;
    }

    public enum TypeLevelStatus
    {
        NotDownloaded, Downloaded, Downloading
    }
}