using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameObject player;
    ARTaptoPlaceObject indicator;
    Pose desPose;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        indicator = GetComponent<ARTaptoPlaceObject>();

    }

    // Update is called once per frame
    void Update()
    {
        desPose = indicator.GetPose();
        MoveObject(desPose);

    }

    public void MoveObject(Pose desPose)
    {
        player.transform.transform.LookAt(desPose.position);
        player.transform.position = Vector3.MoveTowards(player.transform.position, desPose.position, 0.03f);
        player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, 0.0f, player.transform.rotation.z);

    }
}
