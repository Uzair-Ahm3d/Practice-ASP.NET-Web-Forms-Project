//using System;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//namespace SmartCampusPortal.Scripts
//{
//    public partial class studentFeeRecord : Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Student")
//                {
//                    FormsAuthentication.SignOut();
//                    Response.Redirect("~/Login.aspx");
//                }
//                LoadFeeSummary();
//                BindFeeRecordsGridView();
//            }
//        }

//        private void LoadFeeSummary()
//        {
//            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
//            int studentId = Convert.ToInt32(Session["UserID"]);
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    SqlCommand cmd = new SqlCommand(@"
//                        SELECT
//                            ISNULL(SUM(TotalAmount), 0) AS TotalAmountDue,
//                            ISNULL(SUM(PaidAmount), 0) AS TotalPaid
//                        FROM FeeRecords
//                        WHERE StudentID = @StudentID", con);
//                    cmd.Parameters.AddWithValue("@StudentID", studentId);
//                    con.Open();
//                    SqlDataReader reader = cmd.ExecuteReader();

//                    if (reader.Read())
//                    {
//                        lblTotalAmountDue.Text = Convert.ToDecimal(reader["TotalAmountDue"]).ToString("N2");
//                        lblTotalPaid.Text = Convert.ToDecimal(reader["TotalPaid"]).ToString("N2");
//                    }
//                    reader.Close();
//                }
//            }
//            catch (SqlException ex)
//            {
//                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading fee summary: " + ex.Message + "</div>";
//            }
//            catch (Exception ex)
//            {
//                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
//            }
//        }

//        private void BindFeeRecordsGridView()
//        {
//            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
//            int studentId = Convert.ToInt32(Session["UserID"]);
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    SqlCommand cmd = new SqlCommand(@"
//                        SELECT FeeID, TotalAmount, PaidAmount, PaymentStatus
//                        FROM FeeRecords
//                        WHERE StudentID = @StudentID
//                        ORDER BY FeeID DESC", con);
//                    cmd.Parameters.AddWithValue("@StudentID", studentId);
//                    SqlDataAdapter da = new SqlDataAdapter(cmd);
//                    DataTable dt = new DataTable();
//                    da.Fill(dt);
//                    gvFeeRecords.DataSource = dt;
//                    gvFeeRecords.DataBind();

//                    if (dt.Rows.Count == 0)
//                    {
//                        litMessage.Text = "<div class='alert alert-info mt-3'>No detailed fee records found.</div>";
//                    }
//                    else
//                    {
//                        litMessage.Text = string.Empty;
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading detailed fee records: " + ex.Message + "</div>";
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
//            Response.Redirect("~/Login.aspx");
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

namespace SmartCampusPortal.Scripts
{
    public partial class studentFeeRecord : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Student")
            {
                FormsAuthentication.SignOut();
                Response.Redirect(ResolveUrl("~/Scripts/Login.aspx")); 
                return;
            }

            if (!IsPostBack)
            {
                LoadFeeSummary();
                BindFeeRecordsGridView();
            }
        }

        private void LoadFeeSummary()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]); 
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT
                            ISNULL(SUM(TotalAmount), 0) AS TotalAmountDue,
                            ISNULL(SUM(PaidAmount), 0) AS TotalPaid
                        FROM FeeRecords
                        WHERE StudentID = @StudentID", con);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblTotalAmountDue.Text = Convert.ToDecimal(reader["TotalAmountDue"]).ToString("N2");
                        lblTotalPaid.Text = Convert.ToDecimal(reader["TotalPaid"]).ToString("N2");
                    }
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading fee summary: " + ex.Message + "</div>";
            }
            catch (Exception ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>An unexpected error occurred: " + ex.Message + "</div>";
            }
        }

        private void BindFeeRecordsGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            int studentId = Convert.ToInt32(Session["UserID"]); 
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT FeeID, TotalAmount, PaidAmount, PaymentStatus
                        FROM FeeRecords
                        WHERE StudentID = @StudentID
                        ORDER BY FeeID DESC", con);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvFeeRecords.DataSource = dt;
                    gvFeeRecords.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        litMessage.Text = "<div class='alert alert-info mt-3'>No detailed fee records found.</div>";
                    }
                    else
                    {
                        litMessage.Text = string.Empty;
                    }
                }
            }
            catch (SqlException ex)
            {
                litMessage.Text = "<div class='alert alert-danger mt-3'>Database error loading detailed fee records: " + ex.Message + "</div>";
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
            Response.Redirect(ResolveUrl("~/Scripts/Login.aspx")); 
        }
    }
}
