using System;

namespace HPNS.Tasks.Core
{
    public interface IState
    {
        StateState CurrentState { get; }
        bool IsValid { get; }
        event EventHandler StateDidBreak;
        event EventHandler StateDidRecover;
        void Start();
        void Stop();
    }
}