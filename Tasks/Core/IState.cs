using System;

namespace HPNS.Tasks.Core
{
    public interface IState
    {
        bool IsValid { get; }
        event EventHandler StateDidBreak;
        event EventHandler StateDidRecover;
        void Start();
        void Stop();
    }
}