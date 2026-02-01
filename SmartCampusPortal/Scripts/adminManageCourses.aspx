<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="adminManageCourses.aspx.cs" Inherits="SmartCampusPortal.adminManageCourses" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Manage Courses - Admin Panel</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-1ycn6IcaQQ40JuKD4zEZFhylCbh/2s0tFcw5c+w6kUaXhJ3Q3j6tE6Q2z/j5h4M5eT5I1W6uP4f3t2+P5g==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        /* CSS Variables for a flexible and vibrant theme */
        :root {
            --primary-color: #007bff; /* Blue - for main actions, branding, strong links */
            --secondary-color: #28a745; /* Green - for success, positive actions */
            --accent-color-1: #6f42c1; /* Purple */
            --accent-color-2: #fd7e14; /* Orange */
            --background-light: #F8F9FA; /* Very Light Gray Background */
            --card-background: #FFFFFF; /* White Cards */
            --navbar-background: #FFFFFF; /* White Navbar */
            --sidebar-background: #FFFFFF; /* White Sidebar */
            --text-dark: #343a40; /* Darker text for good contrast */
            --text-muted-light: #6c757d;
            --border-light: #DEE2E6; /* Lighter border */
            --shadow-light: rgba(0, 0, 0, 0.05); /* Very light shadow */
            --transition-speed: 0.3s;
        }

        body {
            background-color: var(--background-light);
            color: var(--text-dark);
            font-family: 'Open Sans', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            overflow-x: hidden; /* Prevent horizontal scroll for animations */
        }

        /* Subtle animated background elements */
        body::before, body::after {
            content: '';
            position: fixed;
            z-index: -1;
            filter: blur(80px);
            opacity: 0.2;
            border-radius: 50%;
            transition: all 0.8s ease-in-out; /* Smoother animation */
        }

        body::before {
            width: 250px;
            height: 250px;
            background-color: var(--primary-color);
            top: -50px;
            left: -50px;
            animation: moveGradient1 18s infinite alternate ease-in-out;
        }

        body::after {
            width: 350px;
            height: 350px;
            background-color: var(--secondary-color);
            bottom: -80px;
            right: -80px;
            animation: moveGradient2 22s infinite alternate ease-in-out;
        }

        @keyframes moveGradient1 {
            0% { transform: translate(0, 0) scale(1); }
            50% { transform: translate(80px, 120px) scale(1.1); }
            100% { transform: translate(0, 0) scale(1); }
        }

        @keyframes moveGradient2 {
            0% { transform: translate(0, 0) scale(1); }
            50% { transform: translate(-120px, -70px) scale(1.1); }
            100% { transform: translate(0, 0) scale(1); }
        }

        .navbar {
            background-color: var(--navbar-background) !important;
            border-bottom: 1px solid var(--border-light);
            box-shadow: 0 2px 8px var(--shadow-light);
            transition: background-color var(--transition-speed);
        }
        
        .navbar-brand {
            color: var(--primary-color) !important;
            font-weight: 700;
            font-size: 1.6rem;
            transition: color var(--transition-speed);
            letter-spacing: -0.5px;
        }
        .navbar-brand:hover {
            color: var(--primary-color);
            transform: scale(1.01);
            transition: transform var(--transition-speed);
        }

        .nav-link {
            color: var(--text-dark) !important;
            font-weight: 500;
            padding: 0.5rem 1rem; /* Adjust padding */
            transition: color var(--transition-speed), background-color var(--transition-speed);
        }
        .nav-link:hover {
            color: var(--primary-color) !important;
        }
        .nav-item .nav-link:hover {
             background-color: rgba(var(--primary-color), 0.05); /* Subtle primary color background on hover */
             border-radius: 6px;
        }

        .sidebar {
            background-color: var(--sidebar-background);
            padding-top: 25px;
            height: 100vh;
            border-right: 1px solid var(--border-light);
            box-shadow: 2px 0 8px var(--shadow-light);
        }
        .sidebar-sticky {
            position: -webkit-sticky;
            position: sticky;
            top: 70px; /* Adjust based on navbar height */
            height: calc(100vh - 70px); /* Adjust based on navbar height */
            padding-top: 0.8rem;
            overflow-x: hidden;
            overflow-y: auto;
        }

        .sidebar .nav-link {
            color: var(--text-dark);
            padding: 14px 25px; /* More generous padding */
            border-left: 5px solid transparent;
            transition: all var(--transition-speed);
            font-weight: 500;
            display: flex;
            align-items: center;
            font-size: 1.05em;
        }
        .sidebar .nav-link i {
            margin-right: 12px;
            font-size: 1.2em;
            color: var(--text-muted-light);
            transition: color var(--transition-speed);
        }

        .sidebar .nav-link.active {
            background-color: rgba(var(--primary-color), 0.1);
            border-left-color: var(--primary-color);
            color: var(--primary-color) !important;
            font-weight: 600;
        }
        .sidebar .nav-link.active i {
            color: var(--primary-color);
        }

        .sidebar .nav-link:hover:not(.active) {
            background-color: rgba(var(--secondary-color), 0.05);
            border-left-color: var(--secondary-color);
            color: var(--secondary-color) !important;
            transform: translateX(5px);
        }
        .sidebar .nav-link:hover:not(.active) i {
            color: var(--secondary-color);
        }

        .content {
            padding: 35px;
            margin-top: 60px; /* Space for fixed navbar */
            animation: fadeIn 0.8s ease-out;
        }
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(10px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .card {
            background-color: var(--card-background);
            color: var(--text-dark);
            border: none;
            border-radius: 15px; /* More rounded corners */
            box-shadow: 0 6px 20px var(--shadow-light); /* Stronger but still light shadow */
            margin-bottom: 30px;
            transition: transform var(--transition-speed), box-shadow var(--transition-speed);
            overflow: hidden;
        }
        .card:hover {
            transform: translateY(-8px); /* Lift more on hover */
            box-shadow: 0 12px 30px rgba(0, 0, 0, 0.1); /* Deeper shadow on hover */
        }

        .card-header {
            background-color: rgba(var(--primary-color), 0.1); /* Light primary background */
            border-bottom: 1px solid rgba(var(--primary-color), 0.2);
            color: var(--primary-color);
            font-weight: 700;
            padding: 18px 25px;
            font-size: 1.2em;
            border-top-left-radius: 15px; /* Match card radius */
            border-top-right-radius: 15px;
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
            border-radius: 10px; /* Slightly more rounded */
            transition: border-color var(--transition-speed), box-shadow var(--transition-speed);
        }
        .form-control:focus {
            border-color: var(--primary-color);
            box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25); /* Primary color shadow */
            outline: none;
        }
        .form-control::placeholder {
            color: var(--text-muted-light);
            opacity: 0.9;
        }
        label {
            font-weight: 500;
            color: var(--text-dark);
            margin-bottom: 5px;
        }

        /* Buttons - Enhanced */
        .btn {
            font-weight: 600;
            padding: 12px 25px;
            border-radius: 10px;
            transition: all var(--transition-speed);
            position: relative;
            overflow: hidden;
            z-index: 1;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
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
            transition: all 0.7s cubic-bezier(0.25, 0.46, 0.45, 0.94); /* Smoother shine */
            z-index: -1;
        }
        .btn:hover::before {
            left: 100%;
        }
        .btn:hover {
            transform: translateY(-4px);
            box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
        }

        .btn-primary {
            background-color: var(--primary-color);
            border-color: var(--primary-color);
        }
        .btn-primary:hover {
            background-color: #0056b3;
            border-color: #0056b3;
        }

        .btn-danger {
            background-color: #dc3545;
            border-color: #dc3545;
        }
        .btn-danger:hover {
            background-color: #c82333;
            border-color: #c82333;
        }
        .btn-info { /* Edit button */
            background-color: #17a2b8;
            border-color: #17a2b8;
            color: white;
        }
        .btn-info:hover {
            background-color: #138496;
            border-color: #138496;
        }
        .btn-success { /* Update button */
            background-color: var(--secondary-color);
            border-color: var(--secondary-color);
            color: white;
        }
        .btn-success:hover {
            background-color: #218838;
            border-color: #218838;
        }
        .btn-secondary { /* Cancel button */
            background-color: #6c757d;
            border-color: #6c757d;
            color: white;
        }
        .btn-secondary:hover {
            background-color: #5a6268;
            border-color: #545b62;
        }

        /* Logout Button Specific Styling */
        .nav-link.logout-btn {
            background-color: #dc3545;
            color: white !important;
            padding: 8px 18px; /* Slightly larger padding */
            border-radius: 6px;
            margin-left: 20px;
            display: flex;
            align-items: center;
            font-weight: 500;
        }
        .nav-link.logout-btn i {
            margin-right: 8px;
            color: white; /* Ensure icon is white */
        }
        .nav-link.logout-btn:hover {
            background-color: #c82333;
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
        }

        /* GridView Styling */
        .table {
            color: var(--text-dark);
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 6px 20px var(--shadow-light);
            margin-bottom: 30px;
            background-color: var(--card-background);
        }
        .table thead th {
            background-color: rgba(var(--primary-color), 0.15); /* Slightly darker primary background for header */
            color: var(--primary-color);
            border-bottom: 2px solid var(--primary-color);
            font-weight: 700;
            padding: 15px 20px;
            font-size: 1.05em;
        }
        .table tbody tr {
            transition: background-color var(--transition-speed);
        }
        .table tbody tr:nth-of-type(odd) {
            background-color: #FAFAFA; /* Very slightly off-white for odd rows */
        }
        .table tbody tr:hover {
            background-color: #E0F2F7; /* Lighter blue on hover */
            cursor: pointer;
        }
        .table td, .table th {
            border-top: 1px solid var(--border-light);
            padding: 12px 20px;
            vertical-align: middle;
        }
        
        /* Specific styling for GridView buttons within cells */
        .table .btn-sm {
            padding: 8px 15px; /* Slightly larger padding for grid view buttons */
            font-size: 0.9em;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1); /* Smaller shadow for smaller buttons */
        }
        .table .btn-sm:hover {
             transform: translateY(-2px); /* Slight lift */
             box-shadow: 0 4px 10px rgba(0,0,0,0.15);
        }

        /* Message area styling for litMessage - targeting by ID */
        #<%= litMessage.ClientID %> { 
            display: block;
            margin-top: 25px;
            padding: 18px;
            border-radius: 10px;
            font-weight: 500;
            animation: slideInFromBottom 0.6s cubic-bezier(0.25, 0.46, 0.45, 0.94); /* Smoother animation */
            background-color: #e9ecef; /* Light gray for general messages */
            color: #495057;
            border: 1px solid #ced4da;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        /* If you output messages like "<div class='alert alert-success'>..." from C# */
        #<%= litMessage.ClientID %>.alert-success { background-color: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
        #<%= litMessage.ClientID %>.alert-danger { background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }

        @keyframes slideInFromBottom {
            0% { transform: translateY(30px); opacity: 0; }
            100% { transform: translateY(0); opacity: 1; }
        }
    </style>
