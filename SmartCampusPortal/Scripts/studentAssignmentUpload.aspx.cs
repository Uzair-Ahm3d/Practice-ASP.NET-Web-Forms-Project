using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO; // Required for file operations

namespace SmartCampusPortal // Ensure this namespace matches your project's root namespace
{
    // IMPORTANT: The class name here MUST match the Inherits attribute in studentAssignmentUpload.aspx
    public partial class studentAssignmentUpload : Page
    {
        // Define allowed file extensions and max file size
        private static readonly string[] AllowedFileExtensions = { ".pdf", ".doc", ".docx", ".txt", ".zip", ".rar" };
        private const int MaxFileSizeMB = 5; // Max file size in MB

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Student")
            {
                FormsAuthentication.SignOut();
                Response.Redirect(ResolveUrl("~/Scripts/Login.aspx")); // Redirect to login page
                return; // Stop further execution of Page_Load if redirect occurs
            }

            if (!IsPostBack)
            {
                
                BindCoursesDropdown();
                
                ddlCourse_SelectedIndexChanged(null, null);
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

                    ddlCourse.DataSource = dt;
                    ddlCourse.DataTextField = "CourseName";
                    ddlCourse.DataValueField = "CourseID";
                    ddlCourse.DataBind();

                    // Add a default "Select Course" item
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

        protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear previous assignments
            ddlAssignment.Items.Clear();
            ddlAssignment.Items.Insert(0, new ListItem("-- Select Assignment --", ""));

            if (ddlCourse.SelectedValue != "")
            {
                int courseId = Convert.ToInt32(ddlCourse.SelectedValue);
                BindAssignmentsDropdown(courseId);
            }
        }

        private void BindAssignmentsDropdown(int courseId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT AssignmentID, Title
                        FROM Assignments
                        WHERE CourseID = @CourseID
                        ORDER BY DueDate DESC", con); // Order by due date to show upcoming/recent first
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlAssignment.DataSource = dt;
                    ddlAssignment.DataTextField = "Title";
                    ddlAssignment.DataValueField = "AssignmentID";
                    ddlAssignment.DataBind();

                    // Add a default "Select Assignment" item
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

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            litMessage.Text = string.Empty; // Clear previous messages

            // --- 1. Validate Selections ---
            if (ddlCourse.SelectedValue == "")
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please select a course.</div>";
                return;
            }
            if (ddlAssignment.SelectedValue == "")
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please select an assignment.</div>";
                return;
            }

            // --- 2. Validate File Upload ---
            if (!fileUploadAssignment.HasFile)
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please select a file to upload.</div>";
                return;
            }

            // Check file size
            if (fileUploadAssignment.PostedFile.ContentLength > MaxFileSizeMB * 1024 * 1024)
            {
                litMessage.Text = $"<div class='alert alert-warning mt-3'>File size exceeds the maximum limit of {MaxFileSizeMB}MB.</div>";
                return;
            }

            // Check file extension
            string fileExtension = Path.GetExtension(fileUploadAssignment.FileName).ToLower();
            bool isValidExtension = false;
            foreach (string ext in AllowedFileExtensions)
            {
                if (fileExtension == ext)
                {
                    isValidExtension = true;
                    break;
                }
            }

            if (!isValidExtension)
            {
                litMessage.Text = $"<div class='alert alert-warning mt-3'>Invalid file type. Allowed types are: {string.Join(", ", AllowedFileExtensions)}.</div>";
                return;
            }

            // --- 3. Prepare Data for Database ---
            int studentId = Convert.ToInt32(Session["UserID"]);
            int assignmentId = Convert.ToInt32(ddlAssignment.SelectedValue);
            string fileName = fileUploadAssignment.FileName;
            string contentType = fileUploadAssignment.PostedFile.ContentType;
            byte[] fileContent = fileUploadAssignment.FileBytes;
            string comment = txtComment.Text.Trim();
            DateTime submissionDate = DateTime.Now;

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                   
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Submissions (StudentID, AssignmentID, FileName, FileContent, ContentType, Comment, SubmissionDate)
                        VALUES (@StudentID, @AssignmentID, @FileName, @FileContent, @ContentType, @Comment, @SubmissionDate)", con);

                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    cmd.Parameters.AddWithValue("@FileContent", fileContent);
                    cmd.Parameters.AddWithValue("@ContentType", contentType);
                    cmd.Parameters.AddWithValue("@Comment", string.IsNullOrEmpty(comment) ? (object)DBNull.Value : comment);
                    cmd.Parameters.AddWithValue("@SubmissionDate", submissionDate);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        litMessage.Text = "<div class='alert alert-success mt-3'>Assignment uploaded successfully!</div>";
                        // Optionally, clear the form or reset dropdowns
                        ddlCourse.SelectedIndex = 0;
                        ddlAssignment.Items.Clear();
                        ddlAssignment.Items.Insert(0, new ListItem("-- Select Assignment --", ""));
                        txtComment.Text = string.Empty;
                    }
                    else
                    {
                        litMessage.Text = "<div class='alert alert-danger mt-3'>Failed to upload assignment. Please try again.</div>";
                    }
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error during upload: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred during upload: " + ex.Message + "</div>";
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Redirect(ResolveUrl("~/Scripts/Login.aspx"));
        }
    }
}



