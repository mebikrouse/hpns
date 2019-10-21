using CitizenFX.Core;
using HPNS.Core;

using static CitizenFX.Core.Native.API;

namespace CameraTool
{
    public class CameraControl : IUpdateObject
    {
        private Camera _camera;
        
        public float MovementSpeed { get; set; } = 1.5f;
        public float RotationSpeed { get; set; } = 30f;
        public float FovSpeed { get; set; } = 10f;
        public float MinFov { get; set; } = 10f;
        public float MaxFov { get; set; } = 120f;

        public void SetCamera(Camera camera)
        {
            _camera = camera;
        }
        
        public void Update(float deltaTime)
        {
            //movement
            if (IsControlPressed(0, 172))
                MoveCamera(_camera.UpVector, MovementSpeed * deltaTime);

            if (IsControlPressed(0, 173))
                MoveCamera(-_camera.UpVector, MovementSpeed * deltaTime);

            if (IsControlPressed(0, 174))
                MoveCamera(-_camera.RightVector, MovementSpeed * deltaTime);

            if (IsControlPressed(0, 175))
                MoveCamera(_camera.RightVector, MovementSpeed * deltaTime);

            if (IsControlPressed(0, 312))
                MoveCamera(_camera.ForwardVector, MovementSpeed * deltaTime / 2.5f);

            if (IsControlPressed(0, 313))
                MoveCamera(-_camera.ForwardVector, MovementSpeed * deltaTime / 2.5f);

            //rotation
            if (IsControlPressed(0, 111))
                RotateCamera(RotationSpeed * deltaTime, 0, 0);

            if (IsControlPressed(0, 110))
                RotateCamera(-RotationSpeed * deltaTime, 0, 0);

            if (IsControlPressed(0, 108))
                RotateCamera(0, 0, RotationSpeed * deltaTime);

            if (IsControlPressed(0, 109))
                RotateCamera(0, 0, -RotationSpeed * deltaTime);
            
            //fov
            if (IsControlPressed(0, 96))
                ChangeFov(-FovSpeed * deltaTime);

            if (IsControlPressed(0, 97))
                ChangeFov(FovSpeed * deltaTime);
        }

        private void MoveCamera(Vector3 direction, float distance)
        {
            var position = _camera.Position;
            var nextPosition = position + direction * distance;

            _camera.Position = nextPosition;
        }

        private void RotateCamera(float pitch, float roll, float yaw)
        {
            var rotation = _camera.Rotation;
            var nextRotation = rotation + new Vector3(pitch, roll, yaw);

            _camera.Rotation = nextRotation;
        }

        private void ChangeFov(float value)
        {
            var fov = _camera.FieldOfView;
            
            var nextFov = fov + value;
            if (nextFov > MaxFov) nextFov = MaxFov;
            else if (nextFov < MinFov) nextFov = MinFov;

            _camera.FieldOfView = nextFov;
        }
    }
}