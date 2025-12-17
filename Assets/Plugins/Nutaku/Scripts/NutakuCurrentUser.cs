#if UNITY_ANDROID || UNITY_IOS
using System;
using UnityEngine;

namespace NutakuUnitySdk
{
    /// <summary>
    /// Class that handles the details for the currently logged in Nutaku user
    /// </summary>
    public static class NutakuCurrentUser
    {
        private const string NUGT_PREFKEY = "sdk_nugt";
        private const string USER_ID_PREFKEY = "sdk_user_id";
        private const string USER_NICKNAME_PREFKEY = "sdk_user_nickname";
        private const string USER_TEST_PREFKEY = "sdk_user_type";
        private const string USER_GRADE_PREFKEY = "sdk_user_grade";
        private const string USER_LANGUAGE_PREFKEY = "sdk_user_language";
        private static string _nugt;
        private static int _userId;
        private static string _userNickname;
        private static int _userTest;
        private static int _userGrade;
        private static string _userLanguage;
        private static long _lastUpdatedEpoch;
        private static bool _localStoredFileDataIsUnusable;
        

        internal static string GetNUGT()
        {
            if (_userId == 0)
                RestoreUserInfoFromLocal();
            return _nugt;
        }


        private static void SetNUGT(string nugt)
        {
            _nugt = string.IsNullOrEmpty(nugt) ? "null" : nugt;
            try
            {
                PlayerPrefs.SetString(NUGT_PREFKEY, _nugt);
            }
            catch { _localStoredFileDataIsUnusable = true; }
        }


        ///<summary> Nutaku User ID for the currently logged in user</summary>
        public static int GetUserId()
        {
            if (_userId == 0)
                RestoreUserInfoFromLocal();
            return _userId;
        }


        private static void SetUserId(int userId)
        {
            _userId = userId;
            try
            {
                PlayerPrefs.SetInt(USER_ID_PREFKEY, _userId);
            }
            catch { _localStoredFileDataIsUnusable = true; }
        }


        ///<summary> User Nickname aka DisplayName </summary>
        public static string GetUserNickname()
        {
            if (_userId == 0)
                RestoreUserInfoFromLocal();
            return _userNickname;
        }


        private static void SetUserNickname(string nickname)
        {
            _userNickname = nickname;
            PlayerPrefs.SetString(USER_NICKNAME_PREFKEY, _userNickname);
        }


        ///<summary> Get whether the user is a general account, or an internal/test user (which on production env doesn't count for revenue) </summary>
        public static bool IsTestUser()
        {
            if (_userId == 0)
                RestoreUserInfoFromLocal();
            return _userTest > 0;
        }


        private static void SetUserTest(int test)
        {
            _userTest = test;
            PlayerPrefs.SetInt(USER_TEST_PREFKEY, test);
        }


        ///<summary> Get the saved Nutaku User Grade. 1 means email not validated, 2 means email validated </summary>
        public static int GetUserGrade()
        {
            if (_userId == 0)
                RestoreUserInfoFromLocal();
            return _userGrade;
        }


        private static void SetUserGrade(int grade)
        {
            _userGrade = grade;
            PlayerPrefs.SetInt(USER_GRADE_PREFKEY, _userGrade);
        }


        ///<summary> Get the saved Nutaku User Language. When possible, try to have your game localized in this language </summary>
        public static string GetUserLanguage()
        {
            if (_userId == 0)
                RestoreUserInfoFromLocal();
            return _userLanguage;
        }


        private static void SetUserLanguage(string language)
        {
            _userLanguage = language;
            PlayerPrefs.SetString(USER_LANGUAGE_PREFKEY, _userLanguage);
        }


        internal static bool IsFresh()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds() < _lastUpdatedEpoch + 10800;
        }


        internal static void SaveUserInfo(string nugt, int userId, string nickname, int test, int grade, string language)
        {
            NutakuSdk.Log("saveUserInfo()");
            _lastUpdatedEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();
            SetNUGT(nugt);
            SetUserId(userId);
            _localStoredFileDataIsUnusable = false;
            SetUserNickname(nickname);
            SetUserTest(test);
            SetUserGrade(grade);
            SetUserLanguage(language);
        }


        internal static void RestoreUserInfoFromLocal()
        {
            NutakuSdk.Log("RestoreUserInfoFromLocal()");
            if (_localStoredFileDataIsUnusable)
                return;
            _nugt = PlayerPrefs.GetString(NUGT_PREFKEY);
            _userId = PlayerPrefs.GetInt(USER_ID_PREFKEY);
            _userNickname = PlayerPrefs.GetString(USER_NICKNAME_PREFKEY);
            _userTest = PlayerPrefs.GetInt(USER_TEST_PREFKEY);
            _userGrade = PlayerPrefs.GetInt(USER_GRADE_PREFKEY);
            _userLanguage = PlayerPrefs.GetString(USER_LANGUAGE_PREFKEY);
            if (_userId == 0 || string.IsNullOrEmpty(_nugt) || _nugt == "null")
            {
                _localStoredFileDataIsUnusable = true;
                ClearUserData();
            }
        }


        ///<summary> No need to call this from the game. This method is only public to be accessible from the Unity Editor menu </summary>
        public static void ClearUserData()
        {
            NutakuSdk.Log("ClearUserData()");
            _localStoredFileDataIsUnusable = true;
            _lastUpdatedEpoch = 0;
            try
            {
                SetNUGT("");
                SetUserId(0);
                SetUserNickname("");
                SetUserTest(0);
                SetUserGrade(0);
                SetUserLanguage("");
            }
            catch { }
        }
    }
}
#endif