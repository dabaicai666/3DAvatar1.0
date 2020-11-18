using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TressFXLib;

namespace TressFX
{
	/// <summary>
	/// Tress FX main class.
	/// </summary>
    public class TressFX
	{
        /// <summary>
        /// 资源设置
        /// </summary>
        public bool enabled = true;
        public TressFXHair hairData;
        public HairImportSettings importSettings;
        private Transform bindObjectTransform; ///bind gameobject 

        /// <summary>
        /// The hair vertex positions buffer.
        /// </summary>
        public ComputeBuffer g_HairVertexPositions;
		public ComputeBuffer g_HairVertexPositionsPrev;
		public ComputeBuffer g_HairVertexTangents;
		public ComputeBuffer g_InitialHairPositions;
		public ComputeBuffer g_GlobalRotations;
		public ComputeBuffer g_LocalRotations;

		public ComputeBuffer g_HairThicknessCoeffs;
		public ComputeBuffer g_HairRestLengthSRV;
		public ComputeBuffer g_HairStrandType;
		public ComputeBuffer g_HairRefVecsInLocalFrame;
		public ComputeBuffer g_FollowHairRootOffset;
		public ComputeBuffer g_TexCoords;

        /// <summary>
        /// 创建头发
        /// </summary>
        /// <param name="path"></param>
        public TressFX(Transform transform, string path)
        {
            ///初始位置偏移
            this.bindObjectTransform = transform;

            ///头发资源设置
            this.hairLoad(path);

            this.init();/// 初始化
        }

		/// <summary>
		/// Raises the destroy event.
		/// Cleans up all used resources.
		/// </summary>
		public void OnDestroy()
		{
			this.g_HairVertexPositions.Release ();
			this.g_HairVertexPositionsPrev.Release ();
			this.g_InitialHairPositions.Release ();

			this.g_HairVertexTangents.Release ();
			this.g_GlobalRotations.Release ();
			this.g_LocalRotations.Release ();

			this.g_HairThicknessCoeffs.Release ();
			this.g_HairRestLengthSRV.Release ();
			this.g_HairStrandType.Release ();
			this.g_HairRefVecsInLocalFrame.Release ();
			this.g_FollowHairRootOffset.Release ();
			this.g_TexCoords.Release ();
        }

        /// <summary>
        /// 加载头发
        /// </summary>
        /// <returns></returns>
        private bool hairLoad(string path)
        {
            if (!File.Exists(path))
            {
                this.enabled = false;
                return false;
            }

            // Create new hair asset
            this.hairData = new TressFXHair();

            // Open hair data
            if (importSettings == null)
                this.importSettings = new HairImportSettings();
            importSettings.scale = new TressFXLib.Numerics.Vector3(bindObjectTransform.lossyScale.x, bindObjectTransform.lossyScale.y, bindObjectTransform.lossyScale.z);
            importSettings.position = new TressFXLib.Numerics.Vector3(bindObjectTransform.position.x, bindObjectTransform.position.y, bindObjectTransform.position.z);
            importSettings.rotate = new TressFXLib.Numerics.Quaternion(bindObjectTransform.rotation.x, bindObjectTransform.rotation.y, bindObjectTransform.rotation.z, bindObjectTransform.rotation.w);
            Hair hair = Hair.Import(HairFormat.TFXB, path, importSettings);
            hair.CreateUVs();

            // load hair
            HairSimulationData hairSimulationData = hair.hairSimulationData;

            // Load information variables
            this.hairData.m_NumTotalHairVertices = hairSimulationData.vertexCount;
            this.hairData.m_NumTotalHairStrands = hairSimulationData.strandCount;
            this.hairData.m_NumOfVerticesPerStrand = hairSimulationData.maxNumVerticesPerStrand;
            this.hairData.m_NumGuideHairVertices = hairSimulationData.guideHairVertexCount;
            this.hairData.m_NumGuideHairStrands = hairSimulationData.guideHairStrandCount;
            this.hairData.m_NumFollowHairsPerOneGuideHair = hairSimulationData.followHairsPerOneGuideHair;

            // Load actual hair data
            this.hairData.m_pHairStrandType = hairSimulationData.strandTypes.ToArray();
            this.hairData.m_pRefVectors = Vector4Import(hairSimulationData.referenceVectors.ToArray());
            this.hairData.m_pGlobalRotations = QuaternionsToVector4(QuaternionImport(hairSimulationData.globalRotations.ToArray()));
            this.hairData.m_pLocalRotations = QuaternionsToVector4(QuaternionImport(hairSimulationData.localRotations.ToArray()));
            this.hairData.m_pVertices = Vector4Import(hairSimulationData.vertices.ToArray());
            this.hairData.m_pTangents = Vector4Import(hairSimulationData.tangents.ToArray());
            this.hairData.m_pThicknessCoeffs = hairSimulationData.thicknessCoefficients.ToArray();
            this.hairData.m_pFollowRootOffset = Vector4Import(hairSimulationData.followRootOffsets.ToArray());
            this.hairData.m_pRestLengths = hairSimulationData.restLength.ToArray();

            // Determine how much hair strand types are available
            List<int> strandTypes = new List<int>();
            for (int i = 0; i < this.hairData.m_pHairStrandType.Length; i++)
            {
                if (!strandTypes.Contains(this.hairData.m_pHairStrandType[i]))
                {
                    strandTypes.Add(this.hairData.m_pHairStrandType[i]);
                }
            }

            if (this.hairData.hairPartConfig == null || this.hairData.hairPartConfig.Length != strandTypes.Count)
            {
                this.hairData.hairPartConfig = new HairPartConfig[strandTypes.Count];
                for (int i = 0; i < strandTypes.Count; i++)
                {
                    this.hairData.hairPartConfig[i].Damping = 0.25f;
                    this.hairData.hairPartConfig[i].StiffnessForLocalShapeMatching = 1.0f;
                    this.hairData.hairPartConfig[i].StiffnessForGlobalShapeMatching = 0.2f;
                    this.hairData.hairPartConfig[i].GlobalShapeMatchingEffectiveRange = 0.3f;
                }
            }

            // Load bounding sphere
            this.hairData.m_bSphere = new TressFXBoundingSphere(new Vector3(hair.boundingSphere.center.x,
                hair.boundingSphere.center.y, hair.boundingSphere.center.z), hair.boundingSphere.radius);
            this.hairData.m_TriangleIndices = hair.triangleIndices;
            this.hairData.m_LineIndices = hair.lineIndices;
            this.hairData.m_TexCoords = Vector4Import(hair.texcoords);
            return true;
        }

