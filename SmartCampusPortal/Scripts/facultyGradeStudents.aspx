<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="facultyGradeStudents.aspx.cs" Inherits="SmartCampusPortal.facultyGradeStudents" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Grade Students - Faculty Portal</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-1ycn6IcaQQ40JuKD4zEZFhylCbh/2s0tFcw5c+w6kUaXhJ3Q3j6tE6Q2z/j5h4M5eT5I1W6uP4f3t2+P5g==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        :root {
            --primary-accent: #7A3B7E;
            --secondary-accent: #B36AB7;
            --background-light: #F5F7FA;
            --background-card: #FFFFFF;
            --background-navbar: #5B2C6F;
            --background-sidebar: #6C3F85;
            
            --text-light: #F9F7F7;
            --text-dark: #333333;
            --text-muted: #888888;
            --border-subtle: #E0E4EB;
            --shadow-soft: rgba(0, 0, 0, 0.08);
            --shadow-strong: rgba(0, 0, 0, 0.15);

            --transition-speed: 0.3s;
            --border-radius-card: 15px;
            --border-radius-element: 8px;

            --success-color: #28a745;
            --info-color: #17a2b8;
            --danger-color: #dc3545;
        }

        body {
            background-color: var(--background-light);
            color: var(--text-dark);
            font-family: 'Segoe UI', 'Roboto', 'Helvetica Neue', sans-serif;
            line-height: 1.6;
            overflow-x: hidden;
        }

        .navbar {
            background-color: var(--background-navbar) !important;
            border-bottom: 1px solid rgba(255, 255, 255, 0.15);
            box-shadow: 0 5px 20px rgba(0, 0, 0, 0.3);
            padding: 1rem 1.8rem;
        }
        .navbar-brand {
            color: var(--text-light) !important;
            font-weight: 700;
            font-size: 1.7rem;
            letter-spacing: -0.8px;
            transition: color var(--transition-speed);
        }
        .navbar-brand:hover {
            color: var(--secondary-accent) !important;
        }
        .nav-link {
            color: var(--text-light) !important;
            font-weight: 500;
            padding: 0.8rem 1.2rem;
            transition: all var(--transition-speed);
            border-radius: var(--border-radius-element);
        }
        .nav-link:hover {
            color: var(--secondary-accent) !important;
            background-color: rgba(255, 255, 255, 0.1);
        }
        .logout-btn {
            background-color: #DA3633;
            color: white !important;
            padding: 9px 20px;
            border-radius: var(--border-radius-element);
            margin-left: 20px;
            display: flex;
            align-items: center;
            font-weight: 600;
            box-shadow: 0 4px 10px rgba(218, 54, 51, 0.3);
        }
        .logout-btn i {
            margin-right: 8px;
            color: white;
        }
        .logout-btn:hover {
            background-color: #B12A28;
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(218, 54, 51, 0.4);
        }

        .sidebar {
            background-color: var(--background-sidebar);
            padding-top: 35px;
            height: 100vh;
            border-right: 1px solid rgba(0, 0, 0, 0.1);
            box-shadow: 2px 0 12px rgba(0, 0, 0, 0.1);
        }
        .sidebar-sticky {
            position: -webkit-sticky;
            position: sticky;
            top: 80px;
            height: calc(100vh - 80px);
            padding-top: 1.5rem;
            overflow-x: hidden;
            overflow-y: auto;
        }
        .sidebar .nav-link {
            color: var(--text-light);
            padding: 18px 25px;
            border-left: 5px solid transparent;
            transition: all var(--transition-speed) ease-out;
            font-weight: 500;
            display: flex;
            align-items: center;
            font-size: 1.05em;
        }
        .sidebar .nav-link i {
            margin-right: 18px;
            font-size: 1.3em;
            color: rgba(var(--text-light), 0.7);
            transition: color var(--transition-speed);
        }
        .sidebar .nav-link.active {
            background-color: rgba(var(--primary-accent), 0.2);
            border-left-color: var(--primary-accent);
            color: var(--primary-accent) !important;
            font-weight: 600;
        }
        .sidebar .nav-link.active i {
            color: var(--primary-accent);
        }
        .sidebar .nav-link:hover:not(.active) {
            background-color: rgba(255, 255, 255, 0.1);
            border-left-color: var(--secondary-accent);
            color: var(--secondary-accent) !important;
            transform: translateX(5px);
        }
        .sidebar .nav-link:hover:not(.active) i {
            color: var(--secondary-accent);
        }

        .content {
            padding: 45px;
            margin-top: 70px;
            animation: fadeIn 0.8s ease-out forwards;
        }
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(15px); }
            to { opacity: 1; transform: translateY(0); }
        }

        h1 {
            color: var(--background-navbar);
            font-weight: 700;
            letter-spacing: -0.5px;
            margin-bottom: 40px !important;
            text-align: center;
        }

        .card {
            background-color: var(--background-card);
            color: var(--text-dark);
            border: none;
            border-radius: var(--border-radius-card);
            box-shadow: 0 8px 25px var(--shadow-soft);
            margin-bottom: 30px;
            transition: transform var(--transition-speed), box-shadow var(--transition-speed);
        }
        .card:hover {
            transform: translateY(-8px);
            box-shadow: 0 15px 40px var(--shadow-strong);
        }
        .card-header {
            background-color: var(--primary-accent);
            color: white;
            font-weight: 600;
            padding: 20px 30px;
            font-size: 1.3em;
            border-bottom: none;
            border-top-left-radius: var(--border-radius-card);
            border-top-right-radius: var(--border-radius-card);
            display: flex;
            align-items: center;
        }
        .card-header i {
            margin-right: 12px;
            font-size: 1.2em;
        }
        .card-body {
            padding: 30px;
        }

        .form-group label {
            font-weight: 500;
            color: var(--text-dark);
            margin-bottom: 8px;
            display: block;
        }
        .form-control {
            background-color: #F8F9FA;
            border: 1px solid var(--border-subtle);
            color: var(--text-dark);
            padding: 12px 18px;
            border-radius: var(--border-radius-element);
            transition: border-color var(--transition-speed), box-shadow var(--transition-speed);
        }
        .form-control:focus {
            border-color: var(--primary-accent);
            box-shadow: 0 0 0 0.2rem rgba(122, 59, 126, 0.25);
            outline: none;
        }
        .form-control::placeholder {
            color: var(--text-muted);
            opacity: 0.8;
        }

        .btn {
            font-weight: 600;
            padding: 12px 25px;
            border-radius: var(--border-radius-element);
            transition: all var(--transition-speed) ease-in-out;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            position: relative;
            overflow: hidden;
            z-index: 1;
        }
        .btn i {
            margin-right: 8px;
        }
        .btn::before {
            content: '';
            position: absolute;
            top: 0;
            left: -100%;
            width: 100%;
            height: 100%;
            background: linear-gradient(120deg, transparent, rgba(255,255,255,0.25), transparent);
            transition: all 0.7s cubic-bezier(0.25, 0.46, 0.45, 0.94);
            z-index: -1;
        }
        .btn:hover::before {
            left: 100%;
        }
        .btn:hover {
            transform: translateY(-3px);
            box-shadow: 0 8px 20px var(--shadow-strong);
        }

        .btn-primary {
            background-color: var(--primary-accent);
            border-color: var(--primary-accent);
            color: white;
        }
        .btn-primary:hover {
            background-color: #6A346E;
            border-color: #6A346E;
        }
        .btn-success {
            background-color: var(--success-color);
            border-color: var(--success-color);
            color: white;
        }
        .btn-success:hover {
            background-color: #218838;
            border-color: #1e7e34;
        }
        .btn-info {
            background-color: var(--info-color);
            border-color: var(--info-color);
            color: white;
        }
        .btn-info:hover {
            background-color: #138496;
            border-color: #117a8b;
        }
        .btn-secondary {
            background-color: #6c757d;
            border-color: #6c757d;
            color: white;
        }
        .btn-secondary:hover {
            background-color: #5a6268;
            border-color: #545b62;
        }
        .btn-danger {
            background-color: var(--danger-color);
            border-color: var(--danger-color);
            color: white;
        }
        .btn-danger:hover {
            background-color: #c82333;
            border-color: #bd2130;
        }

        .table {
            color: var(--text-dark);
            border-collapse: separate;
            border-spacing: 0;
            border-radius: var(--border-radius-card);
            overflow: hidden;
            box-shadow: 0 6px 20px var(--shadow-soft);
            background-color: var(--background-card);
        }
        .table thead th {
            background-color: #E6EBF0;
            color: var(--text-dark);
            border-bottom: 2px solid var(--border-subtle);
            font-weight: 700;
            padding: 18px 25px;
            font-size: 1.05em;
        }
        .table tbody tr {
            transition: background-color 0.2s ease-in-out;
        }
        .table tbody tr:nth-of-type(odd) {
            background-color: #FCFCFD;
        }
        .table tbody tr:hover {
            background-color: #F0F5FC;
            cursor: pointer;
        }
        .table td, .table th {
            border-top: 1px solid var(--border-subtle);
            padding: 15px 25px;
            vertical-align: middle;
        }
        .table tbody tr:first-child td {
            border-top: none;
        }
        
        #<%= litMessage.ClientID %> {
            display: block;
            margin-top: 30px;
            padding: 18px 25px;
            border-radius: var(--border-radius-element);
            font-weight: 500;
            background-color: #E9F7EF;
            color: #285A3F;
            border: 1px solid #D4EDDA;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
            animation: fadeIn 0.6s ease-out;
        }
        #<%= litMessage.ClientID %>.alert-success { background-color: #d4edda; color: #155724; border-color: #c3e6cb; }
        #<%= litMessage.ClientID %>.alert-danger { background-color: #f8d7da; color: #721c24; border-color: #f5c6cb; }
    </style>
