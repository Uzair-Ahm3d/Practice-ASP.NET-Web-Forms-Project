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
                BindAdminAnnouncements();
            }
        }

        protected System.Web.UI.WebControls.TextBox txtAnnouncement;
        protected System.Web.UI.WebControls.Button btnSendAnnouncement;
        protected System.Web.UI.WebControls.Literal litAnnounceMsg;
        protected System.Web.UI.WebControls.GridView gvAdminAnnouncements;

        private void BindAdminAnnouncements()
        {
            string cs = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT TOP 10 Message, DatePosted FROM Announcements WHERE CourseID IS NULL ORDER BY DatePosted DESC, AnnouncementID DESC", con);
                System.Data.DataTable dt = new System.Data.DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
                gvAdminAnnouncements.DataSource = dt;
                gvAdminAnnouncements.DataBind();
            }
        }

        protected void btnSendAnnouncement_Click(object sender, EventArgs e)
        {
            litAnnounceMsg.Text = "";
            string message = txtAnnouncement.Text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                litAnnounceMsg.Text = "<div class='alert alert-warning mt-3'>Please type a message.</div>";
                return;
            }
            string cs = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    // Global announcement: no course, no faculty -> visible to all students and faculty
                    SqlCommand cmd = new SqlCommand("INSERT INTO Announcements (FacultyID, CourseID, Message, DatePosted) VALUES (NULL, NULL, @m, GETDATE())", con);
                    cmd.Parameters.AddWithValue("@m", message);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                txtAnnouncement.Text = "";
                litAnnounceMsg.Text = "<div class='alert alert-success mt-3'>Announcement posted to all students and faculty.</div>";
                BindAdminAnnouncements();
            }
            catch (Exception ex)
            {
                litAnnounceMsg.Text = "<div class='alert alert-danger mt-3'>Error posting announcement: " + ex.Message + "</div>";
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