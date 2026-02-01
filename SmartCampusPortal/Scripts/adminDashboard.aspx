<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="adminDashboard.aspx.cs" Inherits="SmartCampusPortal.adminDashboard" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Admin Dashboard - Smart Campus Portal</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-1ycn6IcaQQ40JuKD4zEZFhylCbh/2s0tFcw5c+w6kUaXhJ3Q3j6tE6Q2z/j5h4M5eT5I1W6uP4f3t2+P5g==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        /* CSS Variables for a flexible and light theme */
        :root {
            --primary-color: #4CAF50; /* Green - for main actions, branding */
            --secondary-color: #2196F3; /* Blue - for links, highlights */
            --accent-color-1: #FFC107; /* Amber */
            --accent-color-2: #FF5722; /* Deep Orange */
            --background-light: #F0F2F5; /* Light Gray Background */
            --card-background: #FFFFFF; /* White Cards */
            --navbar-background: #FFFFFF; /* White Navbar */
            --sidebar-background: #FFFFFF; /* White Sidebar */
            --text-dark: #333333;
            --text-muted-light: #6c757d;
            --border-light: #E0E0E0;
            --shadow-light: rgba(0, 0, 0, 0.08);
            --transition-speed: 0.3s;
        }

        body {
            background-color: var(--background-light);
            color: var(--text-dark);
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            overflow-x: hidden; /* Prevent horizontal scroll for animations */
        }

        /* Animated background elements */
        body::before, body::after {
            content: '';
            position: fixed;
            z-index: -1;
            filter: blur(100px);
            opacity: 0.3;
            border-radius: 50%;
        }

        body::before {
            width: 300px;
            height: 300px;
            background-color: var(--primary-color);
            top: -100px;
            left: -100px;
            animation: moveGradient1 15s infinite alternate ease-in-out;
        }

        body::after {
            width: 400px;
            height: 400px;
            background-color: var(--secondary-color);
            bottom: -150px;
            right: -150px;
            animation: moveGradient2 20s infinite alternate ease-in-out;
        }

        @keyframes moveGradient1 {
            0% { transform: translate(0, 0); }
            50% { transform: translate(100px, 150px); }
            100% { transform: translate(0, 0); }
        }

        @keyframes moveGradient2 {
            0% { transform: translate(0, 0); }
            50% { transform: translate(-150px, -100px); }
            100% { transform: translate(0, 0); }
        }

        .navbar {
            background-color: var(--navbar-background) !important;
            border-bottom: 1px solid var(--border-light);
            box-shadow: 0 2px 5px var(--shadow-light);
            transition: background-color var(--transition-speed);
        }
        
        .navbar-brand {
            color: var(--primary-color) !important;
            font-weight: 700;
            font-size: 1.5rem;
            transition: color var(--transition-speed);
        }
        .navbar-brand:hover {
            color: var(--primary-color);
            transform: scale(1.02); /* Slight zoom on hover */
            transition: transform var(--transition-speed);
        }

        .nav-link {
            color: var(--text-dark) !important;
            font-weight: 500;
            transition: color var(--transition-speed), background-color var(--transition-speed);
        }
        .nav-link:hover {
            color: var(--secondary-color) !important;
        }
        .nav-item .nav-link:hover {
             background-color: rgba(var(--secondary-color), 0.05); /* Very subtle background on hover */
             border-radius: 5px;
        }

        .sidebar {
            background-color: var(--sidebar-background);
            padding-top: 20px;
            height: 100vh;
            border-right: 1px solid var(--border-light);
            box-shadow: 2px 0 5px var(--shadow-light);
            transition: width var(--transition-speed); /* For potential future collapse animation */
        }
        .sidebar-sticky {
            position: -webkit-sticky;
            position: sticky;
            top: 60px; /* Adjust based on navbar height */
            height: calc(100vh - 60px); /* Adjust based on navbar height */
            padding-top: 0.5rem;
            overflow-x: hidden;
            overflow-y: auto; /* Scrollable contents if needed */
        }

        .sidebar .nav-link {
            color: var(--text-dark);
            padding: 12px 20px;
            border-left: 5px solid transparent;
            transition: all var(--transition-speed);
            font-weight: 500;
            display: flex;
            align-items: center;
        }
        .sidebar .nav-link i { /* For Font Awesome icons in sidebar */
            margin-right: 10px;
            font-size: 1.1em;
            color: var(--text-muted-light); /* Default icon color */
            transition: color var(--transition-speed);
        }

        .sidebar .nav-link.active {
            background-color: rgba(var(--primary-color), 0.1); /* Lighter primary background */
            border-left-color: var(--primary-color);
            color: var(--primary-color) !important;
            font-weight: 600;
        }
        .sidebar .nav-link.active i {
            color: var(--primary-color); /* Active icon color */
        }

        .sidebar .nav-link:hover:not(.active) {
            background-color: rgba(var(--secondary-color), 0.05);
            border-left-color: var(--secondary-color);
            color: var(--secondary-color) !important;
            transform: translateX(5px); /* Slide effect on hover */
        }
        .sidebar .nav-link:hover:not(.active) i {
            color: var(--secondary-color);
        }

        .content {
            padding: 30px;
            margin-top: 56px; /* Space for fixed navbar */
            animation: fadeIn 0.8s ease-out; /* Fade in content */
        }
        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }

        .card {
            background-color: var(--card-background);
            color: var(--text-dark);
            border: none; /* Remove default bootstrap border */
            border-radius: 12px;
            box-shadow: 0 5px 15px var(--shadow-light);
            margin-bottom: 25px;
            transition: transform var(--transition-speed), box-shadow var(--transition-speed);
            overflow: hidden; /* Ensure content stays within border-radius */
        }
        .card:hover {
            transform: translateY(-5px); /* Lift card on hover */
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15); /* Deeper shadow on hover */
        }

        .card-header {
            background-color: rgba(var(--primary-color), 0.08); /* Lighter primary background */
            border-bottom: 1px solid var(--border-light);
            color: var(--primary-color);
            font-weight: 600;
            padding: 15px 20px;
            font-size: 1.1em;
        }

        .card-body {
            padding: 25px;
        }

        .text-primary-custom {
            color: var(--primary-color) !important; /* Uses the custom primary color */
            font-weight: 600;
        }

        /* Statistic Labels in cards */
        .card-text.display-4 {
            font-size: 3.5rem; /* Larger font size for numbers */
            font-weight: 700;
            color: var(--secondary-color); /* Emphasize numbers with secondary color */
            margin-top: 10px;
            animation: countUp 2s ease-out; /* Example animation for numbers */
        }
        /* Note: countUp animation requires JavaScript to actually count up. 
           This CSS provides a visual effect for when the number is displayed. */
        @keyframes countUp {
            from { transform: translateY(20px); opacity: 0; }
            to { transform: translateY(0); opacity: 1; }
        }
        
        /* Logout Button Specific Styling */
        .nav-link.logout-btn {
            background-color: #dc3545; /* Red for logout */
            color: white !important;
            padding: 8px 15px;
            border-radius: 5px;
            margin-left: 15px;
            transition: background-color var(--transition-speed), transform var(--transition-speed), box-shadow var(--transition-speed);
        }
        .nav-link.logout-btn:hover {
            background-color: #c82333; /* Darker red on hover */
            transform: translateY(-2px);
            box-shadow: 0 4px 10px rgba(0,0,0,0.2);
        }
        
        /* Message area styling for litMessage - targeting by ID */
        /* IMPORTANT: This targets the _output_ of the literal, not the literal control itself. */
        #<%= litMessage.ClientID %> { 
            display: block; /* Ensure it takes full width for styling */
            margin-top: 20px;
            padding: 15px;
            border-radius: 8px;
            font-weight: 500;
            animation: slideInFromBottom 0.5s ease-out;
            /* Default styles for messages, override with specific classes if needed via C# */
            background-color: #e2e3e5; /* Light gray for general messages */
            color: #383d41;
            border: 1px solid #d6d8db;
        }

        /* Example: If you output messages like "<div class='alert-success'>Your message</div>" 
           from the code-behind for litMessage, these styles will apply. */
        /* If litMessage only outputs bare text, you'd need to wrap it in a div in the code-behind 
           or target by ID as shown above and manage colors there. */
        #<%= litMessage.ClientID %>.alert-success { background-color: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
        #<%= litMessage.ClientID %>.alert-danger { background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }


        @keyframes slideInFromBottom {
            0% { transform: translateY(20px); opacity: 0; }
            100% { transform: translateY(0); opacity: 1; }
        }
        
    </style>
