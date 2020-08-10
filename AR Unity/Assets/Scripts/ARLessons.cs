using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class ARLessons : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text StateText;
    [SerializeField] TMPro.TMP_Text PlanesText;
    [SerializeField] ARPlaneManager PlaneManager;
    [SerializeField] ARPointCloudManager PointCloudManager;
    [SerializeField] ARRaycastManager RaycastManager;
    [SerializeField] ARCameraManager CameraManager;
    [SerializeField] Transform Robot;

    private int planesadded = 0;
    private int planesupdated = 0;
    private int planesremoved = 0;
    private int pointsadded = 0;
    private int pointsupdated = 0;
    private int pointsremoved = 0;

    private float width;
    private float height;
    
    private List<ARRaycastHit> hits;
    private ReadOnlyArray<Touch> _touches;
    private Transform RobotInstance; 
    private bool robotInstantiated = false;
    
    private float? averageBrightness;
    private float? averageColorTemperature;
    private Color? colorCorrection;
    
    [SerializeField]  Light currentLight;
    [SerializeField]  Image currentImage;
    

    private void Awake() 
    {
        currentLight = GetComponent<Light>();
    }

    private void OnEnable() 
    {
        CameraManager.frameReceived += FrameUpdated;
    }

    private void OnDisable() 
    {
        CameraManager.frameReceived -= FrameUpdated;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ARSession.stateChanged += Events;
        PlaneManager.planesChanged += Events_planes;
        PointCloudManager.pointCloudsChanged += Events_points;
        hits = new List<ARRaycastHit>();
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        
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
        
        _touches = Touch.activeTouches;
 
        if (_touches.Count == 1)
            SetRobot(_touches[0]);
    }

    void SetRobot(Touch touch)
    {
        if(RaycastManager.Raycast(touch.screenPosition, hits)){
            
            while(hits.Count > 0){
                ARRaycastHit hit = hits[0];
                hits.RemoveAt(0);
                    
                Vector3 positionInstance = hit.pose.position;

                if (!robotInstantiated)
                {
                    RobotInstance = Instantiate(Robot, positionInstance, Quaternion.identity);
                    SetPhysics();
                    robotInstantiated = true;
                }
                else
                {
                    RobotInstance.position = positionInstance;
                }

            }
        }
    }

    void SetPhysics()
    {
        Rigidbody RobotRB = Robot.GetComponent<Rigidbody>();
        if(RobotRB != null){
            RobotRB.isKinematic = true;
            RobotRB.velocity *= 0;
            RobotRB.angularVelocity *= 0;
        }
    }

    private void FrameUpdated(ARCameraFrameEventArgs args)
    {
        if(args.lightEstimation.averageBrightness.HasValue)
        {
            averageBrightness = args.lightEstimation.averageBrightness.Value;
            currentLight.intensity = args.lightEstimation.averageBrightness.Value;
        }

        if(args.lightEstimation.averageColorTemperature.HasValue)
        {
            averageColorTemperature = args.lightEstimation.averageColorTemperature.Value;
            currentLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            currentImage.color = (Color) args.lightEstimation.colorCorrection;
        }

        if(args.lightEstimation.colorCorrection.HasValue)
        {
            colorCorrection = args.lightEstimation.colorCorrection.Value;
            currentLight.color = args.lightEstimation.colorCorrection.Value;
        }
    }
}
