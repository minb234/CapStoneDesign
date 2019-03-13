using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject PlayerPrefab;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        gameObject.name = "LocalPlayer";
    }

    public void SpawnPlayer(Vector3 position, Quaternion rotation, Component anchor)
    {
        // Instantiate Anchor model at the hit pose.
        var playerObject = Instantiate(PlayerPrefab, position, rotation);

        // Anchor must be hosted in the device.
        playerObject.GetComponent<AnchorController>().HostLastPlacedAnchor(anchor);

        // Host can spawn directly without using a Command because the server is running in this instance.
        NetworkServer.Spawn(playerObject);
    }
}
