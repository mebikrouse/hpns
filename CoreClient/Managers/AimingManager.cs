using System;
using CitizenFX.Core;
using HPNS.Core;
using static CitizenFX.Core.Native.API;

namespace HPNS.CoreClient.Managers
{
    public class AimingManager : IUpdateObject
    {
        private int? _prevEntityHandle = null;

        public event EventHandler<int> PlayerDidStartAimingAtEntity;
        public event EventHandler<int> PlayerDidStopAimingAtEntity;

        public bool IsPlayerAimingAtEntity(int entityHandle)
        {
            return _prevEntityHandle != null && _prevEntityHandle.Value == entityHandle;
        }
        
        public void Update(float deltaTime)
        {
            var entityHandle = 0;
            if (GetEntityPlayerIsFreeAimingAt(Game.Player.Handle, ref entityHandle))
            {
                if (_prevEntityHandle == null)
                {
                    PlayerDidStartAimingAtEntity?.Invoke(this, entityHandle);
                    _prevEntityHandle = entityHandle;
                } 
                else if (_prevEntityHandle != entityHandle)
                {
                    PlayerDidStopAimingAtEntity?.Invoke(this, _prevEntityHandle.Value);
                    
                    PlayerDidStartAimingAtEntity?.Invoke(this, entityHandle);
                    _prevEntityHandle = entityHandle;
                }
            } 
            else if (_prevEntityHandle != null)
            {
                PlayerDidStopAimingAtEntity?.Invoke(this, _prevEntityHandle.Value);
                _prevEntityHandle = null;
            }
        }
    }
}