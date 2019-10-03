using System;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Tasks;

namespace QuestTestClient.Tests
{
    public class PlayAnimTaskTest : TaskBase
    {
        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 5f;
            var pedHandle = await Utility.CreateRandomPed(pedPosition, Game.PlayerPed.Heading - 180f);

            var playAnimTask = new PlayAnimTask(pedHandle, "mp_am_hold_up", "holdup_victim_20s", 23000);
            playAnimTask.TaskDidEnd += CurrentTaskTaskOnTaskDidEnd;
            playAnimTask.Start();
            
            _currentTask = playAnimTask;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskOnTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskTaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskOnTaskDidEnd;
            NotifyTaskDidEnd();
        }
    }
}