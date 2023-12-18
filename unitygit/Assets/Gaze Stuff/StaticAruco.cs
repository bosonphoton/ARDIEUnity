using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.Protocols;
using UnityEngine;
using UnityEngine.UI;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]


    public class StaticAruco : MonoBehaviour
    {
        private RosSocket rosSocket;
        private string publication_id;
        private string subscription_id;
        private std_msgs.String message;

        public GameObject image1; //aruco marker initialization
        public GameObject eyes;
        

        public GameObject table1, table2, table3;
        public GameObject solidtable1, solidtable2;
        private GameObject initsolidtable1, initsolidtable2;
        public GameObject projectedred1, projectedblue1, projectedred2, projectedblue2;
        public GameObject projectedgreen1, projectedgreen2;
        private GameObject initializedtable1, initializedtable2, initializedtable3;
        private GameObject initializedprojectedred1, initializedprojectedred2, initializedprojectedblue1, initializedprojectedblue2;
        private GameObject initializedprojectedgreen1, initializedprojectedgreen2;
        private GameObject first_object,second_object, third_object;

        //AR representation objects


        private List<GameObject> allCubes = new List<GameObject>();
        private List<GameObject> objects_at_target = new List<GameObject>();
        private List<GameObject> objects_not_at_target = new List<GameObject>();
    

        public Material table1mat, table2mat, targetmat;
        public Material detectedMaterial, lockedMaterial;

        public List<Material> allMaterials = new List<Material>();

        public float startTime = 0f;
        public float timetoLock = 5f; // 5 seconds to lock in gaze

        bool execute1 = true;
        bool execute2red = true; //for moveobject2
        bool execute2green = true;
        bool execute2blue = true;
        bool execute3 = true; //moveobject3

        bool getaruco = true;
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
        string last_object_placed = "";
        float lockGazeNow = 5f;
        float lockTargetNow = 5f;
        float speed = 0.4f;
        float count = 0; // stores the number of objects moved
        int current_object_to_move = -1;
        bool projectred1 = false;
        bool projectblue1 = false;
        bool projectred2 = false;
        bool projectblue2 = false;
        bool projectgreen1 = false;
        bool projectgreen2 = false;
        bool projectred1_2 = false; // second object projection
        bool projectred2_2 = false;
        bool projectblue2_2 = false;
        bool projectblue1_2 = false;
        bool projectgreen1_2 = false;
        bool projectgreen2_2 = false;
        bool projectred1_3 = false; // second object projection
        bool projectred2_3 = false;
        bool projectblue2_3 = false;
        bool projectblue1_3 = false;
        bool projectgreen1_3 = false;
        bool projectgreen2_3 = false;
        bool initobjects = true;

        string current_table_locked = "";

        bool remove_last = false;

        private string current_obj_to_project = "";

        bool subscribe_publisher_initializer = true;
        int count_subscriber_publisher = 1;

        Vector3 image_position1;
        Vector3 position_table1, position_table2, position_table3;
        Vector3 position_solidtable1, position_solidtable2;
        Vector3 pr1, pb1, pg1, pr2, pb2, pg2;

        Vector3 newpos1, newpos2, newpos3; //positions after dropping


        private Rigidbody first_rb, second_rb, third_rb;

        // Start is called before the first frame update
        void Start()
        {
            current_obj_to_project = "red_1";
            initializedtable1 = Instantiate(table1, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initializedtable1.gameObject.name = "table1";
            initializedtable2 = Instantiate(table2, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initializedtable2.gameObject.name = "table2";
            initializedtable3 = Instantiate(table3, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initializedtable3.gameObject.name = "table3";

            initsolidtable1 = Instantiate(solidtable1, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initsolidtable1.gameObject.name = "solidtable1";
            initsolidtable2 = Instantiate(solidtable2, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initsolidtable2.gameObject.name = "solidtable2";

            allCubes.Add(initializedtable1);
            allCubes.Add(initializedtable2);

            allMaterials.Add(table1mat);
            allMaterials.Add(table2mat);
            allMaterials.Add(targetmat);

            //table1.GetComponent<MeshRenderer>().material = detectedMaterial; //why?


            initializedprojectedred1 = Instantiate(projectedred1, new Vector3(0, 0, 0), Quaternion.identity); //initialize objects when marker is detected
            initializedprojectedred1.gameObject.name = "projectedred1";
            initializedprojectedred2 = Instantiate(projectedred2, new Vector3(0, 0, 0), Quaternion.identity);
            initializedprojectedred2.gameObject.name = "projectedred2";
            initializedprojectedblue1 = Instantiate(projectedblue1, new Vector3(0, 0, 0), Quaternion.identity);
            initializedprojectedblue1.gameObject.name = "projectedblue1";
            initializedprojectedblue2 = Instantiate(projectedblue2, new Vector3(0, 0, 0), Quaternion.identity);
            initializedprojectedblue2.gameObject.name = "projectedblue2";
            initializedprojectedgreen1 = Instantiate(projectedgreen1, new Vector3(0, 0, 0), Quaternion.identity);
            initializedprojectedgreen1.gameObject.name = "projectedgreen1";
            initializedprojectedgreen2 = Instantiate(projectedgreen2, new Vector3(0, 0, 0), Quaternion.identity);
            initializedprojectedgreen2.gameObject.name = "projectedgreen2";


            initializedprojectedred1.gameObject.SetActive(false);
            initializedprojectedred2.gameObject.SetActive(false);
            initializedprojectedblue1.gameObject.SetActive(false);
            initializedprojectedblue2.gameObject.SetActive(false);
            initializedprojectedgreen1.gameObject.SetActive(false);
            initializedprojectedgreen2.gameObject.SetActive(false);



            ///////////////////disable objects/////////////////////
            ///////////////////////////////////////////


            objects_not_at_target.Add(initializedprojectedred1);
            objects_not_at_target.Add(initializedprojectedred2);
            objects_not_at_target.Add(initializedprojectedblue1);
            objects_not_at_target.Add(initializedprojectedblue2);
            objects_not_at_target.Add(initializedprojectedgreen1);
            objects_not_at_target.Add(initializedprojectedgreen2);


            rosSocket = transform.GetComponent<RosConnector>().RosSocket;
            lockGazeNow = Time.time + 100f;
            lockTargetNow = Time.time + 100f;



        }


        public void getCurrentObjectToProject(std_msgs.String data) 
        {
            Debug.Log("The string subscribed is: "+data.data);
            current_obj_to_project = data.data;
    
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

        IEnumerator Wait()
        {
            yield return new WaitForSecondsRealtime(5.0f); // waits before continuing in seconds
                                                   // code to do after the wait
        }



        // Update is called once per frame
        void Update()
        {
            if (count_subscriber_publisher >= 0) 
            {
                publication_id = rosSocket.Advertise<std_msgs.String>("gaze_table");
                subscription_id = rosSocket.Subscribe<std_msgs.String>("object_to_project", getCurrentObjectToProject);
                count_subscriber_publisher -=1;
            }

            //publication_id = rosSocket.Advertise<std_msgs.String>("gaze_table");
            //subscription_id = rosSocket.Subscribe<std_msgs.String>("object_to_project", getCurrentObjectToProject);

            if (getaruco == true) //static aruco --> get once and then intializate
            {
                Debug.Log("Get Aruco True");
                image_position1 = image1.transform.position;
                Debug.Log("Image Position 1" + image_position1);
                eyes.transform.position = image_position1;


                if (image_position1 != Vector3.zero)
                {
                
                    Debug.Log("Get Aruco False");
                    getaruco = false;
                }
            }

       
            

            /////////////////////////////////////////////////////////////////At the start of every update, we check if objects have already been placed or not//////////////////////////////////////////////////////////////////////////

            position_table3 = image_position1;
            position_table3.z = position_table3.z + 1f;
            position_table3.y = position_table3.y - 0.1f;

            position_table1 = position_table3; 
            position_table1.x = position_table1.x - 0.55f;
            position_table1.z = position_table1.z + 0.4f;
            position_table1.y = position_table1.y + 0.4f;
            
            position_table2 = position_table3;
            position_table2.x = position_table2.x + 0.4f;
            position_table2.z = position_table2.z + 0.4f;
            position_table2.y = position_table2.y + 0.4f;

            position_solidtable1 = position_table3;
            position_solidtable1.x = position_solidtable1.x - 0.55f;
            position_solidtable1.z = position_solidtable1.z + 0.4f;
            position_solidtable1.y = position_table3.y;

            position_solidtable2 = position_table2;
            position_solidtable2.x = position_solidtable2.x + 0.1f;
            position_solidtable2.z = position_solidtable1.z;
            position_solidtable2.y = position_solidtable1.y;

            pr1 = position_solidtable1;
            pr1.x = pr1.x - 0.15f;
            pr1.y = position_table3.y + 0.04f;

            pb1 = position_solidtable1;
            pb1.x = pb1.x - 0.05f;
            pb1.y = position_table3.y + 0.04f;

            pg1 = position_solidtable1;
            pg1.x = pg1.x + 0.05f;
            pg1.y = position_table3.y + 0.04f;

            pr2 = position_solidtable2;
            pr2.x = pr2.x - 0.05f;
            pr2.y = position_table3.y + 0.04f;

            pb2 = position_solidtable2;
            pb2.x = pb2.x + 0.05f;
            pb2.y = position_table3.y + 0.04f;

            pg2 = position_solidtable2;
            pg2.x = pg2.x + 0.15f;
            pg2.y = position_table3.y + 0.04f;
            Debug.Log("POSITIONS OF BLOCK pb1" + pb1);

            initializedtable1.transform.position = position_table1;
            initializedtable2.transform.position = position_table2;
            initializedtable3.transform.position = position_table3;

            initsolidtable1.transform.position = position_solidtable1;
            initsolidtable2.transform.position = position_solidtable2;

            if (getaruco == false)
            {
                if (initobjects == true)
                {
                    initializedprojectedred1.transform.position = pr1;
                    initializedprojectedred2.transform.position = pr2;
                    initializedprojectedblue1.transform.position = pb1;
                    initializedprojectedblue2.transform.position = pb2;
                    initializedprojectedgreen1.transform.position = pg1;
                    initializedprojectedgreen2.transform.position = pg2;
                    initobjects = false;
                }
            }



            initializedprojectedred1.gameObject.SetActive(true);
            initializedprojectedred2.gameObject.SetActive(true);
            initializedprojectedblue1.gameObject.SetActive(true);
            initializedprojectedblue2.gameObject.SetActive(true);
            initializedprojectedgreen1.gameObject.SetActive(true);
            initializedprojectedgreen2.gameObject.SetActive(true);


            newpos1 = position_table3; // first object location
            newpos1.y = newpos1.y + 0.04f;
            newpos2 = newpos1;
            newpos2.y = newpos2.y + 0.062f;
            newpos3 = newpos2;
            newpos3.y = newpos3.y + 0.062f;



            /////////////////////////////////////////////////////////////////First must check if objects have already been placed or not//////////////////////////////////////////////////////////////////////////
            Debug.Log("COUNT IS " + count);

            if (objects_at_target.Count == 1)
            {
                Debug.Log("OBJECTS AT TARGET 1" + objects_at_target[0].name);
                objects_at_target[0].transform.position = newpos1;
            }

            if (objects_at_target.Count == 2)
            {
                Debug.Log("OBJECTS AT TARGET 1 count2" + objects_at_target[0].name);
                Debug.Log("OBJECTS AT TARGET 2 count2" + objects_at_target[1].name);
                objects_at_target[0].transform.position = newpos1;
                objects_at_target[1].transform.position = newpos2;
            }

            if (objects_at_target.Count == 3)
            {
                Debug.Log("OBJECTS AT TARGET 1 count3" + objects_at_target[0].name);
                Debug.Log("OBJECTS AT TARGET 2 count3" + objects_at_target[1].name);
                Debug.Log("OBJECTS AT TARGET 3 count3" + objects_at_target[2].name);
                objects_at_target[0].transform.position = newpos1;
                objects_at_target[1].transform.position = newpos2;
                objects_at_target[2].transform.position = newpos3;
            }


            Vector3 eyes_pos;

            eyes_pos = eyes.transform.position;
            Debug.Log("EYE POSITION IS " + eyes_pos);

            float distancetable1, distancetable2;

            distancetable1 = Vector3.Distance(position_table1, eyes_pos);  //finds the distnace between the table and the eyegaze position
            distancetable2 = Vector3.Distance(position_table2, eyes_pos);

            Debug.Log("HEAD DISTANCE TABLE 1 IS " + distancetable1);
            Debug.Log("HEAD DISTANCE TABLE 2 IS " + distancetable2);



            float[] cubeDistances = new float[2]; //initializes the variable cubeDistances with each index as the cube distance
            cubeDistances[0] = distancetable1;
            cubeDistances[1] = distancetable2;


            int closestCubeIndex = GetIndexOfLowestValue(cubeDistances);




            ///////////////////////////////////////// Locking & Highlight Object /////////////////////////////////////////////




            for (int i = 0; i < allCubes.Count; i++)
            {

                if (distancetable1 < 4.6f || distancetable2 < 4.6f)
                {
                    if (desiredObjectHighlighted == false)
                    {
                        if (i == closestCubeIndex)
                        {
                            Debug.Log("MATERIAL BEFORE" + allCubes[i].GetComponent<MeshRenderer>().material);
                            allCubes[i].GetComponent<MeshRenderer>().material = detectedMaterial;
                            Debug.Log("MATERIAL AFTER" + allCubes[i].GetComponent<MeshRenderer>().material);

                            current_object = allCubes[i].gameObject.name;
                            Debug.Log("Closest Table is " + current_object);


                            if (current_object == lastObject)
                            {

                                Debug.Log("Locking time: " + lockGazeNow + "Current time:" + Time.time);

                                if (Time.time > lockGazeNow) //if the current time > past time + 5 seoncds
                                {
                                    Debug.Log("LOCKING MATERIAL ACTIVE" + GameObject.Find(lastObject));
                                    GameObject.Find(lastObject).GetComponent<MeshRenderer>().material = lockedMaterial;

                                    if (lastObject == "table1")
                                    {
                                        Debug.Log("LAST OBJECT IS TABLE 1");
                                        current_table_locked = "table1";

                                        //send to ros;
                                    }
                                    if (lastObject == "table2")
                                    {
                                        Debug.Log("LAST OBJECT IS TABLE 2");
                                        current_table_locked = "table2";

                                        //send to ros;
                                    }


                                    objectLocked = true;
                                    desiredObjectHighlighted = true;

                                }
                            }
                            else
                            {
                                lockGazeNow = Time.time + timetoLock;
                                lastObject = current_object;
                            }
                        }
                        else
                        {
                            Debug.Log("Changing color back to original material.");
                            allCubes[i].GetComponent<MeshRenderer>().material = allMaterials[i]; //turns table back to original color
                        }

                        Debug.Log("Current object name:" + current_object + " :last object" + lastObject);


                    }
                }

                else
                {
                    if (objectLocked == false)
                    {
                        lockGazeNow = Time.time + timetoLock;
                        Debug.Log("Not close enough");
                        allCubes[i].GetComponent<MeshRenderer>().material = allMaterials[i]; //turns table back to original color
                    }

                }

            }




            if (objectLocked)
            {
                message = new std_msgs.String(current_table_locked);
                this.PublishGaze(message);
            }



            /////////////////////////////////////////////////////////////Get Commands From ROS! /////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (moveObject1)
            {
                // insert below stuff inside or not
            }

            switch (current_obj_to_project) //get output of pomdp
            {
                case "red_1":
                    if (count == 0)
                    {
                        projectred1 = true;
                        objectLocked = true;
                        last_object_placed = "projectedred1";
                    }
                    if (count == 1)
                    {
                        projectred1_2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedred1";
                    }
                    if (count == 2)
                    {
                        projectred1_3 = true;
                        objectLocked = true;
                        last_object_placed = "projectedred1";
                    }
                    break;

                case "red_2":
                    if (count == 0)
                    {
                        projectred2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedred2";
                    }
                    if (count == 1)
                    {
                        projectred2_2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedred2";
                    }
                    if (count == 2)
                    {
                        projectred2_3 = true;
                        objectLocked = true;
                        last_object_placed = "projectedred2";
                    }
                    break;

                case "blue_1":
                    if (count == 0)
                    {
                        projectblue1 = true;
                        objectLocked = true;
                        last_object_placed = "projectedblue1";
                    }
                    if (count == 1)
                    {
                        projectblue1_2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedblue1";
                    }
                    if (count == 2)
                    {
                        projectblue1_3 = true;
                        objectLocked = true;
                        last_object_placed = "projectedblue1";
                    }
                    break;

                case "blue_2":
                    if (count == 0)
                    {
                        projectblue2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedblue2";
                    }
                    if (count == 1)
                    {
                        projectblue2_2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedblue2";
                    }
                    if (count == 2)
                    {
                        projectblue2_3 = true;
                        objectLocked = true;
                        last_object_placed = "projectedblue2";
                    }
                    break;

                case "green_1":
                    if (count == 0)
                    {
                        projectgreen1 = true;
                        objectLocked = true;
                        last_object_placed = "projectedgreen1";
                    }
                    if (count == 1)
                    {
                        projectgreen1_2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedgreen1";
                    }
                    if (count == 2)
                    {
                        projectgreen1_3 = true;
                        objectLocked = true;
                        last_object_placed = "projectedgreen1";
                    }
                    break;

                case "green_2":
                    if (count == 0)
                    {
                        projectgreen2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedgreen2";
                    }
                    if (count == 1)
                    {
                        projectgreen2_2 = true;
                        objectLocked = true;
                        last_object_placed = "projectedgreen2";
                    }
                    if (count == 2)
                    {
                        projectgreen2_3 = true;
                        objectLocked = true;
                        last_object_placed = "projectedgreen2";
                    }
                    break;

                case "remove_last":
                    remove_last = true;
                    break;

                default:
                    break;
            }

            if (remove_last)
            {
                if (last_object_placed != "")
                {
                    Debug.Log("removing object: " + last_object_placed);
                    GameObject obj_remove = GameObject.Find(last_object_placed);
                    switch (last_object_placed)
                    {
                        case "projectedred1":
                            projectred1 = false;
                            objects_not_at_target.Add(projectedred1);
                            objects_at_target.Remove(projectedred1);
                            obj_remove.SetActive(false);
                            break;

                        case "projectedred2":
                            projectred2 = false;
                            objects_not_at_target.Add(projectedred2);
                            objects_at_target.Remove(projectedred2);
                            obj_remove.SetActive(false);
                            break;

                        case "projectedblue1":
                            projectblue1 = false;
                            objects_not_at_target.Add(projectedblue1);
                            objects_at_target.Remove(projectedblue1);
                            obj_remove.SetActive(false);
                            break;

                        case "projectedblue2":
                            projectblue2 = false;
                            objects_not_at_target.Add(projectedblue2);
                            objects_at_target.Remove(projectedblue2);
                            obj_remove.SetActive(false);
                            break;

                        case "projectedgreen1":
                            projectgreen1 = false;
                            objects_not_at_target.Add(projectedgreen1);
                            objects_at_target.Remove(projectedgreen1);
                            obj_remove.SetActive(false);
                            break;

                        case "projectedgreen2":
                            projectgreen2 = false;
                            objects_not_at_target.Add(projectedgreen2);
                            objects_at_target.Remove(projectedgreen2);
                            obj_remove.SetActive(false);
                            break;

                        default:
                            break;
                    }
                    last_object_placed = "";
                    remove_last = false;

                }

            }


            if (projectred1)
            {
                Debug.Log("MOVING RED1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedred1); //must remove from list so object can indeed move
                first_object = initializedprojectedred1;
                first_object.SetActive(true);

                //make sure you can see the thing moving there, only until it reaches table3, we then proceed to add the thing to list
                

                if (!objects_at_target.Contains(initializedprojectedred1)) //if list does not contain the red , add the red to list so we can keep these at target
                {                   
                    objects_at_target.Add(initializedprojectedred1);
                    count += 1;
                    targetplaced1 = true;
                }                
                projectred1 = false;

            }

            if (projectblue1)
            {
                Debug.Log("MOVING BLUE1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedblue1); //must remove from list so object can indeed move
                first_object = initializedprojectedblue1;
                first_object.SetActive(true);

                
                if (!objects_at_target.Contains(initializedprojectedblue1)) //if list does not contain the red , add the red to list
                {                   
                    objects_at_target.Add(initializedprojectedblue1);
                    count += 1;
                    targetplaced1 = true;

                }                          
                projectblue1 = false;
            }

            if (projectgreen1)
            {
                Debug.Log("MOVING GREEN1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedgreen1); //must remove from list so object can indeed move
                first_object = initializedprojectedgreen1;
                first_object.SetActive(true);

                
                if (!objects_at_target.Contains(initializedprojectedgreen1)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedgreen1);
                    count += 1;
                    targetplaced1 = true;

                }                              
                projectgreen1 = false;
            }

            if (projectred2)
            {
                Debug.Log("MOVING RED2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedred2); //must remove from list so object can indeed move
                first_object = initializedprojectedred2;
                first_object.SetActive(true);

                
                if (!objects_at_target.Contains(initializedprojectedred2)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedred2);
                    count += 1;
                    targetplaced1 = true;
                }
                
                projectred2 = false;
            }


            if (projectblue2)
            {
                Debug.Log("MOVING BLUE2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedblue2);
                first_object = initializedprojectedblue2;
                first_object.SetActive(true);

                
                if (!objects_at_target.Contains(initializedprojectedblue2))
                {
                    objects_at_target.Add(initializedprojectedblue2);
                    count += 1;
                    targetplaced1 = true;

                }

                projectblue2 = false;
            }

            if (projectgreen2)
            {
                Debug.Log("MOVING GREEN2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedgreen2);
                first_object = initializedprojectedgreen2;
                first_object.SetActive(true);

                
                if (!objects_at_target.Contains(initializedprojectedgreen2))
                {
                    objects_at_target.Add(initializedprojectedgreen2);
                    count += 1;
                    targetplaced1 = true;

                }
                projectgreen2 = false;
            }


            if (targetplaced1 == true)
            {   // initialize all variables to default and then set desiredobjecthighlihgted to false to restart loop
                Debug.Log("TARGETPLACED1");
                moveObject1 = false;
                startTime = 0f;
                timetoLock = 5f;
                lockGazeNow = 5f;
                lockTargetNow = 5f;
                lastObject = "";
                current_object = "";
                current_target = "";
                last_target = "";
                current_object_to_move = -1;
                desiredTargetHighlighted = false;
                targetLocked = false;
                objectLocked = false;
                desiredObjectHighlighted = false;
                targetplaced1 = false;

            }

            //////////////////////////////// DROP 2nd Object /////////////////////////////////////////////////////////
            ///Up 2 here: Update the drop position 2 as the + y position of drop position 1


            if (projectred1_2)
            {
                Debug.Log("MOVING SECOND RED1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedred1); //must remove from list so object can indeed move
                second_object = initializedprojectedred1;
                second_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedred1)) //if list does not contain the red , add the red to list
                {

                    objects_at_target.Add(initializedprojectedred1);
                    count += 1;
                    targetplaced2 = true;
                }           
                projectred1_2 = false;
            }


            if (projectred2_2)
            {
                Debug.Log("MOVING SECOND RED2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedred2); //must remove from list so object can indeed move
                second_object = initializedprojectedred2;
                second_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedred2)) //if list does not contain the red , add the red to list
                {

                    objects_at_target.Add(initializedprojectedred2);
                    count += 1;
                    targetplaced2 = true;
                }
                projectred2_2 = false;
            }


            if (projectblue1_2)
            {
                Debug.Log("MOVING SECOND BLUE1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedblue1); //must remove from list so object can indeed move
                second_object = initializedprojectedblue1;
                second_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedblue1)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedblue1);
                    count += 1;
                    targetplaced2 = true;
                }

                projectblue1_2 = false;
            }


            if (projectblue2_2)
            {
                Debug.Log("MOVING SECOND BLUE2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedblue2); //must remove from list so object can indeed move
                second_object = initializedprojectedblue2;
                second_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedblue2)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedblue2);
                    count += 1;
                    targetplaced2 = true;
                }

                projectblue2_2 = false;
            }
    

            if (projectgreen1_2)
            {
                Debug.Log("MOVING SECOND green1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedgreen1); //must remove from list so object can indeed move
                second_object = initializedprojectedgreen1;
                second_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedgreen1)) 
                {
                    Debug.Log("Adding second green to target");
                    objects_at_target.Add(initializedprojectedgreen1);
                    count += 1;
                    targetplaced2 = true;
                }
                
                projectgreen1_2 = false;
            }

           

            if (projectgreen2_2)
            {
                Debug.Log("MOVING SECOND Green2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedgreen2); //must remove from list so object can indeed move
                second_object = initializedprojectedgreen2;
                second_object.SetActive(true);
                


                if (!objects_at_target.Contains(initializedprojectedgreen2)) //if list does not contain the red , add the red to list
                {

                    objects_at_target.Add(initializedprojectedgreen2);
                    targetplaced2 = true;
                    count += 1;
                }

                projectgreen2_2 = false;
            }


            if (targetplaced2 == true)
            {   // initialize all variables to default and then set desiredobjecthighlihgted to false to restart loop
                Debug.Log("TARGETPLACED2");
                moveObject2 = false;
                startTime = 0f;
                timetoLock = 5f;
                lockGazeNow = 5f;
                lockTargetNow = 5f;
                lastObject = "";
                current_object = "";
                current_target = "";
                last_target = "";
                current_object_to_move = -1;
                desiredTargetHighlighted = false;
                targetLocked = false;
                objectLocked = false;
                desiredObjectHighlighted = false;
                targetplaced2 = false;

            }

            //////////////////////////////// DROP 3rd Object /////////////////////////////////////////////////////////


            if (projectred1_3)
            {
                Debug.Log("MOVING third RED1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedred1); //must remove from list so object can indeed move
                third_object = initializedprojectedred1;
                third_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedred1)) //if list does not contain the red , add the red to list
                {                  
                    objects_at_target.Add(initializedprojectedred1);
                    targetplaced3 = true;
                }
                projectred1_3 = false;
            }


            if (projectred2_3)
            {
                Debug.Log("MOVING third RED2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedred2); //must remove from list so object can indeed move
                third_object = initializedprojectedred2;
                third_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedred2)) //if list does not contain the red , add the red to list
                {                    
                    objects_at_target.Add(initializedprojectedred2);
                    targetplaced3 = true;
                }
                projectred2_3 = false;
            }


            if (projectblue1_3)
            {
                Debug.Log("MOVING third BLUE1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedblue1); //must remove from list so object can indeed move
                third_object = initializedprojectedblue1;
                third_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedblue1)) //if list does not contain the red , add the red to list
                {                   
                    objects_at_target.Add(initializedprojectedblue1);
                    targetplaced3 = true;
                }

                projectblue1_3 = false;
            }


            if (projectblue2_3)
            {
                Debug.Log("MOVING third BLUE2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedblue2); //must remove from list so object can indeed move
                third_object = initializedprojectedblue2;
                third_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedblue2)) //if list does not contain the red , add the red to list
                {
                    objects_at_target.Add(initializedprojectedblue2);
                    targetplaced3 = true;
                }

                projectblue2_3 = false;
            }


            if (projectgreen1_3)
            {
                Debug.Log("MOVING third green1 OBJECT");
                objects_not_at_target.Remove(initializedprojectedgreen1); //must remove from list so object can indeed move
                third_object = initializedprojectedgreen1;
                third_object.SetActive(true);
                

                if (!objects_at_target.Contains(initializedprojectedgreen1))
                {
                    Debug.Log("Adding second green to target");
                    objects_at_target.Add(initializedprojectedgreen1);
                    targetplaced3 = true;
                }

                projectgreen1_3 = false;
            }



            if (projectgreen2_3)
            {
                Debug.Log("MOVING third Green2 OBJECT");
                objects_not_at_target.Remove(initializedprojectedgreen2); 
                third_object = initializedprojectedgreen2;
                third_object.SetActive(true);
                


                if (!objects_at_target.Contains(initializedprojectedgreen2)) 
                {
                    objects_at_target.Add(initializedprojectedgreen2);
                    targetplaced3 = true;
                }

                projectgreen2_3 = false;
            }






        }

    }

}

