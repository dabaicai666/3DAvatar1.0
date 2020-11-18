using GPUTools.Common.Scripts.PL.Abstract;
using GPUTools.Common.Scripts.PL.Attributes;
using GPUTools.Common.Scripts.PL.Config;
using GPUTools.Common.Scripts.PL.Tools;
using GPUTools.Hair.Scripts.Runtime.Render;
using GPUTools.Physics.Scripts.Types.Dynamic;
using UnityEngine;

namespace GPUTools.Hair.Scripts.Runtime.Kernels
{
    public class TesselateKernel : KernelBase
    {
        [GpuData("particles")] public GpuBuffer<GPParticle> Particles { set; get; }
        [GpuData("renderParticles")] public GpuBuffer<RenderParticle> RenderParticles { set; get; }
        [GpuData("tessRenderParticles")] public GpuBuffer<TessRenderParticle> TessRenderParticles { set; get; }

        [GpuData("segments")] public GpuValue<int> Segments { set; get; }
        [GpuData("tessSegments")]public GpuValue<int> TessSegments { set; get; }
        [GpuData("wavinessAxis")] public GpuValue<Vector3> WavinessAxis { set; get; }

        public TesselateKernel() : base("Compute/Tesselate", "CSTesselate")
        {
            
        }

        public override int GetGroupsNumX()
        {
            //TessSegments*standsCount
            return Mathf.CeilToInt(TessRenderParticles.Count/ (float)GpuConfig.NumThreads);
        }
    }
}
