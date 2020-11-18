using TressFXLib.Numerics;

namespace TressFXLib
{
    /// <summary>
    /// Hair import settings structure.
    /// Used to define import setting for the import pipeline.
    /// </summary>
    public class HairImportSettings
    {
        public static HairImportSettings standard = new HairImportSettings();

        public Vector3 scale;
        public Vector3 position;
        public Quaternion rotate;
        public HairImportSettings()
        {
            this.scale = new Vector3(1,1,1);
            this.position = new Vector3(1,1,1);
            this.rotate = new Quaternion(0,0,0,1);
        }
    }
}
