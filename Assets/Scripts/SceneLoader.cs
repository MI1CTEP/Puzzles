using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public sealed class SceneLoader : MonoBehaviour
    {
        public static void LoadMenu() => Load("MenuScene");

        public static void LoadGameplay() => Load("GameplayScene");

        public static void LoadInitScene() => Load("InitScene");

        public static void LoadMenuScenarioScene() => Load("MenuScenario");

        public static void LoadInitializeScene() => Load("Initialize");

        private static void Load(string nameScene)
        {
            SceneManager.LoadScene(nameScene);
        }

        public static void Exit() => Application.Quit();
    }
}