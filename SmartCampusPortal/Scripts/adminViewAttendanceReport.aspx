<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="adminViewAttendanceReport.aspx.cs" Inherits="SmartCampusPortal.adminViewAttendanceReport" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>View Attendance Report - Admin Panel</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-1ycn6IcaQQ40JuKD4zEZFhylCbh/2s0tFcw5c+w6kUaXhJ3Q3j6tE6Q2z/j5h4M5eT5I1W6uP4f3t2+P5g==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        /* CSS Variables for a professional, data-centric theme */
        :root {
            --primary-accent: #1A73E8; /* A Google-inspired blue, strong and professional */
            --secondary-accent: #20C997; /* A refreshing mint green for positive states */
            --background-dark-light: #F0F2F5; /* Very light gray background for the page */
            --background-card: #FFFFFF; /* Pure white cards */
            --background-navbar: #1F2E3D; /* Deep dark blue for navbar */
            --background-sidebar: #283747; /* Slightly lighter deep blue for sidebar */
            
            --text-light: #ECF0F1; /* Light text for dark backgrounds */
            --text-dark: #34495E; /* Dark blue-gray for main content text */
            --text-muted: #7F8C8D; /* Muted gray for secondary text */
            --border-subtle: #DDE4ED; /* Subtle border for elements */
            --shadow-soft: rgba(0, 0, 0, 0.08); /* Soft shadow */
            --shadow-strong: rgba(0, 0, 0, 0.15); /* Stronger shadow on hover */

            --transition-speed: 0.3s;
            --border-radius-large: 12px; /* More rounded corners */
            --border-radius-small: 8px;
        }

        body {
            background-color: var(--background-dark-light);
            color: var(--text-dark);
            font-family: 'Roboto', 'Open Sans', 'Segoe UI', sans-serif; /* Clean, modern font */
            line-height: 1.6;
            overflow-x: hidden;
        }

        /* Navbar Styling */
        .navbar {
            background-color: var(--background-navbar) !important;
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2); /* Deeper shadow for dark navbar */
            padding: 0.9rem 1.5rem;
            transition: all var(--transition-speed);
        }
        .navbar-brand {
            color: var(--text-light) !important;
            font-weight: 700;
            font-size: 1.6rem;
            letter-spacing: -0.5px;
            transition: color var(--transition-speed);
        }
        .navbar-brand:hover {
            color: var(--primary-accent) !important; /* Primary accent on brand hover */
            transform: scale(1.01);
        }
        .nav-link {
            color: var(--text-light) !important;
            font-weight: 500;
            padding: 0.7rem 1.1rem;
            transition: color var(--transition-speed), background-color var(--transition-speed);
            border-radius: var(--border-radius-small);
        }
        .nav-link:hover {
            color: var(--primary-accent) !important;
            background-color: rgba(255, 255, 255, 0.08); /* Light highlight on dark background */
        }

        /* Logout Button Specific Styling */
        .nav-link.logout-btn {
            background-color: #E74C3C; /* Flat red for logout */
            color: white !important;
            padding: 9px 20px;
            border-radius: var(--border-radius-small);
            margin-left: 25px;
            display: flex;
            align-items: center;
            font-weight: 600;
            box-shadow: 0 4px 10px rgba(231, 76, 60, 0.3);
        }
        .nav-link.logout-btn i {
            margin-right: 8px;
            color: white;
        }
        .nav-link.logout-btn:hover {
            background-color: #C0392B; /* Darker red on hover */
            transform: translateY(-2px);
            box-shadow: 0 6px 15px rgba(231, 76, 60, 0.4);
        }

        /* Sidebar Styling */
        .sidebar {
            background-color: var(--background-sidebar);
            padding-top: 30px;
            height: 100vh;
            border-right: 1px solid rgba(255, 255, 255, 0.05);
            box-shadow: 2px 0 10px rgba(0, 0, 0, 0.15); /* Stronger shadow */
        }
        .sidebar-sticky {
            position: -webkit-sticky;
            position: sticky;
            top: 70px;
            height: calc(100vh - 70px);
            padding-top: 1.2rem;
            overflow-x: hidden;
            overflow-y: auto;
        }
        .sidebar .nav-link {
            color: var(--text-light);
            padding: 16px 30px; /* More generous padding */
            border-left: 4px solid transparent;
            transition: all var(--transition-speed);
            font-weight: 500;
            display: flex;
            align-items: center;
            font-size: 1.05em;
        }
        .sidebar .nav-link i {
            margin-right: 15px;
            font-size: 1.25em;
            color: rgba(var(--text-light), 0.7); /* Slightly muted icon */
            transition: color var(--transition-speed);
        }
        .sidebar .nav-link.active {
            background-color: rgba(var(--primary-accent), 0.1); /* Light tint for active */
            border-left-color: var(--primary-accent);
            color: var(--primary-accent) !important;
            font-weight: 600;
        }
        .sidebar .nav-link.active i {
            color: var(--primary-accent);
        }
        .sidebar .nav-link:hover:not(.active) {
            background-color: rgba(var(--text-light), 0.05);
            border-left-color: var(--secondary-accent); /* Different color on hover for variety */
            color: var(--secondary-accent) !important; /* Greenish hover */
            transform: translateX(4px); /* More pronounced slide */
        }
        .sidebar .nav-link:hover:not(.active) i {
            color: var(--secondary-accent);
        }

        /* Main Content Area */
        .content {
            padding: 40px; /* Increased padding */
            margin-top: 60px;
            animation: slideInUp 0.8s ease-out forwards; /* New animation */
        }
        @keyframes slideInUp {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        /* Card Styling */
        .card {
            background-color: var(--background-card);
            color: var(--text-dark);
            border: none;
            border-radius: var(--border-radius-large); /* More rounded */
            box-shadow: 0 6px 20px var(--shadow-soft);
            margin-bottom: 35px; /* More space */
            transition: transform var(--transition-speed), box-shadow var(--transition-speed);
        }
        .card:hover {
            transform: translateY(-6px);
            box-shadow: 0 12px 30px var(--shadow-strong);
        }
        .card-header {
            background-color: var(--primary-accent); /* Strong blue header */
            color: white;
            font-weight: 600;
            padding: 20px 30px;
            font-size: 1.3em;
            border-bottom: none;
            border-top-left-radius: var(--border-radius-large);
            border-top-right-radius: var(--border-radius-large);
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

        /* Form Controls */
        .form-control {
            background-color: #F8F9FA; /* Off-white for inputs */
            border: 1px solid var(--border-subtle);
            color: var(--text-dark);
            padding: 13px 20px;
            border-radius: var(--border-radius-small);
            transition: border-color var(--transition-speed), box-shadow var(--transition-speed);
        }
        .form-control:focus {
            border-color: var(--primary-accent);
            box-shadow: 0 0 0 0.2rem rgba(26, 115, 232, 0.25);
            outline: none;
        }
        .form-control::placeholder {
            color: var(--text-muted);
            opacity: 0.9;
        }
        label {
            font-weight: 500;
            color: var(--text-dark);
            margin-bottom: 8px;
            display: block;
        }

        /* Buttons */
        .btn {
            font-weight: 600;
            padding: 13px 30px;
            border-radius: var(--border-radius-small);
            transition: all var(--transition-speed) ease-in-out;
            position: relative;
            overflow: hidden;
            z-index: 1;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
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
            background: linear-gradient(120deg, transparent, rgba(255,255,255,0.25), transparent);
            transition: all 0.7s cubic-bezier(0.25, 0.46, 0.45, 0.94);
            z-index: -1;
        }
        .btn:hover::before {
            left: 100%;
        }
        .btn:hover {
            transform: translateY(-4px);
            box-shadow: 0 8px 20px var(--shadow-strong);
        }

        .btn-primary {
            background-color: var(--primary-accent);
            border-color: var(--primary-accent);
            color: white;
        }
        .btn-primary:hover {
            background-color: #1565C0; /* Darker blue */
            border-color: #1565C0;
        }

        /* GridView Styling */
        .table {
            color: var(--text-dark);
            border-collapse: separate;
            border-spacing: 0;
            border-radius: var(--border-radius-large);
            overflow: hidden;
            box-shadow: 0 6px 20px var(--shadow-soft);
            background-color: var(--background-card);
        }
        .table thead th {
            background-color: #ECF0F1; /* Light, clean header background */
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
            background-color: #FDFDFD; /* Very subtle zebra striping */
        }
        .table tbody tr:hover {
            background-color: #F5FAFF; /* Very light blue tint on hover */
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

        /* Specific styling for attendance status (applied by JavaScript) */
        .status-Present {
            color: var(--secondary-accent); /* Green */
            font-weight: 600;
        }
        .status-Absent {
            color: #E74C3C; /* Red for absent */
            font-weight: 600;
        }
        .status-Late {
            color: #F39C12; /* Orange/Yellow for late */
            font-weight: 600;
        }

        /* Message Literal Styling */
        #<%= litMessage.ClientID %> {
            display: block;
            margin-top: 30px;
            padding: 18px 25px;
            border-radius: var(--border-radius-small);
            font-weight: 500;
            background-color: #E9F7EF; /* Light green for general messages */
            color: #285A3F;
            border: 1px solid #D4EDDA;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
            animation: fadeInScale 0.6s ease-out;
        }
        /* Specific message types (if your C# code adds classes like "alert-success") */
        #<%= litMessage.ClientID %>.alert-success { background-color: #d4edda; color: #155724; border-color: #c3e6cb; }
        #<%= litMessage.ClientID %>.alert-danger { background-color: #f8d7da; color: #721c24; border-color: #f5c6cb; }

        @keyframes fadeInScale {
            0% { opacity: 0; transform: scale(0.95); }
            100% { opacity: 1; transform: scale(1); }
        }
    </style>
</head>
<body>
    <form id="adminViewAttendanceReportForm" runat="server">
        <nav class="navbar navbar-expand-lg fixed-top">
            <a class="navbar-brand" href="#">Smart Campus Portal <span style="font-weight:400; opacity:0.8;">- Admin</span></a>
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
                                <a class="nav-link" href="adminManageCourses.aspx">
                                    <i class="fas fa-book"></i> Manage Courses
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link active" href="adminViewAttendanceReport.aspx">
                                    <i class="fas fa-clipboard-list"></i> View Attendance
                                </a>
                            </li>
                        </ul>
                    </div>
                </nav>

                <main role="main" class="col-md-9 ml-sm-auto col-lg-10 content">
                    <h1 class="mt-4 mb-5 text-center">Comprehensive Attendance Report</h1>

                    <div class="card mb-5">
                        <div class="card-header">
                            <i class="fas fa-filter"></i> Filter Attendance Data
                        </div>
                        <div class="card-body">
                            <div class="form-row">
                                <div class="form-group col-md-6">
                                    <label for="ddlCourseFilter"><i class="fas fa-book"></i> Course</label>
                                    <asp:DropDownList ID="ddlCourseFilter" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <div class="form-group col-md-6">
                                    <label for="ddlStudentFilter"><i class="fas fa-user-graduate"></i> Student</label>
                                    <asp:DropDownList ID="ddlStudentFilter" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-group col-md-6">
                                    <label for="txtDateFilter"><i class="fas fa-calendar-alt"></i> Date (Optional)</label>
                                    <asp:TextBox ID="txtDateFilter" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="form-group col-md-6">
                                    <label for="ddlStatusFilter"><i class="fas fa-check-circle"></i> Status</label>
                                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Present" Value="Present"></asp:ListItem>
                                        <asp:ListItem Text="Absent" Value="Absent"></asp:ListItem>
                                        <asp:ListItem Text="Late" Value="Late"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <asp:Button ID="btnFilter" runat="server" Text="Apply Filter" CssClass="btn btn-primary mt-3" OnClick="btnFilter_Click" />
                        </div>
                    </div>

                    <div class="card">
                        <div class="card-header">
                            <i class="fas fa-chart-bar"></i> Attendance Records
                        </div>
                        <div class="card-body">
                            <asp:GridView ID="gvAttendance" runat="server" AutoGenerateColumns="False"
                                CssClass="table" HeaderStyle-CssClass="thead-custom">
                                <Columns>
                                    <asp:BoundField DataField="CourseName" HeaderText="Course" />
                                    <asp:BoundField DataField="FullName" HeaderText="Student Name" />
                                    <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:d}" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                </Columns>
                                <PagerStyle CssClass="gridview-pager" />
                                <EmptyDataTemplate>
                                    <p class="text-muted text-center py-4">No attendance records found for the selected filters.</p>
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
        <script type="text/javascript">
            $(document).ready(function () {
                // Function to apply status classes to the GridView
                function applyStatusClasses() {
                   
                    var gridViewRows = $('#<%= gvAttendance.ClientID %>').find('tbody tr');

                    gridViewRows.each(function () {
                        var statusCell = $(this).find('td').eq(3); // 'Status' column is the 4th column (index 3)
                        var statusText = statusCell.text().trim(); // Get the text content and trim whitespace

                        // Remove existing status classes first to prevent conflicts on postbacks
                        statusCell.removeClass('status-Present status-Absent status-Late');

                        // Apply new class based on status text
                        if (statusText === 'Present') {
                            statusCell.addClass('status-Present');
                        } else if (statusText === 'Absent') {
                            statusCell.addClass('status-Absent');
                        } else if (statusText === 'Late') {
                            statusCell.addClass('status-Late');
                        }
                    });
                }
                applyStatusClasses();

                }
            });
        </script>
    </form>
</body>
</html>