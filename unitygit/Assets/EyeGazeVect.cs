using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class EyeGazeVect : MonoBehaviour
    {

        /*
         * |||||||||||||||||||ORIGINAL SCRIPT|||||||||||||||||||
         * 
         * //Stores the user's eye gaze vector
        private Vector3 gazeVector = Vector3.zero;

        public Camera mainCamera;


        // Start is called before the first frame update
        void Start()
        {
            //Starts MLEyes
            MLEyes.Start();
            Debug.Log("MLEyes started");
        }

        // Update is called once per frame
        void Update()
        {
            if (MLEyes.IsStarted)
            {
                //Sets the gaze vector equal to where the user is looking
                gazeVector = MLEyes.FixationPoint - mainCamera.transform.position;

                Debug.Log(gazeVector.ToString());
            }

        }

        void OnDestroy()
        {
            MLEyes.Stop();
        }*/

        [SerializeField, Tooltip("The reference to the class to handle results from.")]
        private BaseRaycast _raycast = null;

        // Stores result of raycast
        private bool _hit = false;

        [SerializeField, Tooltip("The default distance for the cursor when a hit is not detected.")]
        private float _defaultDistance = 9.0f;

        public Vector3 headsetPosition = Vector3.zero;

        public Vector3 eyeGazePoint = Vector3.zero;



        public bool Hit
        {
            get
            {
                return _hit;
            }
        }


        void Awake()
        {

            // Check if the Layer is set to Default and disable any child colliders.
            if (gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                Collider[] colliders = GetComponentsInChildren<Collider>();

                // Disable any active colliders.
                foreach (Collider collider in colliders)
                {
                    collider.enabled = false;
                }

                // Warn user if any colliders had to be disabled.
                if (colliders.Length > 0)
                {
                    Debug.LogWarning("Colliders have been disabled on this RaycastVisualizer.\nIf this is undesirable, change this object's layer to something other than Default.");
                }
            }

            if (_raycast == null)
            {
                Debug.LogError("Error: RaycastVisualizer._raycast is not set, disabling script.");
                enabled = false;
                return;
            }
        }

        public void OnRaycastHit(MLWorldRays.MLWorldRaycastResultState state, RaycastHit result, float confidence)
        {
            if (state != MLWorldRays.MLWorldRaycastResultState.RequestFailed && state != MLWorldRays.MLWorldRaycastResultState.NoCollision)
            {
                // Update the cursor position and normal.
                transform.position = result.point;
                transform.LookAt(result.normal + result.point, _raycast.RayDirection);
                transform.localScale = Vector3.one;

                //Debug.Log(_raycast.RayOrigin.ToString() + ", " + _raycast.RayDirection.ToString() + ", " + result.point.ToString());

                headsetPosition = _raycast.RayOrigin;

                eyeGazePoint = result.point;

                _hit = true;
            }
            else
            {
                // Update the cursor position and normal.
                transform.position = (_raycast.RayOrigin + (_raycast.RayDirection * _defaultDistance));
                transform.LookAt(_raycast.RayOrigin);
                transform.localScale = Vector3.one;

                //Debug.Log(_raycast.RayOrigin.ToString() + ", " + _raycast.RayDirection.ToString() + ", noTarget");

                headsetPosition = _raycast.RayOrigin;

                eyeGazePoint = Vector3.zero;

                _hit = false;
            }
        }
    }
}
