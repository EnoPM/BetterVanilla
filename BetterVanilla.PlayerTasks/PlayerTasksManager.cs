using System;
using System.Collections.Generic;
using System.Linq;
using BetterVanilla.PlayerTasks.Core;
using BetterVanilla.PlayerTasks.Maps;

namespace BetterVanilla.PlayerTasks;

public sealed class PlayerTasksManager
{
    public static PlayerTasksManager Instance { get; } = new();
    
    private List<TaskTypeMappingBase> Mappings { get; } = [];
    public TaskLengthOverride LengthOverride { get; } = new();

    private PlayerTasksManager()
    {
        RegisterMapping<SkeldMapping>();
        RegisterMapping<MiraHqMapping>();
        RegisterMapping<PolusMapping>();
        RegisterMapping<AirshipMapping>();
        RegisterMapping<FungleMapping>();
    }

    private void RegisterMapping<TMapping>() where TMapping : TaskTypeMappingBase, new() => Mappings.Add(new TMapping());

    public TaskTypeMappingBase GetMapping(byte mapId)
    {
        var result = Mappings.FirstOrDefault(x => x.MapId == mapId);
        return result ?? throw new Exception($"Mapping for mapId {mapId} not found");
    }

    public static void AssignTasks(ShipStatus shipStatus, TaskAssignationOptions options)
    {
        var assignation = new TaskAssignation(shipStatus, options);
        assignation.Execute();
    }
}