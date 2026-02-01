using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class facultySendAnnouncement : Page
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
                BindCoursesDropdown();
                BindAnnouncementsGridView();
            }
        }

        private void BindCoursesDropdown()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int facultyId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT DISTINCT C.CourseID, C.CourseName
                        FROM Courses C
                        INNER JOIN Assignments A ON C.CourseID = A.CourseID
                        WHERE A.UploadedBy = @FacultyID
                        ORDER BY C.CourseName", con);
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlCourse.DataSource = dt;
                    ddlCourse.DataTextField = "CourseName";
                    ddlCourse.DataValueField = "CourseID";
                    ddlCourse.DataBind();
                    ddlCourse.Items.Insert(0, new ListItem("-- General Announcement (No Course) --", ""));
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading courses: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        private void BindAnnouncementsGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int facultyId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT A.AnnouncementID, A.Message, A.DatePosted, C.CourseName
                        FROM Announcements A
                        LEFT JOIN Courses C ON A.CourseID = C.CourseID
                        WHERE A.FacultyID = @FacultyID
                        ORDER BY A.DatePosted DESC", con);
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAnnouncements.DataSource = dt;
                    gvAnnouncements.DataBind();
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

        protected void btnSendAnnouncement_Click(object sender, EventArgs e)
        {
            string message = txtAnnouncementMessage.Text.Trim();
            int facultyId = Convert.ToInt32(Session["UserID"]);
            int? courseId = null;

            if (!string.IsNullOrEmpty(ddlCourse.SelectedValue))
            {
                courseId = Convert.ToInt32(ddlCourse.SelectedValue);
            }

            if (string.IsNullOrEmpty(message))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Announcement message cannot be empty.</div>";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Announcements (FacultyID, CourseID, Message, DatePosted) VALUES (@FacultyID, @CourseID, @Message, GETDATE())", con);
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
                    if (courseId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@CourseID", courseId.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CourseID", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@Message", message);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>Announcement sent successfully.</div>";
                    txtAnnouncementMessage.Text = string.Empty;
                    ddlCourse.SelectedIndex = 0;
                    BindAnnouncementsGridView();
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

        protected void gvAnnouncements_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int announcementId = Convert.ToInt32(gvAnnouncements.DataKeys[e.RowIndex].Value);
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Announcements WHERE AnnouncementID = @AnnouncementID", con);
                    cmd.Parameters.AddWithValue("@AnnouncementID", announcementId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>Announcement deleted successfully.</div>";
                    BindAnnouncementsGridView();
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