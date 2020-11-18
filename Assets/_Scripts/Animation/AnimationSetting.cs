using System.Collections;
using System.Collections.Generic;
using TriLib;
using UniGLTF;
using UniHumanoid;
using UniJSON;
using UnityEngine;

public class AnimationSetting : MonoBehaviour
{
    public static AnimationSetting Single;
    public static AnimationSetting Inst {
        get {
            if (Single == null) {
                GameObject obj = new GameObject("AnimationSetting");
                Single = obj.AddComponent<AnimationSetting>();
                DontDestroyOnLoad(obj);             
            }
            return Single;
        }
    }

    public Dictionary<string, string> dir = new Dictionary<string, string>();
  
    private GameObject originals, smpls;
    public bool flag = false;
    // Start is called before the first frame update
    void Start()
    {

    
    }

    // Update is called once per frame
    void Update()
    {
        if (flag) {
            foreach (var OneItem in dir)
            {              
                // smpls.transform.Find(OneItem.Value).localRotation = originals.transform.Find(OneItem.Key).localRotation;
                Vector3 a = originals.transform.Find(OneItem.Key).eulerAngles;
                smpls.transform.Find(OneItem.Value).eulerAngles = new Vector3(-a.x, a.y, -a.z);                
                //smpls.transform.FindDeepChild(OneItem.Value).position = originals.transform.FindDeepChild(OneItem.Key).position; 
            }
        }
    }
    public void ResetAnimation(GameObject original,GameObject smpl) {
        dir = new Dictionary<string, string>()
     {
        { "/male_smpl/m_avg_root" ,"/parentPos/RootNode/Armature/Root"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis" ,"/parentPos/RootNode/Armature/Root/Hips"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_L_Hip" ,"/parentPos/RootNode/Armature/Root/Hips/LeftUpleg"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_L_Hip/m_avg_L_Knee" ,"/parentPos/RootNode/Armature/Root/Hips/LeftUpleg/LeftLeg"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_L_Hip/m_avg_L_Knee/m_avg_L_Ankle" ,"/parentPos/RootNode/Armature/Root/Hips/LeftUpleg/LeftLeg/LeftFoot"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_L_Hip/m_avg_L_Knee/m_avg_L_Ankle/m_avg_L_Foot" ,"/parentPos/RootNode/Armature/Root/Hips/LeftUpleg/LeftLeg/LeftFoot/LeftToes"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_R_Hip" ,"/parentPos/RootNode/Armature/Root/Hips/RightUpLeg"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_R_Hip/m_avg_R_Knee" ,"/parentPos/RootNode/Armature/Root/Hips/RightUpLeg/RightLeg"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_R_Hip/m_avg_R_Knee/m_avg_R_Ankle" ,"/parentPos/RootNode/Armature/Root/Hips/RightUpLeg/RightLeg/RightFoot"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_R_Hip/m_avg_R_Knee/m_avg_R_Ankle/m_avg_R_Foot" ,"/parentPos/RootNode/Armature/Root/Hips/RightUpLeg/RightLeg/RightFoot/RightToes"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1" ,"/parentPos/RootNode/Armature/Root/Hips/Spine"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar/m_avg_L_Shoulder" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder/LeftArm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar/m_avg_L_Shoulder/m_avg_L_Elbow" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder/LeftArm/LeftForeArm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar/m_avg_L_Shoulder/m_avg_L_Elbow/m_avg_L_Wrist" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder/LeftArm/LeftForeArm/LeftHand"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar/m_avg_L_Shoulder/m_avg_L_Elbow/m_avg_L_Wrist/m_avg_L_Hand" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LeftPalm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar/m_avg_R_Shoulder" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder/RightArm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar/m_avg_R_Shoulder/m_avg_R_Elbow" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder/RightArm/RightForeArm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar/m_avg_R_Shoulder/m_avg_R_Elbow/m_avg_R_Wrist" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder/RightArm/RightForeArm/RightHand"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar/m_avg_R_Shoulder/m_avg_R_Elbow/m_avg_R_Wrist/m_avg_R_Hand" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder/RightArm/RightForeArm/RightHand/RightPalm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_Neck" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/Neck"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_Neck/m_avg_Head" ,"/parentPos/RootNode/Armature/Root/Hips/Spine/Chest/UpperChest/Neck/Head"}
     };
        originals = original;
        smpls = smpl;
        //Transform[] test1 = original.GetComponentsInChildren<Transform>();
        //Transform[] test2 = smpl.GetComponentsInChildren<Transform>();
        //Debug.Log("test1：" + test1.Length + "test2:" + test2.Length);
        //for (int i = 0;i<test1.Length; i++) {
        //    dir.Add(test1[i].name ,test2[i].name);
        //    Debug.Log(test1[i].name +"  :  "+ test2[i].name);
        //}       
        flag = true;
        originals.GetComponent<Animator>().enabled = true;
        Debug.Log(dir.Count);
    }
    private void OnGUI()
    {
       
    }
}
