using AOT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
//using Dummiesman;
using LitJson;

public class PhyUtils : MonoBehaviour
{

    public static PhyUtils Inst;

    // brief 物理仿真参数
    [StructLayout(LayoutKind.Sequential)]
    public struct Params
    {
        public int numIterations;                  //!< Number of solver iterations to perform per-substep
        public float rigidThickness;               //!< rigid collision thickness;
        public float selfThickness;                //!< self collision thickness; 
        // common params
        public float dynamicFriction;              //!< Coefficient of friction used when colliding against shapes
        public float staticFriction;               //!< Coefficient of static friction used when colliding against shapes
        // cloth params
        public Vector3 wind;                       //!< Constant acceleration applied to particles that belong to dynamic triangles, drag needs to be > 0 for wind to affect triangles
        public Vector3 gravity;                    //!< Constant acceleration applied to all particles

        public float springStiffness;              //!< springStiffness 
        public float bendingStiffness;             //!< bendingStiffness;
    };

    const string PHY_DLL = "MiGuPhysicd";

    // brief 仿真场景初始化，需要在仿真前进行设置
    [DllImport(PHY_DLL, EntryPoint = "SimInitialize")]
    public static extern bool SimInit();
    // brief 仿真更新步骤。
    [DllImport(PHY_DLL, EntryPoint = "SimUpdate")]
    public static extern bool SimUpdate();
    // brief 更新标示， 为true时， 仿真， 为false时 不仿真
    [DllImport(PHY_DLL, EntryPoint = "SetSimFlag")]
    public static extern void SetSimFlag(bool flag);
    // brief 获取仿真物理参数
    [DllImport(PHY_DLL, EntryPoint = "PhyGetParams")]
    public static extern void GetParams(ref Params parameters);
    // brief 设置仿真物理参数
    [DllImport(PHY_DLL, EntryPoint = "PhySetParams")]
    public static extern void SetParams(ref Params parameters);
    //  brief 创建衣服仿真对象，输入顶点数组， 顶点个数， 面片索引数组， 面片个数， 返回衣服ID 
    [DllImport(PHY_DLL, EntryPoint = "PhyCreateClothFromMesh")]
    public static extern int CreateClothFromMesh(ref Vector4 particles, int numParticles, ref int indices, int numTriangles);
    //  brief  删除衣服仿真对象， 输入ID
    [DllImport(PHY_DLL, EntryPoint = "PhyDeleteCloth")]
    public static extern bool DeleteCloth(int id);
    //  brief  创建人体仿真对象， 输入顶点数组， 顶点个数， 面片索引数组、 面片个数、 返回人体ID
    [DllImport(PHY_DLL, EntryPoint = "PhyCreateBodyFromMesh")]
    public static extern int CreateSkinFromMesh(ref Vector4 particles, int numParticles, ref int indices, int numTriangles);
    //  brief  删除人体仿真对象， 输入ID
    [DllImport(PHY_DLL, EntryPoint = "PhyDeleteBody")]
    public static extern bool DeleteSkin(int id);
    //  brief  获取更新后的顶点buffer, 输入id,  顶点buffer引用
    [DllImport(PHY_DLL, EntryPoint = "GetClothBuffer")]
    public static extern void GetClothBuffer(int id, ref Vector4 partices);
    //  brief  设置动画过程中每一帧的人体顶点buffer
    [DllImport(PHY_DLL, EntryPoint = "SetSkinBuffer")]
    public static extern void SetSkinBuffer(int id, ref Vector4 partices);
    //  brief  设置缝合关系， 输入衣服id,  缝合对， 个数
    [DllImport(PHY_DLL, EntryPoint = "PhySetSeams")]
    public static extern void SetSeam(int id, ref Vector2Int pair, int numPair);

    static Vector2Int Vec2I(string x, string y) => new Vector2Int(int.Parse(x), int.Parse(y));

    public Material clo_mat;
    public Material skin_mat;
    public GameObject clothObj_;
    public GameObject obj_;
    public void SimStart()
    {
        isSim = !isSim;
        Debug.LogFormat("Simulation Flag :{0}", isSim);
        SetSimFlag(isSim);
    }

