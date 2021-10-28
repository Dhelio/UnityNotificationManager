using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace it.dhlworks.minicliptest {

    /// <summary>
    /// Class that handles user interaction with the UI.
    /// </summary>
    public class InteractionController : MonoBehaviour {

        #region PRIVATE_VARIABLES
        private Transform currentTarget = null; //current notification to target
        private GameObject currentGhost = null; //a prefab to hold the place of the notification while hovering
        private PointerEventData eData; //event data for the mouse pointer
        private int targetNotificationOrder; //the starting order of the notification to change.

        [Header("References")]
        [SerializeField] private Transform uiCanvas;
        [SerializeField] private GraphicRaycaster gRaycaster;
        #endregion

        #region PUBLIC_VARIABLES
        public static InteractionController Instance;
        #endregion

        #region UNITY_FUNCTIONS

        private void Awake() {
            //Singleton
            if (Instance == null) {
                Instance = this;
            } else if (Instance != this) {
                Destroy(this);
            }
        }

        private void Update() {

            //When clicking it checks if we're on a MovableUI, and sets it as current target, while spawning a ghost ui underneath.
            if (Input.GetMouseButtonDown(0)) {
                eData = new PointerEventData(EventSystem.current) {
                    position = Input.mousePosition
                };
                List<RaycastResult> lrr = new List<RaycastResult>();
                gRaycaster.Raycast(eData, lrr);
                for (int i = 0; i < lrr.Count; i++) {
                    if (lrr[i].gameObject.tag == "MovableUI") {
                        currentTarget = lrr[i].gameObject.transform;
                        currentGhost = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs\\Notification_Ghost_Prefab"));
                        currentGhost.transform.SetParent(currentTarget.parent);
                        currentGhost.transform.SetSiblingIndex(currentTarget.GetSiblingIndex());
                        currentTarget.SetParent(uiCanvas);
                        currentTarget.tag = "GhostUI"; //Changing the tag, otherwise it would always catch currentTarget while dragging.
                        targetNotificationOrder = currentGhost.transform.GetSiblingIndex();
                        break;
                    }
                }
            }

            //Moves currentTarget with the mouse and contextually moves the GhostUI to let the user see where it will be placed.
            if (currentTarget != null) {
                currentTarget.transform.position = Input.mousePosition;
                eData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
                List<RaycastResult> lrr = new List<RaycastResult>();
                gRaycaster.Raycast(eData, lrr);
                for (int i = 0; i < lrr.Count; i++) {
                    if (lrr[i].gameObject.tag == "MovableUI") {
                        currentGhost.transform.SetSiblingIndex(lrr[i].gameObject.transform.GetSiblingIndex());
                        break;
                    }
                }
            }

            //Drop behaviour.
            if (Input.GetMouseButtonUp(0) && currentTarget != null) {
                currentTarget.SetParent(currentGhost.transform.parent);
                currentTarget.SetSiblingIndex(currentGhost.transform.GetSiblingIndex());
                DestroyImmediate(currentGhost);
                NotificationsManager.Instance.RescheduleNotification(targetNotificationOrder, currentTarget.transform.GetSiblingIndex()); //Reschedules the notifications on the low level library
                AppManager.Instance.UpdateSchedules(targetNotificationOrder, currentTarget.transform.GetSiblingIndex()); //Updates the time on the current screen.
                currentTarget.tag = "MovableUI";
                currentTarget = null;
            }
        }

        #endregion
    }
}