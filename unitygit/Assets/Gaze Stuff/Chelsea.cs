using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.Protocols;
using UnityEngine;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]


    public class Chelsea : MonoBehaviour
    {
        private RosSocket rosSocket;
        private string publication_id;
        private std_msgs.String message;

        public GameObject image1, image2, image3;
        public GameObject eyes;

        public GameObject cube1, cube2, cube3, cube4;
        public GameObject redcup, redplate, greencup, greenplate;

        //public GameObject cube5;

        private GameObject detectedCube1, detectedCube2, detectedCube3, detectedCube4;

        private GameObject detectedredcup, detectedredplate, detectedgreencup, detectedgreenplate;


        private List<GameObject> allCubes = new List<GameObject>();

        public Material cube1Mat, cube2Mat, cube3Mat, cube4Mat;
        public Material detectedMaterial;

        public List<Material> allMaterials = new List<Material>();

        // Start is called before the first frame update
        void Start()
        {
            //detectedCube1 = Instantiate(cube1, new Vector3(100, 100, 0), Quaternion.identity); //initialize the RGB cubes when marker is detected
            //detectedCube1.gameObject.name = "Cube 1 (Red)";     //red cube corresponds to one unique aruco marker
            //detectedCube2 = Instantiate(cube2, new Vector3(100, 100, 0), Quaternion.identity);
            //detectedCube2.gameObject.name = "Cube 2 (Green)";
            //detectedCube3 = Instantiate(cube3, new Vector3(100, 100, 0), Quaternion.identity);
            //detectedCube3.gameObject.name = "Cube 3 (Blue)";


            detectedredcup = Instantiate(redcup, new Vector3(0,0, 0), Quaternion.identity); //initialize the RGB cubes when marker is detected
            detectedredcup.gameObject.name = "Red Cup";     //red cube corresponds to one unique aruco marker
            detectedredplate = Instantiate(redplate, new Vector3(0,0, 0), Quaternion.identity);
            detectedredplate.gameObject.name = "Red Plate";
            detectedgreencup = Instantiate(greencup, new Vector3(0, 0, 0), Quaternion.identity);
            detectedgreencup.gameObject.name = "Green Cup";
            detectedgreenplate = Instantiate(greenplate, new Vector3(0, 0, 0), Quaternion.identity);
            detectedgreenplate.gameObject.name = "Green Plate";


            allCubes.Add(detectedredcup);
            allCubes.Add(detectedredplate);
            allCubes.Add(detectedgreencup);
            allCubes.Add(detectedgreenplate);

            allMaterials.Add(cube1Mat);
            allMaterials.Add(cube2Mat);
            allMaterials.Add(cube3Mat);
            allMaterials.Add(cube4Mat);


            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            publication_id = rosSocket.Advertise<std_msgs.String>("gaze_vector");

        }



        // Update is called once per frame
        void Update()
        {
            Vector3 image_position1, image_position2, image_position3, eyes_pos;
            image_position1 = image1.transform.position;        //gets position of the aruco markers
            image_position2 = image2.transform.position;
            image_position3 = image3.transform.position;

            //detectedCube1.transform.position = image_position1;     // overlay detected cube (colored cube) object on the position of the aruco markers
            //detectedCube1.transform.rotation = image1.transform.rotation;
            //detectedCube2.transform.position = image_position2;
            //detectedCube2.transform.rotation = image2.transform.rotation;
            //detectedCube3.transform.position = image_position3;
            //detectedCube3.transform.rotation = image3.transform.rotation;

            Vector3 position_redcup, position_redplate, position_greencup, position_greenplate;

            position_redcup = image_position1;
            position_redcup.x = position_redcup.x- 0.1f;
            position_redcup.z = position_redcup.z;


            position_redplate = image_position1;
            position_redplate.x = position_redplate.x - 0.35f;
            position_redplate.z = position_redplate.z;

            position_greencup = image_position1;
            position_greencup.x = position_greencup.x + 0.2f;
            position_greencup.z = position_greencup.z;

            position_greenplate = image_position1;
            position_greenplate.x = position_greenplate.x + 0.3f;
            position_greenplate.z = position_greenplate.z;


            detectedredcup.transform.position = position_redcup;     // overlay detected cube (colored cube) object on the position of the aruco markers
            //detectedCup.transform.rotation = image1.transform.rotation;
            detectedredplate.transform.position = position_redplate;
            //detectedPlate.transform.rotation = image1.transform.rotation;
            detectedgreencup.transform.position = position_greencup;
            //detectedSpoon.transform.rotation = image1.transform.rotation;
            detectedgreenplate.transform.position = position_greenplate;

            eyes_pos = eyes.transform.position; //gets eye gaze position 
           // Debug.Log("image1" + image_position1);
           // Debug.Log("image2" + image_position2);
           // Debug.Log("image3" + image_position3);
           // Debug.Log("eyes" + eyes_pos);

            float distanceCube1, distanceCube2, distanceCube3, distanceCube4;

            distanceCube1 = Vector3.Distance(detectedredcup.transform.position, eyes_pos);  //finds the distnace between the cube and the eyegaze position
            distanceCube2 = Vector3.Distance(detectedredplate.transform.position, eyes_pos);
            distanceCube3 = Vector3.Distance(detectedgreencup.transform.position, eyes_pos);
            distanceCube4 = Vector3.Distance(detectedgreenplate.transform.position, eyes_pos);

            Debug.Log(distanceCube1);
            Debug.Log(distanceCube2);
            Debug.Log(distanceCube3);
            Debug.Log(distanceCube4);


            float[] cubeDistances = new float[4]; //intializes the varibale cubeDistances with each index as the cube distance
            cubeDistances[0] = distanceCube1;
            cubeDistances[1] = distanceCube2;
            cubeDistances[2] = distanceCube3;
            cubeDistances[3] = distanceCube4;

            // Debug.Log("Eyes"+ eyes_pos); //why is it only (0,0,1)?
            
            //...........................
            //float[] positions = new float[3]; //intializes the varibale positions with each index as the cube distance
            //positions[0] = eyes.transform.position.x - position_cup.x;
            //positions[1] = eyes.transform.position.x- position_plate.x;
            //positions[2] = eyes.transform.position.x - position_spoon.x;
            //...............................
            //Debug.Log("eyex" + positions[0]);

            int closestCubeIndex = GetIndexOfLowestValue(cubeDistances);


            for (int i = 0; i < allCubes.Count; i++)  //for (initialize var, condition for loop to run, what to do with var)
            {
                if (i == closestCubeIndex) //if cube distance is the least 
                {
                    Debug.Log("Cube Distance of Lowest" + cubeDistances[i]);
                    if (cubeDistances[i] < 0.7f)
                    {
                        allCubes[i].GetComponent<MeshRenderer>().material = detectedMaterial; // gets material of allCubes (which stores all detectedCubes)
                                                                                              // changes that material to detecedMaterial (yellow highlight)
                        message = new std_msgs.String(allCubes[i].gameObject.name);
                        this.PublishGaze(message);      //publishes the name of the closest cube 
                        Debug.Log("Object" + message);
                    }
                }
                else
                {
                    allCubes[i].GetComponent<MeshRenderer>().material = allMaterials[i];
                }
            }  
            


        }

        public int GetIndexIfThreshold(float[] arr) //definition GetIndexOfLowestValue, returns index of cube if passes threshold
        {
            float value = float.PositiveInfinity;
            float threshold = 0.5f; //some threshold value (CHANGE THIS)
            int index = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < value)  //if the distances between gaze and object are less than infinity
                    if (arr[i] < threshold)  //if distance between gaze and object less than some threshold
                    {
                        index = i;     //set index to current index
                        value = arr[i];   //set current distance as the next thing to compare with
                    }
            }
            return index;
         
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
