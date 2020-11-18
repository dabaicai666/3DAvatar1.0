using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//常量地址
public class ConstantValue : MonoBehaviour
{
    public static string ip = Application.streamingAssetsPath + "/ip.txt";
    public const string upLoadImagePath = "http://192.168.3.119:6666/faceconstruct";  //图片上传地址
    public const string downLoadClothPath = "http://192.168.3.119:6666/clothrefine";  //衣服素材下载地址
    public static string filePathLocal = Application.persistentDataPath + "/3DAvatar_Tempfiles/";
    public static string BundlePathLocal = Application.persistentDataPath + "/3DAvatar_Tempbundlefiles/";
}