    void UpdateParams()
    {
        Params prms = new Params();
        GetParams(ref prms);
        prms.numIterations = numIterations;
        prms.rigidThickness = rigidThickness;
        prms.selfThickness = selfThickness;
        prms.springStiffness = springStiffness;
        prms.bendingStiffness = bendingStiffness;
        prms.dynamicFriction = dynamicFriction;
        prms.staticFriction = staticFriction;
        prms.wind = m_Wind;
        prms.gravity = m_Gravity;
        SetParams(ref prms);
    }

    void BuildClothFromMesh(ref GameObject model, JsonData data)
    {
        // m_simMesh = model.transform.GetComponentInChildren<MeshFilter>().mesh;
        foreach (var a in model.GetComponentsInChildren<MeshFilter>())
        {
            m_simMesh = a.mesh;
        }
        Debug.Log(m_simMesh.vertices.Length.ToString());
        if (m_simMesh)
        {
            Vector3[] vertices = m_simMesh.vertices;
            int[] triangles = m_simMesh.triangles;
            if (vertices != null && vertices.Length > 0 && triangles != null && triangles.Length > 0)
            {
                int particleCount = vertices.Length;
                m_cloVertArray = new Vector4[particleCount];
                for (int i = 0; i < particleCount; ++i)
                {
                    Vector3 v = vertices[i];
                    m_cloVertArray[i] = new Vector4(v.x * m_meshLocalScale.x, v.y * m_meshLocalScale.y, v.z * m_meshLocalScale.z, 1.0f);
                }

                int indexCount = triangles.Length;
                int[] indices = new int[indexCount];
                for (int i = 0; i < indexCount; ++i)
                {
                    indices[i] = triangles[i];
                }

                StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + "/clothMesh_bak.obj", false);
                foreach (var v in m_cloVertArray)
                {
                    writer.WriteLine($"v {(float)v.x} {(float)v.y} {(float)v.z}"); ;
                }

                for (int i = 0; i < indexCount / 3; i++)
                {
                    writer.WriteLine($"f {(int)indices[i * 3 + 0] + 1}  {(int)indices[i * 3 + 1] + 1} {(int)indices[i * 3 + 2] + 1}");
                }
                writer.Close();

                //创建衣服数据 
                m_cloID = CreateClothFromMesh(ref m_cloVertArray[0], m_cloVertArray.Length, ref indices[0], indices.Length / 3);
            }
        }

        //create cloth seams
        //var seamStream = new StreamReader(Application.streamingAssetsPath + "/correspond_8_list.txt");
        m_seamPairs = new List<Vector2Int>(); ;

        //  String content = seamStream.ReadLine();
        //  while (null != content)
        //  {
        //Debug.Log(content);
        //      string[] parts = content.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //     m_seamPairs.Add(Vec2I(parts[0], parts[1]));
        //     content = seamStream.ReadLine();
        //  }
        //  seamStream.Close();

        for (int i = 0; i < data["c_list"].Count; i++)
        {
            Debug.Log(data["c_list"][i][0].ToString() + ":" + data["c_list"][i][1].ToString());
            m_seamPairs.Add(Vec2I(data["c_list"][i][0].ToString(), data["c_list"][i][1].ToString()));
        }

        //设置缝合关系
        SetSeam(m_cloID, ref m_seamPairs.ToArray()[0], m_seamPairs.Count);

