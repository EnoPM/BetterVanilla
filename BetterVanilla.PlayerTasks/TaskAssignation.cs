using System;
using System.Linq;
using BetterVanilla.Extensions;
using BetterVanilla.PlayerTasks.Core;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;

namespace BetterVanilla.PlayerTasks;

public sealed class TaskAssignation : ITasksHolder<NormalPlayerTask>
{
    private int CommonTasksCount { get; set; }
    private int LongTasksCount { get; set; }
    private int ShortTasksCount { get; set; }

    public List<NormalPlayerTask> CommonTasks { get; }
    public List<NormalPlayerTask> LongTasks { get; }
    public List<NormalPlayerTask> ShortTasks { get; }

    private ShipStatus Map { get; }
    private TaskAssignationOptions Options { get; }

    public TaskAssignation(ShipStatus map, TaskAssignationOptions options)
    {
        Map = map;
        Options = options;

        CommonTasksCount = Options.CommonTasksCount;
        LongTasksCount = Options.LongTasksCount;
        ShortTasksCount = Options.ShortTasksCount;

        if (CommonTasksCount + LongTasksCount + ShortTasksCount == 0)
        {
            ShortTasksCount = 1;
        }

        CommonTasks = GetMapTasks(Map.CommonTasks, NormalPlayerTask.TaskLength.Common);
        LongTasks = GetMapTasks(Map.LongTasks, NormalPlayerTask.TaskLength.Long);
        ShortTasks = GetMapTasks(Map.ShortTasks, NormalPlayerTask.TaskLength.Short);
    }

    public void Execute()
    {
        Map.numScans = 0;

        AssignTaskIndexes();
        ApplyTasksLengthOverride();

        CommonTasks.Shuffle();
        LongTasks.Shuffle();
        ShortTasks.Shuffle();

        var allPlayers = GameData.Instance.AllPlayers;
        var usedTaskTypes = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();
        var tasks = new Il2CppSystem.Collections.Generic.List<byte>();

        var commonTasks = CommonTasks.ToIl2CppList();
        var longTasks = LongTasks.ToIl2CppList();
        var shortTasks = ShortTasks.ToIl2CppList();

        var commonStart = 1;
        var longStart = 0;
        var shortStart = 0;

        Map.AddTasksFromList(ref commonStart, CommonTasksCount, tasks, usedTaskTypes, commonTasks);

        for (byte playerId = 0; playerId < allPlayers.Count; ++playerId)
        {
            usedTaskTypes.Clear();
            tasks.RemoveRange(CommonTasksCount, tasks.Count - CommonTasksCount);
            Map.AddTasksFromList(ref longStart, LongTasksCount, tasks, usedTaskTypes, longTasks);
            Map.AddTasksFromList(ref shortStart, ShortTasksCount, tasks, usedTaskTypes, shortTasks);

            var player = allPlayers[playerId];
            if (player == null || player.Object == null ||
                player.Object.GetComponent<DummyBehaviour>().enabled) continue;
            var array = tasks.ToArray().ToArray();
            player.RpcSetTasks(array);
        }

        PlayerControl.LocalPlayer.cosmetics.SetAsLocalPlayer();
    }

    private void AssignTaskIndexes()
    {
        var taskIndex = 0;
        AssignIndexesToTasks(ref taskIndex, CommonTasks);
        AssignIndexesToTasks(ref taskIndex, LongTasks);
        AssignIndexesToTasks(ref taskIndex, ShortTasks);
    }

    private void ApplyTasksLengthOverride()
    {
        var mapping = PlayerTasksManager.Instance.GetMapping(Options.MapId);

        PlayerTasksManager.Instance.LengthOverride.Refresh(Options.DefineCommonAsNonCommon);
        PlayerTasksManager.Instance.LengthOverride.Apply(mapping);
        PlayerTasksManager.Instance.LengthOverride.Apply(this);

        CommonTasksCount = Math.Min(CommonTasksCount, CommonTasks.Count);
        LongTasksCount = Math.Min(LongTasksCount, LongTasks.Count);
        ShortTasksCount = Math.Min(ShortTasksCount, ShortTasks.Count);
    }

    private static void AssignIndexesToTasks(ref int index, List<NormalPlayerTask> tasks)
    {
        foreach (var task in tasks)
        {
            task.Index = index++;
        }
    }

    private static List<NormalPlayerTask> GetMapTasks(Il2CppReferenceArray<NormalPlayerTask> tasks,
        NormalPlayerTask.TaskLength taskLength)
    {
        var results = tasks.ToList();
        foreach (var result in results)
        {
            result.Length = taskLength;
        }

        return results;
    }
}