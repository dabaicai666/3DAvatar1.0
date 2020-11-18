using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
//using UnityEngine.UIElements;
//using Newtonsoft;
//using Newtonsoft.Json;
using System.Text;
using System;
using System.IO;
//using LitJson;

public class LoadAni : MonoBehaviour
{
    public RuntimeAnimatorController ani;
    public SkinnedMeshRenderer obj;
    public Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uv;
    Vector3[] normals;
    Hashtable hash = new Hashtable();
    // Start is called before the first frame update
    void Start()
    {
       // mesh = obj.sharedMesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        normals = mesh.normals;
        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        //mesh.RecalculateTangents();
        uv = mesh.uv;
       // mesh.ClearBlendShapes();
        Debug.Log("vertices:" + vertices.Length);
        Debug.Log("triangles:" + triangles.Length);
        Debug.Log("normals:" + normals.Length);
        //Debug.Log(vertices.Length +":"+ triangles.Length +":" + uv.Length + ":" + mesh.vertexCount);
        //hash.Add("vertices",vertices);
        //hash.Add("triangles",triangles);
        //hash.Add("uv",uv);
        Debug.Log(JsonUtil.ToJson(vertices));
        string ver =JsonUtil.ToJson(vertices);
        ver = ver.Replace("{\"x\":","[").Replace("}","]").Replace("\"y\":","").Replace("\"z\":","");
        string tri =JsonUtil.ToJson(triangles);
       


        //GameObject a = new GameObject("AA");
        //a.AddComponent<MeshFilter>();
        //a.AddComponent<MeshRenderer>();
        //Mesh m = new Mesh();
        //m.vertices = vertices;
        //m.normals = normals;
        //m.uv = uv;
        //m.triangles = triangles;
        //a.GetComponent<MeshFilter>().mesh = m;

        string  uvs =JsonUtil.ToJson(uv);
        uvs = uvs.Replace("{\"x\":", "[").Replace("}", "]").Replace("\"y\":", "");
        File.WriteAllText(Application.dataPath + "/vertices_.txt", ver);
        File.WriteAllText(Application.dataPath + "/triangles1.txt", tri);
        File.WriteAllText(Application.dataPath + "/uv1.txt", uvs);
       

        //FiledownloadHelper.Inst.GetAssetBundle(Application.dataPath + "/_AllBundle_PC/test.assetbundle", null, (model, flag) =>
        //{
        //    if (flag)
        //    {
        //        Debug.Log("加载成功");
        //        GameObject obj = Instantiate(model);
        //        obj.GetComponent<Animator>().runtimeAnimatorController = ani;
        //    }
        //    else
        //    {

        //    }
        //});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string HashtableToWxJson(Hashtable data)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (object key in data.Keys)
            {
                object value = data[key];
                sb.Append("\"");
                sb.Append(key);
                sb.Append("\":\"");
                if (!String.IsNullOrEmpty(value.ToString()) && value != DBNull.Value)
                {
                    sb.Append(value).Replace("\\", "/");
                }
                else
                {
                    sb.Append(" ");
                }
                sb.Append("\",");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }
        catch (Exception ex)
        {

            return "";
        }
    }
}
