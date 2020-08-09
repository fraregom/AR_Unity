using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARLessons : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text StateText;
    [SerializeField] TMPro.TMP_Text PlanesText;
    [SerializeField] ARPlaneManager PlaneManager;
    [SerializeField] ARPointCloudManager PointCloudManager;

    private int planesadded = 0;
    private int planesupdated = 0;
    private int planesremoved = 0;
    private int pointsadded = 0;
    private int pointsupdated = 0;
    private int pointsremoved = 0;

    // Start is called before the first frame update
    void Start()
    {
        ARSession.stateChanged += Events;
        PlaneManager.planesChanged += Events_planes;
        PointCloudManager.pointCloudsChanged += Events_points;
    }

    void Events(ARSessionStateChangedEventArgs Event){
        switch(Event.state){
            case ARSessionState.Unsupported:
                StateText.text = "Unsupported";
                break;
            case ARSessionState.CheckingAvailability:
                StateText.text = "CheckingAvailability";
                break;
            case ARSessionState.NeedsInstall:
                StateText.text = "NeedsInstall";
                break;
            case ARSessionState.Installing:
                StateText.text = "Installing";
                break;
            case ARSessionState.Ready:
                StateText.text = "Ready";
                break;
            case ARSessionState.SessionInitializing:
                StateText.text = "SessionInitializing";
                break;
            case ARSessionState.SessionTracking:
                StateText.text = "SessionTracking";
                break;
            default:
                StateText.text = "None";
                break;
        }
    }

    void Events_planes(ARPlanesChangedEventArgs planesChanged){
        planesadded += planesChanged.added.Count;
        planesupdated += planesChanged.updated.Count;
        planesremoved += planesChanged.removed.Count;
    }

    void Events_points(ARPointCloudChangedEventArgs pointCloudsChanged){
        pointsadded += pointCloudsChanged.added.Count;
        pointsupdated += pointCloudsChanged.updated.Count;
        pointsremoved += pointCloudsChanged.removed.Count;
    }

    // Update is called once per frame
    void Update()
    {
        PlanesText.text = "Planes added: " + planesadded + "\n Planes updated: " + planesupdated + "\n Planes removed: " + planesremoved;
        PlanesText.text += "\n Points added: " + pointsadded + "\n Points updated: " + pointsupdated + "\n Points removed: " + pointsremoved;  
    }
}
