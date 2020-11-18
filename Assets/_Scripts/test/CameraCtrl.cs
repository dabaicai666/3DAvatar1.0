
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraCtrl : MonoBehaviour

{

    public Transform CenObj;//围绕的物体
    public float ratio = 1.0f;//放大缩小速率
    public float min_distance = 5.0f; //相机距物体最小距离
    public float max_distance = 10.0f;//相机距物体最大距离

    //滑动结束时的瞬时速度
    Vector3 Speed = Vector3.zero;
    //每帧偏差
    Vector3 offSet = Vector3.zero;

    //速率衰减值
    public float decelerationRate = 0.2f;

    private Vector3 Rotion_Transform;

    private new Camera camera;

    void Start()

    {
        camera = GetComponent<Camera>();

        Rotion_Transform = CenObj.position;
    }

    void Update()

    {

        Ctrl_Cam_Move();

        Cam_Ctrl_Rotation();

    }

    //镜头的远离和接近

    public void Ctrl_Cam_Move()

    {
        Vector3 mousePos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.1f));
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Vector3.Distance(CenObj.position, camera.transform.position) > min_distance) //放大

        {

            camera.transform.position -= ratio * (camera.transform.position - mousePos);

        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Vector3.Distance(CenObj.position, camera.transform.position) < max_distance) //缩小

        {

            camera.transform.position += ratio * (camera.transform.position - mousePos);

        }

    }

    //摄像机的旋转

    public void Cam_Ctrl_Rotation()

    {

        var mouse_x = Input.GetAxis("Mouse X");//获取鼠标X轴移动

        var mouse_y = -Input.GetAxis("Mouse Y");//获取鼠标Y轴移动

        if (Input.GetMouseButton(0))
        {
            offSet.x = mouse_x;
            offSet.y = mouse_y;
            //瞬时速度
            Speed = offSet / Time.deltaTime;
        }
        else
        {
            Speed *= Mathf.Pow(decelerationRate, Time.deltaTime);
            if (Mathf.Abs(Vector3.Magnitude(Speed)) < 1)
            {
                Speed = Vector3.zero;
            }

        }
        Move(Speed);

    }
    public void Move(Vector3 speed)
    {
        if (Vector3.Magnitude(speed) == 0)
        {
            return;
        }
        SendMessage("getmessage",null);
     //   Debug.Log("Move");
        transform.RotateAround(Rotion_Transform, Vector3.up, speed.x * Time.deltaTime * 2.0f);

        transform.RotateAround(Rotion_Transform, transform.right, speed.y * Time.deltaTime * 2.0f);

    }

}

