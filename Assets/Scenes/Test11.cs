using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test11 : MonoBehaviour
{
    public GameObject aa;
    public GameObject dis;
    public bool flag;
    // Start is called before the first frame update
    void Start()
    {
        //aa.transform.rotation = Quaternion.Euler(new Vector3(0,90,0));
       // aa.transform.Rotate(new Vector3(0,90,0));
    }

    // Update is called once per frame
    void Update()
    {
       
        if (flag) {
            aa.transform.rotation = Quaternion.Slerp(aa.transform.rotation, dis.transform.rotation, Time.deltaTime * 2f);
            aa.transform.position = Vector3.Lerp(aa.transform.position, dis.transform.position, Time.deltaTime * 2f);
       }
       // aa.transform.Rotate(new Vector3(0, 90, 0), 0.8f);
    }
     void OnMouseDown()
    {
        flag = true;
        Debug.Log("按下");
    }
     void OnMouseUp()
    {
        flag = false;
        Debug.Log("提起");
    }
}
