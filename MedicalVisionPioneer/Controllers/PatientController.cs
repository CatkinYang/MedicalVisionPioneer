﻿using MedicalVisionPioneer.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace MedicalVisionPioneer.Controllers
{
    public class PatientController : Controller
    {
        private MedicalVisionPioneerEntities db = new MedicalVisionPioneerEntities();


        public ActionResult Index(int? page)
        {
            string currentUserEmail = (string)Session["useremail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                // 当前用户未登录，执行相应的处理逻辑（例如重定向到登录页面）
                return RedirectToAction("Login", "Patient");
            }

            // 根据当前用户的邮箱获取对应的用户数据
            PatientInfo currentUser = db.PatientInfo.FirstOrDefault(p => p.patientEmail == currentUserEmail);
            if (currentUser == null)
            {
                // 用户数据不存在，执行适当的处理逻辑
                // ...
            }

            // 获取当前用户的体重记录
            List<HWinfo> weightRecords = db.HWinfo.Where(r => r.patientId== currentUser.AccountID).ToList();

            int pageSize = 5; // 每页显示的记录数
            int pageNumber = (page ?? 1); // 当前页码

            return View(weightRecords.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Logout()
        {
            Session.Clear(); // 或者使用 Session.Remove("useremail") 删除指定键的会话数据
            return new EmptyResult();
        }


        //实现登录功能
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
            Account patient = db.Account.FirstOrDefault(p => p.patientEmail == email);
            if (patient == null)
            {
                ViewBag.notice = "The user is not exist！";
            }
            else if (patient.patientPwd != password)
            {
                ViewBag.notice = "The pwd is not correct！";
            }
            else
            {
                Session["useremail"] = patient.patientEmail;
                return Redirect("/Patient/Index");
            }

            return View();
        }

        public ActionResult Settings()
        {
            string currentUserEmail = (string)Session["useremail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return RedirectToAction("Login", "Patient");
            }
            PatientInfo currentUser = db.PatientInfo.FirstOrDefault(p => p.patientEmail == currentUserEmail);
            if (currentUser == null)
            {
            }

            return View(currentUser);
        }

        public ActionResult Profile_info()
        {
            string currentUserEmail = (string)Session["useremail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                // 当前用户未登录，执行相应的处理逻辑（例如重定向到登录页面）
                return RedirectToAction("Login", "Patient");
            }
            else
            {
                // 根据当前用户的邮箱获取对应的用户数据
                PatientInfo info = db.PatientInfo.FirstOrDefault(p => p.patientEmail == currentUserEmail);
                if (info == null)
                {
                    // 用户数据不存在，执行适当的处理逻辑
                    // ...
                }

                return View(info);
            }
        }

        public ActionResult Map()
        {
            return View();
        }

        //Modify
        public ActionResult Profile_Modify()
        {
            string currentUserEmail = (string)Session["useremail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                // 当前用户未登录，执行相应的处理逻辑（例如重定向到登录页面）
                return RedirectToAction("Login", "Patient");
            }

            // 根据当前用户的邮箱获取对应的用户数据
            PatientInfo info = db.PatientInfo.FirstOrDefault(p => p.patientEmail == currentUserEmail);
            if (info == null)
            {
                // 用户数据不存在，执行适当的处理逻辑
                // ...
            }

            return View(info);
        }

        [HttpPost]
        public ActionResult Profile_Modify(PatientInfo info)
        {
            string currentUserEmail = (string)Session["useremail"];
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                // 当前用户未登录，执行相应的处理逻辑（例如重定向到登录页面）
                return RedirectToAction("Login", "Patient");
            }

            // 根据当前用户的邮箱获取对应的用户数据
            PatientInfo existingInfo = db.PatientInfo.FirstOrDefault(p => p.patientEmail == currentUserEmail);
            if (existingInfo == null)
            {
                // 用户数据不存在，执行适当的处理逻辑
                // ...
            }

            existingInfo.patientPhone = info.patientPhone;
            existingInfo.patientWeight = info.patientWeight;
            existingInfo.patientAge = info.patientAge;
            existingInfo.patientHeight = info.patientHeight;
            existingInfo.patientBirthday = info.patientBirthday;
            existingInfo.patientSex = info.patientSex;
            existingInfo.patientNickname = info.patientNickname;
            existingInfo.patientName= info.patientName;
            
            HWinfo newHWinfo = new HWinfo();
            newHWinfo.patientId = existingInfo.AccountID;
            newHWinfo.patientHeight = existingInfo.patientHeight; 
            newHWinfo.patientWeight = existingInfo.patientWeight;
            newHWinfo.infoCreaterdate = DateTime.Now;


            db.HWinfo.Add(newHWinfo);
            
            db.Entry(existingInfo).State = System.Data.Entity.EntityState.Modified;

            if (db.SaveChanges() > 0)
            {
                return Content("<script>alert('Update info success!');window.location.href='/Patient/Profile_info';</script>");
            }
            else
            {
                ViewBag.notice = "Update the profile failure! Please retry";
            }

            return View();
        }


        //Regester
        public ActionResult Regester()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Regester(string email, string password)
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

            // 检查数据库中是否存在相同的 email
            bool emailExists = db.Account.Any(a => a.patientEmail == email);
            if (emailExists)
            {
                ViewBag.notice = "The user already exists!";
            }
            else
            {
                // 生成唯一的 id
                int randomId = GenerateUniqueId();

                // 创建新的用户并将其添加到数据库
                Account patient = new Account();
                patient.patientId = randomId;
                patient.patientEmail = email;
                patient.patientPwd = password;
                db.Account.Add(patient);
                db.SaveChanges();

                PatientInfo info = new PatientInfo();
                info.AccountID = randomId;
                info.patientEmail = email;
                db.PatientInfo.Add(info);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Register successful!";
                return Redirect("/Patient/Login");
            }

            return View();
        }


        // 生成唯一的 id
        private int GenerateUniqueId()
        {
            Random random = new Random();
            int randomId = random.Next(100000, 999999); // 随机生成一个六位数的数字

            // 检查数据库中是否存在相同的 id
            bool idExists = db.Account.Any(a => a.patientId == randomId);
            if (idExists)
            {
                // 如果存在相同的 id，则递归调用该方法重新生成唯一的 id
                randomId = GenerateUniqueId();
            }

            return randomId;
        }


    }
}