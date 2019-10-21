using CitizenFX.Core;

namespace Dialogues.Data
{
    public class CameraConfiguration
    {
        public Vector3 Offset { get; set; }
        public Vector3 Rotation { get; set; }
        public float FieldOfView { get; set; }

        public CameraConfiguration(Vector3 offset, Vector3 rotation, float fieldOfView)
        {
            Offset = offset;
            Rotation = rotation;
            FieldOfView = fieldOfView;
        }
    }
}