#if UNITY_ANDROID || UNITY_IOS
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace NutakuUnitySdk
{
    /// <summary>
    /// Internal class used by Nutaku SDK when performing API calls and to trigger the appropriate response delegates
    /// </summary>
    public class NutakuUnityWebRequest
    {
        /// <summary> This delegate function will get triggered by the SDK when the response is received from the server </summary>
        public delegate void callbackFunctionDelegate(NutakuApiRawResult rawResult);

        private callbackFunctionDelegate callbackFunctionDelegateVar;
        private NutakuApiRawResult rawResult;

        internal NutakuUnityWebRequest() { }


        internal void StartSendingRawRequest(string method, MonoBehaviour myMonoBehaviour, Dictionary<string, string> headers, string url, string body, callbackFunctionDelegate callbackFunctionDelegate, string correlationId)
        {
            callbackFunctionDelegateVar = callbackFunctionDelegate;
            rawResult.correlationId = correlationId;

            if (method == "GET")
              myMonoBehaviour.StartCoroutine(GetRequest(url, headers));
            else if (method == "POST")
                myMonoBehaviour.StartCoroutine(PostRequest(url, headers, body));
            else if (method == "PUT")
                myMonoBehaviour.StartCoroutine(PutRequest(url, headers, body));
        }


        private IEnumerator PostRequest(string uri, IEnumerable<KeyValuePair<string, string>> headers, string body)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, body)) // this is a workaround for Unity 202X where Post didn't accept raw json. For Unity 6000 or newer you can replace with .Post(uri, body, "application/json") , if you want but you don't have to.
            {
                webRequest.method = "POST";
                if (headers != null)
                    foreach (var header in headers)
                        webRequest.SetRequestHeader(header.Key, header.Value);

                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError) // for older Unity versions, you might have to replace this with "webRequest.isNetworkError"
                {
                    NutakuSdk.Log("Error: " + webRequest.responseCode + "\n" + webRequest.error);
                    rawResult.body = webRequest.error;
                }
                else
                {
                    NutakuSdk.Log("Received from server: " + webRequest.responseCode + "\n" + webRequest.downloadHandler.text);
                    rawResult.body = webRequest.downloadHandler.text;
                }
                rawResult.responseCode = (int)webRequest.responseCode;
                if (callbackFunctionDelegateVar != null)
                {
                    callbackFunctionDelegate callbackFunctionDelegateInstance = new callbackFunctionDelegate(callbackFunctionDelegateVar);
                    callbackFunctionDelegateInstance(rawResult);
                }
            }
        }


        private IEnumerator GetRequest(string uri, IEnumerable<KeyValuePair<string, string>> headers)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                if (headers != null)
                    foreach (var header in headers)
                        webRequest.SetRequestHeader(header.Key, header.Value);

                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError) // for older Unity versions, you might have to replace this with "webRequest.isNetworkError"
                {
                    NutakuSdk.Log("Error: " + webRequest.responseCode + "\n" + webRequest.error);
                    rawResult.body = webRequest.error;
                }
                else
                {
                    NutakuSdk.Log("Received from server: " + webRequest.responseCode + "\n" + webRequest.downloadHandler.text);
                    rawResult.body = webRequest.downloadHandler.text;
                }
                rawResult.responseCode = (int)webRequest.responseCode;
                if (callbackFunctionDelegateVar != null)
                {
                    callbackFunctionDelegate callbackFunctionDelegateInstance = new callbackFunctionDelegate(callbackFunctionDelegateVar);
                    callbackFunctionDelegateInstance(rawResult);
                }
            }
        }


        private IEnumerator PutRequest(string uri, IEnumerable<KeyValuePair<string, string>> headers, string body)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, body))
            {
                if (headers != null)
                    foreach (var header in headers)
                        webRequest.SetRequestHeader(header.Key, header.Value);

                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ConnectionError) // for older Unity versions, you might have to replace this with "webRequest.isNetworkError"
                {
                    NutakuSdk.Log("Error: " + webRequest.responseCode + "\n" + webRequest.error);
                    rawResult.body = webRequest.error;
                }
                else
                {
                    NutakuSdk.Log("Received from server: " + webRequest.responseCode + "\n" + webRequest.downloadHandler.text);
                    rawResult.body = webRequest.downloadHandler.text;
                }
                rawResult.responseCode = (int)webRequest.responseCode;
                if (callbackFunctionDelegateVar != null)
                {
                    callbackFunctionDelegate callbackFunctionDelegateInstance = new callbackFunctionDelegate(callbackFunctionDelegateVar);
                    callbackFunctionDelegateInstance(rawResult);
                }
            }
        }
    }
}
#endif