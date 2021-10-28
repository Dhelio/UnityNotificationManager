using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.dhlworks.minicliptest {

    [System.Serializable]
    public struct NotificationData {
        public string Title;
        public string Description;

        public NotificationData(string Title, string Description, long Time) {
            this.Title = Title;
            this.Description = Description;
        }

        public NotificationData(AndroidJavaObject JavaNotificationData) {
            Title = JavaNotificationData.Get<string>("Title");
            Description = JavaNotificationData.Get<string>("Description");
        }
    }

}