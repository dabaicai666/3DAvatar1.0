using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTransfer : MonoBehaviour
{
    // correspondding bones
    Dictionary<string, string> bone_pair = new Dictionary<string, string>()
     {
        { "/male_smpl/m_avg_root" ,"Armature/Root"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis" ,"Armature/Root/Hips"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_L_Hip" ,"Armature/Root/Hips/LeftUpleg"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_L_Hip/m_avg_L_Knee" ,"Armature/Root/Hips/LeftUpleg/LeftLeg"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_L_Hip/m_avg_L_Knee/m_avg_L_Ankle" ,"Armature/Root/Hips/LeftUpleg/LeftLeg/LeftFoot"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_L_Hip/m_avg_L_Knee/m_avg_L_Ankle/m_avg_L_Foot" ,"Armature/Root/Hips/LeftUpleg/LeftLeg/LeftFoot/LeftToes"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_R_Hip" ,"Armature/Root/Hips/RightUpLeg"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_R_Hip/m_avg_R_Knee" ,"Armature/Root/Hips/RightUpLeg/RightLeg"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_R_Hip/m_avg_R_Knee/m_avg_R_Ankle" ,"Armature/Root/Hips/RightUpLeg/RightLeg/RightFoot"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_R_Hip/m_avg_R_Knee/m_avg_R_Ankle/m_avg_R_Foot" ,"Armature/Root/Hips/RightUpLeg/RightLeg/RightFoot/RightToes"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1" ,"Armature/Root/Hips/Spine"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2" ,"Armature/Root/Hips/Spine/Chest"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3" ,"Armature/Root/Hips/Spine/Chest/UpperChest"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar" ,"Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar/m_avg_L_Shoulder" ,"Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder/LeftArm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar/m_avg_L_Shoulder/m_avg_L_Elbow" ,"Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder/LeftArm/LeftForeArm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar/m_avg_L_Shoulder/m_avg_L_Elbow/m_avg_L_Wrist" ,"Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder/LeftArm/LeftForeArm/LeftHand"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_L_Collar/m_avg_L_Shoulder/m_avg_L_Elbow/m_avg_L_Wrist/m_avg_L_Hand" ,"Armature/Root/Hips/Spine/Chest/UpperChest/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LeftPalm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar" ,"Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar/m_avg_R_Shoulder" ,"Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder/RightArm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar/m_avg_R_Shoulder/m_avg_R_Elbow" ,"Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder/RightArm/RightForeArm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar/m_avg_R_Shoulder/m_avg_R_Elbow/m_avg_R_Wrist" ,"Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder/RightArm/RightForeArm/RightHand"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_R_Collar/m_avg_R_Shoulder/m_avg_R_Elbow/m_avg_R_Wrist/m_avg_R_Hand" ,"Armature/Root/Hips/Spine/Chest/UpperChest/RightShoulder/RightArm/RightForeArm/RightHand/RightPalm"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_Neck" ,"Armature/Root/Hips/Spine/Chest/UpperChest/Neck"},
        { "/male_smpl/m_avg_root/m_avg_Pelvis/m_avg_Spine1/m_avg_Spine2/m_avg_Spine3/m_avg_Neck/m_avg_Head" ,"Armature/Root/Hips/Spine/Chest/UpperChest/Neck/Head"}
     };
    // Start is called before the first frame update
    GameObject ref_bone, dst_bone;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var OneItem in bone_pair)
        {
            ref_bone = GameObject.Find(OneItem.Key);
            dst_bone = GameObject.Find(OneItem.Value);
            dst_bone.transform.rotation = ref_bone.transform.rotation;
            dst_bone.transform.position = ref_bone.transform.position-(new Vector3(2, 0, 0));
        }
    }
}
