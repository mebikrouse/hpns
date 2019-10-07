using HPNS.InteractivityV2.Core;
using HPNS.InteractivityV2.Core.Task;

namespace InteractivityTests.Core
{
    public interface ITest : ITask
    {
        string Name { get; }
    }
}