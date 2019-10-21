using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Core;
using static CitizenFX.Core.Native.API;

namespace CameraTool
{
    public class Bootstrap : BaseScript
    {
        private Camera _camera;
        private CameraControl _cameraControl;
        private UpdateObjectPool _updateObjectPool;
        private bool _toggled;
        
        public Bootstrap()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }
        
        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            _updateObjectPool = new UpdateObjectPool(0);
            _updateObjectPool.Start();

            _cameraControl = new CameraControl();

            RegisterCommand("start", new Action<int, List<object>, string>((source, args, raw) =>
            {
                PrintToChat("Starting new camera...");
                
                if (_camera != null)
                {
                    _toggled = false;
                    RenderScriptCams(false, false, 0, true, true);
                    
                    _updateObjectPool.RemoveUpdateObject(_cameraControl);
                    
                    _camera.Delete();
                    _camera = null;

                    FreezeEntityPosition(Game.PlayerPed.Handle, false);
                }

                var cameraHandle = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
                _camera = new Camera(cameraHandle);
                _camera.Position = GetPlayerHeadPosition() + Game.PlayerPed.ForwardVector * 1.5f;
                _camera.PointAt(GetPlayerHeadPosition());
                _camera.StopPointing();
                
                _toggled = true;
                RenderScriptCams(true, false, 0, true, true);

                _cameraControl.SetCamera(_camera);
                _updateObjectPool.AddUpdateObject(_cameraControl);

                FreezeEntityPosition(Game.PlayerPed.Handle, true);

            }), false);
            
            RegisterCommand("toggle", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (_camera == null) return;

                if (!_toggled)
                {
                    _toggled = true;
                    RenderScriptCams(true, false, 0, true, true);
                }
                else
                {
                    _toggled = false;
                    RenderScriptCams(false, false, 0, true, true);
                }
            }), false);
            
            RegisterCommand("stop", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (_camera == null) return;
                
                PrintToChat("Stopping camera...");
                
                _toggled = false;
                RenderScriptCams(false, false, 0, true, true);
                
                _updateObjectPool.RemoveUpdateObject(_cameraControl);
                    
                _camera.Delete();
                _camera = null;

                FreezeEntityPosition(Game.PlayerPed.Handle, false);

            }), false);
            
            RegisterCommand("save", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (_camera == null) return;

                PrintToChat("Saving camera configuration...");

                var offset = GetOffsetFromEntityGivenWorldCoords(Game.PlayerPed.Handle, _camera.Position.X,
                    _camera.Position.Y, _camera.Position.Z);
                var headPosition = GetPlayerHeadPosition();
                var headOffset = GetOffsetFromEntityGivenWorldCoords(Game.PlayerPed.Handle, headPosition.X,
                    headPosition.Y, headPosition.Z);

                var result = offset - headOffset;
                Debug.WriteLine($"Camera offset: {result.X}f, {result.Y}f, {result.Z}f");

                var rotation = _camera.Rotation - Game.PlayerPed.Rotation;
                Debug.WriteLine($"Camera rotation: {rotation.X}f, {rotation.Y}f, {rotation.Z}f");

                var fov = _camera.FieldOfView;
                Debug.WriteLine($"Camera fov: {fov}f");
                
            }), false);
        }

        private static void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[CameraTool]", message}
            });
        }
        
        private static Vector3 GetPlayerHeadPosition()
        {
            return GetPedBoneCoords(Game.PlayerPed.Handle, 31086, 0, 0, 0);
        }
    }
}