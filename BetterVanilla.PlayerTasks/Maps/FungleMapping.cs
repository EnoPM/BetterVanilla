using System.Collections.Generic;
using BetterVanilla.PlayerTasks.Core;

namespace BetterVanilla.PlayerTasks.Maps;

public sealed class FungleMapping() : TaskTypeMappingBase((byte)MapNames.Fungle)
{
    public override List<TaskTypes> CommonTasks { get; } =
    [
        TaskTypes.CollectSamples,
        TaskTypes.EnterIdCode,
        TaskTypes.ReplaceParts,
        TaskTypes.RoastMarshmallow
    ];

    public override List<TaskTypes> LongTasks { get; } =
    [
        TaskTypes.CatchFish,
        TaskTypes.CollectVegetables,
        TaskTypes.ExtractFuel,
        TaskTypes.HelpCritter,
        TaskTypes.HoistSupplies,
        TaskTypes.PolishGem,
        TaskTypes.MineOres,
        TaskTypes.ReplaceWaterJug,
        TaskTypes.WaterPlants
    ];

    public override List<TaskTypes> ShortTasks { get; } =
    [
        TaskTypes.AssembleArtifact,
        TaskTypes.BuildSandcastle,
        TaskTypes.CollectShells,
        TaskTypes.CrankGenerator,
        TaskTypes.EmptyGarbage,
        TaskTypes.EmptyGarbage,
        TaskTypes.FixAntenna,
        TaskTypes.FixWiring,
        TaskTypes.LiftWeights,
        TaskTypes.MonitorMushroom,
        TaskTypes.PlayVideogame,
        TaskTypes.RecordTemperature,
        TaskTypes.RecordTemperature,
        TaskTypes.RecordTemperature,
        TaskTypes.TestFrisbee,
        TaskTypes.TuneRadio
    ];
}