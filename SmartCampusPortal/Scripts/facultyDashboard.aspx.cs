using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;

namespace SmartCampusPortal
{
    public partial class facultyDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Faculty")
                {
                    FormsAuthentication.SignOut();
                    Response.Redirect("Login.aspx");
                }
                litWelcomeMessage.Text = $"<h3 class='mb-3'>Welcome, {Session["FullName"]}!</h3>";
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int facultyId = Convert.ToInt32(Session["UserID"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmdCoursesTaught = new SqlCommand("SELECT COUNT(DISTINCT CourseID) FROM Assignments WHERE UploadedBy = @FacultyID", con);
                    cmdCoursesTaught.Parameters.AddWithValue("@FacultyID", facultyId);
                    lblCoursesTaught.Text = cmdCoursesTaught.ExecuteScalar().ToString();

                    SqlCommand cmdPendingGrading = new SqlCommand(@"
                        SELECT COUNT(S.SubmissionID)
                        FROM Submissions S
                        INNER JOIN Assignments A ON S.AssignmentID = A.AssignmentID
                        WHERE A.UploadedBy = @FacultyID AND S.Grade IS NULL", con);
                    cmdPendingGrading.Parameters.AddWithValue("@FacultyID", facultyId);
                    lblPendingGrading.Text = cmdPendingGrading.ExecuteScalar().ToString();

                    SqlCommand cmdAnnouncements = new SqlCommand(@"
                        SELECT TOP 5 A.Message, A.DatePosted, C.CourseName
                        FROM Announcements A
                        INNER JOIN Courses C ON A.CourseID = C.CourseID
                        WHERE A.FacultyID = @FacultyID
                        ORDER BY A.DatePosted DESC", con);
                    cmdAnnouncements.Parameters.AddWithValue("@FacultyID", facultyId);
                    SqlDataReader reader = cmdAnnouncements.ExecuteReader();

                    if (reader.HasRows)
                    {
                        noAnnouncements.Visible = false;
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append("<ul class='list-group'>");
                        while (reader.Read())
                        {
                            sb.Append($"<li class='list-group-item bg-dark text-white border-secondary mb-2'>");
                            sb.Append($"<strong>Course: {reader["CourseName"]}</strong><br />");
                            sb.Append($"Message: {reader["Message"]}<br />");
                            sb.Append($"<small class='text-muted'>Posted on: {Convert.ToDateTime(reader["DatePosted"]).ToShortDateString()}</small>");
                            sb.Append("</li>");
                        }
                        sb.Append("</ul>");
                        litRecentAnnouncements.Text = sb.ToString();
                    }
                    else
                    {
                        noAnnouncements.Visible = true;
                    }
                    reader.Close();
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