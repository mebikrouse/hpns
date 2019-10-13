using System.Threading.Tasks;

namespace HPNS.Core
{
    public static class TaskHelper
    {
        public static Task CompletedTask = Task.FromResult(false);
    }
}