using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class adminManageCourses : Page
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
                BindPrerequisiteDropdown();
                BindCoursesGridView();
            }
        }

        private void BindPrerequisiteDropdown()
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

                    ddlPrerequisite.DataSource = dt;
                    ddlPrerequisite.DataTextField = "CourseName";
                    ddlPrerequisite.DataValueField = "CourseID";
                    ddlPrerequisite.DataBind();
                    ddlPrerequisite.Items.Insert(0, new ListItem("-- None --", ""));
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading prerequisites: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        private void BindCoursesGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT C.CourseID, C.CourseName, C.Department, C.Credits, C.PrerequisiteCourseID, P.CourseName AS PrerequisiteCourseName
                        FROM Courses C
                        LEFT JOIN Courses P ON C.PrerequisiteCourseID = P.CourseID
                        ORDER BY C.CourseName", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvCourses.DataSource = dt;
                    gvCourses.DataBind();
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

        protected void btnAddCourse_Click(object sender, EventArgs e)
        {
            string courseName = txtCourseName.Text.Trim();
            string department = txtDepartment.Text.Trim();
            int credits;
            int? prerequisiteCourseID = null;

            if (!int.TryParse(txtCredits.Text, out credits))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please enter a valid number for Credits.</div>";
                return;
            }

            if (!string.IsNullOrEmpty(ddlPrerequisite.SelectedValue))
            {
                prerequisiteCourseID = Convert.ToInt32(ddlPrerequisite.SelectedValue);
            }

            if (string.IsNullOrEmpty(courseName) || string.IsNullOrEmpty(department))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please fill all required fields.</div>";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Courses (CourseName, Department, Credits, PrerequisiteCourseID) VALUES (@CourseName, @Department, @Credits, @PrerequisiteCourseID)", con);
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    cmd.Parameters.AddWithValue("@Department", department);
                    cmd.Parameters.AddWithValue("@Credits", credits);
                    if (prerequisiteCourseID.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@PrerequisiteCourseID", prerequisiteCourseID.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PrerequisiteCourseID", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>Course added successfully.</div>";
                    ClearFormFields();
                    BindPrerequisiteDropdown(); // Rebind dropdown to include new course
                    BindCoursesGridView();
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

        protected void gvCourses_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCourses.EditIndex = e.NewEditIndex;
            BindCoursesGridView();

            DropDownList ddlEditPrerequisite = (DropDownList)gvCourses.Rows[e.NewEditIndex].FindControl("ddlEditPrerequisite");
            if (ddlEditPrerequisite != null)
            {
                BindPrerequisiteDropdownForEdit(ddlEditPrerequisite);

                int currentPrerequisiteId = 0;
                if (gvCourses.DataKeys[e.NewEditIndex].Values["PrerequisiteCourseID"] != DBNull.Value)
                {
                    currentPrerequisiteId = Convert.ToInt32(gvCourses.DataKeys[e.NewEditIndex].Values["PrerequisiteCourseID"]);
                }

                ListItem item = ddlEditPrerequisite.Items.FindByValue(currentPrerequisiteId.ToString());
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        private void BindPrerequisiteDropdownForEdit(DropDownList ddl)
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

                    ddl.DataSource = dt;
                    ddl.DataTextField = "CourseName";
                    ddl.DataValueField = "CourseID";
                    ddl.DataBind();
                    ddl.Items.Insert(0, new ListItem("-- None --", ""));
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading prerequisites for edit: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        protected void gvCourses_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int courseID = Convert.ToInt32(gvCourses.DataKeys[e.RowIndex].Value);
            string courseName = ((TextBox)gvCourses.Rows[e.RowIndex].Cells[1].Controls[0]).Text.Trim();
            string department = ((TextBox)gvCourses.Rows[e.RowIndex].Cells[2].Controls[0]).Text.Trim();
            int credits;
            if (!int.TryParse(((TextBox)gvCourses.Rows[e.RowIndex].Cells[3].Controls[0]).Text, out credits))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please enter a valid number for Credits.</div>";
                return;
            }

            DropDownList ddlEditPrerequisite = (DropDownList)gvCourses.Rows[e.RowIndex].FindControl("ddlEditPrerequisite");
            int? prerequisiteCourseID = null;
            if (ddlEditPrerequisite != null && !string.IsNullOrEmpty(ddlEditPrerequisite.SelectedValue))
            {
                prerequisiteCourseID = Convert.ToInt32(ddlEditPrerequisite.SelectedValue);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Courses SET CourseName = @CourseName, Department = @Department, Credits = @Credits, PrerequisiteCourseID = @PrerequisiteCourseID WHERE CourseID = @CourseID", con);
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    cmd.Parameters.AddWithValue("@Department", department);
                    cmd.Parameters.AddWithValue("@Credits", credits);
                    if (prerequisiteCourseID.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@PrerequisiteCourseID", prerequisiteCourseID.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PrerequisiteCourseID", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@CourseID", courseID);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>Course updated successfully.</div>";
                    gvCourses.EditIndex = -1;
                    BindCoursesGridView();
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

        protected void gvCourses_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCourses.EditIndex = -1;
            BindCoursesGridView();
        }

        protected void gvCourses_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int courseID = Convert.ToInt32(gvCourses.DataKeys[e.RowIndex].Value);
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    try
                    {
                        SqlCommand cmdDelAnnouncements = new SqlCommand("DELETE FROM Announcements WHERE CourseID = @CourseID", con, transaction);
                        cmdDelAnnouncements.Parameters.AddWithValue("@CourseID", courseID);
                        cmdDelAnnouncements.ExecuteNonQuery();

                        SqlCommand cmdDelAttendance = new SqlCommand("DELETE FROM Attendance WHERE CourseID = @CourseID", con, transaction);
                        cmdDelAttendance.Parameters.AddWithValue("@CourseID", courseID);
                        cmdDelAttendance.ExecuteNonQuery();

                        SqlCommand cmdDelSubmissions = new SqlCommand("DELETE FROM Submissions WHERE AssignmentID IN (SELECT AssignmentID FROM Assignments WHERE CourseID = @CourseID)", con, transaction);
                        cmdDelSubmissions.Parameters.AddWithValue("@CourseID", courseID);
                        cmdDelSubmissions.ExecuteNonQuery();

                        SqlCommand cmdDelAssignments = new SqlCommand("DELETE FROM Assignments WHERE CourseID = @CourseID", con, transaction);
                        cmdDelAssignments.Parameters.AddWithValue("@CourseID", courseID);
                        cmdDelAssignments.ExecuteNonQuery();

                        SqlCommand cmdDelRegistrations = new SqlCommand("DELETE FROM CourseRegistrations WHERE CourseID = @CourseID", con, transaction);
                        cmdDelRegistrations.Parameters.AddWithValue("@CourseID", courseID);
                        cmdDelRegistrations.ExecuteNonQuery();

                        SqlCommand cmdDelCourse = new SqlCommand("DELETE FROM Courses WHERE CourseID = @CourseID", con, transaction);
                        cmdDelCourse.Parameters.AddWithValue("@CourseID", courseID);
                        cmdDelCourse.ExecuteNonQuery();

                        transaction.Commit();
                        litMessage.Text = "<div class='alert alert-success mt-3'>Course and all related data deleted successfully.</div>";
                        BindPrerequisiteDropdown(); // Rebind dropdown as a course might have been deleted
                        BindCoursesGridView();
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
            txtCourseName.Text = string.Empty;
            txtDepartment.Text = string.Empty;
            txtCredits.Text = string.Empty;
            ddlPrerequisite.SelectedIndex = 0;
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