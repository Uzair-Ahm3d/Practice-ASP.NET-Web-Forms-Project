//using System;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//namespace SmartCampusPortal
//{
//    public partial class facultyViewAttendance : Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Faculty")
//                {
//                    FormsAuthentication.SignOut();
//                    Response.Redirect("Login.aspx");
//                }
//                BindCoursesDropdown();
//                BindAttendanceGridView(0); // Initial load, no course selected
//            }
//        }

//        private void BindCoursesDropdown()
//        {
//            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
//            int facultyId = Convert.ToInt32(Session["UserID"]);
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    SqlCommand cmd = new SqlCommand(@"
//                        SELECT DISTINCT C.CourseID, C.CourseName
//                        FROM Courses C
//                        INNER JOIN Assignments A ON C.CourseID = A.CourseID
//                        WHERE A.UploadedBy = @FacultyID
//                        ORDER BY C.CourseName", con);
//                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
//                    SqlDataAdapter da = new SqlDataAdapter(cmd);
//                    DataTable dt = new DataTable();
//                    da.Fill(dt);
//                    ddlCourseFilter.DataSource = dt;
//                    ddlCourseFilter.DataTextField = "CourseName";
//                    ddlCourseFilter.DataValueField = "CourseID";
//                    ddlCourseFilter.DataBind();
//                    ddlCourseFilter.Items.Insert(0, new ListItem("-- Select Course --", ""));
//                }
//            }
//            catch (SqlException ex)
//            {
//                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading courses: " + ex.Message + "</div>";
//            }
//            catch (Exception ex)
//            {
//                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
//            }
//        }

//        protected void ddlCourseFilter_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (ddlCourseFilter.SelectedValue != "")
//            {
//                BindAttendanceGridView(Convert.ToInt32(ddlCourseFilter.SelectedValue));
//            }
//            else
//            {
//                BindAttendanceGridView(0); // Show all attendance if no course is selected
//            }
//        }

//        protected void btnFilterAttendance_Click(object sender, EventArgs e)
//        {
//            int courseId = 0;
//            if (!string.IsNullOrEmpty(ddlCourseFilter.SelectedValue))
//            {
//                courseId = Convert.ToInt32(ddlCourseFilter.SelectedValue);
//            }
//            BindAttendanceGridView(courseId);
//        }

//        private void BindAttendanceGridView(int courseId)
//        {
//            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
//            int facultyId = Convert.ToInt32(Session["UserID"]);
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    string query = @"
//                        SELECT A.Date, A.Status, U.FullName
//                        FROM Attendance A
//                        INNER JOIN Users U ON A.StudentID = U.UserID
//                        INNER JOIN Courses C ON A.CourseID = C.CourseID
//                        WHERE C.CourseID IN (SELECT DISTINCT CourseID FROM Assignments WHERE UploadedBy = @FacultyID)";

//                    SqlCommand cmd = new SqlCommand();
//                    cmd.Connection = con;
//                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);

//                    if (courseId > 0)
//                    {
//                        query += " AND A.CourseID = @CourseID";
//                        cmd.Parameters.AddWithValue("@CourseID", courseId);
//                    }
//                    if (!string.IsNullOrEmpty(txtDateFilter.Text))
//                    {
//                        query += " AND A.Date = @Date";
//                        cmd.Parameters.AddWithValue("@Date", Convert.ToDateTime(txtDateFilter.Text));
//                    }
//                    if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
//                    {
//                        query += " AND A.Status = @Status";
//                        cmd.Parameters.AddWithValue("@Status", ddlStatusFilter.SelectedValue);
//                    }

//                    query += " ORDER BY A.Date DESC, U.FullName";
//                    cmd.CommandText = query;

//                    SqlDataAdapter da = new SqlDataAdapter(cmd);
//                    DataTable dt = new DataTable();
//                    da.Fill(dt);
//                    gvAttendance.DataSource = dt;
//                    gvAttendance.DataBind();

//                    if (dt.Rows.Count == 0)
//                    {
//                        litMessage.Text = "<div class='alert alert-info mt-3'>No attendance records found based on your filters.</div>";
//                    }
//                    else
//                    {
//                        litMessage.Text = string.Empty;
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error: " + ex.Message + "</div>";
//            }
//            catch (Exception ex)
//            {
//                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
//            }
//        }

//        protected void lnkLogout_Click(object sender, EventArgs e)
//        {
//            FormsAuthentication.SignOut();
//            Session.Clear();
//            Session.Abandon();
//            Response.Redirect("Login.aspx");
//        }
//    }
//}

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class facultyViewAttendance : Page
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
                BindCoursesDropdowns(); // Bind both dropdowns
                BindAttendanceGridView(0); // Initial load of existing attendance
            }
        }

        // Binds both the filter and the mark attendance course dropdowns
        private void BindCoursesDropdowns()
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
                        INNER JOIN CourseEnrollments CE ON C.CourseID = CE.CourseID
                        INNER JOIN Users U ON CE.FacultyID = U.UserID
                        WHERE U.UserID = @FacultyID
                        ORDER BY C.CourseName", con);
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // For filtering existing attendance
                    ddlCourseFilter.DataSource = dt;
                    ddlCourseFilter.DataTextField = "CourseName";
                    ddlCourseFilter.DataValueField = "CourseID";
                    ddlCourseFilter.DataBind();
                    ddlCourseFilter.Items.Insert(0, new ListItem("-- Select Course --", ""));

                    // For marking new attendance
                    ddlMarkAttendanceCourse.DataSource = dt;
                    ddlMarkAttendanceCourse.DataTextField = "CourseName";
                    ddlMarkAttendanceCourse.DataValueField = "CourseID";
                    ddlMarkAttendanceCourse.DataBind();
                    ddlMarkAttendanceCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
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

        // Handles selection change for existing attendance filter dropdown
        protected void ddlCourseFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            int courseId = 0;
            if (!string.IsNullOrEmpty(ddlCourseFilter.SelectedValue))
            {
                courseId = Convert.ToInt32(ddlCourseFilter.SelectedValue);
            }
            BindAttendanceGridView(courseId);
        }

        // Handles filter button click for existing attendance
        protected void btnFilterAttendance_Click(object sender, EventArgs e)
        {
            int courseId = 0;
            if (!string.IsNullOrEmpty(ddlCourseFilter.SelectedValue))
            {
                courseId = Convert.ToInt32(ddlCourseFilter.SelectedValue);
            }
            BindAttendanceGridView(courseId);
        }

        // Binds the GridView for existing attendance records
        private void BindAttendanceGridView(int courseId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int facultyId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT Att.Date, Att.Status, U.FullName, C.CourseName
                        FROM Attendance Att
                        INNER JOIN Users U ON Att.StudentID = U.UserID
                        INNER JOIN Courses C ON Att.CourseID = C.CourseID
                        INNER JOIN CourseEnrollments CE ON C.CourseID = CE.CourseID AND CE.StudentID = U.UserID
                        WHERE CE.FacultyID = @FacultyID";

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId);

                    if (courseId > 0)
                    {
                        query += " AND Att.CourseID = @CourseID";
                        cmd.Parameters.AddWithValue("@CourseID", courseId);
                    }
                    if (!string.IsNullOrEmpty(txtDateFilter.Text))
                    {
                        query += " AND Att.Date = @Date";
                        cmd.Parameters.AddWithValue("@Date", Convert.ToDateTime(txtDateFilter.Text));
                    }
                    if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
                    {
                        query += " AND Att.Status = @Status";
                        cmd.Parameters.AddWithValue("@Status", ddlStatusFilter.SelectedValue);
                    }

                    query += " ORDER BY Att.Date DESC, U.FullName";
                    cmd.CommandText = query;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAttendance.DataSource = dt;
                    gvAttendance.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        litMessage.Text = "<div class='alert alert-info mt-3'>No attendance records found based on your filters.</div>";
                    }
                    else
                    {
                        litMessage.Text = string.Empty;
                    }
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error retrieving attendance: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        // --- New Attendance Marking Functionality ---

        // Handles selection change for marking attendance course dropdown
        protected void ddlMarkAttendanceCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlMarkAttendance.Visible = false; // Hide panel until both course and date are selected
            if (!string.IsNullOrEmpty(ddlMarkAttendanceCourse.SelectedValue) && !string.IsNullOrEmpty(txtMarkAttendanceDate.Text))
            {
                BindStudentsForAttendance(Convert.ToInt32(ddlMarkAttendanceCourse.SelectedValue), Convert.ToDateTime(txtMarkAttendanceDate.Text));
                pnlMarkAttendance.Visible = true;
            }
            else if (string.IsNullOrEmpty(ddlMarkAttendanceCourse.SelectedValue))
            {
                litMessage.Text = "<div class='alert alert-info mt-3'>Please select a course to mark attendance.</div>";
            }
            else if (string.IsNullOrEmpty(txtMarkAttendanceDate.Text))
            {
                litMessage.Text = "<div class='alert alert-info mt-3'>Please select a date to mark attendance.</div>";
            }
        }

        // Handles text change for marking attendance date textbox
        protected void txtMarkAttendanceDate_TextChanged(object sender, EventArgs e)
        {
            pnlMarkAttendance.Visible = false; // Hide panel until both course and date are selected
            if (!string.IsNullOrEmpty(ddlMarkAttendanceCourse.SelectedValue) && !string.IsNullOrEmpty(txtMarkAttendanceDate.Text))
            {
                BindStudentsForAttendance(Convert.ToInt32(ddlMarkAttendanceCourse.SelectedValue), Convert.ToDateTime(txtMarkAttendanceDate.Text));
                pnlMarkAttendance.Visible = true;
            }
            else if (string.IsNullOrEmpty(ddlMarkAttendanceCourse.SelectedValue))
            {
                litMessage.Text = "<div class='alert alert-info mt-3'>Please select a course to mark attendance.</div>";
            }
            else if (string.IsNullOrEmpty(txtMarkAttendanceDate.Text))
            {
                litMessage.Text = "<div class='alert alert-info mt-3'>Please select a date to mark attendance.</div>";
            }
        }


        // Binds the GridView for marking attendance with students of the selected course
        private void BindStudentsForAttendance(int courseId, DateTime attendanceDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Select students enrolled in the course, and left join with Attendance table
                    // to retrieve existing attendance for the selected date if it exists.
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT
                            U.UserID AS StudentID,
                            U.FullName,
                            A.Status
                        FROM Users U
                        INNER JOIN CourseEnrollments CE ON U.UserID = CE.StudentID
                        WHERE CE.CourseID = @CourseID AND CE.FacultyID = @FacultyID
                        ORDER BY U.FullName", con); // Ensure only students from THIS faculty's course are shown
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    cmd.Parameters.AddWithValue("@FacultyID", Convert.ToInt32(Session["UserID"]));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtStudents = new DataTable();
                    da.Fill(dtStudents);

                    // Add a column for existing attendance status
                    dtStudents.Columns.Add("CurrentStatus", typeof(string));

                    // Fetch existing attendance for the current date and course
                    SqlCommand cmdExistingAttendance = new SqlCommand(@"
                        SELECT StudentID, Status
                        FROM Attendance
                        WHERE CourseID = @CourseID AND Date = @Date", con);
                    cmdExistingAttendance.Parameters.AddWithValue("@CourseID", courseId);
                    cmdExistingAttendance.Parameters.AddWithValue("@Date", attendanceDate);

                    con.Open();
                    SqlDataReader reader = cmdExistingAttendance.ExecuteReader();
                    System.Collections.Generic.Dictionary<int, string> existingAttendance = new System.Collections.Generic.Dictionary<int, string>();
                    while (reader.Read())
                    {
                        existingAttendance.Add(Convert.ToInt32(reader["StudentID"]), reader["Status"].ToString());
                    }
                    reader.Close();
                    con.Close();

                    // Populate CurrentStatus and set dropdown selected value
                    foreach (DataRow row in dtStudents.Rows)
                    {
                        int studentId = Convert.ToInt32(row["StudentID"]);
                        if (existingAttendance.ContainsKey(studentId))
                        {
                            row["CurrentStatus"] = existingAttendance[studentId];
                        }
                    }

                    gvMarkAttendance.DataSource = dtStudents;
                    gvMarkAttendance.DataBind();

                    if (dtStudents.Rows.Count == 0)
                    {
                        litMessage.Text = "<div class='alert alert-info mt-3'>No students found for this course.</div>";
                        pnlMarkAttendance.Visible = false;
                    }
                    else
                    {
                        litMessage.Text = string.Empty;
                        // Pre-select dropdown values based on existing attendance
                        foreach (GridViewRow row in gvMarkAttendance.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                DropDownList ddl = (DropDownList)row.FindControl("ddlAttendanceStatus");
                                if (ddl != null)
                                {
                                    DataRowView drv = (DataRowView)row.DataItem;
                                    string currentStatus = drv["CurrentStatus"].ToString();
                                    if (!string.IsNullOrEmpty(currentStatus))
                                    {
                                        ddl.SelectedValue = currentStatus;
                                    }
                                    else
                                    {
                                        // Default to Present if no existing record
                                        ddl.SelectedValue = "Present";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading students for attendance: " + ex.Message + "</div>";
                pnlMarkAttendance.Visible = false;
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
                pnlMarkAttendance.Visible = false;
            }
        }

        // Saves the marked attendance to the database
        protected void btnSaveAttendance_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlMarkAttendanceCourse.SelectedValue) || string.IsNullOrEmpty(txtMarkAttendanceDate.Text))
            {
                litMessage.Text = "<div class='alert alert-warning mt-3'>Please select a course and date before saving attendance.</div>";
                return;
            }

            int courseId = Convert.ToInt32(ddlMarkAttendanceCourse.SelectedValue);
            DateTime attendanceDate = Convert.ToDateTime(txtMarkAttendanceDate.Text);
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int recordsAffected = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    foreach (GridViewRow row in gvMarkAttendance.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            int studentId = Convert.ToInt32(gvMarkAttendance.DataKeys[row.RowIndex].Value);
                            DropDownList ddlStatus = (DropDownList)row.FindControl("ddlAttendanceStatus");
                            string status = ddlStatus.SelectedValue;

                            // Check if attendance record already exists for this student, course, and date
                            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Attendance WHERE StudentID = @StudentID AND CourseID = @CourseID AND Date = @Date", con);
                            checkCmd.Parameters.AddWithValue("@StudentID", studentId);
                            checkCmd.Parameters.AddWithValue("@CourseID", courseId);
                            checkCmd.Parameters.AddWithValue("@Date", attendanceDate);
                            int existingCount = (int)checkCmd.ExecuteScalar();

                            if (existingCount > 0)
                            {
                                // Update existing record
                                SqlCommand updateCmd = new SqlCommand("UPDATE Attendance SET Status = @Status WHERE StudentID = @StudentID AND CourseID = @CourseID AND Date = @Date", con);
                                updateCmd.Parameters.AddWithValue("@Status", status);
                                updateCmd.Parameters.AddWithValue("@StudentID", studentId);
                                updateCmd.Parameters.AddWithValue("@CourseID", courseId);
                                updateCmd.Parameters.AddWithValue("@Date", attendanceDate);
                                recordsAffected += updateCmd.ExecuteNonQuery();
                            }
                            else
                            {
                                // Insert new record
                                SqlCommand insertCmd = new SqlCommand("INSERT INTO Attendance (StudentID, CourseID, Date, Status) VALUES (@StudentID, @CourseID, @Date, @Status)", con);
                                insertCmd.Parameters.AddWithValue("@StudentID", studentId);
                                insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                                insertCmd.Parameters.AddWithValue("@Date", attendanceDate);
                                insertCmd.Parameters.AddWithValue("@Status", status);
                                recordsAffected += insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    con.Close();
                    litMessage.Text = $"<div class='alert alert-success mt-3'>Attendance saved successfully! {recordsAffected} records updated/inserted.</div>";
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error saving attendance: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred while saving attendance: " + ex.Message + "</div>";
            }
            finally
            {
                // Rebind the existing attendance grid to show the latest changes
                BindAttendanceGridView(courseId);
                // Reset the attendance marking panel
                pnlMarkAttendance.Visible = false;
                txtMarkAttendanceDate.Text = string.Empty;
                ddlMarkAttendanceCourse.SelectedIndex = 0;
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