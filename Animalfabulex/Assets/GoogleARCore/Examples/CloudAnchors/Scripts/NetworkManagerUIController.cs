//-----------------------------------------------------------------------
// <copyright file="NetworkManagerUIController.cs" company="Google">
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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;
    using UnityEngine.UI;

    /// <summary>
    /// Controller managing UI for joining and creating rooms.
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkManagerUIController : MonoBehaviour
    {
        /// <summary>
        /// The Lobby Screen to see Available Rooms or create a new one.
        /// </summary>
        public Canvas LobbyScreen;

        /// <summary>
        /// The snackbar text.
        /// </summary>
        public Text SnackbarText;

        /// <summary>
        /// The Label showing the current active room.
        /// 현재 참여가능한 방을 보여줌
        /// </summary>
        public GameObject CurrentRoomLabel;

        /// <summary>
        /// The Cloud Anchors Example Controller.
        /// </summary>
        public CloudAnchorsExampleController CloudAnchorsExampleController;

        /// <summary>
        /// The Panel containing the list of available rooms to join.
        /// Join 누른후 참여가능한 목록을 포함한 패널창
        /// </summary>
        public GameObject RoomListPanel;

        /// <summary>
        /// Text indicating that no previous rooms exist.
        /// 존재하는 방이없을떄 텍스트창
        /// </summary>
        public Text NoPreviousRoomsText;

        /// <summary>
        /// The prefab for a row in the available rooms list.
        /// 참여가능한 방을 줄로 보여주는 prefab
        /// </summary>
        public GameObject JoinRoomListRowPrefab;

        /// <summary>
        /// The number of matches that will be shown.
        /// 참여가능한 방의 갯수를 지정
        /// </summary>
        private const int k_MatchPageSize = 5;

        /// <summary>
        /// The Network Manager.
        /// 네트워크매니저
        /// </summary>
        private NetworkManager m_Manager;

        /// <summary>
        /// The current room number.
        /// 현재 방의 번호, 방번호를 통해 연결
        /// </summary>
        private string m_CurrentRoomNumber;

        /// <summary>
        /// The Join Room buttons.
        /// 참여버튼 
        /// </summary>
        private List<GameObject> m_JoinRoomButtonsPool = new List<GameObject>();

        /// <summary>
        /// The Unity Awake() method.
        /// 시작되기전 모든변수와 게임이 초기화되기 위해 호출되는 메소드
        /// </summary>
        public void Awake()
        {
            // Initialize the pool of Join Room buttons.
            // 참여가능한 방 생성하는
            for (int i = 0; i < k_MatchPageSize; i++) //k_MatchPageSize만큼 for문을 돌려서 참여가능한 방을 생성하고 리스트를 보여준다.
            {
                GameObject button = Instantiate(JoinRoomListRowPrefab);
                button.transform.SetParent(RoomListPanel.transform, false); //2번째 인자의 값이 true이면 부모의 위치, 스케일 및 회전이 이전 오브젝트와 같게 만들어준다
                button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100 - (200 * i));
                button.SetActive(true);
                button.GetComponentInChildren<Text>().text = string.Empty;
                m_JoinRoomButtonsPool.Add(button);
            }

            //네트워크 매니저를 변수에 넣어주고 
            m_Manager = GetComponent<NetworkManager>();
            m_Manager.StartMatchMaker();
            // MatchMaker 메소드는 플레이어가 공용 IP주소없이 인터넷을 통해 서로 게임 할수있는 서비스가 포함된다.
            // 네트워크 트래픽은 클라이언트가 직접 연결하는 대신 클라우드에서 Unity가 호스팅하는 릴레이 서버를 통과
            //unity에서 제공하는 matchMaker를 통해 쉽게구현가능하다.
            m_Manager.matchMaker.ListMatches(
                startPageNumber: 0,
                resultPageSize: k_MatchPageSize,
                matchNameFilter: string.Empty,
                filterOutPrivateMatchesFromResults: false,
                eloScoreTarget: 0,
                requestDomain: 0,
                callback: _OnMatchList);

            _ChangeLobbyUIVisibility(true);
        }

        /// <summary>
        /// Handles the user intent to create a new room.
        /// 새로운 방을 생성하는것
        /// </summary>
        public void OnCreateRoomClicked()
        {
            //매치메이커 인자값 매치이름(string), 매치사이즈(int) - 최대 사용자 수, 매치 advertise(bool)-검색시 광고, 매치 패스워드(string) - empty로 설정시 없음, 콜백
            
            m_Manager.matchMaker.CreateMatch(m_Manager.matchName, m_Manager.matchSize,
                                           true, string.Empty, string.Empty, string.Empty,
                                           0, 0, _OnMatchCreate);
        }

        /// <summary>
        /// Handles the user intent to refresh the room list.
        /// 참여가능한 방을 새로고침하는 것
        /// </summary>
        public void OnRefhreshRoomListClicked()
        {
            m_Manager.matchMaker.ListMatches(
                startPageNumber: 0, //결과의 첫페이지
                resultPageSize: k_MatchPageSize, // 각 페이지의 생성되는 방의 갯수
                matchNameFilter: string.Empty, // 이름을 필터링할 문자열 없는걸로 설정해도 상관없음
                filterOutPrivateMatchesFromResults: false, // 응답에 암호를 통해 보호되는지 여부를 나타내는 bool
                eloScoreTarget: 0, //
                requestDomain: 0, // 동일한 도메인의 요청 만 서로 인터페이스 할 수 있다. 같은 도메인에서만 매치리스트를 확인할수 있따.
                callback: _OnMatchList);
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was instantiated and the host request was made.
        /// Cloud Anchor가 인스턴스화되고 호스트 요청이 작성되었음을 나타내는 콜백
        /// </summary>
        /// <param name="isHost">Indicates whether this player is the host.</param>
        public void OnAnchorInstantiated(bool isHost)
        {
            //호스트일 경우와 아닐경우에 대한 안내 텍스트 
            if (isHost)
            {
                SnackbarText.text = "Hosting Cloud Anchor...";
            }
            else
            {
                SnackbarText.text = "Cloud Anchor added to session! Attempting to resolve anchor...";
            }
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was hosted.
        /// 클라우드앵커가 호스팅되었을때의 콜백함수
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorHosted(bool success, string response)
        {
            if (success)
            {
                SnackbarText.text = "Cloud Anchor successfully hosted! Tap to place more stars.";
            }
            else
            {
                SnackbarText.text = "Cloud Anchor could not be hosted. " + response;
            }
        }

        /// <summary>
        /// Callback indicating that the Cloud Anchor was resolved.
        /// 클라우드 앵커가 resolve되었을떄의 콜백함수
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorResolved(bool success, string response)
        {
            if (success)
            {
                SnackbarText.text = "Cloud Anchor successfully resolved! Tap to place more stars.";
            }
            else
            {
                SnackbarText.text = "Cloud Anchor could not be resolved. Will attempt again. " + response;
            }
        }

        /// <summary>
        /// Handles the user intent to join the room associated with the button clicked.
        /// 방을 들어가기위해 버튼을 클릭했을때 조절
        /// </summary>
        /// <param name="match">The information about the match that the user intents to join.</param> 매소드의 매개변수 중 하나를 설명하기 위해 사용되는 것 param
        private void _OnJoinRoomClicked(MatchInfoSnapshot match)
        {
            m_Manager.matchName = match.name;
            // JoinMatch 인자값 네트워크 아이디, 패스워드, 공개 클라이언트 주소(빈문자열 전달할 경우 Matchmaker 또는 서버사용에 영향을 미치지 x), 개인클라이언트 주소(앞과 같음),
            //elo점수(0으로 설정시 비활성화), requestdomain(같은값으로 설정하여야한다), 콜백)
            m_Manager.matchMaker.JoinMatch(match.networkId, string.Empty, string.Empty,
                                         string.Empty, 0, 0, _OnMatchJoined);
            CloudAnchorsExampleController.OnEnterResolvingModeClick();
        }


        /// <summary>
        /// Callback that happens when a <see cref="T:NetworkMatch.ListMatches"/> request has been processed on the
        /// server.
        /// 네트워크상 리스트매치 요청이 서버에서 처리될때 발생하는 콜백
        /// </summary>
        /// <param name="success">Indicates if the request succeeded.</param> - 요청이 성공되었을때
        /// <param name="extendedInfo">A text description for the error if success is false.</param> - 실패했을떄 텍스트 
        /// <param name="matches">A list of matches corresponding to the filters set in the initial list 
        /// request.</param>
        private void _OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            m_Manager.OnMatchList(success, extendedInfo, matches);
            if (!success)
            {
                SnackbarText.text = "Could not list matches: " + extendedInfo;
                return;
            }

            if (m_Manager.matches != null)
            {
                // Reset all buttons in the pool.
                //foreach문은 처음부터 끝까지 멤버전체를 순환 : 참여버튼을 초기화해준다.
                foreach (GameObject button in m_JoinRoomButtonsPool)
                {
                    button.SetActive(false);
                    button.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                    button.GetComponentInChildren<Text>().text = string.Empty;
                }

                NoPreviousRoomsText.gameObject.SetActive(m_Manager.matches.Count == 0);

                // Add buttons for each existing match.
                //각 방마다 참여버튼을 추가
                int i = 0;
                foreach (var match in m_Manager.matches)
                {
                    if (i >= k_MatchPageSize)
                    {
                        break;
                    }

                    var text = "Room " + _GeetRoomNumberFromNetworkId(match.networkId); //부여받은 네트워크아이디와 번호를 통해 
                    GameObject button = m_JoinRoomButtonsPool[i++];
                    button.GetComponentInChildren<Text>().text = text; //자식에서 구성요소를 반환해준다.
                    button.GetComponentInChildren<Button>().onClick.AddListener(() => _OnJoinRoomClicked(match));
                    button.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Callback that happens when a <see cref="T:NetworkMatch.CreateMatch"/> request has been processed on the
        /// server. 
        /// creatematch 요청이 서버에서 처리될때 발생하는 콜백
        /// </summary>
        /// <param name="success">Indicates if the request succeeded.</param>
        /// <param name="extendedInfo">A text description for the error if success is false.</param>
        /// <param name="matchInfo">The information about the newly created match.</param>
        private void _OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            m_Manager.OnMatchCreate(success, extendedInfo, matchInfo);
            if (!success)
            {
                SnackbarText.text = "Could not create match: " + extendedInfo;
                return;
            }

            m_CurrentRoomNumber = _GeetRoomNumberFromNetworkId(matchInfo.networkId);
            _ChangeLobbyUIVisibility(false); //로비상태에서의 버튼 false로 설정시 숨김
            SnackbarText.text = "Find a plane, tap to create a Cloud Anchor.";
            CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + m_CurrentRoomNumber;
        }

        /// <summary>
        /// Callback that happens when a <see cref="T:NetworkMatch.JoinMatch"/> request has been processed on the
        /// server.
        /// joinmatch 요청이 서버에서 처리될때 발생하는 콜백
        /// </summary>
        /// <param name="success">Indicates if the request succeeded.</param>
        /// <param name="extendedInfo">A text description for the error if success is false.</param>
        /// <param name="matchInfo">The info for the newly joined match.</param>
        private void _OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            m_Manager.OnMatchJoined(success, extendedInfo, matchInfo);
            if (!success)
            {
                SnackbarText.text = "Could not join to match: " + extendedInfo;
                return;
            }

            m_CurrentRoomNumber = _GeetRoomNumberFromNetworkId(matchInfo.networkId);
            _ChangeLobbyUIVisibility(false); //로비상태에서의 버튼 false로 설정시 숨김
            SnackbarText.text = "Waiting for Cloud Anchor to be hosted...";
            CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + m_CurrentRoomNumber;
        }

        /// <summary>
        /// Changes the lobby UI Visibility by showing or hiding the buttons.
        /// </summary>
        /// <param name="visible">If set to <c>true</c> the lobby UI will be visible. It will be hidden
        /// otherwise.</param>
        /// 로비UI 
        private void _ChangeLobbyUIVisibility(bool visible)
        {
            LobbyScreen.gameObject.SetActive(visible);
            CurrentRoomLabel.gameObject.SetActive(!visible);
            foreach (GameObject button in m_JoinRoomButtonsPool)
            {
                bool active = visible && button.GetComponentInChildren<Text>().text != string.Empty;
                button.SetActive(active);
            }
        }
        //방번호 생성
        private string _GeetRoomNumberFromNetworkId(UnityEngine.Networking.Types.NetworkID networkID)
        {
            return (System.Convert.ToInt64(networkID.ToString()) % 10000).ToString();
        }
    }
}
