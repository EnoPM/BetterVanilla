using System.Collections.Generic;
using BetterVanilla.PlayerTasks.Core;

namespace BetterVanilla.PlayerTasks.Maps;

public sealed class MiraHqMapping() : TaskTypeMappingBase((byte)MapNames.MiraHQ)
{
    public override List<TaskTypes> CommonTasks { get; } =
    [
        TaskTypes.FixWiring,
        TaskTypes.EnterIdCode
    ];

    public override List<TaskTypes> LongTasks { get; } =
    [
        TaskTypes.SubmitScan,
        TaskTypes.ClearAsteroids,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.WaterPlants,
        TaskTypes.StartReactor,
        TaskTypes.DivertPower
    ];

    public override List<TaskTypes> ShortTasks { get; } =
    [
        TaskTypes.ChartCourse,
        TaskTypes.CleanO2Filter,
        TaskTypes.FuelEngines,
        TaskTypes.AssembleArtifact,
        TaskTypes.SortSamples,
        TaskTypes.PrimeShields,
        TaskTypes.EmptyGarbage,
        TaskTypes.MeasureWeather,
        TaskTypes.DivertPower,
        TaskTypes.BuyBeverage,
        TaskTypes.ProcessData,
        TaskTypes.RunDiagnostics,
        TaskTypes.UnlockManifolds,
        TaskTypes.VentCleaning
    ];
}