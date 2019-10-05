using System;

namespace HPNS.InteractivityV2.Core
{
    public interface ICondition
    {
        ConditionState CurrentState { get; }
        event EventHandler ConditionDidBreak;
        event EventHandler ConditionDidRecover;
        void Abort();
    }
}