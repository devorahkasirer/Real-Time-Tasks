using Microsoft.AspNet.SignalR;
using RealTimeTasks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealTimeTasks.Web
{
    public class SimpleHub : Hub
    {
        public void TaskAdded()
        {
            Clients.All.Reload();
        }
        public void TaskUpdated(int taskId)
        {
            string email = Context.User.Identity.Name;
            var repo = new TasksRepository(Properties.Settings.Default.ConStr);
            User user = repo.GetByEmail(email);
            Task t = repo.GetTaskById(taskId);
            Clients.All.BtnUpdate(new
            {
                id = t.Id,
                title = t.Title,
                isHandled = t.IsHandled,
                working = t.IsHandled == user.Id,
                userFirst = t.User == null ? null : t.User.FirstName,
                userLast = t.User == null ? null : t.User.LastName
            });
        }
    }
}