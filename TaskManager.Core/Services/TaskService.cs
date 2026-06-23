using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public class TaskService : ITaskService
    {
        private List<TaskItem> _tasks = new List<TaskItem>();
        private readonly string _filePath;

        public TaskService(string filePath = "tasks.json")
        {
            _filePath = filePath;
            if (File.Exists(_filePath))
            {
                LoadFromFile(_filePath);
            }
        }

        public TaskItem AddTask(string title, string description, TaskPriority priority, DateTime dueDate, bool isImportant = false)
        {
            var task = new TaskItem
            {
                Title = title,
                Description = description,
                Priority = priority,
                DueDate = dueDate,
                IsImportant = isImportant,
                Status = TaskManager.Core.Models.TaskStatus.New
            };
            _tasks.Add(task);
            SaveToFile(_filePath);
            return task;
        }

        public IEnumerable<TaskItem> GetAllTasks()
        {
            return _tasks;
        }

        public IEnumerable<TaskItem> GetTasksByStatus(TaskManager.Core.Models.TaskStatus status)
        {
            return _tasks.Where(t => t.Status == status);
        }

        public IEnumerable<TaskItem> SearchTasks(string query)
        {
            return _tasks.Where(t =>
                t.Title.ToLower().Contains(query.ToLower()) ||
                t.Description.ToLower().Contains(query.ToLower()));
        }

        public TaskItem GetTaskById(Guid id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public bool UpdateTask(TaskItem task)
        {
            var existing = GetTaskById(task.Id);
            if (existing == null) return false;

            var index = _tasks.IndexOf(existing);
            _tasks[index] = task;
            SaveToFile(_filePath);
            return true;
        }

        public bool DeleteTask(Guid id)
        {
            var task = GetTaskById(id);
            if (task == null) return false;

            _tasks.Remove(task);
            SaveToFile(_filePath);
            return true;
        }

        public IEnumerable<TaskItem> SortByPriority()
        {
            return _tasks.OrderByDescending(t => t.Priority);
        }

        public IEnumerable<TaskItem> SortByDueDate()
        {
            return _tasks.OrderBy(t => t.DueDate);
        }

        public Tuple<int, int, int> GetStatistics()
        {
            int completed = _tasks.Count(t => t.Status == TaskManager.Core.Models.TaskStatus.Completed);
            int overdue = _tasks.Count(t => t.IsOverdue);
            int total = _tasks.Count;
            return Tuple.Create(completed, overdue, total);
        }

        public void SaveToFile(string filePath)
        {
            var json = JsonConvert.SerializeObject(_tasks, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var json = File.ReadAllText(filePath);
            _tasks = JsonConvert.DeserializeObject<List<TaskItem>>(json) ?? new List<TaskItem>();
        }
    }
}