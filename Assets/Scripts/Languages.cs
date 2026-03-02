using UnityEngine;

namespace MyGame
{
    [System.Serializable]
    public class Languages
    {
        [TextArea(1, 100)] public string ru;
        [TextArea(1, 100)] public string en;
        [TextArea(1, 100)] public string de;
        [TextArea(1, 100)] public string zh;
        [TextArea(1, 100)] public string fr;
        [TextArea(1, 100)] public string hi;
        [TextArea(1, 100)] public string it;
        [TextArea(1, 100)] public string ja;
        [TextArea(1, 100)] public string pt;
        [TextArea(1, 100)] public string es;
    }
}