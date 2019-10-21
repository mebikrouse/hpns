using System.Threading.Tasks;
using Dialogues.Data;
using HPNS.Core;
using HPNS.Interactivity.Core.Activity;
using HPNS.Interactivity.Core.Data;
using static CitizenFX.Core.Native.API;

namespace Dialogues.Activities
{
    public class PedCameraActivity : ActivityBase
    {
        private CameraConfiguration _cameraConfiguration;
        private int _cameraHandle;
        
        public IParameter<int> PedHandle;

        public PedCameraActivity(CameraConfiguration cameraConfiguration) : base(nameof(PedCameraActivity))
        {
            _cameraConfiguration = cameraConfiguration;
        }

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            var pedHandle = PedHandle.GetValue();

            var bonePosition = GetPedBoneCoords(pedHandle, 31086, 0, 0, 0);
            var boneOffset = GetOffsetFromEntityGivenWorldCoords(pedHandle, bonePosition.X, bonePosition.Y, bonePosition.Z);

            var offset = boneOffset + _cameraConfiguration.Offset;
            var cameraPosition = GetOffsetFromEntityInWorldCoords(pedHandle, offset.X, offset.Y, offset.Z);

            var pedRotation = GetEntityRotation(pedHandle, 2);
            var cameraRotation = pedRotation + _cameraConfiguration.Rotation;

            _cameraHandle = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
            SetCamCoord(_cameraHandle, cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
            SetCamRot(_cameraHandle, cameraRotation.X, cameraRotation.Y, cameraRotation.Z, 2);
            SetCamFov(_cameraHandle, _cameraConfiguration.FieldOfView);
            
            RenderScriptCams(true, false, 0, true, true);
        }

        protected override void ExecuteAbort()
        {
            RenderScriptCams(false, false, 0, true, true);
            DestroyCam(_cameraHandle, false);
        }

        protected override void ExecuteReset() { }
    }
}