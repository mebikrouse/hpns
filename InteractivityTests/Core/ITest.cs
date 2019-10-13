using HPNS.Interactivity.Core.Task;

namespace InteractivityTests.Core
{
    public interface ITest : ITask
    {
        string TestName { get; }
    }
}