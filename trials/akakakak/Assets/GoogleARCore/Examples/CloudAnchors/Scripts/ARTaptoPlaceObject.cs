using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using UnityEngine.XR;
//using UnityEngine.Experimental.XR;
using System;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

public class ARTaptoPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;
    //public GameObject objectToPlace;

    public Camera camera;
    public Pose placementPose;
    public Text debug1;
    public Text debug2;
   // public Text debug3;

    private bool placementPoseIsValid = false;
    
    private Vector3 desVector;
    private Pose desPose;
    private Quaternion quaternion;

    private bool playerIsValid = false;

    //private OrderController orderController;

    void Start()
    {
        //arOrigin = FindObjectOfType<ARCoreSession>();
        //orderController = GetComponent<OrderController>();
    }


    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        debug2.text = placementPose.ToString();
        if (placementPoseIsValid && !playerIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //PlaceObject();
            playerIsValid = true;
        }


    }

    public void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);

            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);

        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }



    public void UpdatePlacementPose()
    {

        //var screenCenter = camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        //var hit = new RaycastHit();
        //debug.text = camera.transform.position.ToString();

        //ray = Camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2,0));

        TrackableHit hit;
        TrackableHitFlags flags = TrackableHitFlags.PlaneWithinPolygon
                                    | TrackableHitFlags.FeaturePointWithSurfaceNormal;
        
        //Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width /2, Screen.height /2, 0));
        //placementPoseIsValid = Physics.Raycast(ray,  out hit, Mathf.Infinity);       //TrackableType.Planes -> Trackable.FeaturePoint
        //placementPoseIsValid = Physics.Raycast(ray, -camera.transform.forward, out hit, Mathf.Infinity);       //TrackableType.Planes -> Trackable.FeaturePoint

        placementPoseIsValid = Frame.Raycast(Screen.width / 2, Screen.height / 2, flags, out hit);

        //placementPoseIsValid = hit.Count > 0;
        debug1.text = hit.Pose.ToString();
       // debug3.text = placementPoseIsValid.ToString();
        if (placementPoseIsValid)
        {
            placementPose = hit.Pose;

            //움직일 오브젝트의 좌표 값 갱신, 좌표는 항상 움직이지만 터치를 했을 때만 그 값을 가져와야함으로 따로 설정.
            if (playerIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                desPose = hit.Pose;
                desVector = hit.Pose.position;
                debug2.text = desPose.ToString();
            }

            //var cameraForward = Camera.current.transform.forward;
            //var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            //placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    public Pose GetPose()
    {
        return desPose;
    }
    public Vector3 GetdesVector()
    {
        return desVector;
    }
}




