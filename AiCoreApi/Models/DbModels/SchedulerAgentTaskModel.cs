using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiCoreApi.Models.DbModels
{
    [Table("scheduler_agent_task")]
    public class SchedulerAgentTaskModel
    {
        [Key]
        public int SchedulerAgentTaskId { get; set; } = 0;
        public SchedulerAgentTaskState SchedulerAgentTaskState { get; set; } = SchedulerAgentTaskState.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ValidTill { get; set; } = DateTime.UtcNow.AddDays(1);
        public int LoginId { get; set; } = 0;
        public string RequestAccessor { get; set; } = string.Empty;
        public string SchedulerAgentTaskGuid { get; set; } = string.Empty;
        public string CompositeAgentName { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
    }

    public enum SchedulerAgentTaskState
    {
        New = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4,
    }
}