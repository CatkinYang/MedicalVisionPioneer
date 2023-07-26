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
        // GET: Doctor
        public ActionResult Index()
        {
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

            Account patient = db.Account.FirstOrDefault(p => p.patientEmail == email);
            if (doctor == null)
            {
                ViewBag.notice = "The user is not exist！";
            }
            else if (doctor.doctorPwd != password)
            {
                ViewBag.notice = "The pwd is not correct！";
            }
            else
            {
                //在这里需要记住登录成功的用户信息  COOKIE 或者 SESSION =》会话管理（HTTP请求是不会带状态请求的，需要使用COOKIE或者SESSION来标识请求，区分是谁访问服务端） COOKIE和SESSION有什么区别？ SESSION存储的安全性更高，安全数据，隐私数据推荐存储到SESSION中。额外注意：记住用户自己的账号和密码，则是用COOKIE，因为用户本身知道这些数据，存储在SESSION和COOKIE没区别。
                Session["useremail"] = doctor.doctorEmail;

                //登录成功，跳转到后端的页面
                return Redirect("/Patient/Index");
            }

            return View();
        }
    }
}