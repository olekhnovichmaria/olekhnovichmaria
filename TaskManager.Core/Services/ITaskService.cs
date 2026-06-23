using System;
using System.Collections.Generic;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public interface ITaskService
    {
        TaskItem AddTask(string title, string description, TaskPriority priority, DateTime dueDate, bool isImportant = false);
        IEnumerable<TaskItem> GetAllTasks();
        IEnumerable<TaskItem> GetTasksByStatus(TaskManager.Core.Models.TaskStatus status);
        IEnumerable<TaskItem> SearchTasks(string query);
        TaskItem GetTaskById(Guid id);
        bool UpdateTask(TaskItem task);
        bool DeleteTask(Guid id);
        IEnumerable<TaskItem> SortByPriority();
        IEnumerable<TaskItem> SortByDueDate();
        Tuple<int, int, int> GetStatistics();
        void SaveToFile(string filePath);
        void LoadFromFile(string filePath);
    }
}