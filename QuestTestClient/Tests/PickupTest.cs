using System;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core;

using World = HPNS.Core.World;
using Pickup = HPNS.Core.Environment.Pickup;

using static CitizenFX.Core.Native.API;

namespace QuestTestClient.Tests
{
    public class PickupTest : TaskBase
    {
        private Pickup _pickup;
        
        protected override async void ExecuteStarting()
        {
            var modelHash = (uint) GetHashKey("prop_poly_bag_01");
            await Utility.LoadObject(modelHash);

            var pickupPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3f;
            var entityHandle = CreateObject((int) modelHash, pickupPosition.X, pickupPosition.Y, pickupPosition.Z, true, true,
                true);
            PlaceObjectOnGroundProperly(entityHandle);
            SetEntityNoCollisionEntity(Game.PlayerPed.Handle, entityHandle, false);

            var pickup = World.Current.ObjectManager.AddObject(new Pickup(entityHandle, 0.5f));
            pickup.PlayerPickedUp += PickupOnPlayerPickedUp;

            _pickup = pickup;
        }

        protected override void ExecuteAborting()
        {
            _pickup.PlayerPickedUp -= PickupOnPlayerPickedUp;
            World.Current.ObjectManager.DestroyObject(_pickup);
        }

        private void PickupOnPlayerPickedUp(object sender, EventArgs e)
        {
            _pickup.PlayerPickedUp -= PickupOnPlayerPickedUp;
            NotifyTaskDidEnd();
        }
    }
}