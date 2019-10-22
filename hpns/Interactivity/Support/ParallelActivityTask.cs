using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Activity;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Support
{
    public class ParallelActivityTask : TaskBase
    {
        private ITask _task;
        private List<IActivity> _activities;
        
        public ParallelActivityTask(ITask task, IEnumerable<IActivity> activities)
        {
            _task = task;
            _activities = new List<IActivity>(activities);
        }
        
        public ParallelActivityTask(ITask task, params IActivity[] activities)
        {
            _task = task;
            _activities = new List<IActivity>(activities);
        }
        
        protected override async Task ExecutePrepare()
        {
            var prepareTasks = new List<Task>();
            
            prepareTasks.Add(_task.Prepare());
            
            foreach (var activity in _activities)
                prepareTasks.Add(activity.Prepare());

            await Task.WhenAll(prepareTasks);
        }

        protected override void ExecuteStart()
        {
            foreach (var activity in _activities)
                activity.Start();
            
            _task.DidEnd += TaskOnDidEnd;
            _task.Start();
        }

        protected override void ExecuteAbort()
        {
            _task.DidEnd -= TaskOnDidEnd;
            _task.Abort();
            
            AbortActivities();
        }

        protected override void ExecuteReset()
        {
            _task.Reset();
            
            foreach (var activity in _activities)
                activity.Reset();
        }

        private void TaskOnDidEnd(object sender, EventArgs e)
        {
            Debug.WriteLine($"Main task did end. Aborting all activities.");
            
            _task.DidEnd -= TaskOnDidEnd;
            AbortActivities();
            
            NotifyTaskDidEnd();
        }

        private void AbortActivities()
        {
            foreach (var activity in _activities)
                activity.Abort();
        }
    }
}