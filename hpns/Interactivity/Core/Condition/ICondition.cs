using System;
using HPNS.Interactivity.Core.Activity;

namespace HPNS.Interactivity.Core.Condition
{
    public interface ICondition : IActivity
    {
        ConditionState ConditionState { get; }
        event EventHandler DidBreak;
        event EventHandler DidRecover;
    }
}