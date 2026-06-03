using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class facultyGradeStudents : Page
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
                BindAssignmentsDropdown();
                BindGradeCourses();
            }
        }

        // ---- Grade Quizzes / Papers / Projects / Presentations ----
        protected DropDownList ddlGradeCourse;
        protected DropDownList ddlGradeStudent;
        protected DropDownList ddlCategory;
        protected TextBox txtGTitle;
        protected TextBox txtScore;
        protected TextBox txtMaxScore;
        protected TextBox txtRemarks;
        protected Button btnAddGrade;
        protected Literal litGradeMsg;

        private void BindGradeCourses()
        {
            string cs = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int fid = Convert.ToInt32(Session["UserID"]);
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT DISTINCT C.CourseID, C.CourseName
                    FROM Courses C INNER JOIN CourseEnrollments CE ON C.CourseID=CE.CourseID
                    WHERE CE.FacultyID=@f ORDER BY C.CourseName", con);
                cmd.Parameters.AddWithValue("@f", fid);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
                ddlGradeCourse.DataSource = dt;
                ddlGradeCourse.DataTextField = "CourseName";
                ddlGradeCourse.DataValueField = "CourseID";
                ddlGradeCourse.DataBind();
                ddlGradeCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
            }
        }

        private void BindGradeStudents(int courseId)
        {
            string cs = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int fid = Convert.ToInt32(Session["UserID"]);
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT DISTINCT U.UserID, U.FullName
                    FROM Users U INNER JOIN CourseEnrollments CE ON U.UserID=CE.StudentID
                    WHERE CE.CourseID=@c AND CE.FacultyID=@f ORDER BY U.FullName", con);
                cmd.Parameters.AddWithValue("@c", courseId);
                cmd.Parameters.AddWithValue("@f", fid);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
                ddlGradeStudent.DataSource = dt;
                ddlGradeStudent.DataTextField = "FullName";
                ddlGradeStudent.DataValueField = "UserID";
                ddlGradeStudent.DataBind();
                ddlGradeStudent.Items.Insert(0, new ListItem("-- Select Student --", ""));
            }
        }

        protected void ddlGradeCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlGradeStudent.Items.Clear();
            if (!string.IsNullOrEmpty(ddlGradeCourse.SelectedValue))
                BindGradeStudents(Convert.ToInt32(ddlGradeCourse.SelectedValue));
        }

        protected void btnAddGrade_Click(object sender, EventArgs e)
        {
            litGradeMsg.Text = "";
            if (string.IsNullOrEmpty(ddlGradeCourse.SelectedValue) || string.IsNullOrEmpty(ddlGradeStudent.SelectedValue))
            { litGradeMsg.Text = "<div class='alert alert-warning mt-3'>Select a course and student.</div>"; return; }
            int score, maxScore;
            if (!int.TryParse(txtScore.Text.Trim(), out score) || !int.TryParse(txtMaxScore.Text.Trim(), out maxScore) || maxScore <= 0 || score < 0 || score > maxScore)
            { litGradeMsg.Text = "<div class='alert alert-warning mt-3'>Enter a valid score and max score (0 ≤ score ≤ max).</div>"; return; }

            string cs = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand(@"INSERT INTO Grades (StudentID, CourseID, Category, Title, Score, MaxScore, Remarks, GradedBy)
                        VALUES (@s,@c,@cat,@t,@sc,@mx,@r,@by)", con);
                    cmd.Parameters.AddWithValue("@s", Convert.ToInt32(ddlGradeStudent.SelectedValue));
                    cmd.Parameters.AddWithValue("@c", Convert.ToInt32(ddlGradeCourse.SelectedValue));
                    cmd.Parameters.AddWithValue("@cat", ddlCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@t", string.IsNullOrWhiteSpace(txtGTitle.Text) ? (object)DBNull.Value : txtGTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@sc", score);
                    cmd.Parameters.AddWithValue("@mx", maxScore);
                    cmd.Parameters.AddWithValue("@r", string.IsNullOrWhiteSpace(txtRemarks.Text) ? (object)DBNull.Value : txtRemarks.Text.Trim());
                    cmd.Parameters.AddWithValue("@by", Convert.ToInt32(Session["UserID"]));
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                litGradeMsg.Text = "<div class='alert alert-success mt-3'>Grade saved — the student can now see it on their portal.</div>";
                txtGTitle.Text = txtScore.Text = txtMaxScore.Text = txtRemarks.Text = "";
            }
            catch (Exception ex)
            {
                litGradeMsg.Text = "<div class='alert alert-danger mt-3'>Error saving grade: " + ex.Message + "</div>";
            }
        }

        private void BindAssignmentsDropdown()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int facultyId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT DISTINCT C.CourseID, C.CourseName
                        FROM Courses C INNER JOIN Assignments A ON C.CourseID = A.CourseID
                        WHERE A.UploadedBy = @FacultyID
                        ORDER BY C.CourseName", con);
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlAssignment.DataSource = dt;
                    ddlAssignment.DataTextField = "CourseName";
                    ddlAssignment.DataValueField = "CourseID";
                    ddlAssignment.DataBind();
                    ddlAssignment.Items.Insert(0, new ListItem("-- Select Subject --", ""));
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading assignments: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        protected void ddlAssignment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAssignment.SelectedValue != "")
            {
                BindSubmissionsGridView(Convert.ToInt32(ddlAssignment.SelectedValue));
            }
            else
            {
                gvSubmissions.DataSource = null;
                gvSubmissions.DataBind();
            }
        }

        protected void btnLoadSubmissions_Click(object sender, EventArgs e)
        {
            if (ddlAssignment.SelectedValue != "")
            {
                BindSubmissionsGridView(Convert.ToInt32(ddlAssignment.SelectedValue));
            }
            else
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please select an assignment first.</div>";
                gvSubmissions.DataSource = null;
                gvSubmissions.DataBind();
            }
        }

        private void BindSubmissionsGridView(int assignmentId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT S.SubmissionID, S.SubmissionDate, S.Grade, U.FullName AS StudentName, A.Title AS AssignmentTitle
                        FROM Submissions S
                        INNER JOIN Users U ON S.StudentID = U.UserID
                        INNER JOIN Assignments A ON S.AssignmentID = A.AssignmentID
                        WHERE A.CourseID = @CourseID AND A.UploadedBy = @FacultyID
                        ORDER BY A.Title, U.FullName", con);
                    cmd.Parameters.AddWithValue("@CourseID", assignmentId);
                    cmd.Parameters.AddWithValue("@FacultyID", Convert.ToInt32(Session["UserID"]));
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvSubmissions.DataSource = dt;
                    gvSubmissions.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        litMessage.Text = "<div class='alert alert-info mt-3'>No submissions found for this assignment.</div>";
                    }
                    else
                    {
                        litMessage.Text = string.Empty;
                    }
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading submissions: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        // Handles the Download button in the submissions grid: streams the stored file.
        protected void gvSubmissions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DownloadFile")
                return;

            int submissionId = Convert.ToInt32(e.CommandArgument);
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "SELECT FileName, FileContent, ContentType FROM Submissions WHERE SubmissionID = @SubmissionID", con);
                    cmd.Parameters.AddWithValue("@SubmissionID", submissionId);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && reader["FileContent"] != DBNull.Value)
                        {
                            byte[] fileContent = (byte[])reader["FileContent"];
                            string fileName = reader["FileName"].ToString();
                            string contentType = reader["ContentType"] == DBNull.Value
                                ? "application/octet-stream"
                                : reader["ContentType"].ToString();
                            if (string.IsNullOrEmpty(contentType))
                                contentType = "application/octet-stream";

                            Response.Clear();
                            Response.ContentType = contentType;
                            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                            Response.BinaryWrite(fileContent);
                            Response.Flush();
                            Response.End();
                        }
                        else
                        {
                            litMessage.Text = "<div class='alert alert-warning mt-3'>No file found for this submission.</div>";
                        }
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Raised by Response.End() after a successful download - safe to ignore.
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error downloading file: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        protected void gvSubmissions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvSubmissions.EditIndex = e.NewEditIndex;
            if (ddlAssignment.SelectedValue != "")
            {
                BindSubmissionsGridView(Convert.ToInt32(ddlAssignment.SelectedValue));
            }
        }

        protected void gvSubmissions_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int submissionId = Convert.ToInt32(gvSubmissions.DataKeys[e.RowIndex].Value);
            TextBox txtGrade = (TextBox)gvSubmissions.Rows[e.RowIndex].FindControl("txtGrade");
            int? grade = null;

            if (!string.IsNullOrEmpty(txtGrade.Text))
            {
                int parsedGrade;
                if (int.TryParse(txtGrade.Text, out parsedGrade))
                {
                    if (parsedGrade >= 0 && parsedGrade <= 100) // Assuming grades are out of 100
                    {
                        grade = parsedGrade;
                    }
                    else
                    {
                        litMessage.Text = "<div class='alert alert-warning mt-3'>Grade must be between 0 and 100.</div>";
                        return;
                    }
                }
                else
                {
                    litMessage.Text = "<div class='alert alert-warning mt-3'>Please enter a valid number for Grade.</div>";
                    return;
                }
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Submissions SET Grade = @Grade WHERE SubmissionID = @SubmissionID", con);
                    if (grade.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@Grade", grade.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Grade", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@SubmissionID", submissionId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>Grade updated successfully.</div>";
                    gvSubmissions.EditIndex = -1;
                    if (ddlAssignment.SelectedValue != "")
                    {
                        BindSubmissionsGridView(Convert.ToInt32(ddlAssignment.SelectedValue));
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

        protected void gvSubmissions_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSubmissions.EditIndex = -1;
            if (ddlAssignment.SelectedValue != "")
            {
                BindSubmissionsGridView(Convert.ToInt32(ddlAssignment.SelectedValue));
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