using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class studentViewGrades : Page
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
                BindCoursesDropdown();
                BindGradesGridView(0); // Load all grades initially
            }
        }

        private void BindCoursesDropdown()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT C.CourseID, C.CourseName
                        FROM Courses C
                        INNER JOIN CourseRegistrations CR ON C.CourseID = CR.CourseID
                        WHERE CR.StudentID = @StudentID
                        ORDER BY C.CourseName", con);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlCourseFilter.DataSource = dt;
                    ddlCourseFilter.DataTextField = "CourseName";
                    ddlCourseFilter.DataValueField = "CourseID";
                    ddlCourseFilter.DataBind();
                    ddlCourseFilter.Items.Insert(0, new ListItem("-- All Courses --", ""));
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

        protected void ddlCourseFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCourseFilter.SelectedValue != "")
            {
                BindGradesGridView(Convert.ToInt32(ddlCourseFilter.SelectedValue));
            }
            else
            {
                BindGradesGridView(0);
            }
        }

        protected void btnFilterGrades_Click(object sender, EventArgs e)
        {
            int courseId = 0;
            if (!string.IsNullOrEmpty(ddlCourseFilter.SelectedValue))
            {
                courseId = Convert.ToInt32(ddlCourseFilter.SelectedValue);
            }
            BindGradesGridView(courseId);
        }

        private void BindGradesGridView(int courseId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT C.CourseName, A.Title AS AssignmentTitle, S.SubmissionDate, ISNULL(CAST(S.Grade AS NVARCHAR(50)), 'N/A') AS Grade
                        FROM Submissions S
                        INNER JOIN Assignments A ON S.AssignmentID = A.AssignmentID
                        INNER JOIN Courses C ON A.CourseID = C.CourseID
                        WHERE S.StudentID = @StudentID";

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@StudentID", studentId);

                    if (courseId > 0)
                    {
                        query += " AND A.CourseID = @CourseID";
                        cmd.Parameters.AddWithValue("@CourseID", courseId);
                    }

                    query += " ORDER BY C.CourseName, S.SubmissionDate DESC";
                    cmd.CommandText = query;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    // Explicitly define the 'Grade' column as a string type
                    // This prevents the DataTable from inferring a numeric type and failing on 'N/A'
                    dt.Columns.Add("CourseName", typeof(string));
                    dt.Columns.Add("AssignmentTitle", typeof(string));
                    dt.Columns.Add("SubmissionDate", typeof(DateTime));
                    dt.Columns.Add("Grade", typeof(string)); // Crucial change: Define Grade as string

                    da.Fill(dt); // Now da.Fill will respect the predefined column types

                    gvGrades.DataSource = dt;
                    gvGrades.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        litMessage.Text = "<div class='alert alert-info mt-3'>No grades found for the selected criteria.</div>";
                    }
                    else
                    {
                        litMessage.Text = string.Empty;
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

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
