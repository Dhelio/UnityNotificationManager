using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.dhlworks.minicliptest {

    public class NotificationsManager : MonoBehaviour {

        #region PRIVATE_VARIABLES

        private const string TAG = "NotificationsManager";
        private AndroidJavaObject unityActivity;
        private AndroidJavaObject ajo;

        #endregion

        #region PUBLIC_VARIABLES

        [Header("Parameters")]
        [Tooltip("How much seconds pass between one notification and the other. FOR DEBUG ONLY.")]
        public int NotificationsInterval = 60;

        public static NotificationsManager Instance;

        #endregion

        #region PUBLIC_FUNCTIONS

        /// <summary>
        /// Instantly shows a notification, using a placeholder icon.
        /// </summary>
        /// <param name="Title">The title of the notification</param>
        /// <param name="Description">The content of the notification</param>
        public void ShowNotification(string Title, string Description) {
#if UNITY_EDITOR
            Debug.LogWarning($"-{TAG}- Tried to call {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name} from Editor, but it requires Android to run. Test it on the device.");
#elif UNITY_ANDROID
            ajo.Call("Show", Title, Description);
#endif
        }

        /// <summary>
        /// Schedules a notification to be shown in the future. Timing depends on how many notifications are in queue at the moment.
        /// </summary>
        /// <param name="Title">The title of the notification</param>
        /// <param name="Description">The description of the notification</param>
        public void ScheduleNotification(string Title, string Description) {
#if UNITY_EDITOR
            Debug.LogWarning($"-{TAG}- Tried to call {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name} from Editor, but it requires Android to run. Test it on the device.");
#elif UNITY_ANDROID
            ajo.Call("Schedule", Title, Description);
#endif
        }

        /// <summary>
        /// Reschedules a notification to be notified elsewhere in the queue
        /// </summary>
        /// <param name="CurrentPosition">The current position in the queue of the notification</param>
        /// <param name="TargetPosition">The desired position in the queu of the notification</param>
        public void RescheduleNotification(int CurrentPosition, int TargetPosition) {
#if UNITY_EDITOR
            Debug.LogWarning($"-{TAG}- Tried to call {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name} from Editor, but it requires Android to run. Test it on the device.");
#elif UNITY_ANDROID
            ajo.Call("Reschedule", CurrentPosition, TargetPosition);
#endif
        }

        /// <summary>
        /// Deletes all pending notifications
        /// </summary>
        public void DeleteNotifications() {
#if UNITY_EDITOR
            Debug.LogWarning($"-{TAG}- Tried to call {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name} from Editor, but it requires Android to run. Test it on the device.");
#elif UNITY_ANDROID
            ajo.Call("Cancel");
#endif
        }

        /// <summary>
        /// Deletes a specific notification
        /// </summary>
        /// <param name="CurrentPosition">The position of the notification in the queue</param>
        public void DeleteNotification(int CurrentPosition) {
#if UNITY_EDITOR
            Debug.LogWarning($"-{TAG}- Tried to call {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name} from Editor, but it requires Android to run. Test it on the device.");
#elif UNITY_ANDROID
            ajo.Call("Cancel", CurrentPosition);
#endif
        }

        /// <summary>
        /// Gets an array of all pending notifications.
        /// </summary>
        /// <returns>The array of pending notifications.</returns>
        public NotificationData[] GetNotifications() {
#if UNITY_EDITOR
            Debug.LogWarning($"-{TAG}- Tried to call {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name} from Editor, but it requires Android to run. Test it on the device.");
            return null;
#elif UNITY_ANDROID
            AndroidJavaObject[] tmpObj = ajo.Call<AndroidJavaObject[]>("GetNotifications");
            NotificationData[] tmpData = new NotificationData[tmpObj.Length];
            for (int i = 0; i < tmpData.Length; i++) {
                tmpData[i] = new NotificationData(tmpObj[i]);
            }

            return tmpData;
#endif
        }

        /// <summary>
        /// Sets how many seconds pass between notifications. FOR DEBUG PURPOSES ONLY.
        /// </summary>
        /// <param name="Interval">Time in seconds.</param>
        public void SetNotificationsInterval(int Interval) {
#if UNITY_EDITOR
            Debug.LogWarning($"-{TAG}- Tried to call {(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name} from Editor, but it requires Android to run. Test it on the device.");
#elif UNITY_ANDROID
            ajo.Call("SetNotificationsInterval", Interval);
#endif
        }

        #endregion

        #region UNITY_FUNCTIONS

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else if (Instance != this) {
                Destroy(this);
            }

            if (Application.isEditor)
                return;

            unityActivity = (new AndroidJavaClass("com.unity3d.player.UnityPlayer")).GetStatic<AndroidJavaObject>("currentActivity");
            ajo = new AndroidJavaObject("it.dhlworks.utils.Notifications");
        }

        private void Start() {
            if (Application.isEditor)
                return;
            ajo.Call("Initialize", unityActivity);
            ajo.Call("RegisterNotificationChannel", "DhlWorks");

            SetNotificationsInterval(NotificationsInterval);
        }

#endregion
    }

}