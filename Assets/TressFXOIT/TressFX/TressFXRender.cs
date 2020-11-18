using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace TressFX
{
	public class TressFXRender : ATressFXRender
	{
		[Header("Triangle Shadows")]
        /// <summary>
        /// The full shadows flag.
        /// If this is checked, shadow is rendered in triangle topology instead of line topology.
        /// </summary>
        public bool fullShadows = true;

        /// <summary>
        /// 是否渲染
        /// </summary>
        public bool enabled = true;

        /// <summary>
        /// The hair material.
        /// </summary>
        public Material hairMaterial;

		/// <summary>
		/// The random texture.
		/// </summary>
		private Texture2D randomTexture;

        /// <summary>
        /// Start this instance.
        /// Initializes the hair material and all other resources.
        /// </summary>
        public TressFXRender(TressFX hair, Transform trans, string matpath)
        {
            /// 基类初始化
            if (!base.init(hair, trans))
            {
                Debug.LogError("Hair base render initial failed!!!:(");
                this.enabled = false;
                return;
            }

            /// 创建材质
            if (!this.createMaterials(matpath))
            {
                Debug.LogError("Hair material assigned to tressfx hair load failed!!!:(");
                this.enabled = false;
                return;
            }

			// Generate random texture
			this.randomTexture = new Texture2D (128, 128);
			for (int x = 0; x < 128; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					float randomValue = Random.value;
					this.randomTexture.SetPixel (x, y, new Color (randomValue, randomValue, randomValue, randomValue));
				}
			}
			this.randomTexture.Apply ();
		}

        /// <summary>
        /// 数据更新
        /// </summary>
        public override void Update(Transform transform)
        {
            base.Update(transform);

            this.LateUpdate();
        }
        /// <summary>
        /// </summary>
        private void LateUpdate()
        {
            // Set shader buffers
            this.hairMaterial.SetBuffer("g_HairVertexTangents", this._master.g_HairVertexTangents);
            this.hairMaterial.SetBuffer("g_HairVertexPositions", this._master.g_HairVertexPositions);
            this.hairMaterial.SetBuffer("g_TriangleIndicesBuffer", this.g_TriangleIndicesBuffer);
            this.hairMaterial.SetBuffer("g_HairThicknessCoeffs", this._master.g_HairThicknessCoeffs);
            this.hairMaterial.SetBuffer("g_TexCoords", this._master.g_TexCoords);
            this.hairMaterial.SetInt("_VerticesPerStrand", this._master.hairData.m_NumOfVerticesPerStrand);

            // Transformation matrices
            SetSimulationTransformCorrection(this.hairMaterial);
            SetSimulationTransformCorrection(this.shadowMaterial);

            // Set random texture
            this.hairMaterial.SetTexture ("_RandomTex", this.randomTexture);
			
			// Update rendering bounds
			Bounds renderingBounds = new Bounds (this.transform.position + this.renderingBounds.center, this.renderingBounds.size);
			
			// Render meshes
			for (int i = 0; i < this.triangleMeshes.Length; i++)
			{
				this.triangleMeshes[i].bounds = renderingBounds;
				#if UNITY_EDITOR
				if (UnityEditor.SceneView.lastActiveSceneView != null && UnityEditor.SceneView.lastActiveSceneView.camera != null)
					Graphics.DrawMesh (this.triangleMeshes [i], Vector3.zero, Quaternion.identity, this.hairMaterial, 8, UnityEditor.SceneView.lastActiveSceneView.camera, 0, new MaterialPropertyBlock(), this.fullShadows);
				#endif
				foreach (Camera cam in Camera.allCameras)
					Graphics.DrawMesh (this.triangleMeshes [i], Vector3.zero, Quaternion.identity, this.hairMaterial, 8, cam, 0, new MaterialPropertyBlock(), this.fullShadows);
			}

            RenderShadows();
        }

        /// <summary>
        /// 阴影渲染
        /// </summary>
        protected override void RenderShadows()
        {
            if (!this.fullShadows)
                base.RenderShadows();
        }

        /// <summary>
        /// 材质创建
        /// </summary>
        /// <param name="matfile">材质属性设置文件json格式</param>
        /// <returns></returns>
        protected bool createMaterials(string matfile)
        {
            ///TODO  使用不同的shader
            Shader surfaceShader = Shader.Find("TressFX/Surface");
            if (surfaceShader == null)
            {
                return false;
            }
            this.hairMaterial = new Material(surfaceShader);
            if (this.hairMaterial == null)
            {
                return false;
            }
            ///TODO  从matfile读取数据设置shader参数，通过Material的接口设置
            ///      this.hairMaterial.SetColor 
            this.hairMaterial.SetColor("_HairColor", new Color(0, 0, 0, 1));
            this.hairMaterial.SetColor("_SpecularColor", new Color(0, 0, 0, 1));
            this.hairMaterial.SetFloat("g_MatKd", 0.4f);
            this.hairMaterial.SetFloat("g_MatKa", 0.0f);
            this.hairMaterial.SetFloat("g_MatKs1", 0.14f);
            this.hairMaterial.SetFloat("g_MatEx1", 80f);
            this.hairMaterial.SetFloat("g_MatEx2", 8f);
            this.hairMaterial.SetFloat("g_MatKs2", 0.5f);
            this.hairMaterial.SetFloat("g_bThinTip", 0.0f);
            this.hairMaterial.SetFloat("_HairWidth", 0.3f);
            this.hairMaterial.SetFloat("_HairWidth", 0.7f);
            this.hairMaterial.SetTexture("_HairColorTex", Texture2D.whiteTexture);
            this.hairMaterial.SetTexture("_HairSpecularTexEx1", Texture2D.whiteTexture);
            this.hairMaterial.SetTexture("_HairSpecularTexEx2", Texture2D.whiteTexture);
            this.hairMaterial.SetTexture("_RandomTex", Texture2D.whiteTexture);
            return true;
        }
    }
}
