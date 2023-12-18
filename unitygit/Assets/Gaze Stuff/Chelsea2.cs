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

        public GameObject red, green, blue, plane, target;
        public GameObject projectedred, projectedgreen, projectedblue;

        private GameObject initializedred, initializedgreen, initializedblue, initializedplane, initializedtarget;
        private GameObject initializedprojectedred, initializedprojectedgreen, initializedprojectedblue;
        private GameObject first_object, second_object, third_object;
        //AR representation objects


        private List<GameObject> allCubes = new List<GameObject>();
        private List<GameObject> objects_at_target = new List<GameObject>();
        private List<GameObject> objects_not_at_target = new List<GameObject>();

        public Material cube1Mat, cube2Mat, cube3Mat, targetMat;
        public Material detectedMaterial, lockedMaterial;

        public List<Material> allMaterials = new List<Material>();

        public float startTime = 0f;
        public float timetoLock = 5f; // 5 seconds to lock in gaze

        bool desiredObjectHighlighted = false;
        bool desiredTargetHighlighted = false;
        bool targetLocked = false;
        bool objectLocked = false;
        bool moveObject1 = false;
        bool moveObject2 = false;
        bool moveObject3 = false;
        bool targetplaced1 = false; //if first object placed
        bool targetplaced2 = false; //if second object placed
        bool targetplaced3 = false; //if third object placed
        string lastObject = "";
        string current_object = "";
        string current_target = "";
        string last_target = "";
        float lockGazeNow = 5f;
        float lockTargetNow = 5f;
        float speed = 0.4f;
        float count = 0; // stores the number of objects moved
        int current_object_to_move = -1;



        private Rigidbody first_rb,second_rb,third_rb,redrb,greenrb,bluerb;

        // Start is called before the first frame update
        void Start()
        {

            initializedred = Instantiate(red, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initializedred.gameObject.name = "red";     
            initializedgreen = Instantiate(green, new Vector3(0, 0, 0), Quaternion.identity);
            initializedgreen.gameObject.name = "green";
            initializedblue = Instantiate(blue, new Vector3(0, 0, 0), Quaternion.identity);
            initializedblue.gameObject.name = "blue";
            initializedtarget = Instantiate(target, new Vector3(0, 0, 0), Quaternion.identity);
            initializedtarget.gameObject.name = "target";
            initializedplane = Instantiate(plane, new Vector3(0, 0, 0), Quaternion.identity);
            initializedplane.gameObject.name = "plane";

            initializedprojectedred = Instantiate(projectedred, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initializedprojectedred.gameObject.name = "red_Transparent";
            initializedprojectedgreen = Instantiate(projectedgreen, new Vector3(0, 0, 0), Quaternion.identity);
            initializedprojectedgreen.gameObject.name = "green_Transparent";
            initializedprojectedblue = Instantiate(projectedblue, new Vector3(0, 0, 0), Quaternion.identity);
            initializedprojectedblue.gameObject.name = "blue_Transparent";


            allCubes.Add(initializedred);
            allCubes.Add(initializedgreen);
            allCubes.Add(initializedblue);
            allCubes.Add(initializedtarget);
      

            allMaterials.Add(cube1Mat);
            allMaterials.Add(cube2Mat);
            allMaterials.Add(cube3Mat);
            allMaterials.Add(targetMat);


            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            publication_id = rosSocket.Advertise<std_msgs.String>("gaze_vector");
            lockGazeNow = Time.time + 100f;
            lockTargetNow = Time.time + 100f;

            objects_not_at_target.Add(initializedprojectedred);
            objects_not_at_target.Add(initializedprojectedgreen);
            objects_not_at_target.Add(initializedprojectedblue);

        }





        // Update is called once per frame
        void Update()
        {
            Debug.Log("Objects At Target" +  objects_at_target.Count);
            Debug.Log("Objects Not At Target" + objects_not_at_target.Count);

            Vector3 image_position1, eyes_pos;
            image_position1 = image1.transform.position;      //gets position of the aruco markers
            Debug.Log("CHELSEA WHERE"+image_position1);


            Vector3 position_red, position_green, position_blue, position_plane, targetbase;

            position_red = image_position1;
            position_red.x = position_red.x - 0.3f;

            position_green = image_position1;

            position_blue = image_position1;
            position_blue.x = position_blue.x + 0.3f;
            position_blue.y = position_blue.y + 0.05f;

            targetbase = image_position1;
            targetbase.z = targetbase.z - 0.35f;

            position_plane = image_position1;
            position_plane.y = position_plane.y - 0.1f;


            initializedplane.transform.position = position_plane;
            initializedred.transform.position = position_red;
            initializedgreen.transform.position = position_green;
            initializedblue.transform.position = position_blue;
            initializedtarget.transform.position = targetbase;


            eyes_pos = eyes.transform.position; //gets eye gaze position 
            float distanceCube1, distanceCube2, distanceCube3;

            distanceCube1 = Vector3.Distance(initializedred.transform.position, eyes_pos);  //finds the distnace between the cube and the eyegaze position
            distanceCube2 = Vector3.Distance(initializedgreen.transform.position, eyes_pos);
            distanceCube3 = Vector3.Distance(initializedblue.transform.position, eyes_pos);


            float[] cubeDistances = new float[3]; //intializes the varibale cubeDistances with each index as the cube distance
            cubeDistances[0] = distanceCube1;
            cubeDistances[1] = distanceCube2;
            cubeDistances[2] = distanceCube3;


            int closestCubeIndex = GetIndexOfLowestValue(cubeDistances);

            /////////////////////////////////////////////////////////////////At the start of every update, we check if objects have already been placed or not//////////////////////////////////////////////////////////////////////////

            for (int i = 0; i < objects_not_at_target.Count; i++) //keep these in place
            {

                if (objects_not_at_target[i].name == "red_Transparent")
                {
                    objects_not_at_target[i].transform.position = position_red;
                }

                if (objects_not_at_target[i].name == "green_Transparent")
                {
                    objects_not_at_target[i].transform.position = position_green;
                }

                if (objects_not_at_target[i].name == "blue_Transparent")
                {
                    objects_not_at_target[i].transform.position = position_blue;
                }
            }

            for (int i = 0; i < objects_not_at_target.Count; i++) //keep these in place
            {
                Debug.Log("Objects Not at Target" + objects_not_at_target[i].name);
            }

                if (objects_at_target.Count == 1)
            {
                objects_at_target[0].transform.position = targetbase;
            }

            /////////////////////////////////////////////////////////////////First must check if objects have already been placed or not//////////////////////////////////////////////////////////////////////////







            for (int i = 0; i < allCubes.Count; i++)
            {
                if (desiredObjectHighlighted == false)
                {
                    if (i == closestCubeIndex)
                    {
                        allCubes[i].GetComponent<MeshRenderer>().material = detectedMaterial;
                        current_object = allCubes[i].gameObject.name;
                    }
                    else
                    {
                        allCubes[i].GetComponent<MeshRenderer>().material = allMaterials[i]; //turns game object materials back to original color
                    }


                    if (current_object == lastObject) // wont these always be the same?
                    {
                        if (Time.time > lockGazeNow) //if the current time > past time + 5 seoncds
                        {
                            Debug.Log("Test3   " + i);
                            GameObject.Find(lastObject).GetComponent<MeshRenderer>().material = lockedMaterial;
                            objectLocked = true;
                            desiredObjectHighlighted = true;
                            lockGazeNow = Time.time + timetoLock; //5 seconds from now

                            if (lastObject == "red")
                            {
                                current_object_to_move = 0;
                            }
                            if (lastObject == "green")
                            {
                                current_object_to_move = 1;
                            }
                            if (lastObject == "blue")
                            {
                                current_object_to_move = 2;
                            }

                        }
                    }
                    else
                    {
                        lockGazeNow = Time.time + timetoLock;
                        lastObject = current_object;
                    }
                }


            }

                /////////////////////////////////////////////////////    Code to lock target highlight   ////////////////////////////////////////////////////////

                if (objectLocked)
                {
                    float distancefromTarget = Vector3.Distance(initializedtarget.transform.position, eyes_pos);  //distance from target to gaze

                    if (desiredTargetHighlighted == false)
                    {
                        if (distancefromTarget < 2f)
                        {
                            initializedtarget.GetComponent<MeshRenderer>().material = detectedMaterial;
                            current_target = initializedtarget.gameObject.name;
                        }
                        else
                        {
                            initializedtarget.GetComponent<MeshRenderer>().material = targetMat; //turns target back to original color
                            last_target = "";
                        }

                        if (current_target == last_target)
                        {
                            if (Time.time > lockTargetNow)
                            {
                                initializedtarget.GetComponent<MeshRenderer>().material = lockedMaterial;                               
                                targetLocked = true;
                                desiredTargetHighlighted = true;
                                lockTargetNow = Time.time + timetoLock;

                                count += 1;
                                Debug.Log("COUNTT IS" + count);
                                if (count == 1)
                                {
                                    moveObject1 = true;
                                }

                                if (count == 2)
                                {
                                    moveObject2 = true;
                                }
                                else
                                {
                                    moveObject3 = true;
                                }

                            
                            }

                        }
                        else
                        {
                            lockTargetNow = Time.time + timetoLock;
                            last_target = current_target;
                        }

                    }

                }

            /////////////////////////////////////////////////////////////Code for which Object to move based on count /////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                if (moveObject1)
                {
                    Debug.Log("MOVING FIRST OBJECT");

                    if (current_object_to_move == 0)
                    {
                        Debug.Log("MOVING RED OBJECT");
                        objects_not_at_target.Remove(initializedprojectedred);
                        redrb = initializedprojectedred.GetComponent<Rigidbody>();
                        redrb.MovePosition(redrb.transform.position + targetbase * Time.deltaTime * speed);
                        initializedprojectedred.transform.position = Vector3.MoveTowards(initializedprojectedred.transform.position, targetbase, speed * Time.deltaTime);                       
                        

                        //next step: make sure you can see the thing moving there, only until it reaches target, we then proceed to add the thing to list::::
                        if (initializedprojectedred.transform.position == targetbase)
                        {
                            if (!objects_at_target.Contains(initializedprojectedred)) //if list does not contain the red , add the red to list
                            {
                                objects_at_target.Add(initializedprojectedred);

                        }

                        }





                    /*                    ////////ROS STUFF////////////////
                                        message = new std_msgs.String(initializedprojectedred.gameObject.name);
                                        this.PublishGaze(message);
                                        ////////ROS STUFF////////////////*/
                }
                    else if (current_object_to_move == 1)
                    {
                        Debug.Log("MOVING GREEN OBJECT");
                        objects_not_at_target.Remove(initializedprojectedgreen);
                        greenrb = initializedprojectedgreen.GetComponent<Rigidbody>();
                        greenrb.MovePosition(greenrb.transform.position + targetbase * Time.deltaTime * speed);
                        initializedprojectedgreen.transform.position = Vector3.MoveTowards(initializedprojectedgreen.transform.position, targetbase, speed * Time.deltaTime);
                        initializedprojectedred.transform.position = position_red;
                        initializedprojectedblue.transform.position = position_blue;
                        if (!objects_at_target.Contains(initializedprojectedgreen))
                        {
                            objects_at_target.Add(initializedprojectedgreen);
                        }

                    //targetplaced1 = true;


                }
                else if (current_object_to_move == 2)
                    {
                        Debug.Log("MOVING BLUE OBJECT");
                        objects_not_at_target.Remove(initializedprojectedblue);
                        bluerb = initializedprojectedblue.GetComponent<Rigidbody>();
                        bluerb.MovePosition(bluerb.transform.position + targetbase * Time.deltaTime * speed);
                        initializedprojectedblue.transform.position = Vector3.MoveTowards(initializedprojectedblue.transform.position, targetbase, speed * Time.deltaTime);
                        initializedprojectedred.transform.position = position_red;
                        initializedprojectedgreen.transform.position = position_green;

                        if (!objects_at_target.Contains(initializedprojectedblue))
                        {
                            objects_at_target.Add(initializedprojectedblue);
                        }

                    //targetplaced1 = true;

                }

            }

                else
                {
                    initializedprojectedred.transform.position = position_red;
                    initializedprojectedgreen.transform.position = position_green;
                    initializedprojectedblue.transform.position = position_blue; //overlays AR transparent objects onblue
                }



    //            if (moveObject2)
    //            {
    //                Debug.Log("MOVING Second OBJECT");

    //                Vector3 second_object_position;
    //                second_object_position = first_object.transform.position; //get the position of the first object 
    //                second_object_position.y = second_object_position.y + 0.5f; //new position to "drop" the object from

    //                if (first_object == initializedprojectedred)
    //                {
    //                    if (current_object_to_move == 0)
    //                    {
    //                    }

    //                    if (current_object_to_move == 1)
    //                    {
    //                        second_object = initializedprojectedgreen;
    //                        greenrb = second_object.GetComponent<Rigidbody>();
    //                        greenrb.MovePosition(greenrb.transform.position + second_object_position * Time.deltaTime * speed);
    //                        second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, targetbase, speed * Time.deltaTime); //modify code so it only goes near the base
    //                                                                                                                                                      //// add code so it gets drop from a height
    ///*                        targetposition2 = second_object.transform.position;*/
    ///*                        initializedprojectedred.transform.position = position_red;
    //                        initializedprojectedblue.transform.position = position_blue;*/
    //                        targetplaced2 = true;

    //                    }
    //                    else if (current_object_to_move == 2)
    //                    {
    //                        second_object = initializedprojectedblue;
    //                        bluerb = second_object.GetComponent<Rigidbody>();
    //                        bluerb.MovePosition(bluerb.transform.position + targetbase * Time.deltaTime * speed);
    //                        second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, targetbase, speed * Time.deltaTime);

    ///*                        targetposition2 = second_object.transform.position;*/
    ///*                        initializedprojectedred.transform.position = position_red;
    //                        initializedprojectedgreen.transform.position = position_green;*/
    //                        targetplaced2 = true;

    //                    }
    //                }
    //                if (first_object == initializedprojectedgreen)
    //                {
    //                    if (current_object_to_move == 0)
    //                    {
    //                        second_object = initializedprojectedred;
    //                        redrb = second_object.GetComponent<Rigidbody>();
    //                        redrb.MovePosition(redrb.transform.position + targetbase * Time.deltaTime * speed);
    //                        second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, targetbase, speed * Time.deltaTime);
    // /*                       initializedprojectedgreen.transform.position = position_green;
    //                        initializedprojectedblue.transform.position = position_blue;*/
    //                        targetplaced2 = true;


    //                    }
    //                    else if (current_object_to_move == 1)
    //                    {
    //                    }

    //                    else if (current_object_to_move == 2)
    //                    {
    //                        second_object = initializedprojectedblue;
    //                        bluerb = second_object.GetComponent<Rigidbody>();
    //                        bluerb.MovePosition(bluerb.transform.position + targetbase * Time.deltaTime * speed);
    //                        second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, targetbase, speed * Time.deltaTime);
    //                        initializedprojectedred.transform.position = position_red;
    //                        initializedprojectedgreen.transform.position = position_green;
    //                        targetplaced2 = true;

    //                    }
    //                }
    //                if (first_object == initializedprojectedblue)
    //                {
    //                    if (current_object_to_move == 0)
    //                    {
    //                        second_object = initializedprojectedred;
    //                        redrb = second_object.GetComponent<Rigidbody>();
    //                        redrb.MovePosition(redrb.transform.position + targetbase * Time.deltaTime * speed);
    //                        second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, targetbase, speed * Time.deltaTime);
    //                        initializedprojectedgreen.transform.position = position_green;
    //                        initializedprojectedblue.transform.position = position_blue;
    //                        targetplaced2 = true;


    //                    }
    //                    else if (current_object_to_move == 1)
    //                    {
    //                        second_object = initializedprojectedgreen;
    //                        greenrb = second_object.GetComponent<Rigidbody>();
    //                        greenrb.MovePosition(greenrb.transform.position + targetbase * Time.deltaTime * speed);
    //                        second_object.transform.position = Vector3.MoveTowards(second_object.transform.position, targetbase, speed * Time.deltaTime);
    //                        initializedprojectedred.transform.position = position_red;
    //                        initializedprojectedblue.transform.position = position_blue;
    //                        targetplaced2 = true;

    //                    }
    //                    else if (current_object_to_move == 2)
    //                    {
    //                    }
    //                }



    //            }

    //            else
    //            {
    //                initializedprojectedred.transform.position = position_red;
    //                initializedprojectedgreen.transform.position = position_green;
    //                initializedprojectedblue.transform.position = position_blue; //overlays AR transparent objects onblue
    //            }



    //            if (moveObject3)
    //            {

    //            }

    //            else
    //            {
    //                initializedprojectedred.transform.position = position_red;
    //                initializedprojectedgreen.transform.position = position_green;
    //                initializedprojectedblue.transform.position = position_blue; //overlays AR transparent objects onblue
    //            }




                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////Code to place 2nd object /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                if (targetplaced1) //if first object placed at target
                {
                /*                Vector3 positionblue1, positionred1, positiongreen1; //where the positions are at the first step

                                positionblue1 = initializedprojectedblue.transform.position;
                                positionred1 = initializedprojectedred.transform.position;
                                positiongreen1 = initializedprojectedgreen.transform.position;

                                if (positionblue1.x == targetbase.x && positionblue1.z == targetbase.z)
                                {

                                }*/
                    Debug.Log("TARGETPLACED1");
              
                    lastObject = "";
                    current_object = "";
                    current_target = "";
                    last_target = "";
                    desiredTargetHighlighted = false;
                    targetLocked = false;
                    objectLocked = false;
                    desiredObjectHighlighted = false;


          
               
                }

                if (targetplaced2) 
                {
                    Debug.Log("TARGETPLACED2");
                    moveObject1 = false;
                    moveObject2 = false;
                    lastObject = "";
                    current_object = "";
                    current_target = "";
                    last_target = "";
                    desiredTargetHighlighted = false;
                    targetLocked = false;
                    objectLocked = false;
                    desiredObjectHighlighted = false;

                }
    /*
                if (targetplaced3) //if first object placed at target
                {
                    Vector3 positionblue3, positionred3, positiongreen3; //where the positions are at the first step

                    positionblue3 = initializedprojectedblue.transform.position;
                    positionred3 = initializedprojectedred.transform.position;
                    positiongreen3 = initializedprojectedgreen.transform.position;            

                }
    */

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
