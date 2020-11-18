using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Windows.Forms;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using FairyGUI;
using System.Runtime.InteropServices;
using TriLib;
using LitJson;
using System.Linq;
using TressFX;

public class MainUI : MonoBehaviour
{
    public delegate void LoadclothEvent(GameObject obj, bool flag);
    public LoadclothEvent loadclothevent;
    [DllImport("Dll2")]
    private static extern int add(int a, int b);
    public GameObject targetPos;
    public GameObject parentPos;    //下载的模型父节点
    private bool startFlag = false;
    public GameObject bg;
    public GameObject scene;
    public GameObject SmplInitPos;
    public static GameObject smpleModel;
    public Avatar avatar;
    public RuntimeAnimatorController ani;
    private JsonData clothJsondata;
    private Dictionary<string, GameObject> clothList = new Dictionary<string, GameObject>();
    private struct FaceData {
        //public string obj_path;
        public string fbx_texture;
        public string result_path;
        public string fbx_path;
        public string cloth_data;
        public bool success;
    }
   
    // Start is called before the first frame update
    void Start()
    {                   
        Debug.Log(ConstantValue.filePathLocal);       
        InitializationData();             
    }
    // Update is called once per frame
    void Update()
    {      
        if (startFlag) {
            Camera.main.transform.position = Vector3.Lerp(this.transform.position, targetPos.transform.position, Time.deltaTime * 1.0f);
            if (Vector3.Distance(Camera.main.transform.position, targetPos.transform.position) < 0.1f) {
                startFlag = false;
                UnityViewManager.chooseCon.SetSelectedIndex(1);
                Camera.main.GetComponent<CameraCtrl>().enabled = true;
            }
        }
    }
    //选择本地图片
    public void ChooseLocalPhoto()
    {
        OpenFileDialog od = new OpenFileDialog();
        od.Title = "请选择头像图片";
        od.Multiselect = false;
        od.Filter = "图片文件(*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp";
        if (od.ShowDialog() == DialogResult.OK)
        {
            Debug.Log(od.FileName);
            UnityViewManager.LoadingCom.visible = true;
            //      LoadImageFile("file://F:/zengjun/image.jpg", "image");
            LoadImageFile("file://" + od.FileName, od.FileName); //读取图片
        }
    }
    //读取本地PC图片发送服务器   
    void LoadImageFile(string url, string photoName) {
        FiledownloadHelper.Inst.GetTexture(url, null, (texture2d, bytes, flag) => {
            if (flag)
            {
                Debug.Log("读取成功");
                NTexture texture = new NTexture(texture2d);
                UnityViewManager.chooseimageBtn.visible = false;    //选择图片按钮隐藏
                UnityViewManager.imageIcon.texture = texture;       //赋值图片到imageIcon上
                UnityViewManager.bgCon.SetSelectedIndex(1);         //背景图消失                                                      //   byte[] bytes = texture2d.EncodeToJPG();
                Debug.Log(bytes.Length.ToString());
                string url_ = File.ReadAllText(ConstantValue.ip);
                // 上传图片到服务器
                FiledownloadHelper.Inst.UpLoadFile(url_+ "/faceconstruct", bytes, GetCurTime(), (flag1, text) =>
                {
                    if (flag1)
                    {
                        Debug.Log("上传成功:" + text);
                        FaceData f = JsonUtility.FromJson<FaceData>(text);
                        Debug.Log("obj_texture:" + f.fbx_texture);
                        //Debug.Log("obj_path:" + f.obj_path);
                        Debug.Log("fbx_path:" + f.fbx_path);
                        Debug.Log("cloth_data:"+ f.cloth_data);
                        clothJsondata = JsonMapper.ToObject(f.cloth_data);
                      //  PlayerPrefs.SetString("cloth_data", f.cloth_data);
                        UnityViewManager.hairList.itemRenderer = HairItemRender;
                        UnityViewManager.hairList.numItems = clothJsondata["hair"].Count;
                        UnityViewManager.hairList.onClickItem.Set(HairItemClick);
                        UnityViewManager.clothList.itemRenderer = ClothItemRender;
                        UnityViewManager.clothList.numItems = clothJsondata["suit"].Count;
                        UnityViewManager.clothList.onClickItem.Set(ClothItemClick);
                        UnityViewManager.upclothList.itemRenderer = UpClothItemRender;
                        UnityViewManager.upclothList.numItems = clothJsondata["coat"].Count;
                        UnityViewManager.upclothList.onClickItem.Set(UpClothItemClick);
                        UnityViewManager.downclothList.itemRenderer = DownClothItemRender;
                        UnityViewManager.downclothList.numItems = clothJsondata["pants"].Count;
                        UnityViewManager.downclothList.onClickItem.Set(DownClothItemClick);
                        //设置标模滑动条数据       
                        FiledownloadHelper.Inst.GetTexture(f.fbx_texture, null, (image, bytes1, flag2) =>
                        {
                            if (flag2)
                            {                                
                              //AssetDownloader.Inst.AssetURI = f.obj_path;  //赋值obj模型地址给TriLib插件
                                AssetDownloader.Inst.AssetURI = f.fbx_path;  //赋值fbx模型地址给TriLib插件
                                AssetDownloader.Inst.AssetExtension = ".fbx";
                                AssetDownloader.Inst.WrapperGameObject = parentPos;
                                AssetDownloader.Inst.Async = true;                               
                                Action<bool> onLoadfbx = (loadSuccess) =>
                                  {
                                      if (loadSuccess)
                                      {
                                          Debug.Log("fbx加载成功");
                                          foreach (var a in parentPos.GetComponentsInChildren<SkinnedMeshRenderer>())
                                          {
                                              Debug.Log(a.name);
                                              a.material = CreateMaterial(image);                   //获取材质球                                      
                                              UnityViewManager.LoadingCom.visible = false;
                                              scene.SetActive(true);                                //显示场景
                                              bg.SetActive(false);                                  //隐藏背景地图 
                                              startFlag = true;                                     //开始位移相机 
                                              //Todo  缩小模型的网格                                                                                                          
                                                AnimationSetting.Inst.ResetAnimation(GameObject.FindGameObjectWithTag("originalmodel"), parentPos); //添加动画
                                          }
                                          string standpose = File.ReadAllText(UnityEngine.Application.streamingAssetsPath + "/standpose.txt");
                                        
                                          JsonData data = JsonMapper.ToObject(standpose);
                                          Vector3[] stand = new Vector3[data["standpose"].Count];
                                          for (int i = 0; i< data["standpose"].Count; i++) {
                                              stand[i].x =  float.Parse(data["standpose"][i][0].ToString());
                                              stand[i].y = float.Parse(data["standpose"][i][1].ToString());
                                              stand[i].z = float.Parse(data["standpose"][i][2].ToString());
                                          }
                                        
                                             
                                                Debug.Log(stand.Count());
                                                Transform[] bones = parentPos.GetComponentsInChildren<SkinnedMeshRenderer>()[0].bones;
                                                Debug.Log(bones[0].name + bones.Count());                                              
                                                for (int i = 0; i < stand.Count(); i++)
                                                  {
                                                  //  Debug.Log(i+":"+bones[i].name +":"+ stand[i].z);
                                                    bones[i].localEulerAngles = stand[i];                                              
                                                  }                                             
                                      }
                                      else {
                                          Debug.Log("fbx加载失败");
                                          UnityViewManager.LoadingCom.visible = false;
                                      }                                    
                                  };
                               // AssetDownloader.Inst.actt = onLoadfbx;
                                if (AssetDownloader.Inst.AutoStart && !string.IsNullOrEmpty(AssetDownloader.Inst.AssetURI) && !string.IsNullOrEmpty(AssetDownloader.Inst.AssetExtension))
                                {
                                    AssetLoaderOptions assetLoaderOptions = (AssetLoaderOptions)ScriptableObject.CreateInstance("AssetLoaderOptions");
                                    assetLoaderOptions.Scale = 5f;                                  
                                    assetLoaderOptions.RotationAngles = new Vector3(0,0,0);
                                    assetLoaderOptions.Avatar = avatar;
                                    assetLoaderOptions.AnimationWrapMode = WrapMode.Loop;
                                    assetLoaderOptions.AnimatorController = ani;                                   
                                    AssetDownloader.Inst.DownloadAsset(AssetDownloader.Inst.AssetURI, AssetDownloader.Inst.AssetExtension, null, null, assetLoaderOptions, AssetDownloader.Inst.WrapperGameObject, AssetDownloader.Inst.ProgressCallback, onLoadfbx);
                                } 
                                GameObject.FindGameObjectWithTag("originalmodel").GetComponent<Animator>().SetTrigger("bow");
                                //   NTexture t = new NTexture(image);
                                //   UnityViewManager.imageIcon.texture = t;
                                //   保存图片到本地
                                FiledownloadHelper.Inst.SaveAssetLocalFile(ConstantValue.filePathLocal, GetCurTime() + ".jpg", image.EncodeToJPG(), image.EncodeToJPG().Length);
                            }
                            else
                            {
                                Debug.Log("返回的图片读取失败");
                                UnityViewManager.LoadingCom.visible = false;
                            }
                        });
                    }
                    else
                    {
                        Debug.Log("上传失败:" + text);
                        UnityViewManager.LoadingCom.visible = false;
                    }
                });
            }
        });
    }
    IEnumerator ResourcesLoadAsync(string path)
    {
        ResourceRequest request = Resources.LoadAsync(path);
        yield return request;
        if (request != null) {
            if (request.isDone) {
                GameObject obj = request.asset as GameObject;
                  smpleModel = Instantiate(obj,SmplInitPos.transform.position,SmplInitPos.transform.rotation);
                  smpleModel.transform.localScale = SmplInitPos.transform.localScale;
            }
        }
    }
    //初始化数据
    public void InitializationData() {
        UnityViewManager.Inst("Home");
        UnityViewManager.sureGender.onClick.Set(()=> {
            int index = UnityViewManager.genderCon.selectedIndex;
            Debug.Log(index.ToString());
            if (index == 0)    //0:男性  1:女性
            {
                PlayerPrefs.SetString("gender", "male");
            }else {
                PlayerPrefs.SetString("gender", "female");
            }
            UnityViewManager.GenderCom.visible = false;
            UnityViewManager.sliderCon.SetSelectedIndex(1);          
            smpleModel = Instantiate(Resources.Load<GameObject>("model/4138"),SmplInitPos.transform.position,SmplInitPos.transform.rotation);
            smpleModel.transform.localScale = SmplInitPos.transform.localScale;
            PlayerPrefs.SetFloat("yuanshi",smpleModel.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.bounds.size.y);           
            UnityViewManager.slider_bodyheight.value = 165;
            UnityViewManager.slider_bodyweight.value = 50;
            UnityViewManager.slider_chestgirth.value = 0.9f;
            UnityViewManager.slider_bellygirth.value = 0.7f;
            UnityViewManager.slider_hipgirth.value = 0.9f;
            UnityViewManager.slider_shoulderwith.value = 0.4f;
            UnityViewManager.slider_trouserlength.value = 0.7f;
            changevalue();   //更新滑动条上的数值
            CreateMesh.Inst.UtilityJson();  //
            CreateMesh.Inst.setHeightWeight();              
        });
        UnityViewManager.chooseimageBtn.onClick.Set(() => {  //按钮设置监听
            ChooseLocalPhoto();
        });
        UnityViewManager.choosehairBtn.onClick.Set(() => {
            ChooselistId(4);
        });
        UnityViewManager.chooseclothBtn.onClick.Set(() => {
            ChooselistId(3);
        });
        UnityViewManager.chooseclothdownBtn.onClick.Set(() => {
            ChooselistId(2);
        });
        UnityViewManager.chooseclothupBtn.onClick.Set(() => {
            ChooselistId(1);
        });      
        UnityViewManager.slider_bodyheight.onChanged.Add(() => {
            changevalue();          
            CreateMesh.Inst.setHeightWeight();
        });
        UnityViewManager.slider_bodyweight.onChanged.Set(() => {
            changevalue();           
            CreateMesh.Inst.setHeightWeight();
        });
        UnityViewManager.slider_chestgirth.onChanged.Set(() => {
            changevalue();          
            CreateMesh.Inst.setChestGirth();
        });
        UnityViewManager.slider_bellygirth.onChanged.Set(() => {
            changevalue();           
            CreateMesh.Inst.setBellyGirth();
        });
        UnityViewManager.slider_hipgirth.onChanged.Set(() => {
            changevalue();            
            CreateMesh.Inst.setHipGirth();
        });
        UnityViewManager.slider_shoulderwith.onChanged.Set(() => {
            changevalue();           
            CreateMesh.Inst.setShoulderWidth();
        });
        UnityViewManager.slider_trouserlength.onChanged.Set(() => {
            changevalue();          
            CreateMesh.Inst.setTrouserLength();
        });

        //确定选择的体型
        UnityViewManager.sureSize.onClick.Set(()=> {
            //发送数据给服务器
            PlayerPrefs.SetString("vertex",GetMeshVertices(smpleModel.transform.Find("node_easy_model").gameObject));
            PlayerPrefs.SetString("coeff",CreateMesh.Inst.Getshapeparameter());
            UnityViewManager.sliderCon.SetSelectedIndex(0);
            UnityViewManager.chooseimageBtn.visible = true;

            Mesh m = smpleModel.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            Vector3[] oldVertices = m.vertices;
            Vector3[] v = new Vector3[m.vertices.Count()];
            float[] vv = new float[m.vertices.Count()];
            for (int q = 0; q < oldVertices.Count(); q++)
            {
                //  Debug.Log(oldVertices[q].y);
                vv[q] = oldVertices[q].y;
            }
            float a1 = vv[0];
            for (int j = 0; j < vv.Count(); j++)
            {
                if (a1 > vv[j])
                {
                    a1 = vv[j];
                }
            }
            float a = m.bounds.size.y;
            Debug.Log(m.bounds.size.y);
            Debug.Log(a1);
            PlayerPrefs.SetFloat("minY",a1);
            parentPos.transform.position = new Vector3(parentPos.transform.position.x, parentPos.transform.position.y + (smpleModel.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.bounds.size.y / 2 - PlayerPrefs.GetFloat("yuanshi")/2), parentPos.transform.position.z);
            Destroy(smpleModel);
        });
        //
        UnityViewManager.bowAni_Btn.onClick.Set(()=> {
            GameObject.FindGameObjectWithTag("originalmodel").GetComponent<Animator>().SetTrigger("bow");
        });
        UnityViewManager.walkAni_Btn.onClick.Set(()=> {
            GameObject.FindGameObjectWithTag("originalmodel").GetComponent<Animator>().SetTrigger("walk");
        });
        UnityViewManager.runAni_Btn.onClick.Set(()=> {
            GameObject.FindGameObjectWithTag("originalmodel").GetComponent<Animator>().SetTrigger("run");
        });
    }
    public void HairItemRender(int index,GObject obj) {
        GButton btn = obj.asButton;
        btn.icon = clothJsondata["hair"][index]["png_url"].ToString();       
    }
    public void ClothItemRender(int index, GObject obj)
    {
        GButton btn = obj as GButton;    
        btn.icon = clothJsondata["suit"][index]["png_url"].ToString();       
    }
    public void UpClothItemRender(int index, GObject obj)
    {
        GButton btn = obj as GButton;     
        btn.icon = clothJsondata["coat"][index]["png_url"].ToString();
    }
    public void DownClothItemRender(int index, GObject obj)
    {
        GButton btn = obj as GButton;     
        btn.icon = clothJsondata["pants"][index]["png_url"].ToString();       
    }
    public void HairItemClick(EventContext eventContext) {
        GButton btn = (GButton)eventContext.data;
        int num = UnityViewManager.hairList.selectedIndex;
        Debug.Log(clothJsondata["hair"][num]["model_url"]);
        //TODO  头发处理待定

        GameObject hairObj = GameObject.FindGameObjectWithTag("hair");
        Transform[] tran = parentPos.GetComponentsInChildren<Transform>();
        foreach (var v in tran) {
            if (v.name.Equals("Head")) {

                hairObj.transform.position = new Vector3(v.transform.position.x, v.transform.position.y, hairObj.transform.position.z);


                hairObj.transform.parent = v.parent;
               
            }
        }
       
        GameObject.FindGameObjectWithTag("hair1").GetComponent<AvatarHair>().enabled = true;
    }

