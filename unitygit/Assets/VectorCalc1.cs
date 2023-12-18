using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class VectorCalc : MonoBehaviour
    {

        //Stores the user's eye gaze vector
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
    }
}