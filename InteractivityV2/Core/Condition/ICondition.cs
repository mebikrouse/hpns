using System;
using HPNS.InteractivityV2.Core.Activity;

namespace HPNS.InteractivityV2.Core.Condition
{
    public interface ICondition : IActivity
    {
        ConditionState ConditionState { get; }
        event EventHandler DidBreak;
        event EventHandler DidRecover;
    }
}