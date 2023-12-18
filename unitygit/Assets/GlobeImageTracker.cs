using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;

[RequireComponent(typeof(PrivilegeRequester))]
public class GlobeImageTracker : MonoBehaviour
{

    public enum ViewMode : int
    {
        All = 0,
        AxisOnly,
        TrackingCubeOnly,
        DemoOnly,
    }

    private ViewMode _viewMode = ViewMode.AxisOnly;

    public GameObject[] TrackerBehaviours;

    public GlobeTrackingVisualizer visualizer;

    private PrivilegeRequester _privilegeRequester = null;

    private bool _hasStarted = false;

    void Awake()
    {
        // If not listed here, the PrivilegeRequester assumes the request for
        // the privileges needed, CameraCapture in this case, are in the editor.
        _privilegeRequester = GetComponent<PrivilegeRequester>();

        // Before enabling the MLImageTrackerBehavior GameObjects, the scene must wait until the privilege has been granted.
        _privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            UpdateImageTrackerBehaviours(false);

            _hasStarted = false;
        }
    }

    void UpdateVisualizers()
    {
        visualizer.UpdateViewMode(_viewMode);
    }

    void UpdateImageTrackerBehaviours(bool enabled)
    {
        foreach (GameObject obj in TrackerBehaviours)
        {
            obj.SetActive(enabled);
        }
    }

    void StartCapture()
    {
        if (!_hasStarted)
        {
            UpdateImageTrackerBehaviours(true);
            _hasStarted = true;
        }
    }

    void HandlePrivilegesDone(MLResult result)
    {
        if (!result.IsOk)
        {
            if (result.Code == MLResultCode.PrivilegeDenied)
            {
                Instantiate(Resources.Load("PrivilegeDeniedError"));
            }

            Debug.LogErrorFormat("Error: ImageTrackingExample failed to get requested privileges, disabling script. Reason: {0}", result);
            enabled = false;
            return;
        }

        Debug.Log("Succeeded in requesting all privileges");
        StartCapture();
    }

}
