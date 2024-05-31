using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pcg.Models;
using System.Data.SqlClient;
using System.Data;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace pcg.Controllers
{
    public class AdminController : Controller
    {
        const string SessionName = "_Name";
        const string SessionLayout = "_Layout";
        const string SessionType = "_Type";
        const string SessionId = "_Id";

        public SqlConnection con;
        public SqlCommand cmd;
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _dbContext;

        public AdminController(IConfiguration configuration, DatabaseContext dbContext)
        {
            _configuration = configuration;
            con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            _dbContext = dbContext;
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);

            cmd = new SqlCommand("WITH LatestDateStart AS " +
                "(SELECT MAX(DateStart) AS MaxDateStart " +
                "FROM TaskLog) " +
                "SELECT l.LogId, l.TaskId, l.AssignId, l.Status, l.DateStart, l.DateFwd, l.DateRcv, l.DateClr, " +
                "t.Task, t.Remarks, t.Description, " +
                "t.SiteReqId, t.AddedBy, s.Client, s.Site, u.Name, u.Position " +
                "FROM TaskLog l " +
                "LEFT JOIN Tasks t ON t.TaskId = l.TaskId " +
                "LEFT JOIN Sites s ON t.SiteReqId = s.SiteId " +
                "LEFT JOIN Users u ON l.AssignId = u.Id " +
                "CROSS JOIN LatestDateStart " + 
                "WHERE l.DateStart = LatestDateStart.MaxDateStart", con);
            DataSet stask = new DataSet();
            SqlDataAdapter stasks = new SqlDataAdapter(cmd);
            stasks.Fill(stask, "slist");
            ViewBag.startdate = stask.Tables[0];

            cmd = new SqlCommand("WITH LatestDateFwd AS " +
                "(SELECT MAX(DateFwd) AS MaxDateFwd " +
                "FROM TaskLog) " +
                "SELECT l.LogId, l.TaskId, l.AssignId, l.Status, l.DateStart, l.DateFwd, l.DateRcv, l.DateClr, " +
                "t.Task, t.Remarks, t.Description, " +
                "t.SiteReqId, t.AddedBy, s.Client, s.Site, u.Name, u.Position " +
                "FROM TaskLog l " +
                "LEFT JOIN Tasks t ON t.TaskId = l.TaskId " +
                "LEFT JOIN Sites s ON t.SiteReqId = s.SiteId " +
                "LEFT JOIN Users u ON l.AssignId = u.Id " +
                "CROSS JOIN LatestDateFwd " +
                "WHERE l.DateFwd = LatestDateFwd.MaxDateFwd", con);
            DataSet ftask = new DataSet();
            SqlDataAdapter ftasks = new SqlDataAdapter(cmd);
            ftasks.Fill(ftask, "slist");
            ViewBag.fwddate = ftask.Tables[0];

            cmd = new SqlCommand("WITH LatestDateRcv AS " +
                "(SELECT MAX(DateRcv) AS MaxDateRcv " +
                "FROM TaskLog) " +
                "SELECT l.LogId, l.TaskId, l.AssignId, l.Status, l.DateStart, l.DateFwd, l.DateRcv, l.DateClr, " +
                "t.Task, t.Remarks, t.Description, " +
                "t.SiteReqId, t.AddedBy, s.Client, s.Site, u.Name, u.Position " +
                "FROM TaskLog l " +
                "LEFT JOIN Tasks t ON t.TaskId = l.TaskId " +
                "LEFT JOIN Sites s ON t.SiteReqId = s.SiteId " +
                "LEFT JOIN Users u ON l.AssignId = u.Id " +
                "CROSS JOIN LatestDateRcv " +
                "WHERE l.DateRcv = LatestDateRcv.MaxDateRcv", con);
            DataSet rtask = new DataSet();
            SqlDataAdapter rtasks = new SqlDataAdapter(cmd);
            rtasks.Fill(rtask, "slist");
            ViewBag.rcvdate = rtask.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }
        public IActionResult ChangeInfo()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            string sesId = HttpContext.Session.GetString(SessionId);
            using (cmd = new SqlCommand("SELECT Name, Password, Email, ContactNo FROM Users WHERE Id = '" + sesId + "'", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows && reader.Read())
                {
                    var model = new ChangeInfo
                    {
                        CurName = reader["Name"].ToString(),
                        CurPass = reader["Password"].ToString(),
                        CurEmail = reader["Email"].ToString(),
                        CurContact = reader["ContactNo"].ToString()
                    };

                    return View(model);
                }
                else
                {
                    return View();
                }
            }
        }
        [HttpPost]
        public IActionResult ChangeInfo(ChangeInfo info)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            string sesId = HttpContext.Session.GetString(SessionId);
            using (cmd = new SqlCommand("SELECT Name, Email, ContactNo FROM Users WHERE Id = '" + sesId + "'", con))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows && reader.Read())
                {
                    {
                        info.CurName = reader["Name"].ToString();
                        info.CurEmail = reader["Email"].ToString();
                        info.CurContact = reader["ContactNo"].ToString();
                    }
                }
            }
            if (ModelState.IsValid)
            {
                if (info.Name != null)
                {
                    cmd = new SqlCommand("UPDATE Users SET Name = '" + info.Name + "' WHERE Id = '" + sesId + "'", con);
                    cmd.ExecuteNonQuery();
                }
                if (info.Password != null)
                {
                    cmd = new SqlCommand("UPDATE Users SET Password = '" + info.Password + "' WHERE Id = '" + sesId + "'", con);
                    cmd.ExecuteNonQuery();
                }
                if (info.Email != null)
                {
                    cmd = new SqlCommand("UPDATE Users SET Email = '" + info.Email + "' WHERE Id = '" + sesId + "'", con);
                    cmd.ExecuteNonQuery();
                }
                if (info.ContactNo != null)
                {
                    cmd = new SqlCommand("UPDATE Users SET ContactNo = '" + info.ContactNo + "' WHERE Id = '" + sesId + "'", con);
                    cmd.ExecuteNonQuery();
                }
            }
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View(info);
        }
        /*   public IActionResult Sites()
           {
               if (HttpContext.Session.GetString(SessionType) != "Admin")
               {
                   return RedirectToAction("Login", "Home");
               }
               if (con.State == ConnectionState.Closed)
               {
                   con.Open();
               }
               ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
               cmd = new SqlCommand("SELECT s.SiteId, s.Client, s.Site, s.DateAdded, s.Status, s.SiteOM, u.Name FROM Sites s LEFT JOIN Users u ON s.SiteOM = u.Id WHERE s.Status = 'Active'", con);
               DataSet sites = new DataSet();
               SqlDataAdapter ssite = new SqlDataAdapter(cmd);
               ssite.Fill(sites, "slist");

               ViewBag.Sitelist = sites.Tables[0];

               cmd = new SqlCommand("SELECT TaskId, Task, Remarks, Description, SiteReqId FROM Tasks", con);
               DataSet vars = new DataSet();
               SqlDataAdapter vvars = new SqlDataAdapter(cmd);
               vvars.Fill(vars, "varie");

               ViewBag.Tasklist = vars.Tables[0];

               if (con.State == ConnectionState.Open)
               {
                   con.Close();
               }
               return View();
           }
        */

        public IActionResult Sites(SitesModel s)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);

            // Fetch user list
            SqlCommand cmd = new SqlCommand("SELECT u.Id, u.Name, u.Position, p.Usertype FROM Users u LEFT JOIN Positions p ON u.Position = p.Position WHERE u.Status = 'Active'", con);
            DataSet user = new DataSet();
            SqlDataAdapter uuser = new SqlDataAdapter(cmd);
            uuser.Fill(user, "ulist");

            // Store user list in ViewBag
            ViewBag.Userlist = user.Tables[0];

            // Fetch site list
            cmd = new SqlCommand("SELECT s.SiteId, s.Client, s.Site, s.DateAdded, s.Status, s.SiteOM, u.Name FROM Sites s LEFT JOIN Users u ON s.SiteOM = u.Id WHERE s.Status = 'Active'", con);
            DataSet sites = new DataSet();
            SqlDataAdapter ssite = new SqlDataAdapter(cmd);
            ssite.Fill(sites, "slist");

            ViewBag.Sitelist = sites.Tables[0];

            cmd = new SqlCommand("SELECT TaskId, Task, Remarks, Description, SiteReqId FROM Tasks", con);
            DataSet tasks = new DataSet();
            SqlDataAdapter taskAdapter = new SqlDataAdapter(cmd);
            taskAdapter.Fill(tasks, "taskList");

            ViewBag.Tasklist = tasks.Tables[0];


            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            string sesname = HttpContext.Session.GetString(SessionName);
            cmd = new SqlCommand("SELECT t.TaskId AS TaskID_Tasks," +
                " t.Task," +
                " t.Description," +
                " t.Remarks," +
                " t.AddedBy," +
                " t.SiteReqId," +
                " t.DateStart," +
                " t.DateFwd," +
                " t.DateRcv," +
                " t.DateClr," +
                " t.AssignId," +
                " t.ForwardId," +
                " t.Status," +
                " s.Status AS SiteStatus," +
                " u.Id AS Id_user, " +
                " u.Name AS Name, " +
                " u.Position FROM Tasks t " +
                "LEFT JOIN Users u " +
                "ON t.AssignId = u.Id " +
                "LEFT JOIN Sites s " +
                "ON t.SiteReqId = s.SiteId " +
                "WHERE t.SiteReqId = '" + s.SiteId + "' AND s.Status = 'Active'", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasksList = new SqlDataAdapter(cmd);
            tasksList.Fill(task, "slist");

            ViewBag.Tasklog = task.Tables[0];



            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            return View();
        }

        public IActionResult AddSite()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);

            cmd = new SqlCommand("SELECT u.Id, u.Name, u.Position, p.Usertype FROM Users u LEFT JOIN Positions p ON u.Position = p.Position WHERE u.Status = 'Active'", con);
            DataSet user = new DataSet();
            SqlDataAdapter uuser = new SqlDataAdapter(cmd);
            uuser.Fill(user, "ulist");

            ViewBag.Userlist = user.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddSite(SitesModel sm)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            if (ModelState.IsValid)
            {
                cmd = new SqlCommand("SELECT COUNT(Client) FROM Sites WHERE Client = '" + sm.Client + "'", con);
                int ccount = Convert.ToInt32(cmd.ExecuteScalar());
                if (ccount > 0)
                {
                    sm.Clientcheck = sm.Client;
                    ModelState.AddModelError("Client", "Client is already registered");

                    cmd = new SqlCommand("SELECT u.Id, u.Name, u.Position, p.Usertype FROM Users u LEFT JOIN Positions p ON u.Position = p.Position", con);
                    DataSet user = new DataSet();
                    SqlDataAdapter uuser = new SqlDataAdapter(cmd);
                    uuser.Fill(user, "ulist");

                    ViewBag.Userlist = user.Tables[0];

                    return View(sm);
                }
                else
                {
                    DateTime cdt = DateTime.Now;
                    string scdt = cdt.ToString("yyyy-MM-dd HH:mm:ss");
                    cmd = new SqlCommand("INSERT INTO Sites (Client, Site, SiteOM, DateAdded, Status, SiteSOM, SiteSC, SiteTK) " +
                        "VALUES('" + sm.Client + "', " +
                        "'" + sm.Site + "', " +
                        "'" + sm.SiteOM + "', " +
                        "'" + scdt + "', " +
                        "'Pending', "  +
                        "'" + sm.SiteSOM + "', " +
                        "'" + sm.SiteSC + "', " +
                        "'" + sm.SiteTK + "')", con);
                    cmd.ExecuteNonQuery();
                }
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return RedirectToAction("Sites", "Admin");
        }
        public IActionResult AddTask(string siteId)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);            
            cmd = new SqlCommand("SELECT SiteId, Client, Site, SiteOM FROM Sites WHERE SiteId = '" + siteId + "'", con);
            DataSet sites = new DataSet();
            SqlDataAdapter ssite = new SqlDataAdapter(cmd);
            ssite.Fill(sites, "slist");

            ViewBag.Sitelist = sites.Tables[0];

            cmd = new SqlCommand("SELECT u.Id, u.Name, p.Usertype, u.Position FROM Users u LEFT JOIN Positions p ON u.Position = p.Position", con);
            DataSet user = new DataSet();
            SqlDataAdapter users = new SqlDataAdapter(cmd);
            users.Fill(user, "ulist");

            ViewBag.Userlist = user.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            return PartialView("_AddTask");
        }
        [HttpPost]
        public IActionResult AddTask(VariationModel vars)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            if (ModelState.IsValid)
            {
                string sesname = HttpContext.Session.GetString(SessionName);
                string desc = vars.Description + " " + vars.Descquery + vars.Descdocreq + vars.Descvary;
                DateTime cdt = DateTime.Now;
                string scdt = cdt.ToString("yyyy-MM-dd HH:mm:ss");
                cmd = new SqlCommand("INSERT INTO Tasks (Task, Remarks, Description, AddedBy, SiteReqId, AssignId, Status, DateStart) VALUES" +
                    "('" + vars.Task + "', " +
                    "'" + vars.Remarks + "', " +
                    "'" + desc + "', " +
                    "'" + sesname + "', " +
                    "'" + vars.SiteReqId + "', " +
                    "'" + vars.AssignId + "', " +
                    "'Waiting', " +
                    "'" + scdt + "'); SELECT SCOPE_IDENTITY();", con);

                string taskId;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    taskId = Convert.ToString(reader[0]);
                }

                cmd = new SqlCommand("INSERT INTO Tasklog (TaskId, AssignId, DateStart, Status) VALUES ('" + taskId + "', '" + vars.AssignId + "', '" + scdt + "', 'Waiting')", con);
                cmd.ExecuteNonQuery();
            }

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            // Return the AddTask partial view without the layout
            return PartialView("_AddTask", vars);
        }


       
        public IActionResult TaskDetail(SitesModel s)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            string sesname = HttpContext.Session.GetString(SessionName);
            cmd = new SqlCommand("SELECT t.TaskId AS TaskID_Tasks," +
                " t.Task," +
                " t.Description," +
                " t.Remarks," +
                " t.AddedBy," +
                " t.SiteReqId," +
                " t.DateStart," +
                " t.DateFwd," +
                " t.DateRcv," +
                " t.DateClr," +
                " t.AssignId," +
                " t.ForwardId," +
                " t.Status," +
                " s.Status AS SiteStatus," +
                " u.Id AS Id_user, " +
                " u.Name AS Name, " +
                " u.Position FROM Tasks t " +
                "LEFT JOIN Users u " +
                "ON t.AssignId = u.Id " +
                "LEFT JOIN Sites s " +
                "ON t.SiteReqId = s.SiteId " +
                "WHERE t.SiteReqId = '" + s.SiteId + "' AND s.Status = 'Active'", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");

            ViewBag.Tasklog = task.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return PartialView("_TaskDetail", ViewBag.Tasklog); // Return partial view instead of full view
        }



        public IActionResult TaskDetail1(SitesModel s)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            string sesname = HttpContext.Session.GetString(SessionName);
            cmd = new SqlCommand("SELECT t.TaskId AS TaskID_Tasks," +
                " t.Task," +
                " t.Description," +
                " t.Remarks," +
                " t.AddedBy," +
                " t.SiteReqId," +
                " t.DateStart," +
                " t.DateFwd," +
                " t.DateRcv," +
                " t.DateClr," +
                " t.AssignId," +
                " t.ForwardId," +
                " t.Status," +
                " s.Status AS SiteStatus," +
                " u.Id AS Id_user, " +
                " u.Name AS Name, " +
                " u.Position FROM Tasks t " +
                "LEFT JOIN Users u " +
                "ON t.AssignId = u.Id " +
                "LEFT JOIN Sites s " +
                "ON t.SiteReqId = s.SiteId " +
                "WHERE t.SiteReqId = '" + s.SiteId + "' AND s.Status = 'Active'", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");

            ViewBag.Tasklog = task.Tables[0];


            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }



        public IActionResult TaskForward(string taskId)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            cmd = new SqlCommand("SELECT t.TaskId, t.Task, t.Remarks, t.Description, t.AssignId, u.Name, u.Position " +
                "FROM Tasks t " +
                "LEFT JOIN Users u " +
                "ON t.AssignId = u.Id " +
                "WHERE t.TaskId = '" + taskId + "'", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");
            ViewBag.Tasklog = task.Tables[0];

            string sesname = HttpContext.Session.GetString(SessionName);
            cmd = new SqlCommand("SELECT * FROM Users WHERE Name != '" + sesname + "' AND Status = 'Active'", con);
            DataSet user = new DataSet();
            SqlDataAdapter users = new SqlDataAdapter(cmd);
            users.Fill(user, "ulist");

            ViewBag.Userlist = user.Tables[0];
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }
       

        [HttpPost]
        public IActionResult TaskForward(VariationModel vm, string taskId)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            DateTime cdt = DateTime.Now;
            string scdt = cdt.ToString("yyyy-MM-dd HH:mm:ss");
            cmd = new SqlCommand("INSERT INTO Tasklog (TaskId, AssignId, DateFwd, Status) VALUES ('" + taskId + "', '" + vm.AssignId + "', '" + scdt + "', 'Pending')",con);
            cmd.ExecuteNonQuery();

            cmd = new SqlCommand("UPDATE Tasks SET Status = 'Pending', DateFwd = '" + scdt + "', ForwardId = '" + vm.AssignId + "' WHERE TaskId = " + taskId, con);
            cmd.ExecuteNonQuery();

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return PartialView("_ForwardTask", ViewBag.Tasklog); // Return partial view instead of full view
        }
        public IActionResult Pending()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            string sesname = HttpContext.Session.GetString(SessionName);
            cmd = new SqlCommand("SELECT t.TaskId AS TaskID_Tasks, " +
                " t.Task," +
                " t.Description," +
                " t.Remarks," +
                " t.AddedBy," +
                " t.SiteReqId," +
                " t.DateStart," +
                " t.DateFwd," +
                " t.DateRcv," +
                " t.DateClr," +
                " t.AssignId," +
                " t.ForwardId," +
                " t.Status," +
                " s.Client," +
                " s.Site," +
                " uf.Id AS Idf, " +
                " uf.Name AS Namef, " +
                " uf.Position As Positionf, " +
                " ua.Id AS Ida, " +
                " ua.Name AS Namea, " +
                " ua.Position AS Positiona, " +
                " pf.UserType AS UserTypepf, " +
                " pa.UserType As UserTypepa " +
                "FROM Tasks t " +
                "LEFT JOIN Users uf " +
                "ON t.ForwardId = uf.Id " +
                "LEFT JOIN Positions pf " +
                "ON uf.Position = pf.Position " +
                "LEFT JOIN Users ua " +
                "ON t.AssignId = ua.Id " +
                "LEFT JOIN Positions pa " +
                "ON ua.Position = pa.Position " +
                "LEFT JOIN Sites s " +
                "ON t.SiteReqId = s.SiteId " +
                "WHERE t.Status = 'Waiting'" +
                "AND (" +
                "pf.UserType = 'Admin' " +
                "OR " +
                "pa.UserType = 'Admin')", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");

            ViewBag.Tasklog = task.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }
        public IActionResult Forwarded()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            string sesname = HttpContext.Session.GetString(SessionName);
            cmd = new SqlCommand("SELECT t.TaskId AS TaskID_Tasks," +
                " t.Task," +
                " t.Description," +
                " t.Remarks," +
                " t.AddedBy," +
                " t.SiteReqId," +
                " t.DateStart," +
                " t.DateFwd," +
                " t.DateRcv," +
                " t.DateClr," +
                " t.AssignId," +
                " t.ForwardId," +
                " t.Status," +
                " s.Client," +
                " s.Site," +
                " uf.Id AS Idf, " +
                " uf.Name AS Namef, " +
                " uf.Position As Positionf, " +
                " ua.Id AS Ida, " +
                " ua.Name AS Namea, " +
                " ua.Position AS Positiona, " +
                " pf.UserType AS UserTypepf, " +
                " pa.UserType As UserTypepa " +
                "FROM Tasks t " +
                "LEFT JOIN Users uf " +
                "ON t.ForwardId = uf.Id " +
                "LEFT JOIN Positions pf " +
                "ON uf.Position = pf.Position " +
                "LEFT JOIN Users ua " +
                "ON t.AssignId = ua.Id " +
                "LEFT JOIN Positions pa " +
                "ON ua.Position = pa.Position " +
                "LEFT JOIN Sites s " +
                "ON t.SiteReqId = s.SiteId " +
                "WHERE t.Status = 'Pending'" +
                "AND (" +
                "(uf.Name = '" + sesname + "' AND pf.UserType = 'Admin') " +
                "OR " +
                "(ua.Name = '" + sesname + "' AND pa.UserType = 'Admin'))", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");

            ViewBag.Tasklog = task.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }
        [HttpPost]
        public IActionResult Approve(string assignId, string taskId, string fwdId)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            DateTime cdt = DateTime.Now;
            string scdt = cdt.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            if(int.Parse(fwdId) == 0) 
            {
                cmd = new SqlCommand("UPDATE Tasks SET Status = 'Approved', AssignId = '" + assignId + "',  DateRcv = '" + scdt + "' WHERE TaskId = " + taskId, con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("INSERT INTO Tasklog (TaskId, AssignId, DateRcv, Status) VALUES ('" + taskId + "', '" + assignId + "', '" + scdt + "', 'Approved')", con);
                cmd.ExecuteNonQuery();
            }
            else 
            {
                cmd = new SqlCommand("UPDATE Tasks SET Status = 'Approved', DateRcv = '" + scdt + "', AssignId = '" + fwdId + "' WHERE TaskId = " + taskId, con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("INSERT INTO Tasklog (TaskId, AssignId, DateRcv, Status) VALUES ('" + taskId + "', '" + fwdId + "', '" + scdt + "', 'Approved')", con);
                cmd.ExecuteNonQuery();
            }
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return RedirectToAction("Sites", "Admin");
        }
        [HttpPost]
        public IActionResult Decline(string taskId, string assignId)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            DateTime cdt = DateTime.Now;
            string scdt = cdt.ToString("yyyy-MM-dd HH:mm:ss");
            cmd = new SqlCommand("UPDATE Tasks SET DateClr = '" + scdt + "', Status = 'Declined' WHERE TaskId = " + taskId, con);
            cmd.ExecuteNonQuery();

            cmd = new SqlCommand("INSERT INTO Tasklog (TaskId, AssignId, DateClr, Status) VALUES ('" + taskId + "', '" + assignId + "', '" + scdt + "', 'Declined')", con);
            cmd.ExecuteNonQuery();

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return RedirectToAction("Pending", "Admin");
        }
        [HttpPost]
        public IActionResult Complete(string taskId, string assignId)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);

            cmd = new SqlCommand("UPDATE Tasks SET Status = 'Complete' WHERE TaskId = " + taskId, con);
            cmd.ExecuteNonQuery();

            DateTime cdt = DateTime.Now;
            string scdt = cdt.ToString("yyyy-MM-dd HH:mm:ss");
            cmd = new SqlCommand("INSERT INTO Tasklog (TaskId, AssignId, DateRcv) VALUES ('" + taskId + "', '" + assignId + "', '" + scdt + "');", con);
            cmd.ExecuteNonQuery();

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return RedirectToAction("Pending", "Admin");
        }
        public IActionResult History()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);

            cmd = new SqlCommand("SELECT l.LogId, " +
                "l.TaskId, " +
                "l.AssignId, " +
                "l.Status, " +
                "l.DateStart, " +
                "l.DateFwd, " +
                "l.DateRcv, " +
                "l.DateClr, " +
                "t.SiteReqId, " +
                "t.AddedBy, " +
                "s.Client, " +
                "s.Site, " +
                "u.Name, " +
                "u.Position " +
                "FROM Tasklog l " +
                "LEFT JOIN Tasks t ON t.TaskId = l.TaskId " +
                "LEFT JOIN Sites s ON t.SiteReqId = s.SiteId " +
                "LEFT JOIN Users u ON l.AssignId = u.Id " +
                "ORDER BY LogId ASC", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");
            ViewBag.Tasklog = task.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }
        public IActionResult MyTask()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
           }
            if (con.State == ConnectionState.Closed)
            {
               con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            string sesname = HttpContext.Session.GetString(SessionName);
            cmd = new SqlCommand("SELECT t.TaskId AS TaskID_Tasks," +
               " t.Task," +
                " t.Description," +
                " t.Remarks," +
                " t.AddedBy," +
                " t.SiteReqId," +
                " t.DateStart," +
                " t.DateFwd," +
                " t.DateRcv," +
                " t.DateClr," +
                " t.AssignId," +
                " t.ForwardId," +
                " t.Status," +
                " s.Client, " +
                " s.Site, " +
               " u.Id AS Id_user, " +
                " u.Name AS Name, " +
                " u.Position FROM Tasks t " +
                "LEFT JOIN Users u " +
                "ON t.AssignId = u.Id " +
                "LEFT JOIN Sites s " +
                "ON t.SiteReqId = s.SiteId " +
                "WHERE t.Status = 'Approved' AND u.Name = '" + sesname + "' ", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");

            ViewBag.Tasklog = task.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
            
        }
        public IActionResult SiteStatus()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            cmd = new SqlCommand("SELECT s.SiteId, s.Client, s.Site, s.DateAdded, s.Status, s.SiteOM, u.Name FROM Sites s LEFT JOIN Users u ON s.SiteOM = u.Id " +
                "WHERE s.Status = 'Active' OR s.Status = 'Inactive'", con);
            DataSet sites = new DataSet();
            SqlDataAdapter ssite = new SqlDataAdapter(cmd);
            ssite.Fill(sites, "slist");

            ViewBag.Sitelist = sites.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }
        [HttpPost]
        public IActionResult SiteActivate(SitesModel sm)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            cmd = new SqlCommand("UPDATE Sites SET Status = 'Active' WHERE SiteId = '" + sm.SiteId + "'", con);
            cmd.ExecuteNonQuery();
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return RedirectToAction("SiteStatus", "Admin");
        }
        [HttpPost]
        public IActionResult SiteDeactivate(SitesModel sm)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            cmd = new SqlCommand("UPDATE Sites SET Status = 'Inactive' WHERE SiteId = '" + sm.SiteId + "'", con);
            cmd.ExecuteNonQuery();
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return RedirectToAction("SiteStatus", "Admin");
        }
        public IActionResult SitePending()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            cmd = new SqlCommand("SELECT s.SiteId, s.Client, s.Site, s.DateAdded, s.Status, s.SiteOM, u.Name FROM Sites s LEFT JOIN Users u ON s.SiteOM = u.Id " +
                "WHERE s.Status = 'Pending'", con);
            DataSet sites = new DataSet();
            SqlDataAdapter ssite = new SqlDataAdapter(cmd);
            ssite.Fill(sites, "slist");

            ViewBag.Sitelist = sites.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }
        [HttpPost]
        public IActionResult SiteApprove(SitesModel sm)
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);
            cmd = new SqlCommand("UPDATE Sites SET Status = 'Active'" +
                "WHERE SiteId = '" + sm.SiteId + "'", con);
            cmd.ExecuteNonQuery();

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return RedirectToAction("Sites", "Admin");
        }
        public IActionResult AllTask()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);

            cmd = new SqlCommand("SELECT t.TaskId, t.Task, t.Remarks, t.Description, u.Name, s.Client, s.Site FROM Tasks t " +
                "LEFT JOIN Users u ON u.Id =  t.AssignId LEFT JOIN Sites s ON s.SiteId = t.SiteReqId ORDER BY TaskId DESC", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");
            ViewBag.tlist = task.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }

        public IActionResult DocumentRequest()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);

            cmd = new SqlCommand("SELECT t.TaskId, t.Task, t.Remarks, t.Description, u.Name, s.Client, s.Site FROM Tasks t " +
                "LEFT JOIN Users u ON u.Id =  t.AssignId LEFT JOIN Sites s ON s.SiteId = t.SiteReqId ORDER BY TaskId DESC", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");
            ViewBag.tlist = task.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }

        public IActionResult Variation()
        {
            if (HttpContext.Session.GetString(SessionType) != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            ViewBag.Layout = HttpContext.Session.GetString(SessionLayout);

            cmd = new SqlCommand("SELECT t.TaskId, t.Task, t.Remarks, t.Description, u.Name, s.Client, s.Site FROM Tasks t " +
                "LEFT JOIN Users u ON u.Id =  t.AssignId LEFT JOIN Sites s ON s.SiteId = t.SiteReqId ORDER BY TaskId DESC", con);
            DataSet task = new DataSet();
            SqlDataAdapter tasks = new SqlDataAdapter(cmd);
            tasks.Fill(task, "slist");
            ViewBag.tlist = task.Tables[0];

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return View();
        }


    }



}   

