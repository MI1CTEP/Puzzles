#if UNITY_ANDROID || UNITY_IOS
namespace NutakuUnitySdk
{
    /// <summary>
	/// Used with Payment API
    /// </summary>
    public class NutakuPayment
    {
        /// <summary> Nutaku Payment ID </summary>
        public string paymentId;

        /// <summary> Nutaku URL to execute transaction via the browser, when PUT is not possible </summary>
        public string transactionUrl;

        /// <summary> When received with the value "flyout", the payment must be done via browser. If received as "put", both API and browser approaches are possible </summary>
        public string next;

        /// <summary> Identifier, chosen by game team, for the item being purchased. This is not shown to the user, but it is stored in the database for reporting purposes </summary>
        public string skuId;

        /// <summary> Price for one unit of the item, measured in Nutaku Gold </summary>
        public int price;

        /// <summary> Item name. Shown to the user. </summary>
        public string name;

        /// <summary> Item description </summary>
        public string description;

        /// <summary> URL to an image of the item. Must be over https </summary>
        public string imgUrl;

        /// <summary> Optional. Not currently shown to users. Feel free to ignore it </summary>
        public string message = "";

        public static NutakuPayment PaymentCreationInfo(string skuId, string name, int price, string imgUrl, string description)
        {
            return new NutakuPayment() { skuId = skuId, name = name, price = price, imgUrl = imgUrl, description = description };
        }
    }
}
#endif