        Debug.Log("Build Cloth Success");
    }


    void BuildSkinFromMesh(ref GameObject model)
    {
        m_skinMesh = model.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;

        if (m_skinMesh)
        {
            Vector3[] vertices = m_skinMesh.vertices;
            int[] triangles = m_skinMesh.triangles;
            if (vertices != null && vertices.Length > 0 && triangles != null && triangles.Length > 0)
            {
                int particleCount = vertices.Length;
                m_skinVertArray = new Vector4[particleCount];
                for (int i = 0; i < particleCount; ++i)
                {
                    Vector3 v = vertices[i];
                    m_skinVertArray[i] = new Vector4(v.x * m_meshLocalScale.x, v.y * m_meshLocalScale.y, v.z * m_meshLocalScale.z, 1.0f);
                }

                int indexCount = triangles.Length;
                int[] indices = new int[indexCount];
                for (int i = 0; i < indexCount; ++i)
                {
                    indices[i] = triangles[i];
                }

                StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + "/skinMesh_bak.obj", false);
                foreach (var v in m_skinVertArray)
                {
                    writer.WriteLine($"v {(float)v.x} {(float)v.y} {(float)v.z}"); ;
                }

                for (int i = 0; i < indexCount / 3; i++)
                {
                    writer.WriteLine($"f {(int)indices[i * 3 + 0] + 1}  {(int)indices[i * 3 + 1] + 1} {(int)indices[i * 3 + 2] + 1}");
                }
                writer.Close();

                //创建人体数据 
                m_skinID = CreateSkinFromMesh(ref m_skinVertArray[0], m_skinVertArray.Length, ref indices[0], indices.Length / 3);
            }
        }
    }

    void UpdateBuffer()
    {
        SetSkinBuffer(m_skinID, ref m_skinVertArray[0]);
        GetClothBuffer(m_cloID, ref m_cloVertArray[0]);
    }

    void UpdateClothMesh(ref GameObject model, ref Vector4[] particles)
    {
        if (particles != null)
        {
            m_simMesh = model.transform.GetComponentInChildren<MeshFilter>().mesh;
            Vector3[] vertices = m_simMesh.vertices;
            for (int i = 0; i < m_cloVertArray.Length; i++)
            {
                vertices[i].x = particles[i].x;
                vertices[i].y = particles[i].y;
                vertices[i].z = particles[i].z;
            }
            m_simMesh.vertices = vertices;
        }
    }

    void UpdateSkinMesh(ref GameObject model, ref Vector4[] particles)
    {
        if (particles != null)
        {
            m_skinMesh = model.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            Vector3[] vertices = m_skinMesh.vertices;
            for (int i = 0; i < m_cloVertArray.Length; i++)
            {
                particles[i].x = vertices[i].x;
                particles[i].y = vertices[i].y;
                particles[i].z = vertices[i].z;
            }
            //Debug.Log(particles[1000].z.ToString());
        }
    }

    public void SimInitialize(GameObject clothObj, GameObject obj, JsonData data)
    {
        clothObj_ = clothObj;
        obj_ = obj;
        Debug.Log(clothObj_.name + ":" + obj_.name);
        bool b = SimInit();
        Debug.Log(b.ToString());
        BuildSkinFromMesh(ref obj_);
        BuildClothFromMesh(ref clothObj_, data);
    }
    private void Awake()
    {
        Inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isParamsUpdate)
        {
            UpdateParams();
        }
        if (isSim)
        {
            if (m_skinID != -1 && m_skinVertArray != null)
            {
                UpdateSkinMesh(ref obj_, ref m_skinVertArray);
                SetSkinBuffer(m_skinID, ref m_skinVertArray[0]);
            }
            SimUpdate();
            //UpdateBuffer();
            if (m_cloID != -1 && m_cloVertArray != null)
            {
                GetClothBuffer(m_cloID, ref m_cloVertArray[0]);
                UpdateClothMesh(ref clothObj_, ref m_cloVertArray);
            }

        }

    }

    public GameObject Cloth;
    public GameObject Skin;

    Vector4[] m_cloVertArray = null;
    Vector4[] m_skinVertArray = null;
    List<Vector2Int> m_seamPairs = null;
    bool isParamsUpdate = false;
    public bool isSim = false;
    int m_cloID = -1;
    int m_skinID = -1;
    [SerializeField]
    protected Mesh m_simMesh = null;
    [SerializeField]
    protected Mesh m_skinMesh = null;
    [SerializeField]
    Vector3 m_meshLocalScale = Vector3.one;
    [SerializeField]
    int numIterations = 10;
    [SerializeField]
    float rigidThickness = 0.5f;
    [SerializeField]
    float selfThickness = 0.5f;
    [SerializeField]
    Vector3 m_Gravity = new Vector3(0.0f, -0.002f, 0.0f);
    [SerializeField]
    Vector3 m_Wind = new Vector3(0.19612f, 0, 0.98058f);
    [SerializeField]
    float dynamicFriction = 0.35f;
    [SerializeField]
    float staticFriction = 0.35f;
    [SerializeField]
    float springStiffness = 0.5f;
    [SerializeField]
    float bendingStiffness = 0.0f;

}
