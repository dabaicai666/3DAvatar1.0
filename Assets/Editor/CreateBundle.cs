using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

/// <summary>
/// 把Resource下的资源打包成.data到OutPut目录下
/// </summary>
public class CreateBundle : Editor
{
    public static string sourcePath = Application.dataPath + "/_Models/";
    const string AssetBundlesOutputPath = "Assets/_AllBundle_PC/";

    [MenuItem("Custom/BuildAssetBundle")]
    public static void BuildAssetBundle()
    {
          ClearAssetBundlesName();

          Pack(sourcePath);
       // fileWithDepends1();
        string outputPath = AssetBundlesOutputPath;
        // string outputPath = Path.Combine(AssetBundlesOutputPath, Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        //根据BuildSetting里面所激活的平台进行打包 设置过AssetBundleName的都会进行打包
        BuildPipeline.BuildAssetBundles(outputPath, 0, EditorUserBuildSettings.activeBuildTarget);
       // BuildPipeline.BuildAssetBundles(outputPath,BuildAssetBundleOptions.CollectDependencies, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();

        Debug.Log("打包完成");

    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包
    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下
    /// </summary>
    static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
    }

    static void Pack(string source)
    {
        //Debug.Log("Pack source " + source);
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                Debug.Log(files[i].FullName);
                Pack(files[i].FullName);
            }
            else
            {
                if (!files[i].Name.EndsWith(".meta"))
                {
                    Debug.Log("AAAA:"+files[i].FullName );
                    fileWithDepends(files[i].FullName);
                    //file(files[i].FullName);
                }
            }
        }
    }
    //设置要打包的文件
    static void fileWithDepends(string source)
    {
        Debug.Log("file source " + source);
        string _source = Replace(source);
        string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);

        Debug.Log(_assetPath);

        //自动获取依赖项并给其资源设置AssetBundleName
        string[] dps = AssetDatabase.GetDependencies(_assetPath);
        foreach (var dp in dps)
        {
            Debug.Log("dp " + dp);
            if (dp.EndsWith(".cs"))
                continue;
            AssetImporter assetImporter = AssetImporter.GetAtPath(dp);
            string pathTmp = dp.Substring("Assets".Length + 1);
            Debug.Log("pathTmp:" + pathTmp);
            string assetName = pathTmp.Substring(pathTmp.IndexOf("/") + 1);
            Debug.Log("assetName:" + assetName);
            //assetName = assetName.Replace(Path.GetExtension(assetName), ".data");
            assetName = assetName.Replace(Path.GetExtension(assetName), ".assetbundle");
            Debug.Log(assetName);
            assetImporter.assetBundleName = assetName;
        }

    }
    static void fileWithDepends1() {
        string dp = "Assets/_Models/20200813/test.fbx";
        AssetImporter asset = AssetImporter.GetAtPath(dp);
        string pathTmp = dp.Substring("20200813".Length + 1);
        Debug.Log("pathTmp:" + pathTmp);
        string assetName = pathTmp.Substring(pathTmp.IndexOf("/") + 1);
        Debug.Log("assetName:" + assetName);
        //assetName = assetName.Replace(Path.GetExtension(assetName), ".data");
        assetName = assetName.Replace(Path.GetExtension(assetName), ".assetbundle");
        Debug.Log(assetName);
        asset.assetBundleName = assetName;

    }
    //设置要打包的文件
    static void file(string source)
    {
        Debug.Log("file source " + source);
        string _source = Replace(source);
        string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
        string _assetPath2 = _source.Substring(Application.dataPath.Length + 1);
        //Debug.Log (_assetPath);

        //在代码中给资源设置AssetBundleName
        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
        string[] dps = AssetDatabase.GetDependencies(_assetPath);
        foreach (var dp in dps)
        {
            Debug.Log("dp " + dp);
        }
        string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);
        assetName = assetName.Replace(Path.GetExtension(assetName), ".unity3d");
        Debug.Log(assetName);
        assetImporter.assetBundleName = assetName;
    }

    static string Replace(string s)
    {
        return s.Replace("\\", "/");
    }
}

public class Platform
{
    public static string GetPlatformFolder(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "IOS";          
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
          
            default:
                return null;
        }
    }
}
