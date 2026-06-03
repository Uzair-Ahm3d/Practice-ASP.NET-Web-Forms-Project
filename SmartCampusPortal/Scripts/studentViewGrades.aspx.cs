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
            }
        }

        // New controls (declared here to avoid designer churn)
        protected Panel pnlGrades;
        protected GridView gvAssignments;
        protected GridView gvQuizzes;
        protected GridView gvPapers;
        protected GridView gvProjects;
        protected GridView gvPresentations;

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

        protected void ddlCourseFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            litMessage.Text = string.Empty;
            if (string.IsNullOrEmpty(ddlCourseFilter.SelectedValue))
            {
                pnlGrades.Visible = false;
                return;
            }
            pnlGrades.Visible = true;
            int courseId = Convert.ToInt32(ddlCourseFilter.SelectedValue);
            int studentId = Convert.ToInt32(Session["UserID"]);
            try
            {
                BindAssignments(courseId, studentId);
                BindCategory(gvQuizzes, "Quiz", courseId, studentId);
                BindCategory(gvPapers, "Paper", courseId, studentId);
                BindCategory(gvProjects, "Project", courseId, studentId);
                BindCategory(gvPresentations, "Presentation", courseId, studentId);
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Error loading grades: " + ex.Message + "</div>";
            }
        }

        // Assignment grades come from the Submissions table.
        private void BindAssignments(int courseId, int studentId)
        {
            string cs = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT A.Title, S.SubmissionDate,
                           ISNULL(CAST(S.Grade AS NVARCHAR(50)), 'Not graded') AS Grade
                    FROM Submissions S
                    INNER JOIN Assignments A ON S.AssignmentID = A.AssignmentID
                    WHERE S.StudentID = @s AND A.CourseID = @c
                    ORDER BY S.SubmissionDate DESC", con);
                cmd.Parameters.AddWithValue("@s", studentId);
                cmd.Parameters.AddWithValue("@c", courseId);
                DataTable dt = new DataTable();
                dt.Columns.Add("Title", typeof(string));
                dt.Columns.Add("SubmissionDate", typeof(DateTime));
                dt.Columns.Add("Grade", typeof(string));
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
                gvAssignments.DataSource = dt;
                gvAssignments.DataBind();
            }
        }

        // Quiz / Paper / Project / Presentation grades come from the Grades table.
        private void BindCategory(GridView gv, string category, int courseId, int studentId)
        {
            string cs = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT Title,
                           CAST(ISNULL(Score,0) AS NVARCHAR(10)) + ' / ' + CAST(ISNULL(MaxScore,0) AS NVARCHAR(10)) AS ScoreText,
                           ISNULL(Remarks,'') AS Remarks, DateGraded
                    FROM Grades
                    WHERE StudentID = @s AND CourseID = @c AND Category = @cat
                    ORDER BY DateGraded DESC", con);
                cmd.Parameters.AddWithValue("@s", studentId);
                cmd.Parameters.AddWithValue("@c", courseId);
                cmd.Parameters.AddWithValue("@cat", category);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
                gv.DataSource = dt;
                gv.DataBind();
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
