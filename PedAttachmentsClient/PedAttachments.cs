using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;

using static CitizenFX.Core.Native.API;

namespace PedAttachmentsClient
{
    public class PedAttachments : BaseScript
    {
        private bool? _running;
        
        public PedAttachments()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            Debug.WriteLine("KEK");
            
            RegisterCommand("at", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                if (_running == true) _running = false; 
                
                while (_running != null)
                    await Delay(500);
                
                var boneId = 0;
                var vertexIndex = 0;
                var fixedRot = false;
                if (args.Count != 4) return;
                if (!int.TryParse(args[1].ToString(), out boneId)) return;
                if (!int.TryParse(args[2].ToString(), out vertexIndex)) return;
                if (!bool.TryParse(args[3].ToString(), out fixedRot)) return;
                
                var propModelHash = (uint) GetHashKey(args[0].ToString());
                if (!IsModelInCdimage(propModelHash)) return;
                
                var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3;
                var pedHash = (uint) GetHashKey("a_m_m_ktown_01");
                var pedHandle = await CreatePedAtPosition(pedPosition, pedHash);
                var heading = Game.PlayerPed.Heading + 180;
                SetEntityHeading(pedHandle, heading);
                TaskSetBlockingOfNonTemporaryEvents(pedHandle, true);
                PlaceObjectOnGroundProperly((int) pedHash);

                await LoadObject(propModelHash);
                
                var propHandle = CreateObject((int) propModelHash, 0, 0, 0, true, true, true);
                
                var boneIndex = GetPedBoneIndex(pedHandle, boneId);
                AttachEntityToEntity(propHandle, pedHandle, boneIndex, 
                    0f, 0f, 0f, 0, 0, 0, true, 
                    false, false, false, vertexIndex, fixedRot);
                
                _running = true;
                await WaitForPlayerInput(pedHandle, propHandle, boneIndex, vertexIndex, boneId, fixedRot);

            }), false);

            RegisterCommand("st",
                new Action<int, List<object>, string>((source, args, raw) => { _running = false; }), false);
        }
        
        private async Task WaitForPlayerInput(int pedHandle, int propHandle, int boneIndex, int vertexIndex, int boneId, bool fixedRot)
        {
            var p = new Vector3(0, 0, 0);
            var r = new Vector3(0, 0, 0);
            var rotating = false;
            const float distance = 0.01f;
            const float rotation = 1.0f;
            while (_running == true)
            {
                await Delay(0);

                if (IsControlJustReleased(0, 21))
                {
                    rotating = !rotating;
                }
                else if (IsControlJustReleased(0, (int) Control.PhoneDown))
                {
                    if (rotating) r += new Vector3(0, 0, -rotation);
                    else p += new Vector3(0, 0, -distance);
                } 
                else if (IsControlJustReleased(0, (int) Control.PhoneUp))
                {
                    if (rotating) r += new Vector3(0, 0, rotation);
                    else p += new Vector3(0, 0, distance);
                } 
                else if (IsControlJustReleased(0, (int) Control.PhoneLeft))
                {
                    if (rotating) r += new Vector3(-rotation, 0, 0);
                    else p += new Vector3(-distance, 0, 0);
                } 
                else if (IsControlJustReleased(0, (int) Control.PhoneRight))
                {
                    if (rotating) r += new Vector3(rotation, 0, 0);
                    else p += new Vector3(distance, 0, 0);
                } 
                else if (IsControlJustReleased(0, 96))
                {
                    if (rotating) r += new Vector3(0, rotation, 0);
                    else p += new Vector3(0, distance, 0);
                }
                else if (IsControlJustReleased(0, 97))
                {
                    if (rotating) r += new Vector3(0, -rotation, 0);
                    else p += new Vector3(0, -distance, 0);
                }
                else if (IsControlJustReleased(0, 39))
                {
                    vertexIndex--;
                    if (vertexIndex < 0) vertexIndex = 0;
                }
                else if (IsControlJustReleased(0, 40))
                {
                    vertexIndex++;
                }
                
                DetachEntity(propHandle, false, false);
                AttachEntityToEntity(propHandle, pedHandle, boneIndex, 
                    p.X, p.Y, p.Z, r.X, r.Y, r.Z, true, 
                    false, false, false, vertexIndex, fixedRot);
            }

            Debug.WriteLine($"Attachment position: {p.X}f, {p.Y}f, {p.Z}f");
            Debug.WriteLine($"Attachment rotation: {r.X}f, {r.Y}f, {r.Z}f");
            Debug.WriteLine($"Bone id: {boneId}");
            Debug.WriteLine($"Vertex index: {vertexIndex}");
            Debug.WriteLine($"Fixed rot: {fixedRot}");

            DetachEntity(propHandle, false, false);
            DeletePed(ref pedHandle);
            DeleteObject(ref propHandle);

            _running = null;
        }

        private static async Task LoadObject(uint modelHash)
        {
            RequestModel(modelHash);

            while (!HasModelLoaded(modelHash))
                await Delay(500);
        }
        
        private static async Task<int> CreatePedAtPosition(Vector3 position, uint pedHash)
        {
            while (!HasModelLoaded(pedHash))
            {
                RequestModel(pedHash);
                await Delay(500);
            }
            
            var ped = CreatePed(0, pedHash, position.X, position.Y, position.Z, 0f, true, false);
            return ped;
        }
    }
}