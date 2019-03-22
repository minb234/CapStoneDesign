using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARTaptoPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;
    //public GameObject objectToPlace;

    public ARSessionOrigin arOrigin;
    public Pose placementPose;
    private bool placementPoseIsValid = false;

    private Vector3 desVector;
    private Pose desPose;
    private bool playerIsValid = false;

    private OrderController orderController;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        orderController = GetComponent<OrderController>();
    }


    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && !playerIsValid &&Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
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
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);       //TrackableType.Planes -> Trackable.FeaturePoint

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            //움직일 오브젝트의 좌표 값 갱신, 좌표는 항상 움직이지만 터치를 했을 때만 그 값을 가져와야함으로 따로 설정 .
            if (playerIsValid && Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                desVector = placementPose.position;
                desPose = placementPose;
            }

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
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
