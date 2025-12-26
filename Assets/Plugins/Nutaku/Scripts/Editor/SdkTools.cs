#if UNITY_ANDROID || UNITY_IOS
using UnityEditor;

namespace NutakuUnitySdk
{
    /// <summary>
	/// Unity Editor extension of Nutaku Unity SDK.
    /// </summary>
    public static class SdkTools
    {
        /// <summary>
		/// Delete the Nutaku Android SDK login information saved on the PC when running in Unity Editor
        /// </summary>
        [MenuItem("Nutaku Tools/Delete Login Info")]
        static void DeleteLoginInfo()
        {
            NutakuCurrentUser.ClearUserData();
        }
    }
}
#endif