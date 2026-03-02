using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public static class GameData
    {
        public static int CurrentLevel { get; set; }
        public static int CurrentPuzzleStep { get; set; }
        public static int EasyPuzzle { get; } = 2;
        public static int MediumPuzzle { get; } = 3;
        public static int HardPuzzle { get; } = 5;

        //Ёто валюта, за которую открываютс€ уровни. «арабатываетс€ при прохождении.
        public static class Respect
        {
            private static readonly string _key = "Respect";

            public static void Add(int value)
            {
                NewSaves.Save(_key, Load() + value);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(_key, value);
            }

            public static int Load()
            {
                if (NewSaves.TryLoad(_key, out int value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(_key);
            }

        }

        // ”ровень симпатии девушек. id - это девушка.
        public static class Sympathy
        {
            private static readonly string _key = "Sympathy_";

            public static void Save(int id, int value)
            {
                if (Load(id) < value)
                {
                    string key = _key + id.ToString();
                    NewSaves.Save(key, value);
                    NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, value);
                }
            }

            public static int Load(int id)
            {
                string key = _key + id.ToString();
                if (NewSaves.TryLoad(key, out int value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(key);
            }

        }

        //Ёто подарки
        public static class Gifts
        {
            private static string GetKey(int groupId, int id) => $"QuantityGift_{groupId}_{id}";

            public static void AddValue(int groupId, int id, int value)
            {
                string key = GetKey(groupId, id);
                int currentValue = LoadValue(groupId, id);
                NewSaves.Save(key, currentValue + value);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, value);
            }

            public static int LoadValue(int groupId, int id)
            {
                string key = GetKey(groupId, id);
                if (NewSaves.TryLoad(key, out int value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(key);
            }
        }

        //новый класс дл€ работы со стади€ми пазлов
        public static class StageGirlLevel
        {
            private static string GetKey(int levelId, int puzleId) => $"StageGirlLevel_{levelId}_{puzleId}";

            public static void UnlockStage(int levelId, int puzleId)
            {
                if(IsUnlockedStage(levelId, puzleId))
                    return;


                string key = GetKey(levelId, puzleId);
                NewSaves.Save(key, true);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, 1);
            }

            public static bool IsUnlockedStage(int levelId, int puzleId)
            {
                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;

                string key = GetKey(levelId, puzleId);
                if (NewSaves.TryLoad(key, out bool value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.IsHasDialogues(key);
            }
        }

        //ƒл€ сохранени€-загрузки выбранного варианта в диалогах
        public static class Dialogues
        {
            private static string GetKey(int levelId, int dialogueId, int positionId) => $"Dialogue_{levelId}_{dialogueId}_{positionId}";

            public static void Unlock(int levelId, int dialogueId, int positionId)
            {
                if(IsUnlocked(levelId, dialogueId, positionId))
                    return;


                string key = GetKey(levelId, dialogueId, positionId);
                NewSaves.Save(key, true);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, 1);
            }

            public static bool IsUnlocked(int levelId, int dialogueId, int positionId)
            {
                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;

                string key = GetKey(levelId, dialogueId, positionId);
                if (NewSaves.TryLoad(key, out bool value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.IsHasDialogues(key);
            }
        }

        //Ёто последн€€ картинка/уровень, глабальна€ цель игры. ≈≈ купить нельз€.
        public static class ExtraLevel
        {
            public static Vector2Int PartSize { get; } = new(7, 9);
            public static float ChanceEasy { get; } = 0.1f;
            public static float ChanceMedium { get; } = 0.15f;
            public static float ChanceHard { get; } = 0.2f;

            private static string GetKeyPart(int partId) => $"ExtraLevelPart_{partId}";
            private static readonly string _keyOpenedParts = "ExtraLevelOpenedParts";
            private static readonly string _keyLevel = "ExtraLevel";

            public static void UnlockPart(int partId)
            {

                string keyPart = GetKeyPart(partId);
                NewSaves.Save(keyPart, true);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(keyPart, 1);

                int openedParts = GetOpenedPartsValue();
                NewSaves.Save(_keyOpenedParts, openedParts + 1);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(_keyOpenedParts, openedParts + 1);
            }

            public static bool IsUnlockPart(int partId)
            {
                string key = GetKeyPart(partId);
                if (NewSaves.TryLoad(key, out bool value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.IsUnlockPartExtraLevel(key);
            }

            public static int GetOpenedPartsValue()
            {
                if (NewSaves.TryLoad(_keyOpenedParts, out int value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(_keyOpenedParts);
            }

            public static void UnlockLevel()
            {
                int unlockedLevel = UnlockedLevels();
                NewSaves.Save(_keyLevel, unlockedLevel + 1);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(_keyLevel, unlockedLevel + 1);
            }

            public static int UnlockedLevels()
            {
                if (NewSaves.TryLoad(_keyLevel, out int value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(_keyLevel);
            }
        }

        //Ёто достижени€ за которые открываютс€ элементы секретного альбома. levelId - это номер девки, id - это номер достижени€.
        //¬ инвентаре у пользовател€ должен оказатс€ данный элемент
        public static class Achievements 
        {
            private static string GetKey(int levelId, int id) => $"Achievement_{levelId}_{id}";

            public static void Save(int levelId, int id)
            {
                if (IsUnlock(levelId, id))
                    return;


                string key = GetKey(levelId, id);
                NewSaves.Save(key, true);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, 1);
            }

            public static bool IsUnlock(int levelId, int id)
            {
                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;

                string key = GetKey(levelId, id);
                if (NewSaves.TryLoad(key, out bool value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.IsHasAchievements(key);
            }
        }

        //ƒополнительные уровни у каждой девушки
        public static class PaidContent
        {
            public static bool IsUnlock(int id)
            {
                return NutakuAPIInitializator.instance.PuarchaseService.IsAvaliableBonusStage(id);
            }
        }

        //ƒевушки
        public static class Levels
        {
            private static string GetKey(int id) => $"keyLevelOnRespect_{id}";

            public static void SetOpened(int id)
            {
                if (IsOpened(id) || IsOpenedAll())
                    return;

                string key = GetKey(id);
                NewSaves.Save(key, true);
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, 1);
            }

            public static bool IsOpened(int id)
            {
                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;

                //вот тут провер€ем открыта ли девушка по индексу
                string key = GetKey(id);
                if (NewSaves.TryLoad(key, out bool value))
                    return value;
                return NutakuAPIInitializator.instance.PuarchaseService.IsAvaliableShowGirl(id);
            }

            public static bool IsOpenedAll()
            {
                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;

                //вот тут провер€ем открыта ли все девушки
                return NutakuAPIInitializator.instance.PuarchaseService.IsAvaliableAllShowGirls();
            }
        }

        //ƒублирует сохранени€ в течении одной сессии. Ќеобходимо дл€ быстрого доступа к сейвам
        private static class NewSaves
        {
            private static readonly Dictionary<string, object> _newSaves = new();

            public static void Save<T>(string key, T value)
            {
                _newSaves[key] = value;
            }

            public static bool TryLoad<T>(string key, out T value)
            {
                if (_newSaves.TryGetValue(key, out object obj) && obj is T casted)
                {
                    value = casted;
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
        }
    }
}