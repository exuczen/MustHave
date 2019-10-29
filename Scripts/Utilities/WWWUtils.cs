using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MustHave.Utilities
{
    public struct WWWUtils
    {
        public static void LoadBinaryFromWWW(MonoBehaviour context, string url, Action<byte[]> onSuccess, Action<string> onError = null)
        {
            context.StartCoroutine(LoadBinaryFromWWWRoutine(url, onSuccess, onError));
        }

        public static IEnumerator LoadBinaryFromWWWRoutine(string url, Action<byte[]> onSuccess, Action<string> onError = null, int timeout = 30)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.timeout = timeout;
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    onError?.Invoke(www.error);
                }
                else
                {
                    if (www.downloadedBytes > 0 && www.downloadHandler.data != null)
                    {
                        onSuccess(www.downloadHandler.data);
                    }
                    else
                    {
                        onError?.Invoke("data == null || data.Length == 0");
                    }
                }
            }
        }
    } 
}
