#if UNITY_ANDROID || UNITY_IOS
namespace NutakuUnitySdk
{
    /// <summary>
    /// Used by the SDK
    /// </summary>
    public struct NutakuUserProfile
    {
        /// <summary> Used by Nutaku SDK </summary>
        public string nugt;

        /// <summary> Used by Nutaku SDK </summary>
        public int id;

        /// <summary> Used by Nutaku SDK </summary>
        public string nickname;

        /// <summary> Used by Nutaku SDK </summary>
        public int test;

        /// <summary> Used by Nutaku SDK </summary>
        public int grade;

        /// <summary> Used by Nutaku SDK </summary>
        public string language;

        /// <summary> Used by Nutaku SDK </summary>
        public string activeSub;

        /// <summary> Used by Nutaku SDK </summary>
        public int n2i;
    }
}
#endif