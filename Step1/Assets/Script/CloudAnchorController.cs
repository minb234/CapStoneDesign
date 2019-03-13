using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class CloudAnchorController : MonoBehaviour
{
    public UIController UIController;
    public GameObject ARCoreRoot;
    public ARCoreWorldOriginHelper ARCoreWorldOriginHelper;
    private ApplicationMode m_CurrentMode = ApplicationMode.Ready;
    private bool IsQuitting = false;
    private Component LastPlacedAnchor = null;
    private bool IsOriginPlaced = false;
    private bool AnchorAlreadyInstantiated = false;

    




    public enum ApplicationMode
    {
        Ready,
        Hosting,
        Resolving,
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "CloudAnchorController";

    }

    // Update is called once per frame
    void Update()
    {
        _UpdateApplicationLifecycle();

        if (m_CurrentMode != ApplicationMode.Hosting && m_CurrentMode != ApplicationMode.Resolving)
        {
            return;
        }

        // If the origin anchor has not been placed yet, then update in resolving mode is complete.
        if (m_CurrentMode == ApplicationMode.Resolving && !IsOriginPlaced)
        {
            return;
        }

        // If the player has not touched the screen then the update is complete.
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }


        TrackableHit hit;
        if (ARCoreWorldOriginHelper.Raycast(touch.position.x, touch.position.y,
                TrackableHitFlags.PlaneWithinPolygon, out hit))
        {
            LastPlacedAnchor = hit.Trackable.CreateAnchor(hit.Pose);
        }

        else if (!IsOriginPlaced && m_CurrentMode == ApplicationMode.Hosting)
        {
            SetWorldOrigin(LastPlacedAnchor.transform);
            _InstantiateAnchor();
            OnAnchorInstantiated(true);
        }



    }


    public void SetWorldOrigin(Transform anchorTransform)
    {
        if (IsOriginPlaced)
        {
            Debug.LogWarning("The World Origin can be set only once.");
            return;
        }

        IsOriginPlaced = true;    
        ARCoreWorldOriginHelper.SetWorldOrigin(anchorTransform);
    
    }

    private void _InstantiateAnchor()
    {
        // The anchor will be spawned by the host, so no networking Command is needed.
        GameObject.Find("LocalPlayer").GetComponent<PlayerController>()
                  .SpawnPlayer(Vector3.zero, Quaternion.identity, LastPlacedAnchor);
    }

    public void OnEnterHostingModeClick()
    {
        if (m_CurrentMode == ApplicationMode.Hosting)
        {
            m_CurrentMode = ApplicationMode.Ready;
            _ResetStatus();
            return;
        }

        m_CurrentMode = ApplicationMode.Hosting;
        
    }

    public void OnEnterResolvingModeClick()
    {
        if (m_CurrentMode == ApplicationMode.Resolving)
        {
            m_CurrentMode = ApplicationMode.Ready;
            _ResetStatus();
            return;
        }

        m_CurrentMode = ApplicationMode.Resolving;
        
    }

    private void _ResetStatus()
    {
        // Reset internal status.
        m_CurrentMode = ApplicationMode.Ready;
        if (LastPlacedAnchor != null)
        {
            Destroy(LastPlacedAnchor.gameObject);
        }

        LastPlacedAnchor = null;
    }

    public void OnAnchorInstantiated(bool isHost)
    {
        if (AnchorAlreadyInstantiated)
        {
            return;
        }

        AnchorAlreadyInstantiated = true;
    }




    //기본 설정 맞추는 메소드들 3개
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        var sleepTimeout = SleepTimeout.NeverSleep;

#if !UNITY_IOS
        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            sleepTimeout = lostTrackingSleepTimeout;
        }
#endif

        Screen.sleepTimeout = sleepTimeout;

        if (IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
