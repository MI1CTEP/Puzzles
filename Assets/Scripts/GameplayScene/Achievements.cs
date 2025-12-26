using UnityEngine;
using MyGame.Gameplay.Puzzle;
using MyGame.Gameplay.Dialogue;

namespace MyGame.Gameplay
{
    public sealed class Achievements : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private GameObject[] _achievementObjects;

        private PuzzleController _puzzleController;
        private GiftsGiver _giftsGiver;
        private DialogueController _dialogueController;

        public void Init(PuzzleController puzzleController, GiftsGiver giftsGiver, DialogueController dialogueController)
        {
            _puzzleController = puzzleController;
            _giftsGiver = giftsGiver;
            _dialogueController = dialogueController;
            gameObject.SetActive(false);
        }

        public bool IsHaveAchievements()
        {
            bool haveAchievement = false;

            if (TryInstantiate(0)) haveAchievement = true;
            if (_puzzleController.TypeDifficulty == TypeDifficulty.Easy && TryInstantiate(1)) haveAchievement = true;
            if (_giftsGiver.GivedId == 0 && TryInstantiate(2)) haveAchievement = true;
            if (_giftsGiver.GivedId == 1 && TryInstantiate(3)) haveAchievement = true;
            if (_puzzleController.TypeDifficulty == TypeDifficulty.Medium && TryInstantiate(4)) haveAchievement = true;
            if (_giftsGiver.GivedId == 2 && TryInstantiate(5)) haveAchievement = true;
            if (_dialogueController.CurrentSympathy == 0 && TryInstantiate(6)) haveAchievement = true;
            if (_puzzleController.TypeDifficulty == TypeDifficulty.Hard && TryInstantiate(7)) haveAchievement = true;
            if (_dialogueController.CurrentSympathy == _dialogueController.MaxSympathy && TryInstantiate(8)) haveAchievement = true;
            if (_giftsGiver.GivedId == 3 && TryInstantiate(9)) haveAchievement = true;

            if (haveAchievement)
                gameObject.SetActive(true);

            return haveAchievement;
        }

        private bool TryInstantiate(int id)
        {
            if (!GameData.Achievements.IsUnlock(GameData.CurrentLevel, id))
            {
                Instantiate(_achievementObjects[id], _parent);
                GameData.Achievements.Save(GameData.CurrentLevel, id);
                return true;
            }
            return false;
        }
    }
}