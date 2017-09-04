using RealTimeTasks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RealTimeTasks.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return View("Login");
        }
        public ActionResult Login(string email, string password)
        {
            TasksRepository repo = new TasksRepository(Properties.Settings.Default.ConStr);
            User user = repo.Login(email, password);
            if(user == null)
            {
                return View("Login");
            }
            FormsAuthentication.SetAuthCookie(email, true);
            return Redirect("/home/index");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addUser(User user, string password)
        {
            TasksRepository repo = new TasksRepository(Properties.Settings.Default.ConStr);
            repo.AddUser(user, password);
            return Redirect("/home/index");
        }
        public ActionResult allTasks()
        {
            TasksRepository repo = new TasksRepository(Properties.Settings.Default.ConStr);
            IEnumerable<Task> tasks = repo.AllTasks();
            string email = User.Identity.Name;
            User user = repo.GetByEmail(email);
            return Json(tasks.Select(t => new {
                id = t.Id,
                title = t.Title,
                isHandled = t.IsHandled,
                working = t.IsHandled == user.Id,
                userFirst = t.User == null ? null : t.User.FirstName,
                userLast = t.User == null ? null : t.User.LastName
            }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult addTask(string title)
        {
            TasksRepository repo = new TasksRepository(Properties.Settings.Default.ConStr);
            repo.AddTask(title);
            return Redirect("/");
        }
        [HttpPost]
        public ActionResult updateTask(int taskId)
        {
            TasksRepository repo = new TasksRepository(Properties.Settings.Default.ConStr);
            string email = User.Identity.Name;
            User user = repo.GetByEmail(email);
            repo.UpdateTask(taskId, user.Id);
            return Json(taskId, JsonRequestBehavior.AllowGet);
        }
        public void completedTask(int taskId)
        {
            TasksRepository repo = new TasksRepository(Properties.Settings.Default.ConStr);
            repo.completedTask(taskId);
        }
    }
}