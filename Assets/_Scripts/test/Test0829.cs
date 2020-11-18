using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test0829 : MonoBehaviour
{
    public SkinnedMeshRenderer mesh;
    Vector3[] world_vv;
    // Start is called before the first frame update
    void Start()
    {
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        world_vv = new Vector3[mesh.sharedMesh.vertices.Length];
        for (int i = 0; i < mesh.sharedMesh.vertices.Length; ++i)
        {
           Vector3 world_v = localToWorld.MultiplyPoint3x4(mesh.sharedMesh.vertices[i]);
            world_vv[i] = world_v;

        }
    }

    // Update is called once per frame
    void Update()
    {
         
       
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,100),"")) {

            Matrix4x4 localToWorld = transform.localToWorldMatrix;

            for (int i = 0; i < mesh.sharedMesh.vertices.Length; ++i)
            {
                Vector3 world_v = localToWorld.MultiplyPoint3x4(mesh.sharedMesh.vertices[i]);
                if (Vector3.Distance(world_v,world_vv[i]) > 0.00000001f) {
                    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                }
             //   Debug.Log(JsonUtil.ToJson(world_v));
                
            }
        }
    }
}
