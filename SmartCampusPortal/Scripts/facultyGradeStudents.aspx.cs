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
                        SELECT AssignmentID, Title, CourseID
                        FROM Assignments
                        WHERE UploadedBy = @FacultyID
                        ORDER BY DueDate DESC", con);
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlAssignment.DataSource = dt;
                    ddlAssignment.DataTextField = "Title";
                    ddlAssignment.DataValueField = "AssignmentID";
                    ddlAssignment.DataBind();
                    ddlAssignment.Items.Insert(0, new ListItem("-- Select Assignment --", ""));
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
                        SELECT S.SubmissionID, S.SubmissionDate, S.Grade, U.FullName AS StudentName
                        FROM Submissions S
                        INNER JOIN Users U ON S.StudentID = U.UserID
                        WHERE S.AssignmentID = @AssignmentID
                        ORDER BY U.FullName", con);
                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
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