using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class adminManageUsers : Page
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
                BindUsersGridView();
            }
        }

        private void BindUsersGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT UserID, FullName, Email, Role FROM Users", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();
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

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string role = ddlRole.SelectedValue;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please fill all fields.</div>";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Users (FullName, Email, Password, Role) VALUES (@FullName, @Email, @Password, @Role); SELECT SCOPE_IDENTITY();", con);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", role);

                    int newUserId = Convert.ToInt32(cmd.ExecuteScalar());

                    if (role == "Student")
                    {
                        SqlCommand cmdStudent = new SqlCommand("INSERT INTO Students (StudentID, Department, EnrollmentDate) VALUES (@StudentID, 'Default', GETDATE())", con);
                        cmdStudent.Parameters.AddWithValue("@StudentID", newUserId);
                        cmdStudent.ExecuteNonQuery();
                    }
                    else if (role == "Faculty")
                    {
                        SqlCommand cmdFaculty = new SqlCommand("INSERT INTO Faculty (FacultyID, Department, HireDate) VALUES (@FacultyID, 'Default', GETDATE())", con);
                        cmdFaculty.Parameters.AddWithValue("@FacultyID", newUserId);
                        cmdFaculty.ExecuteNonQuery();
                    }

                    litMessage.Text = "<div class='alert alert-success mt-3'>User added successfully.</div>";
                    ClearFormFields();
                    BindUsersGridView();
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

        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUsers.EditIndex = e.NewEditIndex;
            BindUsersGridView();
        }

        protected void gvUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int userId = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);
            string fullName = ((TextBox)gvUsers.Rows[e.RowIndex].Cells[1].Controls[0]).Text.Trim();
            string email = ((TextBox)gvUsers.Rows[e.RowIndex].Cells[2].Controls[0]).Text.Trim();
            string role = ((TextBox)gvUsers.Rows[e.RowIndex].Cells[3].Controls[0]).Text.Trim();

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Users SET FullName = @FullName, Email = @Email, Role = @Role WHERE UserID = @UserID", con);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>User updated successfully.</div>";
                    gvUsers.EditIndex = -1;
                    BindUsersGridView();
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

        protected void gvUsers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUsers.EditIndex = -1;
            BindUsersGridView();
        }

        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userId = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand getUserRoleCmd = new SqlCommand("SELECT Role FROM Users WHERE UserID = @UserID", con);
                    getUserRoleCmd.Parameters.AddWithValue("@UserID", userId);
                    string userRole = getUserRoleCmd.ExecuteScalar()?.ToString();

                    SqlTransaction transaction = con.BeginTransaction();
                    try
                    {
                        if (userRole == "Student")
                        {
                            SqlCommand cmdDelStudentFees = new SqlCommand("DELETE FROM FeeRecords WHERE StudentID = @UserID", con, transaction);
                            cmdDelStudentFees.Parameters.AddWithValue("@UserID", userId);
                            cmdDelStudentFees.ExecuteNonQuery();

                            SqlCommand cmdDelStudentSubmissions = new SqlCommand("DELETE FROM Submissions WHERE StudentID = @UserID", con, transaction);
                            cmdDelStudentSubmissions.Parameters.AddWithValue("@UserID", userId);
                            cmdDelStudentSubmissions.ExecuteNonQuery();

                            SqlCommand cmdDelStudentCourseRegistrations = new SqlCommand("DELETE FROM CourseRegistrations WHERE StudentID = @UserID", con, transaction);
                            cmdDelStudentCourseRegistrations.Parameters.AddWithValue("@UserID", userId);
                            cmdDelStudentCourseRegistrations.ExecuteNonQuery();

                            SqlCommand cmdDelStudentAttendance = new SqlCommand("DELETE FROM Attendance WHERE StudentID = @UserID", con, transaction);
                            cmdDelStudentAttendance.Parameters.AddWithValue("@UserID", userId);
                            cmdDelStudentAttendance.ExecuteNonQuery();

                            SqlCommand cmdDelStudent = new SqlCommand("DELETE FROM Students WHERE StudentID = @UserID", con, transaction);
                            cmdDelStudent.Parameters.AddWithValue("@UserID", userId);
                            cmdDelStudent.ExecuteNonQuery();
                        }
                        else if (userRole == "Faculty")
                        {
                            SqlCommand cmdDelFacultyAssignments = new SqlCommand("UPDATE Assignments SET UploadedBy = NULL WHERE UploadedBy = @UserID", con, transaction);
                            cmdDelFacultyAssignments.Parameters.AddWithValue("@UserID", userId);
                            cmdDelFacultyAssignments.ExecuteNonQuery();

                            SqlCommand cmdDelFacultyAnnouncements = new SqlCommand("DELETE FROM Announcements WHERE FacultyID = @UserID", con, transaction);
                            cmdDelFacultyAnnouncements.Parameters.AddWithValue("@UserID", userId);
                            cmdDelFacultyAnnouncements.ExecuteNonQuery();

                            SqlCommand cmdDelFaculty = new SqlCommand("DELETE FROM Faculty WHERE FacultyID = @UserID", con, transaction);
                            cmdDelFaculty.Parameters.AddWithValue("@UserID", userId);
                            cmdDelFaculty.ExecuteNonQuery();
                        }

                        SqlCommand cmdDelUser = new SqlCommand("DELETE FROM Users WHERE UserID = @UserID", con, transaction);
                        cmdDelUser.Parameters.AddWithValue("@UserID", userId);
                        cmdDelUser.ExecuteNonQuery();

                        transaction.Commit();
                        litMessage.Text = "<div class='alert alert-success mt-3'>User and associated data deleted successfully.</div>";
                        BindUsersGridView();
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
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;
            ddlRole.SelectedIndex = 0;
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