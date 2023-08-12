using MedicalVisionPioneer.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalVisionPioneer.Controllers
{
    public class DoctorController : Controller
    {
        private MedicalVisionPioneerEntities db = new MedicalVisionPioneerEntities();
        // GET: Doctor
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear(); // 或者使用 Session.Remove("useremail") 删除指定键的会话数据
            return new EmptyResult();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(string email, string password, string major, string name)
        {
            if (String.IsNullOrEmpty(email))
            {
                ViewBag.notice = "The email cannot be empty!";
                ViewBag.email = email;
                return View();
            }
            if (String.IsNullOrEmpty(password))
            {
                ViewBag.notice = "The password cannot be empty!";
                return View();
            }
            if (String.IsNullOrEmpty(major))
            {
                ViewBag.notice = "The major cannot be empty!";
                return View();
            }
            if (String.IsNullOrEmpty(name))
            {
                ViewBag.notice = "The name cannot be empty!";
                return View();
            }

            // 检查数据库中是否存在相同的 email
            bool emailExists = db.docAccount.Any(a => a.docEmail == email);
            if (emailExists)
            {
                ViewBag.notice = "The doc already exists!";
            }
            else
            {
                // 创建新的用户并将其添加到数据库
                docAccount doctor = new docAccount();
                doctor.docEmail = email;
                doctor.docPwd = password;
                db.docAccount.Add(doctor);
                db.SaveChanges();

                docInfo info = new docInfo();
                info.docId = doctor.id;
                info.docMajor = major;
                info.docName = name;
                info.docAppointment = false;
                db.docInfo.Add(info);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Register successful!";
                return Redirect("/Doctor/Login");
            }

            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(String email, String password)
        {
            if (String.IsNullOrEmpty(email))
            {
                ViewBag.notice = "The email cannot be empty！";
                ViewBag.email = email;
                return View();
            }
            if (String.IsNullOrEmpty(password))
            {
                ViewBag.notice = "The pwd cannot be empty！";
                return View();
            }
            //去数据库查询是否存在该用户

            docAccount doctor = db.docAccount.FirstOrDefault(d => d.docEmail == email);
            if (doctor == null)
            {
                ViewBag.notice = "The doc is not exist！";
            }
            else if (doctor.docPwd != password)
            {
                ViewBag.notice = "The pwd is not correct！";
            }
            else
            {
                Session["docemail"] = doctor.docEmail;

                return Redirect("/Doctor/Index");
            }

            return View();
        }
        public ActionResult Appointment(int? page)
        {
            string currentUserEmail = (string)Session["docemail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                // 当前用户未登录，执行相应的处理逻辑（例如重定向到登录页面）
                return RedirectToAction("Login", "Doctor");
            }
            docAccount currentUser = db.docAccount.FirstOrDefault(p => p.docEmail == currentUserEmail);
            docInfo currentDoc = db.docInfo.FirstOrDefault(d => d.docId == currentUser.id);
                if (currentDoc == null)
            {
                // 用户数据不存在，执行适当的处理逻辑
                // ...
            }
            // 获取当前用户的体重记录
            List<Appointment> Appointment = db.Appointment.Where(r => r.docid == currentUser.id).ToList();
            int pageSize = 5; // 每页显示的记录数
            int pageNumber = (page ?? 1); // 当前页码

            return View(Appointment.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult doc_profile()
        {
            string currentUserEmail = (string)Session["docemail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return RedirectToAction("Login", "Doctor");
            }
            docAccount currentUser = db.docAccount.FirstOrDefault(p => p.docEmail == currentUserEmail);
            docInfo currentDoc = db.docInfo.FirstOrDefault(d => d.docId == currentUser.id);
            return View(currentDoc);
        }
        
        public ActionResult doc_settings()
        {
            string currentUserEmail = (string)Session["docemail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return RedirectToAction("Login", "Doctor");
            }
            docAccount currentUser = db.docAccount.FirstOrDefault(p => p.docEmail == currentUserEmail);
            docInfo currentDoc = db.docInfo.FirstOrDefault(d => d.docId == currentUser.id);

            return View(currentDoc);
        }

        [HttpPost]
        public ActionResult doc_settings(bool docStatus)
        {
            string currentUserEmail = (string)Session["docemail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return RedirectToAction("Login", "Doctor");
            }
            docAccount currentUser = db.docAccount.FirstOrDefault(p => p.docEmail == currentUserEmail);
            docInfo currentDoc = db.docInfo.FirstOrDefault(d => d.docId == currentUser.id);
            currentDoc.docAppointment = docStatus;
            db.docInfo.AddOrUpdate(currentDoc);
            db.SaveChanges();

            return View(currentDoc);
        }


        [HttpPost]
        public ActionResult ConsultPatient(int appointmentId)
        {
            var record = db.Appointment.FirstOrDefault(a => a.id == appointmentId);
            if (record == null)
            {
                // 处理记录不存在的情况
                return RedirectToAction("Appointment"); // 或者返回错误信息页面
            }

            record.status = true; // 更新记录的状态
            db.SaveChanges(); // 保存更改

            return RedirectToAction("Appointment");
        }

    }
}