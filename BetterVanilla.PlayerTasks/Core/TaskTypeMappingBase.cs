using System.Collections.Generic;

namespace BetterVanilla.PlayerTasks.Core;

public abstract class TaskTypeMappingBase(byte mapId) : ITasksHolder<TaskTypes>
{
    public byte MapId { get; } = mapId;

    public abstract List<TaskTypes> CommonTasks { get; }
    public abstract List<TaskTypes> LongTasks { get; }
    public abstract List<TaskTypes> ShortTasks { get; }
}