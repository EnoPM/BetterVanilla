using System;
using System.Collections.Generic;
using System.Linq;
using BetterVanilla.PlayerTasks.Core;

namespace BetterVanilla.PlayerTasks;

public sealed class TaskLengthOverride
{
    private HashSet<TaskTypes> CommonTasks { get; } = [];
    private HashSet<TaskTypes> LongTasks { get; } = [];
    private HashSet<TaskTypes> ShortTasks { get; } = [];

    public void Refresh(bool defineCommonTasksAsNonCommon)
    {
        if (defineCommonTasksAsNonCommon)
        {
            SetAsLong(TaskTypes.FixWiring);
            SetAsShort(TaskTypes.SwipeCard);
            SetAsShort(TaskTypes.InsertKeys);
            SetAsShort(TaskTypes.ScanBoardingPass);
            SetAsShort(TaskTypes.EnterIdCode);
            SetAsShort(TaskTypes.CollectSamples);
            SetAsShort(TaskTypes.ReplaceParts);
            SetAsShort(TaskTypes.RoastMarshmallow);
        }
        else
        {
            SetAsCommon(TaskTypes.FixWiring);
            SetAsCommon(TaskTypes.SwipeCard);
            SetAsCommon(TaskTypes.InsertKeys);
            SetAsCommon(TaskTypes.ScanBoardingPass);
            SetAsCommon(TaskTypes.EnterIdCode);
            SetAsCommon(TaskTypes.CollectSamples);
            SetAsCommon(TaskTypes.ReplaceParts);
            SetAsCommon(TaskTypes.RoastMarshmallow);
        }
    }

    public void Apply(ITasksHolder<TaskTypes> holder) => Apply(holder, x => x);

    public void Apply(ITasksHolder<NormalPlayerTask> holder) => Apply(holder, x => x.TaskType);

    private void Apply<T>(ITasksHolder<T> holder, Func<T, TaskTypes> taskTypeSelector)
    {
        foreach (var taskType in CommonTasks)
        {
            holder.CommonTasks.AddRange(PickTasksFrom(taskType, taskTypeSelector, holder.LongTasks, holder.ShortTasks));
        }

        foreach (var taskType in LongTasks)
        {
            holder.LongTasks.AddRange(PickTasksFrom(taskType, taskTypeSelector, holder.CommonTasks, holder.ShortTasks));
        }

        foreach (var taskType in ShortTasks)
        {
            holder.ShortTasks.AddRange(PickTasksFrom(taskType, taskTypeSelector, holder.CommonTasks, holder.LongTasks));
        }
    }
    
    private void SetAsCommon(TaskTypes taskType) => SetAs(taskType, CommonTasks, LongTasks, ShortTasks);
    private void SetAsLong(TaskTypes taskType) => SetAs(taskType, LongTasks, CommonTasks, ShortTasks);
    private void SetAsShort(TaskTypes taskType) => SetAs(taskType, ShortTasks, CommonTasks, LongTasks);
    
    private static void SetAs(TaskTypes taskType, HashSet<TaskTypes> to, params HashSet<TaskTypes>[] from)
    {
        foreach (var set in from)
        {
            set.Remove(taskType);
        }
        to.Add(taskType);
    }

    private static T[] PickTasksFrom<T>(TaskTypes taskType, Func<T, TaskTypes> taskTypeSelector,
        params List<T>[] taskLists)
    {
        var results = new List<T>();

        foreach (var list in taskLists)
        {
            var matches = list.Where(x => taskTypeSelector(x) == taskType).ToArray();
            foreach (var task in matches)
            {
                list.Remove(task);
            }
            results.AddRange(matches);
        }
        
        return results.ToArray();
    }
}