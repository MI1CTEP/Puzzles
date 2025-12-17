#if UNITY_ANDROID || UNITY_IOS
namespace NutakuUnitySdk
{
    /// <summary>
	/// Structure that stores the content of HTTP response.
    /// </summary>
    public struct NutakuApiRawResult
    {
        ///<summary> HTTP status code </summary>
        public int responseCode;

        ///<summary> HTTP response body </summary>
        public string body;

        ///<summary> Used to persist API parameters which are not mirrored by the API. Currently only used for payment id in the PutPayment flow </summary>
        public string correlationId;
    }
}
#endif