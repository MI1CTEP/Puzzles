#if UNITY_ANDROID || UNITY_IOS
namespace NutakuUnitySdk
{
    /// <summary>
    /// Nutaku SDK Configuration file. You must set the values of each of these fields correctly when integrating, and for the runtime version field you must make sure it is accurate everytime you prepare a build for the platform.
    /// </summary>
    public static class NutakuSdkConfig
    {
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Unless you want to use a completely custom solution with your server-side,
        public static int RuntimeVersionCode = 1; // You MUST remember to update this field value everytime you prepare a new build for the platform!
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Must match what is in your manifest for Android, and what you specify in the CMS during upload for iOS, otherwise app update notification capability won't be successful and users will be stuck with an old build.

        public static string Environment = "SANDBOX"; // Can use "SANDBOX" or "PRODUCTION"

        public static string TitleId = "hot-stories-puzzles"; // the TITLE ID of your game from the CMS

        public static string AppPackageName = "com.tetragon.sweetstories"; //"com.unitysdk.sample"; // For Android, must match the package name from your Manifest, For iOS must match your BundleIdentifier. Otherwise browser events like logins or payments won't be captured by the game


        public static LoginResultToGameCallbackDelegate loginResultToGameCallbackDelegate; //= TestUIController.LoginResultCallback; // static callback to your game logic for login results. if wasSuccess==true, proceed with loading the game, if false Inform user about the game waiting for browser login completion, with an button to re-trigger NutakuSdk.OpenLoginPageInBrowser() in case they lost the page

        public static PaymentBrowserResultToGameCallbackDelegate paymentBrowserResultToGameCallbackDelegate; //= TestUIController.Callback_PaymentResultFromBrowser; // static callback to the game logic for processing payment events received from browser



        public delegate void LoginResultToGameCallbackDelegate(bool wasSuccess);
        public delegate void PaymentBrowserResultToGameCallbackDelegate(string paymentId, string statusFromBrowser);
    }
}
#endif