using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class studentCourseRegistration : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Student")
                {
                    FormsAuthentication.SignOut();
                    Response.Redirect("Login.aspx");
                }
                BindAvailableCoursesGridView();
                BindRegisteredCoursesGridView();
            }
        }

        private void BindAvailableCoursesGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT C.CourseID, C.CourseName, C.Department, C.Credits, P.CourseName AS PrerequisiteCourseName
                        FROM Courses C
                        LEFT JOIN Courses P ON C.PrerequisiteCourseID = P.CourseID
                        WHERE C.CourseID NOT IN (SELECT CourseID FROM CourseRegistrations WHERE StudentID = @StudentID)
                        ORDER BY C.CourseName", con);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAvailableCourses.DataSource = dt;
                    gvAvailableCourses.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        litMessage.Text = "<div class='alert alert-info mt-3'>No new courses available for registration.</div>";
                    }
                    else
                    {
                        litMessage.Text = string.Empty;
                    }
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading available courses: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        private void BindRegisteredCoursesGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT CR.RegistrationID, C.CourseName, C.Department, C.Credits, CR.RegistrationDate
                        FROM CourseRegistrations CR
                        INNER JOIN Courses C ON CR.CourseID = C.CourseID
                        WHERE CR.StudentID = @StudentID
                        ORDER BY CR.RegistrationDate DESC", con);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvRegisteredCourses.DataSource = dt;
                    gvRegisteredCourses.DataBind();
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading registered courses: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        protected void gvAvailableCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RegisterCourse")
            {
                int courseId = Convert.ToInt32(e.CommandArgument);
                int studentId = Convert.ToInt32(Session["UserID"]);
                string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        SqlCommand checkPrerequisiteCmd = new SqlCommand("SELECT PrerequisiteCourseID FROM Courses WHERE CourseID = @CourseID", con);
                        checkPrerequisiteCmd.Parameters.AddWithValue("@CourseID", courseId);
                        object prerequisiteObj = checkPrerequisiteCmd.ExecuteScalar();

                        if (prerequisiteObj != DBNull.Value && prerequisiteObj != null)
                        {
                            int prerequisiteCourseID = Convert.ToInt32(prerequisiteObj);
                            SqlCommand checkCompletionCmd = new SqlCommand(@"
                                SELECT COUNT(*) FROM Submissions S
                                INNER JOIN Assignments A ON S.AssignmentID = A.AssignmentID
                                WHERE S.StudentID = @StudentID AND A.CourseID = @PrerequisiteCourseID AND S.Grade IS NOT NULL AND S.Grade >= 50", con); // Assuming 50% as passing grade
                            checkCompletionCmd.Parameters.AddWithValue("@StudentID", studentId);
                            checkCompletionCmd.Parameters.AddWithValue("@PrerequisiteCourseID", prerequisiteCourseID);

                            int completedPrerequisiteCount = Convert.ToInt32(checkCompletionCmd.ExecuteScalar());

                            if (completedPrerequisiteCount == 0)
                            {
                                SqlCommand getPrereqNameCmd = new SqlCommand("SELECT CourseName FROM Courses WHERE CourseID = @PrereqID", con);
                                getPrereqNameCmd.Parameters.AddWithValue("@PrereqID", prerequisiteCourseID);
                                string prereqName = getPrereqNameCmd.ExecuteScalar().ToString();
                                litMessage.Text = $"<div class='alert alert-danger mt-3'>Cannot register for this course. You must first successfully complete the prerequisite course: {prereqName}.</div>";
                                return;
                            }
                        }

                        SqlCommand checkRegistrationCmd = new SqlCommand("SELECT COUNT(*) FROM CourseRegistrations WHERE StudentID = @StudentID AND CourseID = @CourseID", con);
                        checkRegistrationCmd.Parameters.AddWithValue("@StudentID", studentId);
                        checkRegistrationCmd.Parameters.AddWithValue("@CourseID", courseId);
                        int existingRegistrations = Convert.ToInt32(checkRegistrationCmd.ExecuteScalar());

                        if (existingRegistrations > 0)
                        {
                            litMessage.Text = "<div class='alert alert-warning mt-3'>You are already registered for this course.</div>";
                            return;
                        }

                        SqlCommand registerCmd = new SqlCommand("INSERT INTO CourseRegistrations (StudentID, CourseID, RegistrationDate) VALUES (@StudentID, @CourseID, GETDATE())", con);
                        registerCmd.Parameters.AddWithValue("@StudentID", studentId);
                        registerCmd.Parameters.AddWithValue("@CourseID", courseId);
                        registerCmd.ExecuteNonQuery();

                        litMessage.Text = "<div class='alert alert-success mt-3'>Course registered successfully!</div>";
                        BindAvailableCoursesGridView();
                        BindRegisteredCoursesGridView();
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
        }

        protected void gvRegisteredCourses_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int registrationID = Convert.ToInt32(gvRegisteredCourses.DataKeys[e.RowIndex].Value);
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM CourseRegistrations WHERE RegistrationID = @RegistrationID", con);
                    cmd.Parameters.AddWithValue("@RegistrationID", registrationID);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    litMessage.Text = "<div class='alert alert-success mt-3'>Course dropped successfully.</div>";
                    BindAvailableCoursesGridView();
                    BindRegisteredCoursesGridView();
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