        /// <summary>
        /// Start this instance.
        /// Initializes all buffers and other resources needed by tressfx simulation and rendering.
        /// </summary>
        private void init()
        {
            // Vertex buffers
            this.g_HairVertexPositions = this.InitializeBuffer(this.hairData.m_pVertices, 16);
            this.g_HairVertexPositionsPrev = this.InitializeBuffer(this.hairData.m_pVertices, 16);
            this.g_InitialHairPositions = this.InitializeBuffer(this.hairData.m_pVertices, 16);

            // Tangents and rotations
            this.g_HairVertexTangents = this.InitializeBuffer(this.hairData.m_pTangents, 16);
            this.g_GlobalRotations = this.InitializeBuffer(this.hairData.m_pGlobalRotations, 16);
            this.g_LocalRotations = this.InitializeBuffer(this.hairData.m_pLocalRotations, 16);

            // Others
            this.g_HairRestLengthSRV = this.InitializeBuffer(this.hairData.m_pRestLengths, 4);
            this.g_HairStrandType = this.InitializeBuffer(this.hairData.m_pHairStrandType, 4);
            this.g_HairRefVecsInLocalFrame = this.InitializeBuffer(this.hairData.m_pRefVectors, 16);
            this.g_FollowHairRootOffset = this.InitializeBuffer(this.hairData.m_pFollowRootOffset, 16);
            this.g_HairThicknessCoeffs = this.InitializeBuffer(this.hairData.m_pThicknessCoeffs, 4);
            this.g_TexCoords = this.InitializeBuffer(this.hairData.m_TexCoords, 16);
        }

        /// <summary>
        /// Initializes the a new ComputeBuffer.
        /// </summary>
        /// <returns>The buffer.</returns>
        /// <param name="data">Data.</param>
        /// <param name="stride">Stride.</param>
        private ComputeBuffer InitializeBuffer(System.Array data, int stride)
        {
            ComputeBuffer returnBuffer = new ComputeBuffer(data.Length, stride);
            returnBuffer.SetData(data);
            return returnBuffer;
        }

        /// <summary>
		/// Quaternions to vector4 casting function.
		/// </summary>
		/// <returns>The to vector4.</returns>
		/// <param name="quaternions">Quaternions.</param>
		private Vector4[] QuaternionsToVector4(Quaternion[] quaternions)
        {
            Vector4[] vectors = new Vector4[quaternions.Length];
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = new Vector4(quaternions[i].x, quaternions[i].y, quaternions[i].z, quaternions[i].w);
            return vectors;
        }

        /// <summary>
        /// Vector3 import function.
        /// Casts the tressfx lib vectors to unity vectors.
        /// </summary>
        /// <returns>The import.</returns>
        /// <param name="vectors">Vectors.</param>
        public Vector3[] Vector3Import(TressFXLib.Numerics.Vector3[] vectors)
        {
            Vector3[] returnVectors = new Vector3[vectors.Length];
            for (int i = 0; i < vectors.Length; i++)
                returnVectors[i] = new Vector3(vectors[i].x, vectors[i].y, vectors[i].z);
            return returnVectors;
        }

        /// <summary>
        /// Vector4 import function.
        /// Casts the tressfx lib vectors to unity vectors.
        /// </summary>
        /// <returns>The import.</returns>
        /// <param name="vectors">Vectors.</param>
        public Vector4[] Vector4Import(TressFXLib.Numerics.Vector4[] vectors)
        {
            Vector4[] returnVectors = new Vector4[vectors.Length];
            for (int i = 0; i < vectors.Length; i++)
                returnVectors[i] = new Vector4(vectors[i].x, vectors[i].y, vectors[i].z, vectors[i].w);
            return returnVectors;
        }

        /// <summary>
        /// Quaternion import function.
        /// Casts the tressfx lib vectors to unity vectors.
        /// </summary>
        /// <returns>The import.</returns>
        /// <param name="vectors">Vectors.</param>
        public Quaternion[] QuaternionImport(TressFXLib.Numerics.Quaternion[] quaternions)
        {
            Quaternion[] returnQuaternion = new Quaternion[quaternions.Length];
            for (int i = 0; i < quaternions.Length; i++)
                returnQuaternion[i] = new Quaternion(quaternions[i].x, quaternions[i].y, quaternions[i].z, quaternions[i].W);
            return returnQuaternion;
        }

    }
}