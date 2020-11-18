using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestAA : MonoBehaviour
{
    public GameObject obj1, Obj2;
    // Start is called before the first frame update
    void Start()
    {
        AA();
        BB();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AA() {
        Vector3[] oldVertices = obj1.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.vertices;
        Vector3[] v = new Vector3[oldVertices.Count()];
        float[] vv = new float[oldVertices.Count()];
        for (int q = 0; q < oldVertices.Count(); q++)
        {
            Debug.Log(oldVertices[q].y);
            vv[q] = oldVertices[q].y;
            //if (oldVertices[q].y == 0) {
            //    Debug.Log("111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111");
            //    break;
            //}
        }
        float a1 = vv[0];
        for (int j = 0; j < vv.Count(); j++)
        {

            if (a1 > vv[j])
            {

                a1 = vv[j];
             //   Debug.Log(a1);
            }
        }
    }
    public void BB() {
        Mesh a = Obj2.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        Vector3[] oldVertices = Obj2.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.vertices;
        Vector3[] v = new Vector3[oldVertices.Count()];
        float[] vv = new float[oldVertices.Count()];
        for (int q = 0; q < oldVertices.Count(); q++)
        {
            Debug.Log(oldVertices[q].y);
            vv[q] = oldVertices[q].y;

        }
        float a1 = vv[0];
        for (int j = 0; j < vv.Count(); j++)
        {

            if (a1 > vv[j])
            {
                a1 = vv[j];
                Debug.Log(a1);
            }
        }
        Mesh m = new Mesh();
        for (int i = 0; i < oldVertices.Count(); i++)
        {
            v[i].x = -oldVertices[i].x;
            v[i].y = oldVertices[i].y - 1.119426f;
            v[i].z = -oldVertices[i].z;
        }
       
        m.vertices = v;
        m.uv = a.uv;
        m.triangles = a.triangles;
        m.RecalculateNormals();
        m.RecalculateTangents();
        m.RecalculateBounds();
        Obj2.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = m;
    }
}
