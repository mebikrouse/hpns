using System;
using HPNS.Interactivity.Core.Activity;

namespace HPNS.Interactivity.Core.Task
{
    public interface ITask : IActivity
    {
        event EventHandler DidEnd;
    }
}