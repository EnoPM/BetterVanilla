namespace BetterVanilla.PlayerTasks;

public sealed class TaskAssignationOptions
{
    public required int CommonTasksCount { get; set; }
    public required int LongTasksCount { get; set; }
    public required int ShortTasksCount { get; set; }
    public required bool DefineCommonAsNonCommon { get; set; }
    public required byte MapId { get; set; }
}