</head>
<body>
    <form id="facultyGradeStudentsForm" runat="server">
        <nav class="navbar navbar-expand-lg fixed-top">
            <a class="navbar-brand" href="#">Smart Campus Portal <span style="font-weight:400; opacity:0.8;">- Faculty</span></a>
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ml-auto">
                    <li class="nav-item">
                        <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" CssClass="nav-link logout-btn">
                            <i class="fas fa-sign-out-alt"></i> Logout
                        </asp:LinkButton>
                    </li>
                </ul>
            </div>
        </nav>

        <div class="container-fluid">
            <div class="row">
                <nav class="col-md-2 d-none d-md-block sidebar">
                    <div class="sidebar-sticky">
                        <ul class="nav flex-column">
                            <li class="nav-item">
                                <a class="nav-link" href="facultyDashboard.aspx">
                                    <i class="fas fa-chart-line"></i> Dashboard
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="facultyUploadAssignment.aspx">
                                    <i class="fas fa-upload"></i> Upload Assignments
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link active" href="facultyGradeStudents.aspx">
                                    <i class="fas fa-marker"></i> Grade Students
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="facultySendAnnouncement.aspx">
                                    <i class="fas fa-bullhorn"></i> Send Announcement
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="facultyViewAttendance.aspx">
                                    <i class="fas fa-clipboard-check"></i> View Attendance
                                </a>
                            </li>
                        </ul>
                    </div>
                </nav>

                <main role="main" class="col-md-9 ml-sm-auto col-lg-10 content">
                    <h1>Grade Students' Submissions</h1>

                    <div class="card mb-4">
                        <div class="card-header">
                            <i class="fas fa-filter"></i> Filter Submissions
                        </div>
                        <div class="card-body">
                            <div class="form-group">
                                <label for="ddlAssignment"><i class="fas fa-tasks"></i> Select Assignment</label>
                                <asp:DropDownList ID="ddlAssignment" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlAssignment_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <asp:Button ID="btnLoadSubmissions" runat="server" Text="Load Submissions" CssClass="btn btn-primary" OnClick="btnLoadSubmissions_Click" />
                        </div>
                    </div>

                    <div class="card">
                        <div class="card-header">
                            <i class="fas fa-list-alt"></i> Submissions for Selected Assignment
                        </div>
                        <div class="card-body">
                            <asp:GridView ID="gvSubmissions" runat="server" AutoGenerateColumns="False" DataKeyNames="SubmissionID"
                                CssClass="table" HeaderStyle-CssClass="thead-custom"
                                OnRowEditing="gvSubmissions_RowEditing" OnRowUpdating="gvSubmissions_RowUpdating"
                                OnRowCancelingEdit="gvSubmissions_RowCancelingEdit">
                                <Columns>
                                    <asp:BoundField DataField="SubmissionID" HeaderText="ID" ReadOnly="True" />
                                    <asp:BoundField DataField="StudentName" HeaderText="Student Name" ReadOnly="True" />
                                    <asp:BoundField DataField="SubmissionDate" HeaderText="Submission Date" DataFormatString="{0:d}" ReadOnly="True" />
                                    <asp:TemplateField HeaderText="Grade">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrade" runat="server" Text='<%# Eval("Grade") == DBNull.Value ? "N/A" : Eval("Grade") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtGrade" runat="server" Text='<%# Eval("Grade") == DBNull.Value ? "" : Eval("Grade") %>' TextMode="Number" CssClass="form-control"></asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-info"><i class="fas fa-edit"></i> Grade</asp:LinkButton>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn btn-sm btn-success"><i class="fas fa-check"></i> Update</asp:LinkButton>
                                            <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn btn-sm btn-secondary ml-2"><i class="fas fa-times"></i> Cancel</asp:LinkButton>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <p class="text-muted text-center py-4">No submissions found for the selected assignment.</p>
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="thead-custom" />
                            </asp:GridView>
                        </div>
                    </div>
                    <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                </main>
            </div>
        </div>

        <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
        <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    </form>
</body>
</html>