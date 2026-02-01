using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class adminViewAttendanceReport : Page
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
                BindCoursesDropdown();
                BindStudentsDropdown();
                BindAttendanceGridView();
            }
        }

        private void BindCoursesDropdown()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT CourseID, CourseName FROM Courses ORDER BY CourseName", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlCourseFilter.DataSource = dt;
                    ddlCourseFilter.DataTextField = "CourseName";
                    ddlCourseFilter.DataValueField = "CourseID";
                    ddlCourseFilter.DataBind();
                    ddlCourseFilter.Items.Insert(0, new ListItem("-- Select Course --", ""));
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

        private void BindStudentsDropdown()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT UserID, FullName FROM Users WHERE Role = 'Student' ORDER BY FullName", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlStudentFilter.DataSource = dt;
                    ddlStudentFilter.DataTextField = "FullName";
                    ddlStudentFilter.DataValueField = "UserID";
                    ddlStudentFilter.DataBind();
                    ddlStudentFilter.Items.Insert(0, new ListItem("-- Select Student --", ""));
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading students: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        private void BindAttendanceGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT A.Date, A.Status, U.FullName, C.CourseName
                        FROM Attendance A
                        INNER JOIN Users U ON A.StudentID = U.UserID
                        INNER JOIN Courses C ON A.CourseID = C.CourseID
                        WHERE 1=1";

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    if (!string.IsNullOrEmpty(ddlCourseFilter.SelectedValue))
                    {
                        query += " AND A.CourseID = @CourseID";
                        cmd.Parameters.AddWithValue("@CourseID", Convert.ToInt32(ddlCourseFilter.SelectedValue));
                    }
                    if (!string.IsNullOrEmpty(ddlStudentFilter.SelectedValue))
                    {
                        query += " AND A.StudentID = @StudentID";
                        cmd.Parameters.AddWithValue("@StudentID", Convert.ToInt32(ddlStudentFilter.SelectedValue));
                    }
                    if (!string.IsNullOrEmpty(txtDateFilter.Text))
                    {
                        query += " AND A.Date = @Date";
                        cmd.Parameters.AddWithValue("@Date", Convert.ToDateTime(txtDateFilter.Text));
                    }
                    if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
                    {
                        query += " AND A.Status = @Status";
                        cmd.Parameters.AddWithValue("@Status", ddlStatusFilter.SelectedValue);
                    }

                    query += " ORDER BY A.Date DESC, C.CourseName, U.FullName";
                    cmd.CommandText = query;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAttendance.DataSource = dt;
                    gvAttendance.DataBind();
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

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindAttendanceGridView();
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