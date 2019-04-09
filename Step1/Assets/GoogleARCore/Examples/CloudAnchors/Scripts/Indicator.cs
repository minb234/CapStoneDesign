using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using UnityEngine.XR;
//using UnityEngine.Experimental.XR;
using System;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public Camera camera;
    public GameObject placementIndicator;
    public Pose placementPose;

    private bool placementPoseIsValid = false;
    private bool playerIsValid = false;


    private Vector3 desVector;
    private Pose desPose;
    private Quaternion quaternion;




    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    public void UpdatePlacementPose()
    {
        //ray = Camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2,0));

        TrackableHit hit;
        TrackableHitFlags flags = TrackableHitFlags.PlaneWithinPolygon
                                    | TrackableHitFlags.FeaturePointWithSurfaceNormal;

        //Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width /2, Screen.height /2, 0));
        //placementPoseIsValid = Physics.Raycast(ray,  out hit, Mathf.Infinity);       //TrackableType.Planes -> Trackable.FeaturePoint
        //placementPoseIsValid = Physics.Raycast(ray, -camera.transform.forward, out hit, Mathf.Infinity);       //TrackableType.Planes -> Trackable.FeaturePoint

        placementPoseIsValid = Frame.Raycast(Screen.width / 2, Screen.height / 2, flags, out hit);

        //placementPoseIsValid = hit.Count > 0;


        if (placementPoseIsValid)
        {
            placementPose = hit.Pose;

            //움직일 오브젝트의 좌표 값 갱신, 좌표는 항상 움직이지만 터치를 했을 때만 그 값을 가져와야함으로 따로 설정.
            if (playerIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                desPose = hit.Pose;
                desVector = hit.Pose.position;
            }


        }
    }


    public void UpdatePlacementIndicator()
    {
        //앵커가 생성되있으면 indicator
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


}
