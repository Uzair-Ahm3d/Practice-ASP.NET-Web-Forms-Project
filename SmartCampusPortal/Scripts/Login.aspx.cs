using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;

namespace SmartCampusPortal
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.IsAuthenticated)
                {
                    string role = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).UserData;
                    RedirectUser(role);
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please enter both email and password.</div>";
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserID, FullName, Role FROM Users WHERE Email = @Email AND Password = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string role = reader["Role"].ToString();
                            string userId = reader["UserID"].ToString();
                            string fullName = reader["FullName"].ToString();

                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                1, // version
                                email, // user name
                                DateTime.Now, // creation
                                DateTime.Now.AddMinutes(30), // expiration
                                false, // persistent
                                role // user data (roles)
                            );

                            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                            Response.Cookies.Add(new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));

                            Session["UserID"] = userId;
                            Session["FullName"] = fullName;
                            Session["UserRole"] = role;

                            RedirectUser(role);
                        }
                        else
                        {
                            litMessage.Text = "<div class='alert alert-danger mt-3'>Invalid email or password.</div>";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        private void RedirectUser(string role)
        {
            switch (role)
            {
                case "Admin":
                    Response.Redirect("adminDashboard.aspx");
                    break;
                case "Student":
                    Response.Redirect("studentDashboard.aspx");
                    break;
                case "Faculty":
                    Response.Redirect("facultyDashboard.aspx");
                    break;
                default:
                    FormsAuthentication.SignOut();
                    Response.Redirect("Login.aspx");
                    break;
            }
        }
    }
}