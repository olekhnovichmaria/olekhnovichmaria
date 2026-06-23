using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.WinForms
{
    public partial class Form1 : Form
    {
        private readonly ITaskService _taskService;
        private TaskItem _selectedTask;

        public Form1()
        {
            InitializeComponent();
            _taskService = new TaskService("tasks.json");
            SetupColumns();
            SetupPriorityCombo();
            SetupFilterCombo();
            RefreshTasks();
            UpdateStatistics();
        }

        private void SetupColumns()
        {
            if (lvTasks == null)
            {
                MessageBox.Show("lvTasks is NULL!");
                return;
            }
            lvTasks.Columns.Clear();
            lvTasks.Columns.Add("⭐", 30);
            lvTasks.Columns.Add("Название", 150);
            lvTasks.Columns.Add("Описание", 200);
            lvTasks.Columns.Add("Приоритет", 80);
            lvTasks.Columns.Add("Срок", 100);
            lvTasks.Columns.Add("Статус", 100);
            lvTasks.Columns.Add("Просрочена", 80);
        }

        private void SetupPriorityCombo()
        {
            cmbPriority.Items.Add(TaskPriority.Low);
            cmbPriority.Items.Add(TaskPriority.Medium);
            cmbPriority.Items.Add(TaskPriority.High);
            cmbPriority.SelectedIndex = 1;
        }

        private void SetupFilterCombo()
        {
            cmbFilter.Items.Add("Все");
            cmbFilter.Items.Add("New");
            cmbFilter.Items.Add("InProgress");
            cmbFilter.Items.Add("Completed");
            cmbFilter.SelectedIndex = 0;
        }

        private void RefreshTasks()
        {
            lvTasks.Items.Clear();
            var tasks = _taskService.GetAllTasks();
            foreach (var task in tasks)
            {
                AddTaskToListView(task);
            }
        }

        private void AddTaskToListView(TaskItem task)
        {
            var item = new ListViewItem(task.IsImportant ? "⭐" : "");
            item.SubItems.Add(task.Title);
            item.SubItems.Add(task.Description);
            item.SubItems.Add(task.Priority.ToString());
            item.SubItems.Add(task.DueDate.ToShortDateString());
            item.SubItems.Add(task.Status.ToString());
            item.SubItems.Add(task.IsOverdue ? "Да" : "Нет");

            if (task.IsImportant)
                item.BackColor = Color.LightYellow;
            if (task.IsOverdue)
                item.BackColor = Color.LightCoral;

            item.Tag = task;
            lvTasks.Items.Add(item);
        }

        private void UpdateStatistics()
        {
            var stats = _taskService.GetStatistics();
            lblStats.Text = string.Format("Всего: {0} | Завершено: {1} | Просрочено: {2}",
                stats.Item3, stats.Item1, stats.Item2);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название задачи!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _taskService.AddTask(
                txtTitle.Text,
                txtDescription.Text,
                (TaskPriority)cmbPriority.SelectedItem,
                dtpDueDate.Value,
                chkImportant.Checked
            );

            txtTitle.Clear();
            txtDescription.Clear();
            chkImportant.Checked = false;
            RefreshTasks();
            UpdateStatistics();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Выберите задачу!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _taskService.DeleteTask(_selectedTask.Id);
            _selectedTask = null;
            RefreshTasks();
            UpdateStatistics();
        }

        private void btnInProgress_Click(object sender, EventArgs e)
        {
            UpdateTaskStatus(TaskManager.Core.Models.TaskStatus.InProgress);
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            UpdateTaskStatus(TaskManager.Core.Models.TaskStatus.Completed);
        }

        private void UpdateTaskStatus(TaskManager.Core.Models.TaskStatus status)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Выберите задачу!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedTask.Status = status;
            _taskService.UpdateTask(_selectedTask);
            RefreshTasks();
            UpdateStatistics();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                RefreshTasks();
                return;
            }

            lvTasks.Items.Clear();
            var results = _taskService.SearchTasks(txtSearch.Text);
            foreach (var task in results)
            {
                AddTaskToListView(task);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (cmbFilter.SelectedIndex == 0)
            {
                RefreshTasks();
                return;
            }

            var status = (TaskManager.Core.Models.TaskStatus)Enum.Parse(typeof(TaskManager.Core.Models.TaskStatus), cmbFilter.SelectedItem.ToString());
            lvTasks.Items.Clear();
            var filtered = _taskService.GetTasksByStatus(status);
            foreach (var task in filtered)
            {
                AddTaskToListView(task);
            }
        }

        private void btnSortPriority_Click(object sender, EventArgs e)
        {
            lvTasks.Items.Clear();
            var sorted = _taskService.SortByPriority();
            foreach (var task in sorted)
            {
                AddTaskToListView(task);
            }
        }

        private void btnSortDate_Click(object sender, EventArgs e)
        {
            lvTasks.Items.Clear();
            var sorted = _taskService.SortByDueDate();
            foreach (var task in sorted)
            {
                AddTaskToListView(task);
            }
        }

        private void lvTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvTasks.SelectedItems.Count > 0)
            {
                _selectedTask = (TaskItem)lvTasks.SelectedItems[0].Tag;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}