using System.Collections;
using UnityEngine;
using TMPro;
using System;

namespace it.dhlworks.minicliptest {

    /// <summary>
    /// Class that handles initialization for the Notification prefab, as well as some utility methods.
    /// </summary>
    public class NotificationHandler : MonoBehaviour {

        #region PRIVATE_VARIABLES
        private TextMeshProUGUI titleText;
        private TextMeshProUGUI descriptionText;
        private TextMeshProUGUI ttlText;
        private int ttl;
        private int elapsedTtl;
        private Coroutine countdownCoroutine = null;
        private bool has_Awoken = false;
        #endregion

        #region PRIVATE_FUNCTIONS
        private IEnumerator Countdown() {
            WaitForSeconds wfs = new WaitForSeconds(1);
            for (; ttl >= 0; ttl--, elapsedTtl++) {
                yield return wfs;
                ttlText.text = ttl.ToString();
            }
            Destroy(this.gameObject);
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        public void Set(string Title, string Description) {
            if (!has_Awoken)
                Awake();

            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);

            titleText.text = Title;
            descriptionText.text = Description;
            ttl = (transform.GetSiblingIndex() + 1) * NotificationsManager.Instance.NotificationsInterval;
            elapsedTtl = 0;
            ttlText.text = ttl.ToString();

            countdownCoroutine = StartCoroutine(Countdown());

        }

        /// <summary>
        /// Updates the time to live of the notifications prefabs.
        /// </summary>
        public void UpdateTTL() {
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);
            ttl = (transform.GetSiblingIndex() + 1) * (NotificationsManager.Instance.NotificationsInterval) - elapsedTtl;           
            ttlText.text = ttl.ToString();
            countdownCoroutine = StartCoroutine(Countdown());
        }

        public void Cancel() {
            NotificationsManager.Instance.DeleteNotification(transform.GetSiblingIndex());
            Destroy(this.gameObject);
        }

        #endregion

        #region UNITY_FUNCTIONS

        private void Awake() {
            titleText = transform.GetChild(0).Find("Label_Title").GetComponent<TextMeshProUGUI>();
            descriptionText = transform.GetChild(0).Find("Label_Description").GetComponent<TextMeshProUGUI>();
            ttlText = transform.GetChild(0).Find("Label_TimeLeft").GetComponent<TextMeshProUGUI>();
            has_Awoken = true;
            elapsedTtl = 0;
        }

        #endregion

    }

}