<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="facultyDashboard.aspx.cs" Inherits="SmartCampusPortal.facultyDashboard" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Faculty Dashboard - Smart Campus Portal</title>
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
        .card-body {
            padding: 30px;
            position: relative;
        }
        .card-body::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 10px;
            background: linear-gradient(to right, var(--primary-accent), var(--secondary-accent));
            border-top-left-radius: var(--border-radius-card);
            border-top-right-radius: var(--border-radius-card);
        }
        .card-title {
            color: var(--primary-accent);
            font-size: 1.4rem;
            font-weight: 600;
            margin-bottom: 15px;
            padding-top: 10px;
        }
        .card-text.display-4 {
            font-size: 3.5rem;
            font-weight: 700;
            color: var(--background-navbar);
            line-height: 1;
            margin-top: 15px;
        }

        .text-primary-custom {
            color: var(--primary-accent) !important;
        }
        
        .alert-info-custom {
            background-color: #E6EBF8;
            border-color: #A0B9EB;
            color: #4C6AB0;
            padding: 18px 25px;
            border-radius: var(--border-radius-element);
            margin-bottom: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            animation: fadeIn 0.6s ease-out;
        }

        .announcement-item {
            background-color: #F8F9FA;
            border: 1px solid var(--border-subtle);
            border-left: 5px solid var(--secondary-accent);
            padding: 15px 20px;
            margin-bottom: 15px;
            border-radius: var(--border-radius-element);
            transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
        }
        .announcement-item:hover {
            transform: translateY(-3px);
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
        }
        .announcement-title {
            font-weight: 600;
            color: var(--primary-accent);
            margin-bottom: 5px;
            font-size: 1.15em;
        }
        .announcement-date {
            font-size: 0.85em;
            color: var(--text-muted);
            margin-bottom: 10px;
            display: block;
        }
        .announcement-content {
            color: var(--text-dark);
            font-size: 0.95em;
        }

        #<%= litWelcomeMessage.ClientID %> {
            font-size: 1.8rem;
            font-weight: 500;
            color: var(--background-navbar);
            text-align: center;
            margin-bottom: 50px;
            display: block;
            opacity: 0;
            animation: fadeInScaleUp 1s ease-out 0.2s forwards;
        }
        @keyframes fadeInScaleUp {
            from { opacity: 0; transform: scale(0.95) translateY(10px); }
            to { opacity: 1; transform: scale(1) translateY(0); }
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
        #noAnnouncements {
            padding: 20px;
            text-align: center;
            font-style: italic;
            color: var(--text-muted);
        }
    </style>
</head>
<body>
    <form id="facultyDashboardForm" runat="server">
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
                                <a class="nav-link active" href="facultyDashboard.aspx">
                                    <i class="fas fa-chart-line"></i> Dashboard
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="facultyUploadAssignment.aspx">
                                    <i class="fas fa-upload"></i> Upload Assignments
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="facultyGradeStudents.aspx">
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
                    <h1>Faculty Dashboard</h1>
                    <asp:Literal ID="litWelcomeMessage" runat="server"></asp:Literal>

                    <div class="row">
                        <div class="col-md-6">
                            <div class="card text-center">
                                <div class="card-body">
                                    <h5 class="card-title">Courses Taught <i class="fas fa-book-open"></i></h5>
                                    <p class="card-text display-4">
                                        <asp:Label ID="lblCoursesTaught" runat="server" Text="0"></asp:Label>
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="card text-center">
                                <div class="card-body">
                                    <h5 class="card-title">Pending Grading <i class="fas fa-tasks"></i></h5>
                                    <p class="card-text display-4">
                                        <asp:Label ID="lblPendingGrading" runat="server" Text="0"></asp:Label>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card mt-4">
                        <div class="card-header" style="background-color: var(--primary-accent); color: white; border-bottom: none; border-top-left-radius: var(--border-radius-card); border-top-right-radius: var(--border-radius-card); font-size: 1.3em;">
                            <i class="fas fa-bell"></i> Recent Announcements
                        </div>
                        <div class="card-body">
                            <%-- The content of litRecentAnnouncements is expected to be HTML for each announcement --%>
                            <asp:Literal ID="litRecentAnnouncements" runat="server"></asp:Literal>
                            <p class="text-muted" id="noAnnouncements" runat="server" visible="false">No recent announcements from your courses.</p>
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