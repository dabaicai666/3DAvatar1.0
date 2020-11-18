using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.IO;
public delegate void LoadCompleteCallback(NTexture texture);
public delegate void LoadErrorCallback(string error);

public class IconManger: MonoBehaviour
{
    private static GameObject obj;
    static IconManger _instance;
    public string path;
    public static IconManger inst
    {
        get
        {
            if (_instance == null)
            {
                obj = new GameObject("IconManger");
                DontDestroyOnLoad(obj);
                _instance = obj.AddComponent<IconManger>();
            }
            return _instance;
        }
    }
    public const int POOL_CHECK_TIME = 30;
    public const int MAX_POOL_SIZE = 200;

    List<LoadItem> _item;
    bool _started;
    Hashtable _pool;
    // Use this for initialization
    void Awake()
    {
        path = Application.persistentDataPath;
        _item = new List<LoadItem>();
        _pool = new Hashtable();
        StartCoroutine(FreeIdleIcons());

    }
    public void LoadIcon(string url, LoadCompleteCallback onSuccess, LoadErrorCallback onFail)
    {
        LoadItem item = new LoadItem();
        item.url = url;
        item.onSuccess = onSuccess;
        item.onFail = onFail;
        _item.Add(item);
        if (!_started)
        {
            StartCoroutine(Run());
        }
    }
    IEnumerator Run()
    {
        _started = true;
        LoadItem item = null;
        while (true)
        {
            if (_item.Count > 0)
            {
                item = _item[0];
                _item.RemoveAt(0);
            }
            else
            {
                break;
            }
            if (_pool.ContainsKey(item.url))
            {
                NTexture texture = _pool[item.url] as NTexture;
                texture.refCount++;
                if (item.onSuccess != null)
                {
                    item.onSuccess(texture);
                    continue;
                }
            }
            WWW www = new WWW(item.url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D image = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                www.LoadImageIntoTexture(image);
                NTexture texture = new NTexture(image);
                _pool[item.url] = texture;

                if (item.onSuccess != null)
                    item.onSuccess(texture);
                //  Debug.Log(texture.width+"");
            }
            else
            {
                if (item.onFail != null)
                    item.onFail(www.error);
            }
        }
        _started = false;
    }

    IEnumerator FreeIdleIcons()
    {
        while (true)
        {
            yield return new WaitForSeconds(POOL_CHECK_TIME); //check the pool every 30 seconds

            int cnt = _pool.Count;
            if (cnt > MAX_POOL_SIZE)
            {
                ArrayList toRemove = null;
                foreach (DictionaryEntry de in _pool)
                {
                    string key = (string)de.Key;
                    NTexture texture = (NTexture)de.Value;
                    if (texture.refCount == 0)
                    {
                        if (toRemove == null)
                            toRemove = new ArrayList();
                        toRemove.Add(key);
                        texture.Dispose();

                        //Debug.Log("free icon " + de.Key);

                        cnt--;
                        if (cnt <= 8)
                            break;
                    }
                }
                if (toRemove != null)
                {
                    foreach (string key in toRemove)
                        _pool.Remove(key);
                }
            }
        }
    }
}
class LoadItem
{
    public string url; //模型Icon
    public LoadCompleteCallback onSuccess;
    public LoadErrorCallback onFail;
}
