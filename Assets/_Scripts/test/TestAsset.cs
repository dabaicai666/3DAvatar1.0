using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using LitJson;
using System.Linq;

public class TestAsset : MonoBehaviour
{
    int[] vertexMap;
    Vector3[] vertex_unity;
    // Start is called before the first frame update
    void Start()
    {
        
        //string mapdata = File.ReadAllText(Application.dataPath + "/_TestFile/mapdata.txt");  //获取vertexMap数组
        //JsonData mapjson = JsonMapper.ToObject(mapdata);
        //vertexMap = new int[mapjson["vertexMap"].Count];
        //Debug.Log(mapjson["vertexMap"].Count);
        //for (int i = 0;i< mapjson["vertexMap"].Count; i++) {
        //    vertexMap[i] = (int)mapjson["vertexMap"][i];
        //}
        //string json_u = File.ReadAllText(Application.dataPath + "/_TestFile/json_u.txt");  //获取unity模型顶点
        //JsonData udata = JsonMapper.ToObject(json_u);
        //vertex_unity = new Vector3[udata["vertices"].Count];
        //for (int i = 0;i< udata["vertices"].Count;i++) {
        //    vertex_unity[i].x = float.Parse(udata["vertices"][i][0].ToString());
        //    vertex_unity[i].y = float.Parse(udata["vertices"][i][1].ToString());
        //    vertex_unity[i].z = float.Parse(udata["vertices"][i][2].ToString());
        //}
        //Vector3[] samp = new Vector3[352];  //衣服形变后的数据
        //Vector3[] newSample = new Vector3[mapjson["vertexMap"].Count];
        //for (int i=0;i< mapjson["vertexMap"].Count;i++) {
        //    newSample[i] = samp[vertexMap[i]];
        //}
        //FiledownloadHelper.Inst.GetAssetBundle(Application.dataPath + "/_AllBundle_PC/cloth_test_1.assetbundle", null, (clothObj, flag) =>
        //{
        //    Instantiate(clothObj);
        //    Mesh objmesh = clothObj.GetComponent<MeshFilter>().sharedMesh;
        //    //  Material material = clothObj.GetComponent<MeshRenderer>().sharedMaterial;

        //    //TODO 对衣服mesh进行更改
        //    //  Instantiate(clothObj, parentPos.transform.position, parentPos.transform.rotation);


        //    GameObject a = new GameObject("AA");
        //    a.AddComponent<MeshFilter>();
        //    a.AddComponent<MeshRenderer>();
        //    Mesh m = new Mesh();
        //    // m.vertices = tempvertices;
        //    //  m.normals = objmesh.normals;
        //    //  m.uv = objmesh.uv;
        //    m.triangles = objmesh.triangles;
        //    a.GetComponent<MeshFilter>().mesh = m;
        //    //  a.GetComponent<MeshRenderer>().material = material;
        //    Debug.Log("衣服初始化成功");
        //}); ;
       // Debug.Log(vertexMap.Count());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,100),"")){
            for (int i = 0; i < 50000; i++)
            {
                Debug.Log(i);
            }
        }
    }
}
