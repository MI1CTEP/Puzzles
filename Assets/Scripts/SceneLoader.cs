using UnityEngine.SceneManagement;

namespace MyGame
{
    public static class SceneLoader
    {
        public static void LoadMenu() => Load("MenuTempScene");

        public static void LoadGameplay() => Load("GameplayScene");

        private static void Load(string nameScene)
        {
            SceneManager.LoadScene(nameScene);
        }
    }
}