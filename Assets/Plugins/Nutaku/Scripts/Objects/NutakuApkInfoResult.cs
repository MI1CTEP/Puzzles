#if UNITY_ANDROID || UNITY_IOS
namespace NutakuUnitySdk
{
    /// <summary>
	/// Structure that stores the result of NutakuApi.CheckForNewerPublishedApp()
    /// </summary>
    public struct NutakuAppInfoResult
    {
        /// <summary> If this is true, it means a newer published version is available on the Nutaku website </summary>
        public bool newerVersionAvailable;

        /// <summary> The URL where the user can grab the latest published APK/IPA for your game </summary>
        public string url;

        /// <summary> The version of the newest published apk/ipa </summary>
        public int version;
    }
}
#endif