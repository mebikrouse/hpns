using System;
using System.Collections.Generic;

namespace Dialogues.Data
{
    public class Configuration
    {
        private Random _random = new Random();
        
        public List<CameraConfiguration> Configurations { get; set; }

        public CameraConfiguration GetRandomCameraConfiguration()
        {
            return Configurations[_random.Next(0, Configurations.Count)];
        }
    }
}