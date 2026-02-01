using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartCampusPortal
{
    public partial class studentViewProfile : Page
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
                LoadStudentProfile();
                LoadAcademicRecord();
            }
        }

        private void LoadStudentProfile()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT U.UserID, U.FullName, U.Email, S.Department, S.EnrollmentDate
                        FROM Users U
                        INNER JOIN Students S ON U.UserID = S.StudentID
                        WHERE U.UserID = @StudentID", con);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblStudentID.Text = reader["UserID"].ToString();
                        lblFullName.Text = reader["FullName"].ToString();
                        lblEmail.Text = reader["Email"].ToString();
                        lblDepartment.Text = reader["Department"].ToString();
                        lblEnrollmentDate.Text = Convert.ToDateTime(reader["EnrollmentDate"]).ToShortDateString();
                    }
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading profile: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        private void LoadAcademicRecord()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]);
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT C.CourseName, C.Department, C.Credits, CR.RegistrationDate
                        FROM CourseRegistrations CR
                        INNER JOIN Courses C ON CR.CourseID = C.CourseID
                        WHERE CR.StudentID = @StudentID
                        ORDER BY CR.RegistrationDate DESC", con);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAcademicRecord.DataSource = dt;
                    gvAcademicRecord.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        litMessage.Text = "<div class='alert alert-info mt-3'>You are not currently registered for any courses.</div>";
                    }
                    else
                    {
                        litMessage.Text = string.Empty;
                    }
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading academic record: " + ex.Message + "</div>";
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