using UnityEngine;
using UnityEngine.Video;
using MyGame.Gameplay.Dialogue;

namespace MyGame.Gameplay
{
    [System.Serializable]
    public sealed class ScenarioStage
    {
        [SerializeField] private TypeStage _typeStage;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private bool _isAnim;
        [SerializeField] private int _puzzleValueX;
        [SerializeField] private VideoClip _videoClip;
        [SerializeField] private SimpleDialogue _simpleDialogue;

        public TypeStage TypeStage => _typeStage;
        public Sprite Sprite => _sprite;
        public bool IsAnim => _isAnim;
        public int PuzzleValueX => _puzzleValueX;
        public VideoClip VideoClip => _videoClip;
        public SimpleDialogue SimpleDialogue => _simpleDialogue;
    }

    public enum TypeStage
    {
        SetPuzzle, SetVideo, SetDialogue, SetGiftsGiver
    }
}
