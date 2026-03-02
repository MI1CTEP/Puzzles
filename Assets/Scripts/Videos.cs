using UnityEngine;
using UnityEngine.Video;

namespace MyGame
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Videos")]
    public sealed class Videos : ScriptableObject
    {
        public VideoClip[] videoClips;
    }
}