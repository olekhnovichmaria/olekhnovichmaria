using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.Tests
{
    [TestClass]
    public class TaskServiceTests
    {
        private string _testFile = "test_tasks.json";

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(_testFile))
                File.Delete(_testFile);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFile))
                File.Delete(_testFile);
        }

        [TestMethod]
        public void AddTask_ShouldAddTaskToList()
        {
            var service = new TaskService(_testFile);
            var task = service.AddTask("Test", "Desc", TaskPriority.High, DateTime.Now.AddDays(1));

            Assert.AreNotEqual(Guid.Empty, task.Id);
            Assert.AreEqual("Test", task.Title);
            Assert.AreEqual(1, service.GetAllTasks().Count());
        }

        [TestMethod]
        public void GetTasksByStatus_ShouldFilterCorrectly()
        {
            var service = new TaskService(_testFile);
            service.AddTask("Task1", "Desc", TaskPriority.Low, DateTime.Now.AddDays(1));
            var task2 = service.AddTask("Task2", "Desc", TaskPriority.Medium, DateTime.Now.AddDays(2));

            task2.Status = TaskStatus.InProgress;
            service.UpdateTask(task2);

            var inProgress = service.GetTasksByStatus(TaskStatus.InProgress);
            Assert.AreEqual(1, inProgress.Count());
        }

        [TestMethod]
        public void SearchTasks_ShouldFindByTitleOrDescription()
        {
            var service = new TaskService(_testFile);
            service.AddTask("UniqueTitle", "Desc", TaskPriority.Low, DateTime.Now.AddDays(1));
            service.AddTask("Other", "UniqueDesc", TaskPriority.Low, DateTime.Now.AddDays(1));

            var results = service.SearchTasks("Unique").ToList();
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void DeleteTask_ShouldRemoveTask()
        {
            var service = new TaskService(_testFile);
            var task = service.AddTask("ToDelete", "Desc", TaskPriority.Low, DateTime.Now.AddDays(1));
            var result = service.DeleteTask(task.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(0, service.GetAllTasks().Count());
        }

        [TestMethod]
        public void SortByPriority_ShouldSortDescending()
        {
            var service = new TaskService(_testFile);
            service.AddTask("Low", "Desc", TaskPriority.Low, DateTime.Now.AddDays(1));
            service.AddTask("High", "Desc", TaskPriority.High, DateTime.Now.AddDays(1));

            var sorted = service.SortByPriority().ToList();
            Assert.AreEqual(TaskPriority.High, sorted[0].Priority);
        }

        [TestMethod]
        public void GetStatistics_ShouldReturnCorrectCounts()
        {
            var service = new TaskService(_testFile);
            service.AddTask("Task1", "Desc", TaskPriority.Low, DateTime.Now.AddDays(1));
            var overdue = service.AddTask("Task2", "Desc", TaskPriority.Low, DateTime.Now.AddDays(-1));
            var completed = service.AddTask("Task3", "Desc", TaskPriority.Low, DateTime.Now.AddDays(1));

            completed.Status = TaskStatus.Completed;
            service.UpdateTask(completed);

            var stats = service.GetStatistics();
            Assert.AreEqual(1, stats.Item1);   // completed
            Assert.AreEqual(1, stats.Item2);   // overdue
            Assert.AreEqual(3, stats.Item3);   // total
        }

        [TestMethod]
        public void SaveAndLoad_ShouldPersistData()
        {
            var service = new TaskService(_testFile);
            service.AddTask("Persistent", "Desc", TaskPriority.High, DateTime.Now.AddDays(1));
            service.SaveToFile(_testFile);

            var newService = new TaskService(_testFile);
            var tasks = newService.GetAllTasks().ToList();

            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("Persistent", tasks[0].Title);
        }

        [TestMethod]
        public void IsOverdue_ShouldBeTrueForPastDueDate()
        {
            var service = new TaskService(_testFile);
            var task = service.AddTask("Overdue", "Desc", TaskPriority.Low, DateTime.Now.AddDays(-1));
            Assert.IsTrue(task.IsOverdue);
        }
    }
}