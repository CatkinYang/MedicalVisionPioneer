using MedicalVisionPioneer.Models;
using System;
using System.Collections.Generic;
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
        
        public ActionResult Appointment() {

            return View();
        }

        [HttpPost]
        public ActionResult Appointment(string selectedDate)
        {
            string currentUserEmail = (string)Session["docemail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                // 当前用户未登录，执行相应的处理逻辑（例如重定向到登录页面）
                return RedirectToAction("Login", "Doctor");
            }
            docAccount existingInfo = db.docAccount.FirstOrDefault(d => d.docEmail == currentUserEmail);
            if (existingInfo == null)
            {
                // 用户数据不存在，执行适当的处理逻辑
                // ...
            }
            openAppointment newAppointment = new openAppointment();
            newAppointment.docId = existingInfo.id;
            newAppointment.date = DateTime.Parse(selectedDate);
            db.openAppointment.Add(newAppointment);
            db.SaveChanges();

            return View();
        }

        public ActionResult doc_profile()
        {
            return View();
        }

        public ActionResult doc_settings()
        {
            return View();
        }
    
    
    }
}