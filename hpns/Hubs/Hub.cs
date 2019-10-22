using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.CoreClient;
using Hubs.Data;

namespace Hubs
{
    public class Hub : IUpdateObject
    {
        private HubConfiguration _configuration;
        private List<int> _peds = new List<int>();
        private bool _hidden = true;
        private float _timeout;
        
        public Hub(HubConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void Update(float deltaTime)
        {
            var playerPosition = Game.PlayerPed.Position;
            if (Vector3.Distance(playerPosition, _configuration.HubCenter) > _configuration.HubRadius)
            {
                _timeout += deltaTime;
                if (_timeout < _configuration.Timeout) return;

                if (_hidden) return;
                
                Debug.WriteLine("Removing peds");
                
                _hidden = true;
                DeletePeds();
            }
            else
            {
                _timeout = 0;
                if (!_hidden) return;

                Debug.WriteLine("Spawning peds");
                
                _hidden = false;
                _ = SpawnPeds();
            }
        }

        private void DeletePeds()
        {
            foreach (var ped in _peds)
                Utility.RemovePed(ped);
        }

        private async Task SpawnPeds()
        {
            var spawnTasks = new List<Task<int>>();
            foreach (var spawnConfiguration in _configuration.SpawnConfigurations)
                spawnTasks.Add(Utility.CreatePedAtPositionAsync(spawnConfiguration.Position, spawnConfiguration.Heading,
                    spawnConfiguration.PedConfiguration.Model, false));

            foreach (var spawnTask in spawnTasks)
            {
                var pedHandle = await spawnTask;
                _peds.Add(pedHandle);
            }
        }
    }
}