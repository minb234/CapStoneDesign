using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public Transform firePos;
    private float bulletSpeed = 0.2f;
    private float limitTimes = 0.0f;

    Pose AttackDesPose;

    // Start is called before the first frame update
    void Start()
    {

        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    private void Update()
    {
        this.transform.Translate(Vector3.forward * bulletSpeed);
        limitTimes += Time.deltaTime;
        if(limitTimes> 1)
        {
            DestroyImmediate(this);
        }
        //limitTimes += Time.deltaTime;
        //this.transform.Translate(firePos.forward * bulletSpeed);

        //if (limitTimes > 1.0f)
        //{
        //    DestroyImmediate(this);
        //    limitTimes = 0.0f;
        //}


    }
}
