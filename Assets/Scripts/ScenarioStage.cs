using UnityEngine;
using UnityEngine.Video;

namespace MyGame
{
    [System.Serializable]
    public sealed class ScenarioStage
    {
        [SerializeField] private TypeStage _typeStage;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _puzzleValueX;
        [SerializeField] private VideoClip _videoClip;
        [SerializeField] private Dialogue _dialogue;

        public TypeStage TypeStage => _typeStage;
        public Sprite Sprite => _sprite;
        public int PuzzleValueX => _puzzleValueX;
        public VideoClip VideoClip => _videoClip;
    }

    public enum TypeStage
    {
        SetSprite, SetPuzzle, SetVideo, SetDialogue
    }
}
