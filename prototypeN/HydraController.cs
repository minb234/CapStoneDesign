using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using UnityEngine.XR;
//using UnityEngine.Experimental.XR;
using System;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.AI;

public class HydraController : NetworkBehaviour
{
   
    public Pose placementPose;
    //public Button button;

    private bool placementPoseIsValid = false;
    private bool playerIsValid = false;
    private Vector3 desVector;
    private Pose desPose;


    // Start is called before the first frame update
    void Start()
    {
        
        gameObject.name = "HydraController";
        //button.onClick.AddListener(UpdatePlacementPose);
    }

    
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
}
 

    public void UpdatePlacementPose()
    {
        //if (!isLocalPlayer) {
        //    return;
        //}
        Ray ray;
        //TrackableHit hit;
        RaycastHit hit;
        //GameObject indicator = GameObject.FindGameObjectWithTag("indicator");
        //추가
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if(Physics.Raycast(ray, out hit))
        {

            this.transform.LookAt(hit.point);
            //lerp는 정해진위치까지 정확히 도달하지는 x, movetoward는 정해진위치까지 도달
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, hit.point, 0.5f * Time.deltaTime);
            //this.transform.position = Vector3.Lerp(this.transform.position, hit.point, 0.5f);

        }
        
        //TrackableHitFlags flags = TrackableHitFlags.PlaneWithinPolygon
        //                            | TrackableHitFlags.FeaturePointWithSurfaceNormal;
        
        //placementPoseIsValid = Frame.Raycast(Screen.width / 2, Screen.height / 2, flags, out hit);

        //if (placementPoseIsValid && Input.touchCount > 0)
        //{
        //    placementPose = hit.Pose;

        //    this.transform.LookAt(placementPose.position e);


        //    this.transform.position = Vector3.MoveTowards(this.transform.position, placementPose.position, 0.05f);


            //움직일 오브젝트의 좌표 값 갱신, 좌표는 항상 움직이지만 터치를 했을 때만 그 값을 가져와야함으로 따로 설정.
            //if (playerIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //    desPose = hit.Pose;
            //    desVector = hit.Pose.position;

            //}




            //var cameraForward = Camera.current.transform.forward;
            //var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            //placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        //}
    }
}

