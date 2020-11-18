using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using FairyGUI;
//      加载外部图片
public class MyGloader : GLoader
{
    protected override void LoadExternal()
    {
        IconManger.inst.LoadIcon(this.url, OnLoadSuccess, OnLoadFail);
    }
    protected override void FreeExternal(NTexture texture)
    {
        texture.refCount--;
    }
    void OnLoadSuccess(NTexture texture)
    {

        if (string.IsNullOrEmpty(this.url))
            return;
        this.onExternalLoadSuccess(texture);

    }
    void OnLoadFail(string error)
    {
        Debug.Log("load" + this.url + "failed" + error);
        this.onExternalLoadFailed();
    }
}