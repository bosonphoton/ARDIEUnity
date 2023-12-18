using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;


[RequireComponent(typeof(MLImageTrackerBehavior))]
public class GlobeTrackingVisualizer : MonoBehaviour
{

    public Vector3 picture;

    private MLImageTrackerBehavior _trackerBehavior = null;
    private bool _targetFound = false;

    [SerializeField, Tooltip("Game Object showing the axis")]
    private GameObject _axis = null;

    public Vector3 imagePosition = Vector3.zero;

    public bool foundImage;

    void Awake()
    {

        if (null == _axis)
        {
            Debug.LogError("Error: ImageTrackingVisualizer._axis is not set, disabling script.");
            enabled = false;
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _trackerBehavior = GetComponent<MLImageTrackerBehavior>();
        _trackerBehavior.OnTargetFound += OnTargetFound;
        _trackerBehavior.OnTargetLost += OnTargetLost;

        RefreshViewModel();
    }

    private void Update()
    {
        if (_axis.transform.position != Vector3.zero && _targetFound)
        {
            //Debug.Log(_axis.transform.position.ToString());
            imagePosition = _axis.transform.position;
        }
        else
        {
            imagePosition = Vector3.zero;
        }

        foundImage = _targetFound;
    }

    public void UpdateViewMode(GlobeImageTracker.ViewMode viewMode)
    {
        RefreshViewModel();
    }

    void RefreshViewModel()
    {
        _axis.SetActive(_targetFound);
    }

    private void OnTargetFound(bool isReliable)
    {
        _targetFound = true;
        RefreshViewModel();
    }

    private void OnTargetLost()
    {
        _targetFound = false;
        RefreshViewModel();
    }


}
