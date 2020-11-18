using UnityEngine;
using System.Collections.Generic;
using System;

namespace TressFX
{
    public class AvatarHair : MonoBehaviour
    {
        /// <summary>
        /// 头发相关的数据资源
        /// </summary>
        private class HairDataOIT
        {
            private TressFX _hair;
            private TressFXOITRenderer _render;
            private TressFXSimulation _simulation;
            public string _materialpath;
            public bool enabled = true;

            /// <summary>
            /// 初始化头发
            /// </summary>
            /// <param name="hairpath"></param>
            /// <param name="matpat"></param>
            /// <returns></returns>
            public bool initHair(Transform transform, string hairpath, string matpath)
            {
                this._materialpath = matpath;
                this._hair = new TressFX(transform, hairpath);
                this._render = new TressFXOITRenderer(this._hair, transform, matpath);
                this._simulation = new TressFXSimulation(this._hair, transform);
                this.enabled = this._hair.enabled && this._render.enabled && this._simulation.enabled;
                return this.enabled;
            }

            /// <summary>
            /// 渲染线程更新头发的实时仿真效果
            /// </summary>
            /// <param name="transform"></param>
            public void Update(Transform transform)
            {
                if (this.enabled && _render != null && _simulation != null)
                {
                    this._render.Update(transform);
                    this._simulation.Update(transform);
                }
            }

            /// <summary>
            /// 回收数据
            /// </summary>
            public void OnDestroy()
            {
                if (this.enabled && _render != null && _simulation != null)
                {
                    this._hair.OnDestroy();
                    this._render.OnDestroy();
                }
            }
        }

        /// <summary>
        /// 数据存储结构
        /// </summary>
        private Dictionary<string, HairDataOIT> _hTressHairOIT = new Dictionary<string, HairDataOIT>();

        /// <summary>
        /// Use this for initialization
        /// </summary>
        /// <param name="hairpath">头发资源路径</param>
        /// <param name="matpath">头发材质路径</param>
        /// <returns>加载成功返回true;否则返回false</returns>
        public bool loadHairOIT(string hairpath, string matpath)
        {
            if (!_hTressHairOIT.ContainsKey(hairpath))
            {
                foreach (string key in this._hTressHairOIT.Keys)
                {
                    _hTressHairOIT[key].enabled = false;
                }

                HairDataOIT dta = new HairDataOIT();
                if (!dta.initHair(this.transform, hairpath, matpath))
                {
                    Debug.unityLogger.Log("Hair", "Hair initial failed !!!");
                    return false;
                }
                _hTressHairOIT.Add(hairpath, dta);
            }
            return true;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        /// <param name="transform">头发变换</param>
        public void renderOneframe()
        {
            foreach (string key in this._hTressHairOIT.Keys)
            {
                this._hTressHairOIT[key].Update(this.transform);
            }
        }

        public virtual void Awake()
        {
            this.loadHairOIT(Application.dataPath + "/TressFXOIT/Resources/Hairs/hairCurve.tfxb", "");
        }

        public virtual void Update()
        {
            this.renderOneframe();
        }

        public void OnDestroy()
        {
            foreach (string key in this._hTressHairOIT.Keys)
            {
                this._hTressHairOIT[key].OnDestroy();
            }
        }
    }
}