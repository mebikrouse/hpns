using System;
using HPNS.InteractivityV2.Core.Activity;

namespace HPNS.InteractivityV2.Core.Task
{
    public interface ITask : IActivity
    {
        event EventHandler DidEnd;
    }
}