using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace HPNS.Core.Environment
{
    public class Pickup : IObject
    {
        private const float PLAYER_HEIGHT = 2;
        
        private int _entityHandle;
        private float _radius;

        public event EventHandler PlayerPickedUp;
        
        public Pickup(int entityHandle, float radius)
        {
            _entityHandle = entityHandle;
            _radius = radius;
        }
        
        public void Update(float deltaTime)
        {
            var entityPosition = GetEntityCoords(_entityHandle, !IsEntityDead(_entityHandle));
            if (GetDistance(Game.PlayerPed.Position, entityPosition) > _radius) return;

            PlaySoundFrontend(-1, "PICK_UP", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
            
            PlayerPickedUp?.Invoke(this, EventArgs.Empty);
            World.Current.ObjectManager.DestroyObject(this);
        }

        public void OnCreate() { }

        public void OnDestroy()
        {
            DeleteEntity(ref _entityHandle);
        }

        private float GetDistance(Vector3 playerCenter, Vector3 objectCenter)
        {
            var a = playerCenter + Vector3.Up * PLAYER_HEIGHT / 2f;
            var b = playerCenter - Vector3.Up * PLAYER_HEIGHT / 2f;

            if (objectCenter.Y > a.Y || objectCenter.Y < b.Y)
                return Vector3.Distance(objectCenter, objectCenter.Y > a.Y ? a : b);
            
            var dx = playerCenter.X - objectCenter.X;
            var dy = playerCenter.Y - objectCenter.Y;
                
            return (float) Math.Sqrt(dx * dx + dy * dy);
        }
    }
}