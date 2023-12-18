using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class EyeGazeExample : MonoBehaviour
    {

        [SerializeField, Tooltip("Raycast from eyegaze.")]
        private WorldRaycastEyes _raycastEyes = null;

        private float _confidence = 0.0f;

        void Awake()
        {
            if (_raycastEyes == null)
            {
                Debug.LogError("Error: RaycastExample._raycastEyes is not set, disabling script.");
                enabled = false;
                return;
            }
        }

        public void OnRaycastHit(MLWorldRays.MLWorldRaycastResultState state, RaycastHit result, float confidence)
        {
            _confidence = confidence;
        }
    }
}
