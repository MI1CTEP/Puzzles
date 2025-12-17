#if UNITY_ANDROID || UNITY_IOS
namespace NutakuUnitySdk
{
    /// <summary>
	/// Store the result of Game Handshake API
    /// </summary>
    public struct NutakuGameHandshakeResponse
    {
        /// <summary> HTTP Status Code of the response from the Game Handshake API. If zero, it means Nutaku did not receive a response </summary>
        public int game_rc;

        /// <summary> HTTP response body Code from the Game Handshake API, unless game_rc is zero, in which case this string is from Nutaku server instead </summary>
        public string message;
    }
}
#endif