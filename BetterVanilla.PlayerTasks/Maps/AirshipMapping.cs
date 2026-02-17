using System.Collections.Generic;
using BetterVanilla.PlayerTasks.Core;

namespace BetterVanilla.PlayerTasks.Maps;

public sealed class AirshipMapping() : TaskTypeMappingBase((byte)MapNames.Airship)
{
    public override List<TaskTypes> CommonTasks { get; } =
    [
        TaskTypes.FixWiring,
        TaskTypes.EnterIdCode
    ];

    public override List<TaskTypes> LongTasks { get; } =
    [
        TaskTypes.CalibrateDistributor,
        TaskTypes.ResetBreakers,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UnlockSafe,
        TaskTypes.StartFans,
        TaskTypes.EmptyGarbage,
        TaskTypes.EmptyGarbage,
        TaskTypes.EmptyGarbage,
        TaskTypes.DevelopPhotos,
        TaskTypes.FuelEngines,
        TaskTypes.RewindTapes,
        TaskTypes.EmptyGarbage,
        TaskTypes.EmptyGarbage
    ];

    public override List<TaskTypes> ShortTasks { get; } =
    [
        TaskTypes.PolishRuby,
        TaskTypes.StabilizeSteering,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.UploadData,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.DivertPower,
        TaskTypes.PickUpTowels,
        TaskTypes.CleanToilet,
        TaskTypes.DressMannequin,
        TaskTypes.SortRecords,
        TaskTypes.PutAwayPistols,
        TaskTypes.PutAwayRifles,
        TaskTypes.Decontaminate,
        TaskTypes.MakeBurger,
        TaskTypes.FixShower,
        TaskTypes.VentCleaning
    ];
}