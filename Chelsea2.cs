using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.Protocols;
using UnityEngine;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]


    public class Chelsea2 : MonoBehaviour
    {
        private RosSocket rosSocket;
        private string publication_id;
        private std_msgs.String message;

        public GameObject image1; //aruco marker initialization
        public GameObject eyes;

        public GameObject bottom, middle, top, target;

        private GameObject initializedbottom, initializedmiddle, initializedtop, initializedtarget;
        private GameObject projectedbottom, projectedmiddle, projectedtop; //AR representation objects


        private List<GameObject> allCubes = new List<GameObject>();

        public Material cube1Mat, cube2Mat, cube3Mat, targetMat;
        public Material detectedMaterial, detectedTargetMaterial;

        public List<Material> allMaterials = new List<Material>();

        // Start is called before the first frame update
        void Start()
        {

            initializedbottom = Instantiate(bottom, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initializedbottom.gameObject.name = "Bottom";     
            initializedmiddle = Instantiate(middle, new Vector3(0, 0, 0), Quaternion.identity);
            initializedmiddle.gameObject.name = "Middle";
            initializedtop = Instantiate(top, new Vector3(0, 0, 0), Quaternion.identity);
            initializedtop.gameObject.name = "Top";
            initializedtarget = Instantiate(target, new Vector3(0, 0, 0), Quaternion.identity);
            initializedtarget.gameObject.name = "Target";


            allCubes.Add(initializedbottom);
            allCubes.Add(initializedmiddle);
            allCubes.Add(initializedtop);
            allCubes.Add(initializedtarget);
      

            allMaterials.Add(cube1Mat);
            allMaterials.Add(cube2Mat);
            allMaterials.Add(cube3Mat);
            allMaterials.Add(targetMat);



            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            publication_id = rosSocket.Advertise<std_msgs.String>("gaze_vector");

        }



        // Update is called once per frame
        void Update()
        {
            Vector3 image_position1, eyes_pos;
            image_position1 = image1.transform.position;      //gets position of the aruco markers


            Vector3 position_bottom, position_middle, position_top, targetbase, targetmiddle, targettop;

            position_bottom = image_position1;
            position_bottom.x = position_bottom.x - 0.25f;
            position_bottom.z = position_bottom.z;

            position_middle = image_position1;
            position_middle.x = position_middle.x - 0.45f;
            position_middle.z = position_middle.z;

            position_top = image_position1;
            position_top.x = position_top.x + 0.1f;
            position_top.z = position_top.z;

            targetbase = image_position1;
            targetbase.x = targetbase.x + 0.35f;
            targetbase.z = targetbase.z;

            targetmiddle = image_position1;
            targetmiddle.x = targetmiddle.x + 0.35f;
            targetmiddle.z = targetmiddle.z;
            targetmiddle.y = targetmiddle.y + 0.3f; //modify accoridng to object height

            targettop = image_position1;
            targettop.x = targettop.x + 0.35f;
            targettop.z = targettop.z;
            targettop.y = targettop.y + 0.6f; //modify accoridng to object height


            initializedbottom.transform.position = position_bottom;   
            initializedmiddle.transform.position = position_middle;
            initializedtop.transform.position = position_top;
            initializedtarget.transform.position = targetbase;


            eyes_pos = eyes.transform.position; //gets eye gaze position 


            //if ("p1s1")
            //    projectedbottom.transform.position = targetbase;

            //if ("p1s2")
            //    projectedbottom.transform.position = targetbase;
            //    projectedmiddle.transform.position = targetmiddle;

            //if ("p1s3")
            //    projectedbottom.transform.position = targetbase;
            //    projectedmiddle.transform.position = targetmiddle;
            //    projectedtop.transform.position = targettop;

            //if ("p2s1")
            //    projectedmiddle.transform.position = targetbase;

            //if ("p2s2")
            //    projectedmiddle.transform.position = targetbase;
            //    projectedbottom.transform.position = targetmiddle;

            //if ("p2s3")
            //    projectedmiddle.transform.position = targetbase;
            //    projectedbottom.transform.position = targetmiddle;
            //    projectedtop.transform.position = targettop;



            float distanceCube1, distanceCube2, distanceCube3, distanceTarget;

            distanceCube1 = Vector3.Distance(initializedbottom.transform.position, eyes_pos);  //finds the distnace between the cube and the eyegaze position
            distanceCube2 = Vector3.Distance(initializedmiddle.transform.position, eyes_pos);
            distanceCube3 = Vector3.Distance(initializedtop.transform.position, eyes_pos);
            distanceTarget = Vector3.Distance(initializedtarget.transform.position, eyes_pos);


            float[] cubeDistances = new float[3]; //intializes the varibale cubeDistances with each index as the cube distance
            cubeDistances[0] = distanceCube1;
            cubeDistances[1] = distanceCube2;
            cubeDistances[2] = distanceCube3;


            int closestCubeIndex = GetIndexOfLowestValue(cubeDistances);


            for (int i = 0; i < allCubes.Count; i++)  //for (initialize var, condition for loop to run, what to do with var)
            {
                if (i == closestCubeIndex) //if cube distance is the least 
                {
                    //Debug.Log("Cube Distance of Lowest" + cubeDistances[i]);

                    allCubes[i].GetComponent<MeshRenderer>().material = detectedMaterial; // gets material of allCubes (which stores all detectedCubes)
                                                                                            // changes that material to detecedMaterial (yellow highlight)

                    message = new std_msgs.String(allCubes[i].gameObject.name);
                    this.PublishGaze(message);      //publishes the name of the closest cube 
                                                    //Debug.Log("Object" + message);


                    if (distanceTarget < 0.6f)
                    {
                        Debug.Log("This should be working" + distanceTarget);
                        initializedtarget.GetComponent<MeshRenderer>().material = detectedTargetMaterial; //not working 

                        //target_position = targetbase
                        //allCubes[i].GetComponent<GameObject>().transform.position = target_position; //places the object we are looking at on the base
                        allCubes[i].GetComponent<GameObject>().transform.position = targetbase;

                    }

                    else
                    {
                        initializedtarget.GetComponent<MeshRenderer>().material = cube2Mat; //turns base back to original color
                    }
                    
                }
                else
                {
                    allCubes[i].GetComponent<MeshRenderer>().material = allMaterials[i]; //turns game object materials back to original color
                }
            }

        }



        public void PublishGaze(std_msgs.String message)
        {
            rosSocket.Publish(publication_id, message);
        }

        public int GetIndexOfLowestValue(float[] arr) //definition GetIndexOfLowestValue
        {
            float value = float.PositiveInfinity;
            int index = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < value)
                {
                    index = i;
                    value = arr[i];
                }
            }
            return index;

        }


    }
}
