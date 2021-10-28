using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.dhlworks.minicliptest {
    /// <summary>
    /// Class that updates a TextMeshProUGUI component to show the extras of calling intent for this application.
    /// </summary>
    public class NotificationContentHandler : MonoBehaviour {
        private const string extraTag = "NotificationExtra";

        private void Awake() {
            AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getIntent");
            string tmp = "Nothing to show.";
            if (intent.Call<bool>("hasExtra", extraTag)) {
                tmp = (intent.Call<AndroidJavaObject>("getExtras")).Call<string>("getString", extraTag);
            } else {
                Debug.LogWarning($"-NotificationContentHandler- launching intent has no extra arguments with key {extraTag}.");
            }
            GetComponent<TMPro.TextMeshProUGUI>().text = tmp;
        }
    }
}