</head>
<body>
    <form id="adminManageCoursesForm" runat="server">
        <nav class="navbar navbar-expand-lg fixed-top">
            <a class="navbar-brand" href="#">Smart Campus Portal - Admin</a>
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
                                <a class="nav-link" href="adminManageUsers.aspx">
                                    <i class="fas fa-users"></i> Manage Users
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link active" href="adminManageCourses.aspx">
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
                    <h1 class="mt-4 mb-4">Manage Courses</h1>

                    <div class="card mb-4">
                        <div class="card-header">Add New Course</div>
                        <div class="card-body">
                            <div class="form-row">
                                <div class="form-group col-md-6">
                                    <label for="txtCourseName">Course Name</label>
                                    <asp:TextBox ID="txtCourseName" runat="server" CssClass="form-control" placeholder="e.g., Introduction to Programming"></asp:TextBox>
                                </div>
                                <div class="form-group col-md-6">
                                    <label for="txtDepartment">Department</label>
                                    <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control" placeholder="e.g., Computer Science"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-group col-md-6">
                                    <label for="txtCredits">Credits</label>
                                    <asp:TextBox ID="txtCredits" runat="server" TextMode="Number" CssClass="form-control" placeholder="e.g., 3"></asp:TextBox>
                                </div>
                                <div class="form-group col-md-6">
                                    <label for="ddlPrerequisite">Prerequisite Course (Optional)</label>
                                    <asp:DropDownList ID="ddlPrerequisite" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <asp:Button ID="btnAddCourse" runat="server" Text="Add Course" CssClass="btn btn-primary" OnClick="btnAddCourse_Click" />
                        </div>
                    </div>

                    <div class="card">
                        <div class="card-header">Existing Courses</div>
                        <div class="card-body">
                            <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" DataKeyNames="CourseID"
                                CssClass="table" HeaderStyle-CssClass="thead-custom"
                                OnRowDeleting="gvCourses_RowDeleting" OnRowEditing="gvCourses_RowEditing"
                                OnRowUpdating="gvCourses_RowUpdating" OnRowCancelingEdit="gvCourses_RowCancelingEdit">
                                <Columns>
                                    <asp:BoundField DataField="CourseID" HeaderText="ID" ReadOnly="True" />
                                    <asp:BoundField DataField="CourseName" HeaderText="Course Name" />
                                    <asp:BoundField DataField="Department" HeaderText="Department" />
                                    <asp:BoundField DataField="Credits" HeaderText="Credits" />
                                    <asp:TemplateField HeaderText="Prerequisite">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrerequisite" runat="server" Text='<%# Eval("PrerequisiteCourseName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList ID="ddlEditPrerequisite" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-info"><i class="fas fa-edit"></i> Edit</asp:LinkButton>
                                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to delete this course? This will also delete related assignments, registrations, and attendance.');"><i class="fas fa-trash-alt"></i> Delete</asp:LinkButton>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn btn-sm btn-success"><i class="fas fa-check-circle"></i> Update</asp:LinkButton>
                                            <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn btn-sm btn-secondary"><i class="fas fa-times-circle"></i> Cancel</asp:LinkButton>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="gridview-pager" />
                                <EmptyDataTemplate>
                                    <p class="text-muted text-center py-4">No courses found. Add a new course above!</p>
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