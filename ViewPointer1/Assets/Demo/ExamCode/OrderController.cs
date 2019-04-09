using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderController : MonoBehaviour
{
    public GameObject player;
    ARTaptoPlaceObject indicator;
    HydraController hydraController;

    Pose desPose;
    int menu = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player_Hydra");
        indicator = GetComponent<ARTaptoPlaceObject>();
        hydraController = GetComponent<HydraController>();

    }

    // Update is called once per frame
    void Update()
    {
        desPose = indicator.GetPose();
    }

    public void Move()
    {
        hydraController.SetMenu(1);
        player.transform.LookAt(desPose.position);
        player.transform.position = Vector3.MoveTowards(player.transform.position, desPose.position, 0.015f);
    }
    public void Attack()
    {
        hydraController.SetMenu(2);
    }


    public void SetMenu(int menu)
    {
        this.menu = menu;
    }
    public int GetMenu()
    {
        return this.menu;
    }
}
