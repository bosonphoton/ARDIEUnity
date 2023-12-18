using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class EyeGaze : MonoBehaviour
{

    public GameObject eyeMarker; //The actual thing that will follow your eye gaze
    public Camera mainCamera; //MLCamera
    public Vector3 headlook; //Where you're looking <<<<<<IMPORTANT>>>>>>


    // Start is called before the first frame update
    void Start()
    {
        MLEyes.Start();        
    }

    // Update is called once per frame
    void Update()
    {
        if (MLEyes.IsStarted)
        {
            headlook = MLEyes.FixationPoint - mainCamera.transform.position;

            RaycastHit _hit;
            if(Physics.Raycast(mainCamera.transform.position, headlook, out _hit))
            {
                eyeMarker.transform.position = _hit.point;
                eyeMarker.transform.LookAt(_hit.normal + _hit.point);

            }
        }
    }
}
