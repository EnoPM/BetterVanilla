using System.Collections.Generic;
using BetterVanilla.PlayerTasks.Core;

namespace BetterVanilla.PlayerTasks.Maps;

public sealed class PolusMapping() : TaskTypeMappingBase((byte)MapNames.Polus)
{
    public override List<TaskTypes> CommonTasks { get; } =
    [
        TaskTypes.SwipeCard,
        TaskTypes.InsertKeys,
        TaskTypes.ScanBoardingPass,
        TaskTypes.FixWiring
    ];

    public override List<TaskTypes> LongTasks { get; } =
    [
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.StartReactor,
        TaskTypes.FuelEngines,
        TaskTypes.OpenWaterways,
        TaskTypes.InspectSample,
        TaskTypes.ReplaceWaterJug,
        TaskTypes.FixWeatherNode,
        TaskTypes.FixWeatherNode,
        TaskTypes.FixWeatherNode,
        TaskTypes.FixWeatherNode,
        TaskTypes.RebootWifi
    ];

    public override List<TaskTypes> ShortTasks { get; } =
    [
        TaskTypes.MonitorOxygen,
        TaskTypes.UnlockManifolds,
        TaskTypes.StoreArtifacts,
        TaskTypes.FillCanisters,
        TaskTypes.EmptyGarbage,
        TaskTypes.ChartCourse,
        TaskTypes.SubmitScan,
        TaskTypes.ClearAsteroids,
        TaskTypes.FixWeatherNode,
        TaskTypes.FixWeatherNode,
        TaskTypes.AlignTelescope,
        TaskTypes.RepairDrill,
        TaskTypes.RecordTemperature,
        TaskTypes.RecordTemperature
    ];
}