using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;

namespace MagicLeap
{
    public class ROSVectorCalc : MonoBehaviour
    {

        public Vector3 headsetPos = Vector3.zero;

        public Vector3 eyeGaze = Vector3.zero;

        Vector3 imagePos = Vector3.zero;

        public Vector3 originPos = Vector3.zero;

        Vector3 target3 = Vector3.zero;

        public Vector2 ROStarget = Vector2.zero;

        bool found = false;

        bool set = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            if (set)
            {
                headsetPos = GameObject.Find("EyeGaze").GetComponent<EyeGazeVect>().headsetPosition;
            
                eyeGaze = GameObject.Find("EyeGaze").GetComponent<EyeGazeVect>().eyeGazePoint;

                ROStarget = new Vector2(-(eyeGaze.x - headsetPos.x - originPos.x), -((-eyeGaze.z) - headsetPos.z + originPos.z));
            }
            else
            {
                found = GameObject.Find("GlobeImageVisBehav").GetComponent<GlobeTrackingVisualizer>().foundImage;

                if (found)
                { 
                    imagePos = GameObject.Find("GlobeImageVisBehav").GetComponent<GlobeTrackingVisualizer>().imagePosition;
                    headsetPos = GameObject.Find("EyeGaze").GetComponent<EyeGazeVect>().headsetPosition;
                    originPos = imagePos; /*+ headsetPos*/
                    set = true;
                    GameObject.Find("[ImageRecognition]").transform.gameObject.SetActive(false);
                    Debug.Log("Image Found!");
                }
            }
            
        }
    }

}
