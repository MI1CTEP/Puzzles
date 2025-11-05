using UnityEngine;
using Zenject;
using MyGame.Gameplay.Puzzle;

namespace MyGame.Gameplay
{
    public sealed class SceneInstaller : MonoInstaller
    {
        [SerializeField] private VideoController _videoController;
        [SerializeField] private Content _content;
        [SerializeField] private Transform _puzzleParent;
        [SerializeField] private PuzzlePart _puzzlePartPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(_videoController).AsSingle();
            Container.BindInstance(_content).AsSingle();
            InstallPuzzle();
        }

        private void InstallPuzzle()
        {
            Container.BindInstance(_puzzleParent).AsSingle();
            Container.BindInstance(_puzzlePartPrefab).AsSingle();
            Container.Bind<PuzzleMixer>().AsSingle();
            Container.BindInterfacesAndSelfTo<PuzzleBackground>().AsSingle();
            Container.Bind<PuzzleBoard>().AsSingle();
            Container.Bind<PuzzlePartsPool>().AsSingle();
            Container.Bind<PuzzlePartsGenerator>().AsSingle();
            Container.BindInterfacesAndSelfTo<PuzzleController>().AsSingle();
        }
    }
}
