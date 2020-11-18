using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;
using MathNet;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;

public class CreateMesh : MonoBehaviour
{
    public static CreateMesh Single;
    public static CreateMesh Inst
    {
        get
        {
            if (Single == null)
            {
                GameObject obj = new GameObject("CreateMesh");
                DontDestroyOnLoad(obj);
                Single = obj.AddComponent<CreateMesh>();
            }
            return Single;
        }
    }
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles; 
    Mesh m;
    JsonData data;
    int morph_count = 21;  
    int bs_count = 10;
    DenseVector morph_mu;
    DenseMatrix morph_cov;
    DenseMatrix morph_semantic_trans;
    DenseMatrix blend_shapes;
    // Start is called before the first frame update
    void Start()
    {     
        //cont = new control  //生成人体模拟数据
        //{
        //    body_height = 150,
        //    body_weight = 90,
        //    chest_girth = 0.83f,
        //    belly_girth = 0.69f,
        //    hip_girth = 0.91f,
        //    shoulder_width = 0.41f,
        //    trouser_length = 0.77f
        //};
        //  UtilityJson();   //生成数据  
        //setHeightWeight();  //
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //初始化标模json数据
    public void UtilityJson()
    {
        //string str = File.ReadAllText(Application.dataPath + "/easy_data_female1.json");
        string str = File.ReadAllText(Application.streamingAssetsPath + "/easy_data_female1.json");
        Debug.Log(str);
        data = JsonMapper.ToObject(str);      
            newVertices = new Vector3[data["vertices"].Count];
            for (int i = 0; i < data["vertices"].Count; i++)
            {
                newVertices[i].x = float.Parse(data["vertices"][i][0].ToString());
                newVertices[i].y = float.Parse(data["vertices"][i][1].ToString());
                newVertices[i].z = -float.Parse(data["vertices"][i][2].ToString());
                //  Debug.Log(newVertices[i].x);
            }
            morph_mu = new DenseVector(morph_count);
            // mu = new double[data["mu"].Count];
            for (int i = 0; i < data["mu"].Count; i++)
            {
                morph_mu[i] = (double)data["mu"][i];
                //  Debug.Log(mu[i].ToString());
            }
            //计算cov
            morph_cov = new DenseMatrix(morph_count, morph_count);
            for (int i = 0; i < data["cov"].Count; i++)
            {
                for (int j = 0; j < data["cov"][i].Count; j++)
                {
                    morph_cov[i, j] = (double)data["cov"][i][j];
                    // Debug.Log(cov_[j].ToString());
                }
            }
            //计算semantic_trans
            morph_semantic_trans = new DenseMatrix(bs_count, morph_count);
            for (int i = 0; i < data["semantic_trans"].Count; i++)
            {
                double[] mantic_ = new double[data["semantic_trans"][i].Count];
                for (int j = 0; j < data["semantic_trans"][i].Count; j++)
                {
                    mantic_[j] = (double)data["semantic_trans"][i][j];
                    morph_semantic_trans[i, j] = (double)data["semantic_trans"][i][j];
                }
            }
            //计算blend_shapes
            blend_shapes = new DenseMatrix(bs_count, newVertices.Count() * 3);
            for (int i = 0; i < data["blend_shapes"].Count; i++)
            {
                double[] mantic_ = new double[data["blend_shapes"][i].Count];
                for (int j = 0; j < data["blend_shapes"][i].Count; j++)
                {
                    mantic_[j] = (double)data["blend_shapes"][i][j];
                    blend_shapes[i, j] = (double)data["blend_shapes"][i][j];
                }
            }          
                Debug.Log(newVertices.Count());
                CreateMeshObj(newVertices);
           
    }
    //更新mesh网格到物体上
    public void CreateMeshObj(Vector3[] vertices)
    {
        Mesh mesh  = MainUI.smpleModel.transform.Find("node_easy_model").GetComponent<SkinnedMeshRenderer>().sharedMesh; 
      //  Mesh mesh = GameObject.FindGameObjectWithTag("obj").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        newTriangles = mesh.triangles;
        newUV = mesh.uv;
        m = new Mesh
        {
            vertices = vertices,
            triangles = newTriangles,
            uv = newUV
        };
       
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.RecalculateTangents();
        MainUI.smpleModel.transform.Find("node_easy_model").GetComponent<SkinnedMeshRenderer>().sharedMesh = m;
    }
   
    DenseVector getBlendWeights(DenseVector coeff)
    {
        var gamma = coeff - morph_mu;
        DenseVector beta = morph_semantic_trans * gamma;
        return beta;
    }

    void setMorph(DenseVector coeff)
    {
        DenseVector beta = getBlendWeights(coeff);
        DenseVector beta_ = beta * blend_shapes;
        Vector3[] newv1 = new Vector3[newVertices.Count()];
        for(int i = 0;i<newVertices.Count();i++) {
            newv1[i] = newVertices[i] + new Vector3((float)beta_[i*3], (float)beta_[i * 3 +1], -(float)beta_[i * 3 + 2]);              
        }
        //改变网格信息
          CreateMeshObj(newv1);        
        //TODO setBs()
    }
    DenseVector getBodyCoeffAll(Dictionary<int,float> coeffIn) {
        if (coeffIn.Count == 0)
        {
            coeffIn.Add(0, (float)UnityViewManager.slider_bodyheight.value * 0.01f);
            coeffIn.Add(1, (float)UnityViewManager.slider_bodyweight.value);
            coeffIn.Add(2, (float)UnityViewManager.slider_chestgirth.value);
            coeffIn.Add(3, (float)UnityViewManager.slider_bellygirth.value);
            coeffIn.Add(4, (float)UnityViewManager.slider_hipgirth.value);
            coeffIn.Add(5, (float)UnityViewManager.slider_shoulderwith.value);
            coeffIn.Add(7, (float)UnityViewManager.slider_trouserlength.value);
        }
        List<int> l_s = new List<int>();
        List<int> l_h = new List<int>();
        List<bool> l = Enumerable.Repeat(false, morph_count).ToList();
        var keyColl = coeffIn.Keys;
        foreach (int i in keyColl)
        {
            l_s.Add(i);
            l[i] = true;
        }

        for (int i = 0; i < morph_count; ++i)
        {
            if (l[i] == false)
            {
                l_h.Add(i);
            }
        }

        var S_ss = new DenseMatrix(l_s.Count, l_s.Count);
        var S_hs = new DenseMatrix(l_h.Count, l_s.Count);
        var x_s = new DenseVector(l_s.Count);
        var m_s = new DenseVector(l_s.Count);
        var m_h = new DenseVector(l_h.Count);

        for (int i = 0; i < l_s.Count; ++i)
        {
            x_s[i] = coeffIn[l_s[i]];
            m_s[i] = morph_mu[l_s[i]];
        }
        for (int i = 0; i < l_h.Count; ++i)
        {
            m_h[i] = morph_mu[l_h[i]];
        }
        for (int i = 0; i < l_s.Count; ++i)
        {
            for (int j = 0; j < l_s.Count; ++j)
            {
                S_ss[i, j] = morph_cov[l_s[i],l_s[j]];
            }
        }
        for (int i = 0; i < l_h.Count; ++i)
        {
            for (int j = 0; j < l_s.Count; ++j)
            {
                S_hs[i, j] = morph_cov[l_h[i],l_s[j]];
            }
        }

        var x_h = m_h + S_hs * (S_ss.Inverse()) * (x_s - m_s);

        var coeffOut = new DenseVector(morph_count);
        //List<float> coeffOut = Enumerable.Repeat(0.0, morph_count).ToList();
        for (int i = 0; i < l_h.Count; ++i)
        {
            coeffOut[l_h[i]] = x_h[i];
        }
        for (int i = 0; i < l_s.Count; ++i)
        {
            coeffOut[l_s[i]] = x_s[i];
        }
        //for(int i = 0; i < coeffOut.Count; ++i)
        //{
        //	coeffOut[i] = coeffOut[i]-morph_mu[i];
        //}
        return coeffOut;
    }
    
    //身高体重输入时触发
   public  void setHeightWeight()
    {
        
        Dictionary<int, float> coeffIn = new Dictionary<int, float>();
        coeffIn.Add(0,  (float)UnityViewManager.slider_bodyheight.value * 0.01f);
        coeffIn.Add(1, (float)UnityViewManager.slider_bodyweight.value );
        var coeffAll = getBodyCoeffAll(coeffIn);
        setMorph(coeffAll);
        //todo
        UnityViewManager.slider_chestgirth.value = (float)(coeffAll[2]);
        UnityViewManager.slider_chestgirth_text.text = ((float)(coeffAll[2])).ToString();
        UnityViewManager.slider_bellygirth.value = (float)(coeffAll[3] );
        UnityViewManager.slider_bellygirth_text.text = ((float)(coeffAll[3])).ToString();
        UnityViewManager.slider_hipgirth.value = (float)(coeffAll[4]);
        UnityViewManager.slider_hipgirth_text.text = ((float)(coeffAll[4])).ToString();
        UnityViewManager.slider_shoulderwith.value  = (float)(coeffAll[5]);
        UnityViewManager.slider_shoulderwith_text.text = ((float)(coeffAll[5])).ToString();
        UnityViewManager.slider_trouserlength.value  = (float)(coeffAll[7]);
        UnityViewManager.slider_trouserlength_text.text = ((float)(coeffAll[7])).ToString();
    }
    //胸围输入时触发
  public  void setChestGirth()
    {
        Dictionary<int, float> coeffIn = new Dictionary<int, float>();
        coeffIn.Add(0, (float)UnityViewManager.slider_bodyheight.value * 0.01f);
        coeffIn.Add(1, (float)UnityViewManager.slider_bodyweight.value);
        coeffIn.Add(2, (float)UnityViewManager.slider_chestgirth.value);
        coeffIn.Add(7, (float)UnityViewManager.slider_trouserlength.value);
        var coeffAll = getBodyCoeffAll(coeffIn);
        setMorph(coeffAll);
        //to
        UnityViewManager.slider_bellygirth.value = (float)(coeffAll[3]);
        UnityViewManager.slider_hipgirth.value = (float)(coeffAll[4] );
        UnityViewManager.slider_shoulderwith.value  = (float)(coeffAll[5] );

        UnityViewManager.slider_bellygirth_text.text = ((float)(coeffAll[3])).ToString();
        UnityViewManager.slider_hipgirth_text.text = ((float)(coeffAll[4])).ToString();
        UnityViewManager.slider_shoulderwith_text.text = ((float)(coeffAll[5])).ToString();
    }

    //腰围输入时触发
    public void setBellyGirth()
    {
        Dictionary<int, float> coeffIn = new Dictionary<int, float>();
        coeffIn.Add(0, (float)UnityViewManager.slider_bodyheight.value* 0.01f);
        coeffIn.Add(1, (float)UnityViewManager.slider_bodyweight.value);
        coeffIn.Add(3, (float)UnityViewManager.slider_bellygirth.value );
        coeffIn.Add(7, (float)UnityViewManager.slider_trouserlength.value );
        var coeffAll = getBodyCoeffAll(coeffIn);
        setMorph(coeffAll);
        //todo
        UnityViewManager.slider_chestgirth.value  = (float)(coeffAll[2] );
        UnityViewManager.slider_hipgirth.value = (float)(coeffAll[4] );
        UnityViewManager.slider_shoulderwith.value = (float)(coeffAll[5] );

        UnityViewManager.slider_chestgirth_text.text = ((float)(coeffAll[2])).ToString(); 
        UnityViewManager.slider_hipgirth_text.text = ((float)(coeffAll[4])).ToString(); 
        UnityViewManager.slider_shoulderwith_text.text = ((float)(coeffAll[5])).ToString(); 
    }
    //臀围输入时触发
    public void setHipGirth()
    {
        Dictionary<int, float> coeffIn = new Dictionary<int, float>();
        coeffIn.Add(0, (float)UnityViewManager.slider_bodyheight.value  * 0.01f);
        coeffIn.Add(1, (float)UnityViewManager.slider_bodyweight.value );
        coeffIn.Add(4, (float)UnityViewManager.slider_hipgirth.value );
        coeffIn.Add(7, (float)UnityViewManager.slider_trouserlength.value);
        var coeffAll = getBodyCoeffAll(coeffIn);
        setMorph(coeffAll);
        UnityViewManager.slider_chestgirth.value  = (float)(coeffAll[2] ) ;
        UnityViewManager.slider_bellygirth.value  = (float)(coeffAll[3] ) ;
        UnityViewManager.slider_shoulderwith.value  = (float)(coeffAll[5] ) ;

        UnityViewManager.slider_chestgirth_text.text = ((float)(coeffAll[2])).ToString();
        UnityViewManager.slider_bellygirth_text.text = ((float)(coeffAll[3])).ToString();
        UnityViewManager.slider_shoulderwith_text.text = ((float)(coeffAll[5])).ToString();
    }

    //肩宽输入时触发
    public void setShoulderWidth()
    {
        Dictionary<int, float> coeffIn = new Dictionary<int, float>();
        coeffIn.Add(0, (float)UnityViewManager.slider_bodyheight.value  * 0.01f);
        coeffIn.Add(1, (float)UnityViewManager.slider_bodyweight.value );
        coeffIn.Add(5, (float)UnityViewManager.slider_shoulderwith.value );
        coeffIn.Add(7, (float)UnityViewManager.slider_trouserlength.value );
        var coeffAll = getBodyCoeffAll(coeffIn);
        setMorph(coeffAll);
        UnityViewManager.slider_chestgirth.value = (float)(coeffAll[2] );
        UnityViewManager.slider_bellygirth.value  = (float)(coeffAll[3] );
        UnityViewManager.slider_hipgirth.value  = (float)(coeffAll[4] );

        UnityViewManager.slider_chestgirth_text.text = ((float)(coeffAll[2])).ToString();
        UnityViewManager.slider_bellygirth_text.text = ((float)(coeffAll[3])).ToString();
        UnityViewManager.slider_hipgirth_text.text = ((float)(coeffAll[4])).ToString();
    }
    //腿长（裤长）输入时触发
    public void setTrouserLength()
    {
        Dictionary<int, float> coeffIn = new Dictionary<int, float>();
        coeffIn.Add(0, (float)UnityViewManager.slider_bodyheight.value  * 0.01f);
        coeffIn.Add(1, (float)UnityViewManager.slider_bodyweight.value );
        coeffIn.Add(5, (float)UnityViewManager.slider_shoulderwith.value );
        coeffIn.Add(7, (float)UnityViewManager.slider_trouserlength.value );
        var coeffAll = getBodyCoeffAll(coeffIn);
        setMorph(coeffAll);
        UnityViewManager.slider_chestgirth.value  = (float)(coeffAll[2] ) ;
        UnityViewManager.slider_bellygirth.value  = (float)(coeffAll[3] ) ;
        UnityViewManager.slider_hipgirth.value = (float)(coeffAll[4] );

        UnityViewManager.slider_chestgirth_text.text = ((float)(coeffAll[2])).ToString();
        UnityViewManager.slider_bellygirth_text.text = ((float)(coeffAll[3])).ToString();
        UnityViewManager.slider_hipgirth_text.text = ((float)(coeffAll[4])).ToString();
    }

    public string Getshapeparameter() {
        Dictionary<int, float> dir = new Dictionary<int, float>();
        dir.Add(0, (float)UnityViewManager.slider_bodyheight.value * 0.01f);
        dir.Add(1, (float)UnityViewManager.slider_bodyweight.value);
        dir.Add(2, (float)UnityViewManager.slider_chestgirth.value);
        dir.Add(3, (float)UnityViewManager.slider_bellygirth.value);
        dir.Add(4, (float)UnityViewManager.slider_hipgirth.value);
        dir.Add(5, (float)UnityViewManager.slider_shoulderwith.value);
        dir.Add(7, (float)UnityViewManager.slider_trouserlength.value);
        string json = JsonConvert.SerializeObject(dir);
        Debug.Log(json);
        return json;
    }
}












[Serializable]
public class control
{
    /// <summary>
    /// 身高
    /// </summary>
    public float body_height;
    /// <summary>
    /// 体重
    /// </summary>
    public float body_weight;
    /// <summary>
    /// 胸围
    /// </summary>
    public float chest_girth;
    /// <summary>
    /// 腰围
    /// </summary>
    public float belly_girth;
    /// <summary>
    /// 臀围
    /// </summary>
    public float hip_girth;
    /// <summary>
    /// 肩宽
    /// </summary>
    public float shoulder_width;
    /// <summary>
    /// 裤长
    /// </summary>
    public float trouser_length; 

}



