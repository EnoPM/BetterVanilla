using System.Collections.Generic;

namespace BetterVanilla.PlayerTasks.Core;

public interface ITasksHolder<TTask>
{
    public List<TTask> CommonTasks { get; }
    public List<TTask> LongTasks { get; }
    public List<TTask> ShortTasks { get; }
}