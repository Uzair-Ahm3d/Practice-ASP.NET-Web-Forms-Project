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
        // Controls added for the payment panel (declared here to avoid designer churn)
        protected Panel pnlPay;
        protected Label lblOutstanding;
        protected TextBox txtPayAmount;
        protected DropDownList ddlMethod;
        protected TextBox txtCardName;
        protected TextBox txtCardNumber;
        protected TextBox txtExpiry;
        protected TextBox txtCvv;
        protected Button btnPay;
        protected Literal litPayMsg;

        private static bool IsDigits(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char c in s) if (!char.IsDigit(c)) return false;
            return true;
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            litPayMsg.Text = "";
            int studentId = Convert.ToInt32(Session["UserID"]);

            decimal amount;
            if (!decimal.TryParse(txtPayAmount.Text.Trim(), out amount) || amount <= 0)
            { litPayMsg.Text = "<div class='alert alert-warning mt-3'>Please enter a valid payment amount.</div>"; return; }

            string cardNum = (txtCardNumber.Text ?? "").Replace(" ", "");
            if (string.IsNullOrWhiteSpace(txtCardName.Text))
            { litPayMsg.Text = "<div class='alert alert-warning mt-3'>Please enter the cardholder name.</div>"; return; }
            if (cardNum.Length < 13 || cardNum.Length > 19 || !IsDigits(cardNum))
            { litPayMsg.Text = "<div class='alert alert-warning mt-3'>Please enter a valid card number.</div>"; return; }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtExpiry.Text.Trim(), @"^(0[1-9]|1[0-2])\/\d{2}$"))
            { litPayMsg.Text = "<div class='alert alert-warning mt-3'>Please enter the expiry as MM/YY.</div>"; return; }
            string cvv = txtCvv.Text.Trim();
            if (cvv.Length < 3 || cvv.Length > 4 || !IsDigits(cvv))
            { litPayMsg.Text = "<div class='alert alert-warning mt-3'>Please enter a valid CVV.</div>"; return; }

            string cs = ConfigurationManager.ConnectionStrings["SmartCampusPortalConnection"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    decimal outstanding;
                    using (SqlCommand c = new SqlCommand("SELECT ISNULL(SUM(TotalAmount-PaidAmount),0) FROM FeeRecords WHERE StudentID=@s", con))
                    { c.Parameters.AddWithValue("@s", studentId); outstanding = Convert.ToDecimal(c.ExecuteScalar()); }

                    if (outstanding <= 0)
                    { litPayMsg.Text = "<div class='alert alert-info mt-3'>You have no outstanding dues.</div>"; return; }
                    if (amount > outstanding) amount = outstanding;

                    DataTable feeDt = new DataTable();
                    using (SqlCommand sel = new SqlCommand("SELECT FeeID, TotalAmount, PaidAmount FROM FeeRecords WHERE StudentID=@s AND (TotalAmount-PaidAmount)>0 ORDER BY FeeID", con))
                    {
                        sel.Parameters.AddWithValue("@s", studentId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sel)) da.Fill(feeDt);
                    }
                    decimal remaining = amount;
                    foreach (DataRow row in feeDt.Rows)
                    {
                        if (remaining <= 0) break;
                        decimal total = Convert.ToDecimal(row["TotalAmount"]);
                        decimal paid = Convert.ToDecimal(row["PaidAmount"]);
                        decimal pay = Math.Min(total - paid, remaining);
                        decimal newPaid = paid + pay;
                        string status = (newPaid >= total) ? "Paid" : "Partial";
                        using (SqlCommand up = new SqlCommand("UPDATE FeeRecords SET PaidAmount=@p, PaymentStatus=@st WHERE FeeID=@f", con))
                        {
                            up.Parameters.AddWithValue("@p", newPaid);
                            up.Parameters.AddWithValue("@st", status);
                            up.Parameters.AddWithValue("@f", Convert.ToInt32(row["FeeID"]));
                            up.ExecuteNonQuery();
                        }
                        remaining -= pay;
                    }

                    string reference = "SCP" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    using (SqlCommand log = new SqlCommand("INSERT INTO Payments (StudentID, Amount, Method, Reference) VALUES (@s,@a,@m,@r)", con))
                    {
                        log.Parameters.AddWithValue("@s", studentId);
                        log.Parameters.AddWithValue("@a", amount);
                        log.Parameters.AddWithValue("@m", ddlMethod.SelectedValue);
                        log.Parameters.AddWithValue("@r", reference);
                        log.ExecuteNonQuery();
                    }

                    litPayMsg.Text = "<div class='alert alert-success mt-3'><i class='fas fa-check-circle'></i> Payment of $" +
                        amount.ToString("N2") + " successful! Reference: <strong>" + reference + "</strong></div>";
                    txtCardName.Text = txtCardNumber.Text = txtExpiry.Text = txtCvv.Text = "";
                }
                LoadFeeSummary();
                BindFeeRecordsGridView();
            }
            catch (Exception ex)
            {
                litPayMsg.Text = "<div class='alert alert-danger mt-3'>Payment error: " + ex.Message + "</div>";
            }
        }

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
                        decimal due = Convert.ToDecimal(reader["TotalAmountDue"]);
                        decimal paid = Convert.ToDecimal(reader["TotalPaid"]);
                        decimal outstanding = due - paid;
                        if (outstanding < 0) outstanding = 0;
                        lblTotalAmountDue.Text = due.ToString("N2");
                        lblTotalPaid.Text = paid.ToString("N2");
                        lblOutstanding.Text = outstanding.ToString("N2");
                        txtPayAmount.Text = outstanding.ToString("0.00");
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