    //根据体型处理衣服形变并初始化
    public void ProcessClothMesh(string cloth_type,int cloth_id) {
        UnityViewManager.LoadingCom.visible = true;
        FiledownloadHelper.Inst.GetAssetBundle(clothJsondata[cloth_type][cloth_id]["model_url"].ToString(),null,(clothObj,f)=>{
            if (f){
                Debug.Log("加载模型完成");
                FiledownloadHelper.Inst.DownLoadCloth(cloth_type, (cloth_id + 1).ToString(), (flag, text) => {
                    if (flag)
                    {
                        Debug.Log(text);
                        File.WriteAllText(UnityEngine.Application.dataPath + "/衣服.txt", text);
                        text = text.Replace("\"[[", "[[").Replace("]]\"", "]]").Replace("\"[", "[").Replace("]\"", "]");                      
                        JsonData data = JsonMapper.ToObject(text);
                        bool onSuccess = (bool)data["success"];
                        if (onSuccess)
                        {
                            Debug.Log("拿到数据");
                            Vector3[] tempvertices = new Vector3[data["cloth_vertex"].Count];  //衣服形变后原始顶点数据
                            for (int i = 0; i < data["cloth_vertex"].Count; i++)
                            {
                                tempvertices[i].x = float.Parse(data["cloth_vertex"][i][0].ToString());
                                tempvertices[i].y = float.Parse(data["cloth_vertex"][i][1].ToString()) - PlayerPrefs.GetFloat("minY");
                                tempvertices[i].z = -float.Parse(data["cloth_vertex"][i][2].ToString());   //z值需要取反 否则衣服法向是反的
                            }
                            int[] vertexMap = new int[data["mapdata"].Count];      //获取mapdata数据                                  
                            //for (int i = 0; i < data["mapdata"].Count; i++)
                            //{
                            //    Debug.Log(i);
                            //    vertexMap[i] = (int)data["mapdata"][i];
                            //}                            
                                    Debug.Log("开始初始化");                                   
                                    foreach (Transform t in parentPos.GetComponentsInChildren<Transform>())
                                    {
                                        if (t.name.Equals("Hips"))   //找到hip节点计算衣服y轴数值
                                        {
                                            Mesh objmesh = clothObj.GetComponentInChildren<MeshFilter>().sharedMesh;
                                            int meshCount = objmesh.vertices.Count();
                                            Debug.Log("unity模型长度" + objmesh.vertices.Count());
                                            Vector3[] newSample = new Vector3[objmesh.vertices.Count()];                                                                                                                                
                                            for (int i = 0; i < meshCount; i++)
                                            {
                                                newSample[i] = tempvertices[(int)data["mapdata"][i]];                                       
                                                //newSample[i] = tempvertices[vertexMap[i]];                                              
                                            }                                               
                                                           File.WriteAllText(UnityEngine.Application.dataPath + "/unity显示的模型顶点数据.txt", JsonUtil.ToJson(newSample));
                                                           objmesh.vertices = newSample;
                                                           objmesh.RecalculateNormals();
                                                           objmesh.RecalculateBounds();
                                                           objmesh.RecalculateTangents();
                                                           clothObj.GetComponentInChildren<MeshFilter>().sharedMesh = objmesh;
                                                           float hipOffset = 7.54264f;
                                                           float clothOffset = 0.436f;
                                                           float hip_y = t.localPosition.y;
                                                           float obj_y = ((hip_y - 91.43156f) * clothOffset / hipOffset) + 11.964f;
                                                           if (cloth_type.Equals("suit"))
                                                           {
                                                               foreach (string key in clothList.Keys)
                                                               {
                                                                   Destroy(clothList[key]);                                                                   
                                                               }
                                                               clothList.Remove("suit");
                                                           }
                                                           else {
                                                               if (clothList.ContainsKey("suit")) {
                                                                   Destroy(clothList["suit"]);
                                                                   clothList.Remove("suit");
                                                               }
                                                               if (clothList.ContainsKey(cloth_type)) {
                                                                   Destroy(clothList[cloth_type]);
                                                                   clothList.Remove(cloth_type);
                                                               }
                                                           }                                
                                                            clothObj = Instantiate(clothObj, new Vector3(parentPos.transform.position.x, parentPos.transform.position.y, parentPos.transform.position.z), t.rotation); //根据胸部高度匹配衣服位置
                                                            Debug.Log(JsonUtil.ToJson(clothObj.GetComponent<MeshFilter>().mesh.vertices));
                                                            clothObj.transform.localScale = new Vector3(5, 5, 5);                                                           
                                                            //  clothObj.transform.parent = parentPos.transform;                                    
                                                           //PhyUtils.Inst.SimInitialize(clothObj,parentPos, data);  // 添加衣服仿真
                                                           clothList.Add(cloth_type, clothObj);                                                          
                                                           UnityViewManager.LoadingCom.visible = false;
                                                           Debug.Log("衣服初始化成功");                                                                                                                                       
                                        }
                                    }
                        }
                        else
                        {
                            Debug.Log("衣服形变后的顶点数据获取失败");
                            UnityViewManager.LoadingCom.visible = false;
                        }
                    }
                    else
                    {
                        Debug.Log(text);
                        UnityViewManager.LoadingCom.visible = false;
                    }
                });
            }
            else {
                Debug.Log("衣服模型获取失败");
            }
        });
    }    
    public void UpClothItemClick(EventContext eventContext)
    {
        GButton btn = (GButton)eventContext.data;
        int num = UnityViewManager.upclothList.selectedIndex;
        Debug.Log("UpclothIndex:" + num.ToString());
        ProcessClothMesh("coat",num);
    }
    public void DownClothItemClick(EventContext eventContext)
    {
        GButton btn = (GButton)eventContext.data;
        int num = UnityViewManager.downclothList.selectedIndex;
        Debug.Log("DownIndex:" + num.ToString());
        ProcessClothMesh("pants", num);
    }
    public void ClothItemClick(EventContext eventContext)
    {
        GButton btn = (GButton)eventContext.data;
        int num = UnityViewManager.clothList.selectedIndex;
        Debug.Log("ClothIndex:" + num.ToString());
        ProcessClothMesh("suit", num);
    }
    void ChooselistId(int selectID) {
        UnityViewManager.listCon.SetSelectedIndex(selectID);
    }
    // 获取当前年月日时分秒，如201803081916
    private static string GetCurTime()
    {
        return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()
            + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
    }
    //生成材质球
    public Material CreateMaterial(Texture2D texture) {
        Material mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = texture;
        return mat;
    }
    //获取mesh顶点集合
    public string GetMeshVertices(GameObject obj)
    {
        Mesh mesh = obj.GetComponent<SkinnedMeshRenderer>().sharedMesh;       
        Vector3[] vertices = mesh.vertices;
        string json = JsonUtil.ToJson(vertices);
        Debug.Log(json);
        return json;       
    }
    //更新体型滑动条上方的数值
    public void changevalue() {     
            UnityViewManager.slider_bodyheight_text.text = ((int)UnityViewManager.slider_bodyheight.value).ToString();       
            UnityViewManager.slider_bodyweight_text.text = (UnityViewManager.slider_bodyweight.value).ToString();         
            UnityViewManager.slider_chestgirth_text.text = (UnityViewManager.slider_chestgirth.value).ToString();       
            UnityViewManager.slider_bellygirth_text.text = (UnityViewManager.slider_bellygirth.value).ToString();          
            UnityViewManager.slider_hipgirth_text.text = (UnityViewManager.slider_hipgirth.value).ToString();          
            UnityViewManager.slider_shoulderwith_text.text = (UnityViewManager.slider_shoulderwith.value).ToString();          
            UnityViewManager.slider_trouserlength_text.text = (UnityViewManager.slider_trouserlength.value).ToString();    
    }
    public void getmessage() {
        if (UnityViewManager.listCon.selectedIndex != 0) {
            UnityViewManager.listCon.SetSelectedIndex(0);
        }       
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "set"))
        {
          //  Debug.Log(parentPos.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.bounds.extents.y - 0.8255718);

           // Debug.Log(PlayerPrefs.GetFloat("minY"));
            //parentPos.transform.position = new Vector3(parentPos.transform.position.x,parentPos.transform.position.y - PlayerPrefs.GetFloat("minY")/3,parentPos.transform.position.z);
          //  parentPos.transform.position = new Vector3(parentPos.transform.position.x, parentPos.transform.position.y + (parentPos.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.bounds.size.y/2), parentPos.transform.position.z);
             PhyUtils.Inst.SimStart();

        }
        }
}
