using System;
using System.Threading;
using RosSharp.RosBridgeClient.Protocols;
using UnityEngine;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;
using MagicLeap;

namespace RosSharp.RosBridgeClient
{

    [RequireComponent(typeof(RosConnector))]
    public class GazePublisher : MonoBehaviour
    {
        private RosSocket rosSocket;
        private string publication_id;
        private std_msgs.String message;

        public void Start()
        {
            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            publication_id = rosSocket.Advertise<std_msgs.String>("gaze_vector");
        }

        public void Update()
        {
            //if(GameObject.Find("[Controller]").GetComponent<ControllerInput>().triggerPressed == 1)
            //{
            //    Debug.Log(GameObject.Find("VectCalc").GetComponent<ROSVectorCalc>().ROStarget.x + ", " + GameObject.Find("VectCalc").GetComponent<ROSVectorCalc>().ROStarget.y + ", " + GameObject.Find("[Controller]").GetComponent<ControllerInput>().triggerPressed);
            //}
            message = new std_msgs.String("Green");
            this.PublishGaze(message);
        }

        public void PublishGaze(std_msgs.String message)
        {
            rosSocket.Publish(publication_id, message);
        }
    }
}