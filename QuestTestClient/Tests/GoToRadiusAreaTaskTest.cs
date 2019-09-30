using System;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Tasks;

namespace QuestTestClient.Tests
{
    public class GoToRadiusAreaTaskTest : TaskBase
    {
        private const float AREA_RADIUS = 25f;
        private static readonly Vector3 AREA_CENTER = new Vector3(55.84977f, -1572.498f, 28.95687f);
        
        private ITask _currentTask;
        
        protected override void ExecuteStarting()
        {
            var goToRadiusAreaTask = new GoToRadiusAreaTask(AREA_CENTER, AREA_RADIUS);
            goToRadiusAreaTask.TaskDidEnd += CurrentTaskTaskDidEnd;
            goToRadiusAreaTask.Start();

            _currentTask = goToRadiusAreaTask;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
            NotifyTaskDidEnd();
        }
    }
}