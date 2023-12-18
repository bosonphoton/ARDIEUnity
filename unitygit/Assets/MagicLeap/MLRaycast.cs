using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class MLRaycast : MonoBehaviour
{

    public Transform ctransform;
    public GameObject prefab;

    private IEnumerator NormalMaker(Vector3 point, Vector3 normal)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        GameObject go = Instantiate(prefab, point, rotation);
        yield return new WaitForSeconds(2);
        Destroy(go);
    }

    void HandleOnRecieveRaycast(MLWorldRays.MLWorldRaycastResultState state, UnityEngine.Vector3 point, UnityEngine.Vector3 normal, float confidence)
    {
        if (state == MLWorldRays.MLWorldRaycastResultState.HitObserved)
        {
            StartCoroutine(NormalMaker(point, normal));
        }
    }

    private void OnDestroy()
    {
        MLEyes.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        MLEyes.Start();
    }

    // Update is called once per frame
    void Update()
    {
        MLWorldRays.QueryParams _raycastParams = new MLWorldRays.QueryParams
        {
            Position = ctransform.position,
            Direction = ctransform.forward,
            UpVector = ctransform.up,
            Width = 1,
            Height = 1
        };
        MLWorldRays.GetWorldRays(_raycastParams, HandleOnRecieveRaycast);
    }
}
