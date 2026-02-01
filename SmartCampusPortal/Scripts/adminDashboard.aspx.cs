using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;

namespace SmartCampusPortal
{
    public partial class adminDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
                {
                    FormsAuthentication.SignOut();
                    Response.Redirect("Login.aspx");
                }
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmdStudents = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Role = 'Student'", con);
                    lblTotalStudents.Text = cmdStudents.ExecuteScalar().ToString();

                    SqlCommand cmdFaculty = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Role = 'Faculty'", con);
                    lblTotalFaculty.Text = cmdFaculty.ExecuteScalar().ToString();

                    SqlCommand cmdCourses = new SqlCommand("SELECT COUNT(*) FROM Courses", con);
                    lblTotalCourses.Text = cmdCourses.ExecuteScalar().ToString();

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

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}