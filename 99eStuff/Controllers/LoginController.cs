using _99eStuff.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using _99eStuff.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace _99eStuff.Controllers
{
    public class LoginController : Controller
    {

        ConnectionData connectionData = new ConnectionData();
        DataTable dt;

        // GET: /Login/
        public ActionResult LoginRegister()
        {
            return View();
        }
        public ActionResult LoginActive()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Autherize(UsersLoginViewModel userModel)
        {
            string mycmd = "select * from UserData";
            dt = new DataTable();

            dt = connectionData.GetAll(mycmd);


            List<UsersLoginViewModel> list = new List<UsersLoginViewModel>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                UsersLoginViewModel users = new UsersLoginViewModel
                {
                    UserID = Convert.ToInt32(dt.Rows[i]["IdUser"]),
                    UserName = dt.Rows[i]["UserName"].ToString(),
                    Password = dt.Rows[i]["Password"].ToString(),
                    Mail = dt.Rows[i]["Mail"].ToString(),
                    NameClient = dt.Rows[i]["NameClient"].ToString(),
                };

                list.Add(users);
            }

                var userDetails = list.Where(x => x.UserName == userModel.UserName && x.Password == userModel.Password).FirstOrDefault();
                if (userDetails == null)
                {
                    userModel.LoginErrorMessage = "Wrong username or password.";
                    return View("LoginRegister", userModel);
                }
                else
                {
                    Session["userID"] = userDetails.UserID;
                    Session["userName"] = userDetails.UserName;
                    Session["mail"] = userDetails.Mail;
                    Session["nameClient"] = userDetails.NameClient;
                    return RedirectToAction("LoginActive", "Login");
                }
        }

        [HttpPost]
        public ActionResult Register(UsersLoginViewModel client)
        {
            string constr = ConfigurationManager.ConnectionStrings["ProductsModel"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO dbo.UserData(UserName, Password, Mail, NameClient) VALUES(@UserName, @Password, @Mail, @NameClient)";
                query += " SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    cmd.Parameters.AddWithValue("@UserName", client.UserName);
                    cmd.Parameters.AddWithValue("@Password", client.Password);
                    cmd.Parameters.AddWithValue("@Mail", client.Mail);
                    cmd.Parameters.AddWithValue("@NameClient", client.NameClient);
                    client.UserID = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
            }

            return RedirectToAction("LoginRegister", "Login"); ;
        }



        public ActionResult LogOut()
        {
            int userId = (int)Session["userID"];
            Session.Abandon();
            return RedirectToAction("LoginRegister", "Login");
        }
    }
}