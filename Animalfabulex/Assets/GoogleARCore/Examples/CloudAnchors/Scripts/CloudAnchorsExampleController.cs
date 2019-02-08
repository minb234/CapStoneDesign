//-----------------------------------------------------------------------
// <copyright file="CloudAnchorsExampleController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.CloudAnchors
{
    using GoogleARCore;
    using UnityEngine;

    /// <summary>
    /// Controller for the Cloud Anchors Example. Handles the ARCore lifecycle.
    /// </summary>
    public class CloudAnchorsExampleController : MonoBehaviour
    {
        [Header("ARCore")]

        /// <summary>
        /// The UI Controller.
        /// </summary>
        public NetworkManagerUIController UIController;

        /// <summary>
        /// The root for ARCore-specific GameObjects in the scene.
        
        /// </summary>
        public GameObject ARCoreRoot;

        /// <summary>
        /// The helper that will calculate the World Origin offset when performing a raycast or generating planes.
        /// 평면생성 또는 raycast를 생성할때 world 좌표값을 계산도와주는 것
        /// </summary>
        public ARCoreWorldOriginHelper ARCoreWorldOriginHelper;

        [Header("ARKit")]

        /// <summary>
        /// The root for ARKit-specific GameObjects in the scene.
        /// </summary>
        public GameObject ARKitRoot;

        /// <summary>
        /// The first-person camera used to render the AR background texture for ARKit.
        /// </summary>
        public Camera ARKitFirstPersonCamera;

        /// <summary>
        /// A helper object to ARKit functionality.
        /// </summary>
        private ARKitHelper m_ARKit = new ARKitHelper();

        /// <summary>
        /// Indicates whether the Origin of the new World Coordinate System, i.e. the Cloud Anchor, was placed.
        /// 새로운 좌표시스템의 원점 즉 클라우드 앵커가 놓여졌는지 나타낸다.
        /// </summary>
        private bool m_IsOriginPlaced = false;

        /// <summary>
        /// Indicates whether the Anchor was already instantiated.
        /// 앵커가 인스턴스화 됫는지 나타냄
        /// </summary>
        private bool m_AnchorAlreadyInstantiated = false;

        /// <summary>
        /// Indicates whether the Cloud Anchor finished hosting.
        /// 클라우드앵커가 호스팅을 완료했는지 나타냄
        /// </summary>
        private bool m_AnchorFinishedHosting = false;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// 앱이 arcore 연결오류로 꺼진다면 true, 아니면 false
        /// </summary>
        private bool m_IsQuitting = false;

        /// <summary>
        /// The last placed anchor.
        /// 마지막으로 배치된 앵커
        /// </summary>
        private Component m_LastPlacedAnchor = null;

        /// <summary>
        /// The current cloud anchor mode.
        /// 현재 클라우드앵커 모드설정
        /// </summary>
        private ApplicationMode m_CurrentMode = ApplicationMode.Ready;

        /// <summary>
        /// Enumerates modes the example application can be in.
        /// 사용가능한 모드를 열거한다. 
        /// </summary>
        public enum ApplicationMode
        {
            Ready,
            Hosting,
            Resolving,
        }

        /// <summary>
        /// The Unity Start() method.
        /// </summary>
        public void Start()
        {
            // A Name is provided to the Game Object so it can be found by other Scripts instantiated as prefabs in the
            // scene.
            
            gameObject.name = "CloudAnchorsExampleController"; //name은 게임오브젝트로 제공되어지고 씬이 prefab으로 인스턴스화된다.
            ARCoreRoot.SetActive(false); //초기화
            ARKitRoot.SetActive(false);
            _ResetStatus();
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();
            //업데이트 실행
            // If we are neither in hosting nor resolving mode then the update is complete.
            if (m_CurrentMode != ApplicationMode.Hosting && m_CurrentMode != ApplicationMode.Resolving)
            {
                return;
            }

            // If the origin anchor has not been placed yet, then update in resolving mode is complete.
            if (m_CurrentMode == ApplicationMode.Resolving && !m_IsOriginPlaced)
            {
                return;
            }

            // If the player has not touched the screen then the update is complete.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            // 플레이어가 터치한 평면의 raycast
            if (Application.platform != RuntimePlatform.IPhonePlayer) //애플리케이션이 동작하는 플랫폼을 나타냄. RuntimePlatform.android는 안드로이드 장치에서 동작하는 플레이어를 뜻함
            {
                TrackableHit hit; //hit변수 만드렁주고 worldorginhelper에서 world좌표값 가져온다
                if (ARCoreWorldOriginHelper.Raycast(touch.position.x, touch.position.y,
                        TrackableHitFlags.PlaneWithinPolygon, out hit))
                {
                    m_LastPlacedAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                }
            }
            else
            {
                Pose hitPose;
                if (m_ARKit.RaycastPlane(ARKitFirstPersonCamera, touch.position.x, touch.position.y, out hitPose))
                {
                    m_LastPlacedAnchor = m_ARKit.CreateAnchor(hitPose);
                }
            }

            // If there was an anchor placed, then instantiate the corresponding object.
            if (m_LastPlacedAnchor != null) 
            {
                // The first touch on the Hosting mode will instantiate the origin anchor. Any subsequent touch will
                // instantiate a star, both in Hosting and Resolving modes.
                //첫번쨰 터치는 앵커를 인스턴스화, 두번쨰 터치는 호스팅모드와 리솔브모드에서 별을 인스턴스화한다.
                if (_CanPlaceStars())
                {
                    _InstantiateStar();
                }
                else if (!m_IsOriginPlaced && m_CurrentMode == ApplicationMode.Hosting)
                {
                    SetWorldOrigin(m_LastPlacedAnchor.transform);
                    _InstantiateAnchor();
                    OnAnchorInstantiated(true);
                }
            }
        }

        /// <summary>
        /// Sets the apparent world origin so that the Origin of Unity's World Coordinate System coincides with the
        /// Anchor. This function needs to be called once the Cloud Anchor is either hosted or resolved.
        /// </summary>
        /// 앵커와 유니티상에서와의 좌표를 일치시켜준다.
        /// <param name="anchorTransform">Transform of the Cloud Anchor.</param>
        public void SetWorldOrigin(Transform anchorTransform)
        {
            if (m_IsOriginPlaced)
            {
                Debug.LogWarning("The World Origin can be set only once.");
                return;
            }

            m_IsOriginPlaced = true;

            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreWorldOriginHelper.SetWorldOrigin(anchorTransform);
            }
            else
            {
                m_ARKit.SetWorldOrigin(anchorTransform);
            }
        }

        /// <summary>
        /// Handles user intent to enter a mode where they can place an anchor to host or to exit this mode if
        /// already in it.
        /// 내 현상태의 모드에 따라 호스팅 으로들어갈지 입력을 처리한다.
        /// </summary>
        public void OnEnterHostingModeClick()
        {
            if (m_CurrentMode == ApplicationMode.Hosting) //호스팅모드
            {
                m_CurrentMode = ApplicationMode.Ready;
                _ResetStatus();
                return;
            }

            m_CurrentMode = ApplicationMode.Hosting;
            _SetPlatformActive();
        }

        /// <summary>
        /// Handles a user intent to enter a mode where they can input an anchor to be resolved or exit this mode if
        /// already in it.
        /// 리솔브모드에서 ㅇ입력을 처리
        /// </summary>
        public void OnEnterResolvingModeClick()
        {
            if (m_CurrentMode == ApplicationMode.Resolving)
            {
                m_CurrentMode = ApplicationMode.Ready;
                _ResetStatus();
                return;
            }

            m_CurrentMode = ApplicationMode.Resolving;
            _SetPlatformActive();
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was instantiated and the host request was made.
        /// 클라우드앵커가 인스턴스화되고 호스트요청이 만들어졌다는 콜백
        /// </summary>
        /// <param name="isHost">Indicates whether this player is the host.</param> //호스트인지 여부를 나타내는 인자
        public void OnAnchorInstantiated(bool isHost)
        {
            if (m_AnchorAlreadyInstantiated)
            {
                return;
            }

            m_AnchorAlreadyInstantiated = true;
            UIController.OnAnchorInstantiated(isHost);
        }

        //클라우드앵커가 호스트나 resolve된것을 나타내주는 콜백들(271~292)
        /// <summary>
        /// Callback indicating that the Cloud Anchor was hosted.
        /// 클라우드앵커가 호스트가된것을 나타내는 콜백
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted successfully.</param> 성공적으로 호스팅
        /// <param name="response">The response string received.</param> 응답
        public void OnAnchorHosted(bool success, string response)
        {
            m_AnchorFinishedHosting = success;
            UIController.OnAnchorHosted(success, response);
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was resolved.
        /// 클라우드 앵커가 해결된 것을 나타내는 콜백
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorResolved(bool success, string response)
        {
            UIController.OnAnchorResolved(success, response);
        }



        /// <summary>
        /// Instantiates the anchor object at the pose of the m_LastPlacedAnchor Anchor. This will host the Cloud
        /// Anchor.
        /// 
        /// </summary>
        private void _InstantiateAnchor()
        {
            // The anchor will be spawned by the host, so no networking Command is needed.
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                      .SpawnAnchor(Vector3.zero, Quaternion.identity, m_LastPlacedAnchor);
        }

        /// <summary>
        /// Instantiates a star object that will be synchronized over the network to other clients.
        /// </summary>
        private void _InstantiateStar()
        {
            // Star must be spawned in the server so a networking Command is used.
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                      .CmdSpawnStar(m_LastPlacedAnchor.transform.position, m_LastPlacedAnchor.transform.rotation);
        }

        /// <summary>
        /// Sets the corresponding platform active.
        /// </summary>
        private void _SetPlatformActive()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreRoot.SetActive(true);
                ARKitRoot.SetActive(false);
            }
            else
            {
                ARCoreRoot.SetActive(false);
                ARKitRoot.SetActive(true);
            }
        }

        /// <summary>
        /// Indicates whether a star can be placed.
        /// </summary>
        /// <returns><c>true</c>, if stars can be placed, <c>false</c> otherwise.</returns>
        private bool _CanPlaceStars()
        {
            if (m_CurrentMode == ApplicationMode.Resolving)
            {
                return m_IsOriginPlaced;
            }

            if (m_CurrentMode == ApplicationMode.Hosting)
            {
                return m_IsOriginPlaced && m_AnchorFinishedHosting;
            }

            return false;
        }

        /// <summary>
        /// Resets the internal status.
        /// </summary>
        private void _ResetStatus()
        {
            // Reset internal status.
            m_CurrentMode = ApplicationMode.Ready;
            if (m_LastPlacedAnchor != null)
            {
                Destroy(m_LastPlacedAnchor.gameObject);
            }

            m_LastPlacedAnchor = null;
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
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

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
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

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
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
}
