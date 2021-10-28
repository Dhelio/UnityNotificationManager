using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.dhlworks.minicliptest {

    public class AppManager : MonoBehaviour {

        #region PRIVATE_VARIABLES
        private const string TAG = "AppManager";

        [Header("Parameters")]
        [Tooltip("List of notifications to schedule.")]
        [SerializeField] private NotificationData[] notifications;

        [Header("References")]
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform notificationList;
        #endregion

        #region PUBLIC_VARIABLES
        public static AppManager Instance;
        #endregion

        #region PRIVATE_FUNCTIONS

        /// <summary>
        /// Schedules a notification.
        /// </summary>
        /// <param name="Id">Id for the notification.</param>
        /// <param name="Title">Title for the notification.</param>
        /// <param name="Description">Description of the notification.</param>
        /// <param name="TTL">How many senconds in the future should it be shown.</param>
        private void ScheduleNotification(string Title, string Description) {
            NotificationsManager.Instance.ScheduleNotification(Title, Description);
            AddNotificationToList(Title, Description);
        }

        /// <summary>
        /// Adds a notification to the UI.
        /// </summary>
        /// <param name="Id">Id for the notification.</param>
        /// <param name="Title">Title for the notification.</param>
        /// <param name="Description">Description of the notification.</param>
        /// <param name="TTL">How many senconds in the future should it be shown.</param>
        private void AddNotificationToList(string Title, string Description) {
            GameObject tmp = GameObject.Instantiate<GameObject>(notificationPrefab);
            tmp.transform.SetParent(notificationList, false);
            tmp.GetComponent<NotificationHandler>().Set(Title, Description);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        /// <summary>
        /// Function called by the Schedule button. Schedules all the notifications specified in the Editor.
        /// </summary>
        public void Schedule() {
            for (int i = 0; i < notifications.Length; i++) {
                ScheduleNotification(notifications[i].Title, notifications[i].Description);
            }
        }

        /// <summary>
        /// Function called by the Cancel button. Cancels all pending notifications.
        /// </summary>
        public void Cancel() {
            if (notificationList.childCount > 0) {
                NotificationsManager.Instance.DeleteNotifications();
                while (notificationList.childCount > 0) {
                    DestroyImmediate(notificationList.GetChild(0).gameObject);
                }
            }
        }

        /// <summary>
        /// Updates the schedules of the notifcation list
        /// </summary>
        /// <param name="FirstOrder">An index from where to start</param>
        /// <param name="LastOrder">An index where to end</param>
        public void UpdateSchedules(int FirstOrder, int LastOrder) {
            //Sanity check
            if (FirstOrder > LastOrder) {
                int tmp = FirstOrder;
                FirstOrder = LastOrder;
                LastOrder = tmp;
            }
            for (int i = FirstOrder; i <= LastOrder; i++) {
                notificationList.GetChild(i).GetComponent<NotificationHandler>().UpdateTTL();
            }
        }

        #endregion

        #region UNITY_FUNCTIONS

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else if (Instance != this) {
                Destroy(this);
            }
        }

        private void OnApplicationPause(bool pause) {
            if (!pause) {
                if (notificationList.childCount > 0) {
                    while (notificationList.childCount > 0) {
                        DestroyImmediate(notificationList.GetChild(0).gameObject);
                    }
                }
                NotificationData[] notifications = NotificationsManager.Instance.GetNotifications();
                if (notifications != null) {
                    for (int i = 0; i < notifications.Length; i++) {
                        AddNotificationToList(notifications[i].Title, notifications[i].Description);
                    }
                }
            }
        }

        #endregion

    }

}