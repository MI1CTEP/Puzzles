using System;
using UnityEngine;
using Zenject;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleController : IInitializable, IDisposable
    {
        private readonly Content _content;
        private readonly VideoController _videoController;
        private readonly PuzzlePartsPool _partsPool;
        private readonly PuzzleMixer _mixer;
        private readonly PuzzlePartsGenerator _partsGenerator;
        private PuzzlePart[,] _parts;
        private readonly PuzzleBackground _background;
        private readonly PuzzleBoard _board;
        private readonly Transform _puzzleParent;
        private Vector2Int _partsLength;
        private int _step;

        public PuzzleController(Content content, VideoController videoController, PuzzlePartsPool partsPool, PuzzleMixer mixer, PuzzlePartsGenerator partsGenerator, PuzzleBackground background, PuzzleBoard board, Transform puzzleParent)
        {
            _content = content;
            _videoController = videoController;
            _partsPool = partsPool;
            _mixer = mixer;
            _partsGenerator = partsGenerator;
            _background = background;
            _board = board;
            _puzzleParent = puzzleParent;
        }

        public void Initialize()
        {
            _board.OnComplete += ShowVideo;
            _videoController.OnEnd += TryStartNextStep;
            Start();
            Play();
        }

        private void TryStartNextStep()
        {
            _step++;
            if(_step < _content.PartContents.Length)
            {
                for (int y = 0; y < _partsLength.y; y++)
                    for (int x = 0; x < _partsLength.x; x++)
                        _partsPool.Return(_parts[x, y]);

                _puzzleParent.gameObject.SetActive(true);
                _board.ResetProgress();
                Start();
                Play();
            }
            else
            {
                OnEnd();
            }
        }

        private void Start()
        {
            Texture2D sourceTexture = _content.PartContents[_step].firstFrame;
            _background.SetBackground(sourceTexture);
            _videoController.SetVideoClip(_content.PartContents[_step].videoClip);
            UpdateCameraSize();
        }

        private void Play()
        {
            _parts = _partsGenerator.GetPuzzleParts(_content.PartContents[_step].firstFrame, 2);
            _partsLength = new(_parts.GetUpperBound(0) + 1, _parts.GetUpperBound(1) + 1);
            _board.SetParts(_parts, _partsLength);
            _mixer.MixParts(_parts, _partsLength);
            _board.SetPartsPosition();
            _background.SetMask(_partsLength * _partsGenerator.PartSize);
        }

        private void ShowVideo()
        {
            _puzzleParent.gameObject.SetActive(false);
            _videoController.Play();
        }

        private void OnEnd()
        {

        }

        private void UpdateCameraSize()
        {
            Camera camera = Camera.main;
            float aspectRatio = (float)Screen.height / Screen.width;
            camera.orthographicSize = aspectRatio * _content.PartContents[0].firstFrame.width / 200;
        }

        public void Dispose()
        {
            _board.OnComplete -= ShowVideo;
            _videoController.OnEnd -= TryStartNextStep;
        }
    }
}