</head>
<body>
    <form id="adminDashboardForm" runat="server">
        <nav class="navbar navbar-expand-lg fixed-top">
            <a class="navbar-brand" href="#">Smart Campus Portal - Admin</a>
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ml-auto">
                    <li class="nav-item">
                        <%-- Added custom class for logout button styling --%>
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
                                <a class="nav-link active" href="adminDashboard.aspx">
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
                                <a class="nav-link" href="adminViewAttendanceReport.aspx">
                                    <i class="fas fa-clipboard-list"></i> View Attendance
                                </a>
                            </li>
                        </ul>
                    </div>
                </nav>

                <main role="main" class="col-md-9 ml-sm-auto col-lg-10 content">
                    <h1 class="mt-4 mb-4">Admin Dashboard</h1>

                    <div class="row">
                        <div class="col-md-4">
                            <div class="card text-center">
                                <div class="card-body">
                                    <h5 class="card-title text-primary-custom">Total Students</h5>
                                    <p class="card-text display-4">
                                        <asp:Label ID="lblTotalStudents" runat="server" Text="0"></asp:Label>
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="card text-center">
                                <div class="card-body">
                                    <h5 class="card-title text-primary-custom">Total Faculty</h5>
                                    <p class="card-text display-4">
                                        <asp:Label ID="lblTotalFaculty" runat="server" Text="0"></asp:Label>
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="card text-center">
                                <div class="card-body">
                                    <h5 class="card-title text-primary-custom">Total Courses</h5>
                                    <p class="card-text display-4">
                                        <asp:Label ID="lblTotalCourses" runat="server" Text="0"></asp:Label>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card mt-4">
                        <div class="card-header">
                            Latest Activities
                        </div>
                        <div class="card-body">
                            <asp:Literal ID="litRecentActivities" runat="server"></asp:Literal>
                            <p class="text-muted">No recent activities to display.</p>
                        </div>
                    </div>
                    <%-- Corrected: litMessage no longer has CssClass directly on the Literal control --%>
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