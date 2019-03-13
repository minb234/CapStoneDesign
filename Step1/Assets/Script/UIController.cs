using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public CloudAnchorController CloudAnchorController;
    public Button button;
    private NetworkManager networkManager;


    public void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        networkManager.StartMatchMaker();
        networkManager.matchMaker.ListMatches(
            startPageNumber: 0,
            resultPageSize: 2,
            matchNameFilter: string.Empty,
            filterOutPrivateMatchesFromResults: false,
            eloScoreTarget: 0,
            requestDomain: 0,
            callback: _OnMatchList);
            

    }

    private void _OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        networkManager.OnMatchList(success, extendedInfo, matches);

        //throw new NotImplementedException();
    }

    public void OnCreateRoomClicked()
    {
        networkManager.matchMaker.CreateMatch(networkManager.matchName, networkManager.matchSize,
                                       true, string.Empty, string.Empty, string.Empty,
                                       0, 0, _OnMatchCreate);
        button.gameObject.SetActive(false);
    }
    private void _OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        Debug.Log("match success!");
        networkManager.OnMatchCreate(success, extendedInfo, matchInfo);
    }

    
}




