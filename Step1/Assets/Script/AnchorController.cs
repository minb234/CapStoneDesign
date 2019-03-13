using GoogleARCore;
using GoogleARCore.CrossPlatform;
using UnityEngine;
using UnityEngine.Networking;

public class AnchorController : NetworkBehaviour
{
    [SyncVar(hook = "_OnChangeId")]
    private string CloudAnchorId = string.Empty;
    private bool IsHost = false;
    private bool ShouldResolve = false;
    private CloudAnchorController CloudAnchorController;


    public void Start()
    {
        CloudAnchorController = GameObject.Find("CloudAnchorController")
                                                        .GetComponent<CloudAnchorController>();
    }

    public override void OnStartClient()
    {
        if (CloudAnchorId != string.Empty)
        {
            ShouldResolve = true;
        }
    }

    public void Update()
    {
        if (ShouldResolve)
        {
            _ResolveAnchorFromId(CloudAnchorId);
        }
    }

    [Command]
    public void CmdSetCloudAnchorId(string cloudAnchorId)
    {
        CloudAnchorId = cloudAnchorId;
    }

    public string GetCloudAnchorId()
    {
        return CloudAnchorId;
    }

    public void HostLastPlacedAnchor(Component lastPlacedAnchor)
    {
        IsHost = true;

#if !UNITY_IOS
        var anchor = (Anchor)lastPlacedAnchor;
#elif ARCORE_IOS_SUPPORT
            var anchor = (UnityEngine.XR.iOS.UnityARUserAnchorComponent)lastPlacedAnchor;
#endif

#if !UNITY_IOS || ARCORE_IOS_SUPPORT
        XPSession.CreateCloudAnchor(anchor).ThenAction(result =>
        {
            if (result.Response != CloudServiceResponse.Success)
            {
                Debug.Log(string.Format("Failed to host Cloud Anchor: {0}", result.Response));

                //CloudAnchorController.OnAnchorHosted(false, result.Response.ToString());
                return;
            }

            Debug.Log(string.Format("Cloud Anchor {0} was created and saved.", result.Anchor.CloudId));
            CmdSetCloudAnchorId(result.Anchor.CloudId);

            //CloudAnchorController.OnAnchorHosted(true, result.Response.ToString());
        });
#endif
    }

    private void _ResolveAnchorFromId(string cloudAnchorId)
    {
        CloudAnchorController.OnAnchorInstantiated(false);

        // If device is not tracking, let's wait to try to resolve the anchor.
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        ShouldResolve = false;

        XPSession.ResolveCloudAnchor(cloudAnchorId).ThenAction((System.Action<CloudAnchorResult>)(result =>
        {
            if (result.Response != CloudServiceResponse.Success)
            {
                Debug.LogError(string.Format("Client could not resolve Cloud Anchor {0}: {1}",
                                             cloudAnchorId, result.Response));

                //CloudAnchorController.OnAnchorResolved(false, result.Response.ToString());
                ShouldResolve = true;
                return;
            }

            Debug.Log(string.Format("Client successfully resolved Cloud Anchor {0}.",
                                    cloudAnchorId));

            //CloudAnchorController.OnAnchorResolved(true, result.Response.ToString());
            _OnResolved(result.Anchor.transform);
        }));
    }

    

    private void _OnResolved(Transform anchorTransform)
    {
        var cloudAnchorController = GameObject.Find("CloudAnchorController")
                                              .GetComponent<CloudAnchorController>();
        cloudAnchorController.SetWorldOrigin(anchorTransform);
    }

    /// <summary>
    /// Callback invoked once the Cloud Anchor Id changes.
    /// </summary>
    /// <param name="newId">New identifier.</param>
    private void _OnChangeId(string newId)
    {
        if (!IsHost && newId != string.Empty)
        {
            CloudAnchorId = newId;
            ShouldResolve = true;
        }
    }

}
