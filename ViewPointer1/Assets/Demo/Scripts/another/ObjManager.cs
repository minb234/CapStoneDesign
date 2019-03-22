using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour
{
    //싱글톤
    static ObjManager st;
    public static ObjManager Call() { return st; }
    private void Awake()
    {
        st = this;
    }

    private void OnDestroy()
    {
        MemoryDelete();
        st = null;
    }

    public GameObject[] Origin;
    public List

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetObject(string _Name, int _Count = 20)
    {
        GameObject obj = null;
        int Count = Origin.Length;
    }
}
