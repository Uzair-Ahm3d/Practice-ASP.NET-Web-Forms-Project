using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class facultyUploadAssignment : Page
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
                BindAssignmentsGridView();
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
                        SELECT C.CourseID, C.CourseName
                        FROM Courses C
                        INNER JOIN Assignments A ON C.CourseID = A.CourseID
                        WHERE A.UploadedBy = @FacultyID
                        UNION
                        SELECT CourseID, CourseName FROM Courses
                        ORDER BY CourseName", con); // Include all courses, faculty might add assignments to newly assigned courses
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlCourse.DataSource = dt;
                    ddlCourse.DataTextField = "CourseName";
                    ddlCourse.DataValueField = "CourseID";
                    ddlCourse.DataBind();
                    ddlCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
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

        private void BindAssignmentsGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int facultyId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT A.AssignmentID, A.Title, A.Description, A.DueDate, C.CourseName
                        FROM Assignments A
                        INNER JOIN Courses C ON A.CourseID = C.CourseID
                        WHERE A.UploadedBy = @FacultyID
                        ORDER BY A.DueDate DESC", con);
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAssignments.DataSource = dt;
                    gvAssignments.DataBind();
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

        protected void btnUploadAssignment_Click(object sender, EventArgs e)
        {
            if (ddlCourse.SelectedValue == "")
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please select a course.</div>";
                return;
            }

            int courseId = Convert.ToInt32(ddlCourse.SelectedValue);
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            DateTime dueDate;
            int facultyId = Convert.ToInt32(Session["UserID"]);

            if (!DateTime.TryParse(txtDueDate.Text, out dueDate))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please enter a valid due date.</div>";
                return;
            }

            if (string.IsNullOrEmpty(title))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Assignment title cannot be empty.</div>";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Assignments (CourseID, Title, Description, DueDate, UploadedBy) VALUES (@CourseID, @Title, @Description, @DueDate, @UploadedBy)", con);
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
                    cmd.Parameters.AddWithValue("@UploadedBy", facultyId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>Assignment uploaded successfully.</div>";
                    ClearFormFields();
                    BindAssignmentsGridView();
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

        protected void gvAssignments_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvAssignments.EditIndex = e.NewEditIndex;
            BindAssignmentsGridView();
        }

        protected void gvAssignments_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int assignmentId = Convert.ToInt32(gvAssignments.DataKeys[e.RowIndex].Value);
            string title = ((TextBox)gvAssignments.Rows[e.RowIndex].Cells[2].Controls[0]).Text.Trim();
            string description = ((TextBox)gvAssignments.Rows[e.RowIndex].Cells[3].Controls[0]).Text.Trim();
            DateTime dueDate;

            if (!DateTime.TryParse(((TextBox)gvAssignments.Rows[e.RowIndex].Cells[4].Controls[0]).Text, out dueDate))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please enter a valid due date.</div>";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Assignments SET Title = @Title, Description = @Description, DueDate = @DueDate WHERE AssignmentID = @AssignmentID", con);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>Assignment updated successfully.</div>";
                    gvAssignments.EditIndex = -1;
                    BindAssignmentsGridView();
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

        protected void gvAssignments_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvAssignments.EditIndex = -1;
            BindAssignmentsGridView();
        }

        protected void gvAssignments_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int assignmentId = Convert.ToInt32(gvAssignments.DataKeys[e.RowIndex].Value);
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    try
                    {
                        SqlCommand cmdDelSubmissions = new SqlCommand("DELETE FROM Submissions WHERE AssignmentID = @AssignmentID", con, transaction);
                        cmdDelSubmissions.Parameters.AddWithValue("@AssignmentID", assignmentId);
                        cmdDelSubmissions.ExecuteNonQuery();

                        SqlCommand cmdDelAssignment = new SqlCommand("DELETE FROM Assignments WHERE AssignmentID = @AssignmentID", con, transaction);
                        cmdDelAssignment.Parameters.AddWithValue("@AssignmentID", assignmentId);
                        cmdDelAssignment.ExecuteNonQuery();

                        transaction.Commit();
                        litMessage.Text = "<div class='alert alert-success mt-3'>Assignment and all submissions deleted successfully.</div>";
                        BindAssignmentsGridView();
                    }
                    catch (SqlException txEx)
                    {
                        transaction.Rollback();
                        litMessage.Text = "<div class='alert alert-danger mt-3'>Transaction error during deletion: " + txEx.Message + "</div>";
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

        private void ClearFormFields()
        {
            ddlCourse.SelectedIndex = 0;
            txtTitle.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtDueDate.Text = string.Empty;
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