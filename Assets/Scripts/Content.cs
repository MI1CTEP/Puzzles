using UnityEngine;
using UnityEngine.Video;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Content")]
    public sealed class Content : ScriptableObject
    {
        [SerializeField] private PartContent[] _partContents;

        public PartContent[] PartContents => _partContents;
    }

    [System.Serializable]
    public sealed class PartContent
    {
        public Texture2D firstFrame;
        public VideoClip videoClip;
    }
}