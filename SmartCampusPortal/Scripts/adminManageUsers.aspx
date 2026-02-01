<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="adminManageUsers.aspx.cs" Inherits="SmartCampusPortal.adminManageUsers" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Manage Users - Admin Panel</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-1ycn6IcaQQ40JuKD4zEZFhylCbh/2s0tFcw5c+w6kUaXhJ3Q3j6tE6Q2z/j5h4M5eT5I1W6uP4f3t2+P5g==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        /* CSS Variables for a clean, modern theme */
        :root {
            --primary-blue: #007bff; /* Standard Bootstrap Blue */
            --secondary-gray: #6c757d; /* Standard Bootstrap Gray */
            --accent-green: #28a745; /* Standard Bootstrap Green */
            --danger-red: #dc3545; /* Standard Bootstrap Red */
            --info-cyan: #17a2b8; /* Standard Bootstrap Cyan */

            --background-page: #F0F2F5; /* Light gray background, slightly textured feel */
            --background-card: #FFFFFF; /* Pure white cards */
            --background-navbar: #FFFFFF; /* Pure white navbar */
            --background-sidebar: #FFFFFF; /* Pure white sidebar */
            
            --text-dark: #343a40; /* Darker text for readability */
            --text-muted: #6C757D;
            --border-light: #DEE2E6; /* Light gray border */
            --shadow-subtle: rgba(0, 0, 0, 0.08); /* Subtle shadow */
            --shadow-hover: rgba(0, 0, 0, 0.15); /* Slightly darker shadow on hover */

            --transition-speed: 0.3s;
            --border-radius-base: 10px; /* Consistent rounded corners */
        }

        body {
            background-color: var(--background-page);
            color: var(--text-dark);
            font-family: 'Inter', 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; /* Modern, clean font */
            line-height: 1.6;
            overflow-x: hidden;
        }

        /* Subtle background gradient animation for visual interest */
        body::before {
            content: '';
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: linear-gradient(135deg, rgba(0,123,255,0.05) 0%, rgba(255,255,255,0) 50%, rgba(40,167,69,0.03) 100%);
            z-index: -1;
            animation: backgroundShift 25s ease-in-out infinite alternate;
        }

        @keyframes backgroundShift {
            0% { background-position: 0% 0%; }
            100% { background-position: 100% 100%; }
        }

        /* Navbar Styling */
        .navbar {
            background-color: var(--background-navbar) !important;
            border-bottom: 1px solid var(--border-light);
            box-shadow: 0 2px 10px var(--shadow-subtle);
            padding: 0.8rem 1.5rem;
            transition: all var(--transition-speed);
        }
        .navbar-brand {
            color: var(--primary-blue) !important;
            font-weight: 700;
            font-size: 1.5rem;
            letter-spacing: -0.5px;
            transition: color var(--transition-speed);
        }
        .navbar-brand:hover {
            color: var(--primary-blue);
            transform: scale(1.02); /* Slight scale on hover */
        }
        .nav-link {
            color: var(--text-dark) !important;
            font-weight: 500;
            padding: 0.6rem 1rem;
            transition: color var(--transition-speed), background-color var(--transition-speed);
            border-radius: var(--border-radius-base); /* Inherit from variables */
        }
        .nav-link:hover {
            color: var(--primary-blue) !important;
            background-color: rgba(0, 123, 255, 0.05); /* Light primary tint on hover */
        }

        /* Logout Button Specific Styling */
        .nav-link.logout-btn {
            background-color: var(--danger-red);
            color: white !important;
            padding: 8px 18px;
            border-radius: var(--border-radius-base);
            margin-left: 20px;
            display: flex;
            align-items: center;
            font-weight: 600;
            box-shadow: 0 4px 10px rgba(var(--danger-red), 0.3);
        }
        .nav-link.logout-btn i {
            margin-right: 8px;
            color: white;
        }
        .nav-link.logout-btn:hover {
            background-color: #c82333; /* Darker red */
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(var(--danger-red), 0.4);
        }

        /* Sidebar Styling */
        .sidebar {
            background-color: var(--background-sidebar);
            padding-top: 25px;
            height: 100vh;
            border-right: 1px solid var(--border-light);
            box-shadow: 2px 0 10px var(--shadow-subtle);
        }
        .sidebar-sticky {
            position: -webkit-sticky;
            position: sticky;
            top: 70px; /* Account for navbar height */
            height: calc(100vh - 70px);
            padding-top: 1rem;
            overflow-x: hidden;
            overflow-y: auto;
        }
        .sidebar .nav-link {
            color: var(--text-dark);
            padding: 15px 25px; /* More generous padding */
            border-left: 4px solid transparent;
            transition: all var(--transition-speed);
            font-weight: 500;
            display: flex;
            align-items: center;
            font-size: 1.05em;
        }
        .sidebar .nav-link i {
            margin-right: 12px;
            font-size: 1.2em;
            color: var(--text-muted);
            transition: color var(--transition-speed);
        }
        .sidebar .nav-link.active {
            background-color: rgba(var(--primary-blue), 0.08); /* Lighter primary tint */
            border-left-color: var(--primary-blue);
            color: var(--primary-blue) !important;
            font-weight: 600;
        }
        .sidebar .nav-link.active i {
            color: var(--primary-blue);
        }
        .sidebar .nav-link:hover:not(.active) {
            background-color: rgba(var(--primary-blue), 0.03); /* Very light hover effect */
            border-left-color: var(--primary-blue);
            color: var(--primary-blue) !important;
            transform: translateX(3px); /* Subtle slide effect */
        }
        .sidebar .nav-link:hover:not(.active) i {
            color: var(--primary-blue);
        }

        /* Main Content Area */
        .content {
            padding: 35px;
            margin-top: 60px; /* Space for fixed navbar */
            animation: fadeIn 0.8s ease-out;
        }
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(15px); }
            to { opacity: 1; transform: translateY(0); }
        }

        /* Card Styling */
        .card {
            background-color: var(--background-card);
            color: var(--text-dark);
            border: none;
            border-radius: var(--border-radius-base);
            box-shadow: 0 5px 20px var(--shadow-subtle);
            margin-bottom: 30px;
            transition: transform var(--transition-speed), box-shadow var(--transition-speed);
            overflow: hidden; /* Ensures rounded corners are visible for header */
        }
        .card:hover {
            transform: translateY(-5px); /* Lift slightly on hover */
            box-shadow: 0 10px 25px var(--shadow-hover); /* Deeper shadow on hover */
        }
        .card-header {
            background-color: var(--primary-blue); /* Solid primary blue header */
            color: white;
            font-weight: 600;
            padding: 18px 25px;
            font-size: 1.2em;
            border-bottom: none; /* No bottom border as it's part of the card */
            border-top-left-radius: var(--border-radius-base);
            border-top-right-radius: var(--border-radius-base);
            display: flex;
            align-items: center;
        }
        .card-header i {
            margin-right: 10px;
            font-size: 1.1em;
        }
        .card-body {
            padding: 30px;
        }

        /* Form Controls */
        .form-control {
            background-color: #FDFDFD; /* Very light background for inputs */
            border: 1px solid var(--border-light);
            color: var(--text-dark);
            padding: 12px 18px;
            border-radius: var(--border-radius-base);
            transition: border-color var(--transition-speed), box-shadow var(--transition-speed);
        }
        .form-control:focus {
            border-color: var(--primary-blue);
            box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
            outline: none;
        }
        .form-control::placeholder {
            color: var(--text-muted);
            opacity: 0.8;
        }
        label {
            font-weight: 500;
            color: var(--text-dark);
            margin-bottom: 8px; /* More space below labels */
            display: block; /* Ensures label takes full width */
        }

        /* Buttons - Modern Flat/Gradient Style */
        .btn {
            font-weight: 600;
            padding: 12px 25px;
            border-radius: var(--border-radius-base);
            transition: all var(--transition-speed) ease-in-out;
            position: relative;
            overflow: hidden;
            z-index: 1;
            box-shadow: 0 4px 12px var(--shadow-subtle);
            letter-spacing: 0.5px;
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
            background: linear-gradient(120deg, transparent, rgba(255,255,255,0.2), transparent);
            transition: all 0.6s cubic-bezier(0.25, 0.46, 0.45, 0.94);
            z-index: -1;
        }
        .btn:hover::before {
            left: 100%;
        }
        .btn:hover {
            transform: translateY(-3px);
            box-shadow: 0 8px 20px var(--shadow-hover);
        }

        .btn-primary {
            background-color: var(--primary-blue);
            border-color: var(--primary-blue);
            color: white;
        }
        .btn-primary:hover {
            background-color: #0056b3;
            border-color: #0056b3;
        }

        .btn-danger {
            background-color: var(--danger-red);
            border-color: var(--danger-red);
            color: white;
        }
        .btn-danger:hover {
            background-color: #c82333;
            border-color: #c82333;
        }
        .btn-info { /* Edit button */
            background-color: var(--info-cyan);
            border-color: var(--info-cyan);
            color: white;
        }
        .btn-info:hover {
            background-color: #117a8b;
            border-color: #117a8b;
        }
        .btn-success { /* Update button */
            background-color: var(--accent-green);
            border-color: var(--accent-green);
            color: white;
        }
        .btn-success:hover {
            background-color: #1e7e34;
            border-color: #1e7e34;
        }
        .btn-secondary { /* Cancel button */
            background-color: var(--secondary-gray);
            border-color: var(--secondary-gray);
            color: white;
        }
        .btn-secondary:hover {
            background-color: #545b62;
            border-color: #545b62;
        }

        /* GridView Styling */
        .table {
            color: var(--text-dark);
            border-collapse: separate; /* Allows for rounded corners */
            border-spacing: 0;
            border-radius: var(--border-radius-base);
            overflow: hidden; /* Ensures content respects rounded corners */
            box-shadow: 0 5px 20px var(--shadow-subtle);
            background-color: var(--background-card);
        }
        .table thead th {
            background-color: #E9ECEF; /* A softer, neutral gray header */
            color: var(--text-dark);
            border-bottom: 2px solid var(--border-light);
            font-weight: 600;
            padding: 15px 20px;
            font-size: 1em;
        }
        .table tbody tr {
            transition: background-color 0.2s ease-in-out;
        }
        .table tbody tr:nth-of-type(odd) {
            background-color: #FCFCFC; /* Slightly different shade for zebra striping */
        }
        .table tbody tr:hover {
            background-color: #E8F5FF; /* Light blue tint on hover */
            cursor: pointer;
        }
        .table td, .table th {
            border-top: 1px solid var(--border-light);
            padding: 12px 20px;
            vertical-align: middle;
        }
        /* Remove top border for the first row to avoid double border with header */
        .table tbody tr:first-child td {
            border-top: none;
        }
        
        /* Specific styling for GridView action buttons */
        .table .btn-sm {
            padding: 7px 14px;
            font-size: 0.85em;
            border-radius: 7px;
            margin-right: 5px; /* Spacing between buttons */
            box-shadow: 0 2px 5px rgba(0,0,0,0.05);
        }
        .table .btn-sm:hover {
            transform: translateY(-1px); /* More subtle lift */
            box-shadow: 0 3px 8px rgba(0,0,0,0.1);
        }

        /* Message Literal Styling */
        #<%= litMessage.ClientID %> {
            display: block;
            margin-top: 25px;
            padding: 15px 20px;
            border-radius: var(--border-radius-base);
            font-weight: 500;
            background-color: #e2e3e5; /* Neutral background for messages */
            color: #495057;
            border: 1px solid #d6d8db;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
            animation: slideInFromBottom 0.5s ease-out;
        }
        /* Specific message types (if your C# code adds classes like "alert-success") */
        #<%= litMessage.ClientID %>.alert-success { background-color: #d4edda; color: #155724; border-color: #c3e6cb; }
        #<%= litMessage.ClientID %>.alert-danger { background-color: #f8d7da; color: #721c24; border-color: #f5c6cb; }

    </style>
