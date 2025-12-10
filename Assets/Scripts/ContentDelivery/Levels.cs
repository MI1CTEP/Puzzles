using System.Collections.Generic;

namespace MyGame.ContentDelivery
{
    public sealed class Levels
    {
        public List<Level> levels;
    }

    public sealed class Level
    {
        public int id;
        public string type;
        public float price;
        public int version;
        public bool isNeedToDownload;
    }
}