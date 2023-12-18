/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;



public class Testing : MonoBehaviour
{

    public GameObject red, green, blue, plane, target;
    public GameObject projectedred, projectedgreen, projectedblue;

    private GameObject initializedred, initializedgreen, initializedblue, initializedplane, initializedtarget;
    private GameObject initializedprojectedred, initializedprojectedgreen, initializedprojectedblue;
    //AR representation objects


    private List<GameObject> allCubes = new List<GameObject>();

    public Material cube1Mat, cube2Mat, cube3Mat, targetMat;
    public Material detectedMaterial, lockedMaterial;

    public List<Material> allMaterials = new List<Material>();

    public float startTime = 0f;
    public float timetoLock = 5f; // 5 seconds to lock in gaze

    bool desiredObjectHighlighted = false;
    bool desiredTargetHighlighted = false;
    bool targetLocked = false;
    bool objectLocked = false;

    string lastObject = "";
    string current_object = "";
    string current_target = "";
    string last_target = "";
    float lockGazeNow = 5f;
    float lockTargetNow = 5f;
    float speed = 0.4f;
    float height = 0f; //stores the current height of the variable


    int current_object_to_move = -1;

    bool moveObject = false;
    bool targetplaced1 = false; //if first object placed
    bool targetplaced2 = false; //if second object placed
    bool targetplaced3 = false; //if third object placed

    private Rigidbody redrb, greenrb, bluerb;

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


        lockGazeNow = Time.time + 100f;
        lockTargetNow = Time.time + 100f;
    }





    // Update is called once per frame
    void Update()
    {
        Vector3 position_red, position_green, position_blue, position_plane, targetbase;

        position_red = new Vector3(0, 0, 0);
        position_red.x = position_red.x - 0.3f;

        position_green = new Vector3(0, 0, 0);

        position_blue = new Vector3(0, 0, 0);
        position_blue.x = position_blue.x + 0.3f;
        position_blue.y = position_blue.y + 0.05f;

        targetbase = new Vector3(0, 0, 0);
        targetbase.z = targetbase.z - 0.35f;

        position_plane = new Vector3(0, 0, 0);
        position_plane.y = position_plane.y - 0.05f;


        initializedplane.transform.position = position_plane;
        initializedred.transform.position = position_red;
        initializedgreen.transform.position = position_green;
        initializedblue.transform.position = position_blue;
        initializedtarget.transform.position = targetbase;





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
            if (desiredTargetHighlighted == false)
            {
                if (//target has on mouse down)
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
                        moveObject = true;


                    }

                }
                else
                {
                    lockTargetNow = Time.time + timetoLock;
                    last_target = current_target;
                }

            }

        }

        /////////////////////////////////////////////////////////////Code to move Object/////////////////////////////////////////////////////////////////////////////////////


        if (moveObject)
        {
            if (current_object_to_move == 0)
            {

                redrb = initializedprojectedred.GetComponent<Rigidbody>();
                redrb.MovePosition(redrb.transform.position + targetbase * Time.deltaTime * speed);
                initializedprojectedred.transform.position = Vector3.MoveTowards(initializedprojectedred.transform.position, targetbase, speed * Time.deltaTime);
                initializedprojectedgreen.transform.position = position_green;
                initializedprojectedblue.transform.position = position_blue; //overlays AR transparent objects onblue
                targetplaced1 = true;


            }
            else if (current_object_to_move == 1)
            {
                greenrb = initializedprojectedgreen.GetComponent<Rigidbody>();
                greenrb.MovePosition(greenrb.transform.position + targetbase * Time.deltaTime * speed);
                initializedprojectedgreen.transform.position = Vector3.MoveTowards(initializedprojectedgreen.transform.position, targetbase, speed * Time.deltaTime);
                initializedprojectedred.transform.position = position_red;
                initializedprojectedblue.transform.position = position_blue;
                targetplaced1 = true;


            }
            else if (current_object_to_move == 2)
            {
                bluerb = initializedprojectedblue.GetComponent<Rigidbody>();
                bluerb.MovePosition(bluerb.transform.position + targetbase * Time.deltaTime * speed);
                initializedprojectedblue.transform.position = Vector3.MoveTowards(initializedprojectedblue.transform.position, targetbase, speed * Time.deltaTime);
                initializedprojectedred.transform.position = position_red;
                initializedprojectedgreen.transform.position = position_green;
                targetplaced1 = true;


            }
        }

        else
        {
            initializedprojectedred.transform.position = position_red;
            initializedprojectedgreen.transform.position = position_green;
            initializedprojectedblue.transform.position = position_blue; //overlays AR transparent objects onblue
        }

        ///////////////////////////////////////////////////////////////////////////////Code to place 2nd object /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        if (targetplaced1) //if first object placed at target
        {
            ////// call ROS script to confirm Yes or No //////////////
            /// if ROS returns Yes
            /// proceed to {function2 --> which moves the second object}
            /// else: {restart loop by doing desiredObjectHighlighted = false}?
        }


    

    








*/