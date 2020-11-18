
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class CreateAssetBunldesMain{
    [MenuItem("AssetBundle/Build_PC AssetBunldes")]
    static void Build_PCAssetBunldes()
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);


            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            //			string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";
            //IOS平台
            //string targetPath = Application.dataPath + "/_AllBundle_IOS/" + obj.name + ".assetbundle";
              string targetPath = Application.dataPath + "/_AllBundle_PC/" + obj.name + ".assetbundle";


            //if (BuildPipeline.BuildAssetBundles(Application.dataPath + "/_Models/20200813", BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows))
            //{
            //    Debug.Log(obj.name + "资源打包成功");
            //}
            //else
            //{
            //    Debug.Log(obj.name + "资源打包失败");
            //}
            //if (BuildPipeline.BuildAssetBundles(Application.dataPath + "/_Models/20200813", BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows))
            //{
            //    Debug.Log(obj.name + "资源打包成功");
            //}
            //else
            //{
            //    Debug.Log(obj.name + "资源打包失败");
            //}
            if (BuildPipeline.BuildAssetBundle(obj, null,targetPath, BuildAssetBundleOptions.CollectDependencies, EditorUserBuildSettings.activeBuildTarget))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }


        }
        //刷新编辑器
        AssetDatabase.Refresh ();	

	}
    [MenuItem("AssetBundle/Build_IOS AssetBunldes")]
    static void Build_IOSAssetBunldes()
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            //			string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";
            //IOS平台
            string targetPath = Application.dataPath + "/_AllBundle_IOS/" + obj.name + ".assetbundle";
           // string targetPath = Application.dataPath + "/_AllBundle_Android/" + obj.name + ".assetbundle";

            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.iOS))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }


        //刷新编辑器
        AssetDatabase.Refresh();

    }
}