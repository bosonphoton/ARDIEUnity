using System.Collections;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    [RequireComponent(typeof(ControllerConnectionHandler))]
    public class ControllerInput : MonoBehaviour
    {

        #region Public Enum
        [System.Flags]
        public enum DeviceTypesAllowed : byte
        {
            Controller = 1,
            MobileApp = 2,
            All = Controller | MobileApp,
        }
        #endregion

        private ControllerConnectionHandler _controllerConnectionHandler = null;

        public int triggerPressed = 0;

        //private int TP = 0;

        //private int oldTP = 0;

        // Start is called before the first frame update
        void Start()
        {
            _controllerConnectionHandler = GetComponent<ControllerConnectionHandler>();
        }

        // Update is called once per frame
        void Update()
        {
            //oldTP = TP;
            
            float tV = getTriggerPosition();

            if(tV > 0.2f)
            {
                //if (oldTP == 0)
                //{
                //    TP = 1;
                //    triggerPressed = 1;
                //    feedback();
                //}
                //if(oldTP == 1 && TP == 1)
                //{
                //    triggerPressed = 0;
                //}

                triggerPressed = 1;
                feedback();
            }
            else
            {
               // TP = 0;
                triggerPressed = 0;
            }

            //Debug.Log("Trigger Pressed: " + triggerPressed);
        }

        private float getTriggerPosition()
        {
            if (_controllerConnectionHandler.IsControllerValid())
            {
                MLInputController controller = _controllerConnectionHandler.ConnectedController;

                return controller.TriggerValue;
            }

            return -1;
        }

        private void feedback()
        {
            MLInputController controller = _controllerConnectionHandler.ConnectedController;
            controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Click, MLInputControllerFeedbackIntensity.Medium);
        }
    }
}
