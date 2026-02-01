using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;

namespace SmartCampusPortal
{
    public partial class studentDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Always check session for authentication and role first, regardless of postback
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Student")
            {
                FormsAuthentication.SignOut();
                Response.Redirect("Login.aspx");
                return; // Important: Stop further execution if redirection occurs
            }

            if (!IsPostBack)
            {
                litWelcomeMessage.Text = $"<h3 class='mb-3'>Welcome, {Session["FullName"]}!</h3>";
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmdRegisteredCourses = new SqlCommand("SELECT COUNT(*) FROM CourseRegistrations WHERE StudentID = @StudentID", con);
                    cmdRegisteredCourses.Parameters.AddWithValue("@StudentID", studentId);
                    lblRegisteredCourses.Text = cmdRegisteredCourses.ExecuteScalar().ToString();

                    SqlCommand cmdPendingFees = new SqlCommand("SELECT ISNULL(SUM(TotalAmount - PaidAmount), 0) FROM FeeRecords WHERE StudentID = @StudentID AND PaymentStatus != 'Fully Paid'", con);
                    cmdPendingFees.Parameters.AddWithValue("@StudentID", studentId);
                    lblPendingFees.Text = Convert.ToDecimal(cmdPendingFees.ExecuteScalar()).ToString("N2");

                    SqlCommand cmdAnnouncements = new SqlCommand(@"
                        SELECT TOP 5 A.Message, A.DatePosted, C.CourseName
                        FROM Announcements A
                        LEFT JOIN Courses C ON A.CourseID = C.CourseID
                        WHERE A.CourseID IS NULL OR A.CourseID IN (SELECT CourseID FROM CourseRegistrations WHERE StudentID = @StudentID)
                        ORDER BY A.DatePosted DESC", con);
                    cmdAnnouncements.Parameters.AddWithValue("@StudentID", studentId);
                    SqlDataReader reader = cmdAnnouncements.ExecuteReader();

                    if (reader.HasRows)
                    {
                        noAnnouncements.Visible = false;
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append("<ul class='list-group'>");
                        while (reader.Read())
                        {
                            string courseName = reader["CourseName"] == DBNull.Value ? "General" : reader["CourseName"].ToString();
                            sb.Append($"<li class='list-group-item bg-dark text-white border-secondary mb-2'>");
                            sb.Append($"<strong>{(courseName == "General" ? "General Announcement" : "Course: " + courseName)}</strong><br />");
                            sb.Append($"Message: {reader["Message"]}<br />");
                            sb.Append($"<small class='text-muted'>Posted on: {Convert.ToDateTime(reader["DatePosted"]).ToShortDateString()}</small>");
                            sb.Append("</li>");
                        }
                        sb.Append("</ul>");
                        litLatestAnnouncements.Text = sb.ToString();
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