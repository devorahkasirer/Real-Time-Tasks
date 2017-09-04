using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTasks.Data
{
    public class TasksRepository
    {
        private string _connectionString;
        public TasksRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser(User user, string password)
        {
            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.HashPassword(password, salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            using(var context = new TasksRepositoryDataContext(_connectionString))
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }
        }
        public User Login(string email, string password)
        {
            User user = GetByEmail(email);
            if(user == null)
            {
                return null;
            }
            bool match = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (!match)
            {
                return null;
            }
            return user;
        }
        public User GetByEmail(string email)
        {
            using(var context = new TasksRepositoryDataContext(_connectionString))
            {
                return context.Users.First(u => u.Email == email);
            }
        }
        public IEnumerable<Task> AllTasks()
        {
            using (var context = new TasksRepositoryDataContext(_connectionString))
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Task>(t => t.User);
                context.LoadOptions = loadOptions;
                return context.Tasks.Where(t => !t.Completed).ToList();
            }
        }
        public void AddTask(string title)
        {
            using(var context = new TasksRepositoryDataContext(_connectionString))
            {
                Task task = new Task
                {
                    Title = title,
                    Completed = false
                };
                context.Tasks.InsertOnSubmit(task);
                context.SubmitChanges();
            }
        }
        public void UpdateTask(int taskId, int userId)
        {
            using (var context = new TasksRepositoryDataContext(_connectionString))
            {
                context.ExecuteCommand("UPDATE Tasks SET IsHandled = {0} WHERE Id = {1}", userId, taskId);
            }
        }
        public void completedTask(int taskId)
        {
            using (var context = new TasksRepositoryDataContext(_connectionString))
            {
                context.ExecuteCommand("UPDATE Tasks SET Completed = 1 WHERE Id = {0}", taskId);
            }
        }
        public Task GetTaskById(int taskId)
        {
            using (var context = new TasksRepositoryDataContext(_connectionString))
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Task>(t => t.User);
                context.LoadOptions = loadOptions;
                return context.Tasks.First(t => t.Id == taskId);
            }
        }

    }
}
