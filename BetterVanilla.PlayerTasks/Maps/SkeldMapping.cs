using System.Collections.Generic;
using BetterVanilla.PlayerTasks.Core;

namespace BetterVanilla.PlayerTasks.Maps;

public sealed class SkeldMapping() : TaskTypeMappingBase((byte)MapNames.Skeld)
{
    public override List<TaskTypes> CommonTasks { get; } =
    [
        TaskTypes.SwipeCard,
        TaskTypes.FixWiring
    ];

    public override List<TaskTypes> LongTasks { get; } =
    [
        TaskTypes.ClearAsteroids,
        TaskTypes.AlignEngineOutput,
        TaskTypes.SubmitScan,
        TaskTypes.InspectSample,
        TaskTypes.FuelEngines,
        TaskTypes.StartReactor,
        TaskTypes.EmptyChute,
        TaskTypes.EmptyGarbage
    ];

    public override List<TaskTypes> ShortTasks { get; } =
    [
        TaskTypes.UploadData,
        TaskTypes.CalibrateDistributor,
        TaskTypes.ChartCourse,
        TaskTypes.CleanO2Filter,
        TaskTypes.UnlockManifolds,
        TaskTypes.UploadData,
        TaskTypes.StabilizeSteering,
        TaskTypes.UploadData,
        TaskTypes.PrimeShields,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.VentCleaning
    ];
}