</head>
<body>
    <form id="adminManageUsersForm" runat="server">
        <nav class="navbar navbar-expand-lg fixed-top">
            <a class="navbar-brand" href="#">Smart Campus Portal <span style="color:var(--text-muted); font-weight:400;">- Admin</span></a>
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
                                <a class="nav-link" href="adminDashboard.aspx">
                                    <i class="fas fa-chart-line"></i> Dashboard
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link active" href="adminManageUsers.aspx">
                                    <i class="fas fa-users"></i> Manage Users
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="adminManageCourses.aspx">
                                    <i class="fas fa-book"></i> Manage Courses
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="adminViewAttendanceReport.aspx">
                                    <i class="fas fa-clipboard-list"></i> View Attendance
                                </a>
                            </li>
                        </ul>
                    </div>
                </nav>

                <main role="main" class="col-md-9 ml-sm-auto col-lg-10 content">
                    <h1 class="mt-4 mb-4">Manage Users</h1>

                    <div class="card mb-4">
                        <div class="card-header">
                            <i class="fas fa-user-plus"></i> Add New User
                        </div>
                        <div class="card-body">
                            <div class="form-row">
                                <div class="form-group col-md-6">
                                    <label for="txtFullName">Full Name</label>
                                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="e.g., John Doe"></asp:TextBox>
                                </div>
                                <div class="form-group col-md-6">
                                    <label for="txtEmail">Email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" placeholder="e.g., john.doe@example.com"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-group col-md-6">
                                    <label for="txtPassword">Password</label>
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter password"></asp:TextBox>
                                </div>
                                <div class="form-group col-md-6">
                                    <label for="ddlRole">Role</label>
                                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="Student" Value="Student"></asp:ListItem>
                                        <asp:ListItem Text="Faculty" Value="Faculty"></asp:ListItem>
                                        <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <asp:Button ID="btnAddUser" runat="server" Text="Add User" CssClass="btn btn-primary" OnClick="btnAddUser_Click" />
                        </div>
                    </div>

                    <div class="card">
                        <div class="card-header">
                             <i class="fas fa-users-cog"></i> Existing Users
                        </div>
                        <div class="card-body">
                            <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" DataKeyNames="UserID"
                                CssClass="table" HeaderStyle-CssClass="thead-custom"
                                OnRowDeleting="gvUsers_RowDeleting" OnRowEditing="gvUsers_RowEditing"
                                OnRowUpdating="gvUsers_RowUpdating" OnRowCancelingEdit="gvUsers_RowCancelingEdit">
                                <Columns>
                                    <asp:BoundField DataField="UserID" HeaderText="ID" ReadOnly="True" />
                                    <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                                    <asp:BoundField DataField="Email" HeaderText="Email" />
                                    <asp:BoundField DataField="Role" HeaderText="Role" />
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-info"><i class="fas fa-edit"></i> Edit</asp:LinkButton>
                                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to delete this user?');"><i class="fas fa-trash-alt"></i> Delete</asp:LinkButton>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn btn-sm btn-success"><i class="fas fa-check-circle"></i> Update</asp:LinkButton>
                                            <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn btn-sm btn-secondary"><i class="fas fa-times-circle"></i> Cancel</asp:LinkButton>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="gridview-pager" />
                                <EmptyDataTemplate>
                                    <p class="text-muted text-center py-4">No users found. Add a new user above!</p>
                                </EmptyDataTemplate>
                                <SortedAscendingHeaderStyle CssClass="sorted-asc-header" />
                                <SortedDescendingHeaderStyle CssClass="sorted-desc-header" />
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