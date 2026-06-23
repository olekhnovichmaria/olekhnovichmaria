using System;

namespace TaskManager.Core.Models
{
    public enum TaskStatus
    {
        New,
        InProgress,
        Completed
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public bool IsImportant { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsOverdue => Status != TaskStatus.Completed && DueDate < DateTime.Now;
    }
}