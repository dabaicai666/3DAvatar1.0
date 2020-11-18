using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TressFX
{
    public class TressFXOITRenderer : ATressFXRender
    {
        #region Instances

        public static List<TressFXOITRenderer> renderers = new List<TressFXOITRenderer>();

        #endregion
        /// </summary>
        public bool enabled = true;

        [Header("Configuration")]
        /// <summary>
        /// The hair material used for rendering the fill pass.
        /// </summary>
        public Material hairMaterial;

        /// <summary>
        /// How many lights do this hair pickup max?
        /// </summary>
        public int maxLightCount;

        [Range(0,1)]
        public float selfShadowStrength = 0.75f;

        [Range(0, 1)]
        private float Alpha_threshold = 0.0f;
        private float Hair_fiber_width = 0.08f;
        private float Hair_multiplier = 0.1f;
        private float Thin_tip = 0.0f;
        private float Scale = 1.0f; //头发缩放后需要：Hair_multiplier *= scale
        private Color Hair_color = new Color(0.34f, 0.34f, 0.34f, 1);

        /// <summary>
        /// 初始化render类
        /// </summary>
        /// <param name="hair"></param>
        /// <param name="trans"></param>
        /// <param name="matpath"></param>
        public TressFXOITRenderer(TressFX hair, Transform trans, string matpath)
        {
            /// 基类初始化
            if (!base.init(hair, trans))
            {
                Debug.LogError("Hair base render initial failed!!!:(");
                this.enabled = false;
                return;
            }

            /// 创建材质
            if (!this.createMaterials(matpath, trans.localScale.x))
            {
                Debug.LogError("Hair material assigned to tressfx hair load failed!!!:(");
                this.enabled = false;
                return;
            }
            renderers.Add(this);
        }

        /// <summary>
        /// Sets the shader parameters of this renderer to the specified material.
        /// </summary>
        /// <param name="material"></param>
        public void SetShaderParams(Material material)
        {
            // Set all properties
            material.SetBuffer("HairVertexTangents", this._master.g_HairVertexTangents);
            material.SetBuffer("HairVertexPositions", this._master.g_HairVertexPositions);
            material.SetBuffer("TriangleIndicesBuffer", this.g_TriangleIndicesBuffer);
            material.SetBuffer("LineIndicesBuffer", this.g_LineIndicesBuffer);
            material.SetBuffer("GlobalRotations", this._master.g_GlobalRotations);
            material.SetBuffer("HairThicknessCoeffs", this._master.g_HairThicknessCoeffs);
            material.SetBuffer("TexCoords", this._master.g_TexCoords);
            material.SetFloat("_ThinTip", this.hairMaterial.GetFloat("_ThinTip"));
            material.SetFloat("_HairWidth", this.hairMaterial.GetFloat("_HairWidth"));
            material.SetFloat("_HairWidthMultiplier", this.hairMaterial.GetFloat("_HairWidthMultiplier"));

            SetSimulationTransformCorrection(material);
        }

        /// <summary>
        /// pass render
        /// </summary>
        public void RenderFillPass()
        {
            // Prepare all properties
            SetShaderParams(this.hairMaterial);
            SetShaderParams(this.shadowMaterial);

            // Render
            int renderCount = (int)(this._master.hairData.m_TriangleIndices.Length * 1);
            this.hairMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, renderCount);
        }

        /// <summary>
        /// 材质创建
        /// </summary>
        /// <param name="matfile">材质属性设置文件json格式</param>
        /// <returns></returns>
        protected bool createMaterials(string matfile, float scale = 1.0f)
        {
            ///TODO  使用不同的shader
            Shader surfaceShader = Shader.Find("Unlit/TressFXOIT");
            if (surfaceShader == null)
            {
                return false;
            }
            if (this.hairMaterial == null)
                this.hairMaterial = new Material(surfaceShader);

            /////TODO  从matfile读取数据设置shader参数，通过Material的接口设置
            /////      this.hairMaterial.SetColor   
            this.Scale = scale;
            this.hairMaterial.SetColor("_HairColor", Hair_color);
            this.hairMaterial.SetFloat("_AlphaThreshold", Alpha_threshold);
            this.hairMaterial.SetFloat("_HairWidth", Hair_fiber_width);
            this.hairMaterial.SetFloat("_HairWidthMultiplier", Hair_multiplier * Scale);
            this.hairMaterial.SetFloat("_ThinTip", Thin_tip);
            return true;
        }
    }
}