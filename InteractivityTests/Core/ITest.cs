using HPNS.InteractivityV2.Core;

namespace InteractivityTests.Core
{
    public interface ITest : ITask
    {
        string Name { get; }
    }
}