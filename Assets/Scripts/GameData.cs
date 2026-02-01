using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public static class GameData
    {
        public static int CurrentLevel { get; set; }


        //Это валюта, за которую открываются уровни. Зарабатывается при прохождении.
        public static class Respect
        {
            private static readonly string _key = "Respect";
            private static int _loadedValue;
            private static bool _isSaved;

            public static void Add(int value)
            {
                if (!_isSaved)
                {
                    _loadedValue = Load();
                    _isSaved = true;
                }
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(_key, value);
                _loadedValue += value;

                //int currentValue = Load();
                //PlayerPrefs.SetInt(_key, currentValue + value);
            }

            public static int Load()
            {
                if (_isSaved)
                    return _loadedValue;
                else
                    return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(_key);

                //return PlayerPrefs.GetInt(_key);
            }

        }


        // Уровень симпатии девушек. id - это девушка.
        public static class Sympathy
        {
            private static readonly string _key = "Sympathy_";
            private static List<int> _loadedValue = new();
            private static List<bool> _isSaved = new();

            public static void Save(int id, int value)
            {
                if(_loadedValue.Count <= id)
                {
                    for (int i = _loadedValue.Count; i <= id; i++)
                    {
                        _loadedValue.Add(0);
                        _isSaved.Add(false);
                    }
                }

                string key = _key + id.ToString();
                float currentValue = Load(id);
                if (currentValue < value)
                {
                    if (!_isSaved[id])
                    {
                        _isSaved[id] = true;
                    }
                    NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, value);
                    _loadedValue[id] = value;
                }


                //float currentValue = Load(id);
                //if (currentValue < value)
                //    PlayerPrefs.SetInt(_key + id.ToString(), value);
            }

            public static int Load(int id)
            {
                if (_isSaved.Count > id && _isSaved[id]) 
                {
                    return _loadedValue[id];
                }
                else
                {
                    string key = _key + id.ToString();
                    return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(key);
                }
                //return PlayerPrefs.GetInt(_key + id.ToString());
            }

        }


        //Это подарки
        public static class Gifts
        {
            private static List<List<int>> _loadedValue = new();
            private static List<List<bool>> _isSaved = new();

            //Оставить вызов где он как подарок. Убрать/Переделать где как покупка. Что за groupId?
            public static void AddValue(int groupId, int id, int value)
            {
                //int currentValue = LoadValue(groupId, id);

                if (_loadedValue.Count <= groupId)
                {
                    for (int i = _loadedValue.Count; i <= groupId; i++)
                    {
                        _loadedValue.Add(new());
                        _isSaved.Add(new());
                    }
                }
                if (_loadedValue[groupId].Count <= id)
                {
                    for (int i = _loadedValue[groupId].Count; i <= id; i++)
                    {
                        _loadedValue[groupId].Add(0);
                        _isSaved[groupId].Add(false);
                    }
                }

                if (!_isSaved[groupId][id])
                {
                    _loadedValue[groupId][id] = LoadValue(groupId, id);
                    _isSaved[groupId][id] = true;
                }

                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(GetKey(groupId, id), value);
                _loadedValue[groupId][id] += value;

                //int currentValue = LoadValue(groupId, id);
                //PlayerPrefs.SetInt(GetKey(groupId, id), currentValue + value);
            }


            public static int LoadValue(int groupId, int id)
            {
                if (_isSaved.Count > groupId && _isSaved[groupId].Count > id && _isSaved[groupId][id])
                {
                    return _loadedValue[groupId][id];
                }
                else
                {
                    string key = GetKey(groupId, id);
                    return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(key);
                }
                //return PlayerPrefs.GetInt(GetKey(groupId, id));
            }


            private static string GetKey(int groupId, int id) => $"QuantityGift_{groupId}_{id}";
        }

        //В диалогах можно не дублировать сохранения в локальные переменные, так как данные будут нужны только после перезапуска уровня
        public static class Dialogues
        {
            private static readonly string _key = "Dialogue";

            public static void Unlock(int levelId, int dialogueId, int positionId)
            {
                string key = $"{_key}_{levelId}_{dialogueId}_{positionId}";
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, 1);

                //PlayerPrefs.SetInt($"{_key}_{levelId}_{dialogueId}_{positionId}", 1);
            }

            public static bool IsUnlock(int levelId, int dialogueId, int positionId)
            {

                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;


                string key = $"{_key}_{levelId}_{dialogueId}_{positionId}";
                return NutakuAPIInitializator.instance.PuarchaseService.IsHasDialogues(key);

                //return PlayerPrefs.GetInt($"{_key}_{levelId}_{dialogueId}_{positionId}") == 1;
            }
        }


        //Это последняя картинка/уровень, глабальная цель игры. ЕЕ купить нельзя.
        public static class ExtraLevel
        {
            public static Vector2Int PartSize { get; } = new(7, 9);
            public static float ChanceEasy { get; } = 0.1f;
            public static float ChanceMedium { get; } = 0.15f;
            public static float ChanceHard { get; } = 0.2f;

            private static readonly string _keyOpenedParts = "ExtraLevelOpenedParts";
            private static readonly string _keyPart = "ExtraLevelPart";
            private static readonly string _keyLevel = "ExtraLevel";

            public static void UnlockPart(int partId)
            {
                string keyPart = $"{_keyPart}_{partId}";
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(keyPart, 1);
                int openedParts = GetOpenedPartsValue();
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(_keyOpenedParts, openedParts + 1);

                //PlayerPrefs.SetInt($"{_keyPart}_{partId}", 1);
                //int openedParts = GetOpenedPartsValue();
                //PlayerPrefs.SetInt(_keyOpenedParts, openedParts + 1);
            }

            public static bool IsUnlockPart(int partId)
            {
                string key = $"{_keyPart}_{partId}";
                return NutakuAPIInitializator.instance.PuarchaseService.IsUnlockPartExtraLevel(key);
                //return PlayerPrefs.GetInt($"{_keyPart}_{partId}") == 1;
            }

            public static int GetOpenedPartsValue()
            {
                return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(_keyOpenedParts);
                //return PlayerPrefs.GetInt(_keyOpenedParts);
            }


            // !!!!!!!!!!!!!  Если этот сброс вобще нужен. Вот тут цикл надо переделать. Слишком долгая логика, переберает и запрашивает все данные и еще и записывает. Например узнать есть ли максимальное количество. Или завсести отдельный ключ, если все открыто!!!!!!
            public static void UnlockLevel()
            {
                int unlockedLevel = UnlockedLevels();
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(_keyLevel, unlockedLevel + 1);


                //int unlockedLevel = UnlockedLevels();
                //PlayerPrefs.SetInt(_keyLevel, unlockedLevel + 1);
                //for (int i = 0; ; i++)
                //{
                //    if (PlayerPrefs.HasKey($"{_keyPart}_{i}"))
                //        PlayerPrefs.SetInt($"{_keyPart}_{i}", 0);
                //    else break;
                //}
            }

            public static int UnlockedLevels()
            {
                return NutakuAPIInitializator.instance.PuarchaseService.GetOpenedPartsValue(_keyLevel);
                //return PlayerPrefs.GetInt(_keyLevel);
            }

        }

        //Это достижения за которые открываются элементы секретного альбома. levelId - это номер девки, id - это номер достижения.
        //В инвентаре у пользователя должен оказатся данный элемент
        public static class Achievements
        {
            private static readonly string _key = "Achievement";

            public static void Save(int levelId, int id)
            {
                string key = $"{_key}_{levelId}_{id}";
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, 1);
                //PlayerPrefs.SetInt($"{_key}_{levelId}_{id}", 1);
            }

            public static bool IsUnlock(int levelId, int id)
            {
                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;

                string key = $"{_key}_{levelId}_{id}";

                return NutakuAPIInitializator.instance.PuarchaseService.IsHasAchievements(key);
               // return PlayerPrefs.GetInt($"{_key}_{levelId}_{id}") == 1;
            }
        }


        //Дополнительные уровни у каждой девушки
        public static class PaidContent
        {
            private static readonly string _key = "PaidContent";

            public static void Save(int id)
            {
                PlayerPrefs.SetInt($"{_key}_{id}", 1);
            }

            public static bool IsUnlock(int id)
            {
                //if (NutakuAPIInitializator.instance.IsOpenAllContent)
                //    return true;

                // return PlayerPrefs.GetInt($"{_key}_{id}") == 1;
                return NutakuAPIInitializator.instance.PuarchaseService.IsAvaliableBonusStage(id);
            }
        }


        //Девушки
        public static class Levels
        {
            private static List<bool> _loadedValue = new();
            private static List<bool> _isSaved = new();
            private static readonly string _key = "Level";
            private static readonly string _keyAll = "LevelsAll";

            private static readonly string _keyLevelOnRespect = "keyLevelOnRespect";

            public static void SetOpened(int id)
            {
                if (_loadedValue.Count <= id)
                {
                    for (int i = _loadedValue.Count; i <= id; i++)
                    {
                        _loadedValue.Add(false);
                        _isSaved.Add(false);
                    }
                }
                //Спросить Вову в каком это моменте?
                //Это эксернал открытие? Нет ЭТО ОТКРЫТИЕ ВЫЗЫВАЕТСЯ ПО ДОСТИЖЕНИИ ОПРЕДЕЛЕННОГО КОЛИЧЕСТВА РЕСПЕКТА
                //PlayerPrefs.SetInt($"{_key}_{id}", 1);
                //метод открытия девушки

                if (!_isSaved[id])
                {
                    _loadedValue[id] = IsOpened(id);
                    _isSaved[id] = true;
                }

                string key = $"{_keyLevelOnRespect}_{id}";
                NutakuAPIInitializator.instance.PuarchaseService.StartMakeInventoryRequest(key, 1);
                _loadedValue[id] = true;
            }

            public static bool IsOpened(int id)
            {
                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;

                //вот тут проверяем открыта ли девушка по индексу
                if (_isSaved.Count > id && _isSaved[id])
                    return _loadedValue[id];
                else
                    return NutakuAPIInitializator.instance.PuarchaseService.IsAvaliableShowGirl(id);

                //return PlayerPrefs.GetInt($"{_key}_{id}") == 1;
            }

            public static void OpenAll()
            {

                PlayerPrefs.SetInt(_keyAll, 1);
                //метод открытия всех девушек
            }

            public static bool IsOpenedAll()
            {
                if (NutakuAPIInitializator.instance.IsOpenAllContent)
                    return true;

                //вот тут проверяем открыта ли все девушки
                //return PlayerPrefs.GetInt(_keyAll) == 1;
                return NutakuAPIInitializator.instance.PuarchaseService.IsAvaliableAllShowGirls();
            }
        }
    }
}