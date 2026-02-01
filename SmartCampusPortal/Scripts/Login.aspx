<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SmartCampusPortal.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Smart Campus Portal - Login</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" integrity="sha512-1ycn6IcaQQ40JuKD4zEZFhylCbh/2s0tFcw5c+w6kUaXhJ3Q3j6tE6Q2z/j5h4M5eT5I1W6uP4f3t2+P5g==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <style>
        /* CSS Variables for easy color management */
        :root {
            --primary-color: #4CAF50; /* Vibrant Green */
            --secondary-color: #2196F3; /* Bright Blue */
            --accent-color-1: #FFC107; /* Warm Amber */
            --accent-color-2: #FF5722; /* Energetic Orange */
            --background-light: #F8F9FA; /* Very Light Gray */
            --card-background: #FFFFFF; /* Pure White */
            --text-dark: #333333;
            --text-muted-light: #6c757d;
            --border-light: #E0E0E0;
            --shadow-light: rgba(0, 0, 0, 0.1);
            --transition-speed: 0.3s;
        }

        body {
            background-color: var(--background-light);
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            margin: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            color: var(--text-dark);
            overflow: hidden; /* For background animations */
        }

        /* Subtle background animation */
        body::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: linear-gradient(135deg, var(--secondary-color) 0%, var(--primary-color) 100%);
            opacity: 0.1;
            z-index: -1;
            animation: backgroundGradient 20s infinite alternate ease-in-out;
        }

        @keyframes backgroundGradient {
            0% { background: linear-gradient(135deg, var(--secondary-color) 0%, var(--primary-color) 100%); }
            50% { background: linear-gradient(135deg, var(--primary-color) 0%, var(--accent-color-1) 100%); }
            100% { background: linear-gradient(135deg, var(--accent-color-2) 0%, var(--secondary-color) 100%); }
        }

        .login-container {
            background-color: var(--card-background);
            padding: 45px;
            border-radius: 15px;
            box-shadow: 0 10px 30px var(--shadow-light);
            width: 100%;
            max-width: 480px;
            text-align: center;
            position: relative;
            z-index: 1;
            opacity: 0; /* Start hidden for initial animation */
            transform: translateY(20px); /* Start slightly below for initial animation */
            animation: fadeInScale 0.8s ease-out forwards;
            overflow: hidden; /* Ensures content respects border-radius */
        }

        @keyframes fadeInScale {
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .login-container h2 {
            margin-bottom: 25px;
            color: var(--primary-color);
            font-weight: 700;
            font-size: 2.2em;
            letter-spacing: 0.5px;
        }

        .form-group {
            margin-bottom: 20px;
            text-align: left;
        }

        .form-group label {
            font-weight: 600;
            margin-bottom: 8px;
            color: var(--text-dark);
            display: block;
        }

        .form-control {
            background-color: #F8F8F8;
            border: 1px solid var(--border-light);
            color: var(--text-dark);
            padding: 12px 15px;
            border-radius: 8px;
            transition: border-color var(--transition-speed), box-shadow var(--transition-speed);
        }

        .form-control:focus {
            border-color: var(--primary-color);
            box-shadow: 0 0 0 0.2rem rgba(76, 175, 80, 0.25); /* Primary color shadow */
            outline: none;
        }

        .form-control::placeholder {
            color: var(--text-muted-light);
            opacity: 0.8;
        }

        /* Styles for the primary login button */
        .btn-primary {
            background-color: var(--primary-color);
            border-color: var(--primary-color);
            width: 100%;
            padding: 12px 20px;
            font-size: 1.15em;
            margin-top: 25px;
            border-radius: 8px;
            font-weight: 600;
            transition: background-color var(--transition-speed), transform var(--transition-speed), box-shadow var(--transition-speed);
            position: relative;
            overflow: hidden;
            z-index: 1;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1); /* Initial subtle shadow */
        }

        .btn-primary::before {
            content: '';
            position: absolute;
            top: 0;
            left: -100%;
            width: 100%;
            height: 100%;
            background: linear-gradient(120deg, transparent, rgba(255,255,255,0.3), transparent);
            transition: all 0.7s ease-in-out; /* Slower shimmer transition */
            z-index: -1;
        }

        .btn-primary:hover {
            background-color: #43A047; /* Darker green on hover */
            border-color: #43A047;
            transform: translateY(-3px); /* More pronounced lift */
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.25); /* Deeper shadow on hover */
        }

        .btn-primary:hover::before {
            left: 100%; /* Shimmer effect on hover */
        }

        .form-check-label {
            color: var(--text-muted-light);
            font-size: 0.95em;
        }

        .form-check-label a {
            color: var(--secondary-color);
            font-weight: 500;
            text-decoration: none;
            transition: color var(--transition-speed);
        }

        .form-check-label a:hover {
            color: #1976D2; /* Darker blue on hover */
            text-decoration: underline;
        }

        .alert {
            margin-top: 20px;
            padding: 12px 20px;
            border-radius: 8px;
            animation: slideInFromTop 0.5s ease-out;
            color: #FFF; /* Ensure alert text is visible on colored background */
            font-weight: 500;
        }
        /* Example: Style specific alert types if needed (adjust based on your litMessage output) */
        .alert-success { background-color: #28a745; border-color: #28a745; }
        .alert-danger { background-color: #dc3545; border-color: #dc3545; }


        @keyframes slideInFromTop {
            0% { transform: translateY(-20px); opacity: 0; }
            100% { transform: translateY(0); opacity: 1; }
        }

        /* Styles for autofill buttons (demo accounts) */
        .autofill-buttons {
            display: flex;
            justify-content: center;
            gap: 10px; /* Space between buttons */
            margin-top: 25px;
        }

        .autofill-buttons .btn-secondary { /* Target Bootstrap's btn-secondary */
            background-color: var(--secondary-color);
            border-color: var(--secondary-color);
            color: white;
            padding: 8px 18px;
            border-radius: 6px;
            font-size: 0.95em;
            transition: background-color var(--transition-speed), transform var(--transition-speed), box-shadow var(--transition-speed);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }

        .autofill-buttons .btn-secondary:hover {
            background-color: #1976D2; /* Darker blue on hover */
            border-color: #1976D2;
            transform: translateY(-2px); /* Subtle lift on hover */
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
        }

        .logo-icon {
            font-size: 3.5rem;
            color: var(--primary-color);
            margin-bottom: 25px;
            animation: bounceIn 1s ease-out;
        }

        @keyframes bounceIn {
            0%, 20%, 40%, 60%, 80%, 100% {
                transition-timing-function: cubic-bezier(0.215, 0.610, 0.355, 1.000);
            }
            0% {
                opacity: 0;
                transform: scale3d(.3, .3, .3);
            }
            20% {
                transform: scale3d(1.1, 1.1, 1.1);
            }
            40% {
                transform: scale3d(.9, .9, .9);
            }
            60% {
                opacity: 1;
                transform: scale3d(1.03, 1.03, 1.03);
            }
            80% {
                transform: scale3d(.97, .97, .97);
            }
            100% {
                opacity: 1;
                transform: scale3d(1, 1, 1);
            }
        }

        .text-muted {
            color: var(--text-muted-light) !important;
            font-size: 1.0em;
            margin-bottom: 30px;
        }
    </style>
</head>
<body>
    <form id="loginForm" runat="server" class="login-container">
        <div class="logo-icon">
            <%-- Using Font Awesome 5 for better icon --%>
            <i class="fas fa-graduation-cap"></i> 
        </div>
        <h2>Smart Campus Portal</h2>
        <p class="text-muted">Sign in to access your university dashboard</p>

        <div class="form-group">
            <label for="txtEmail">Email address</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" Placeholder="Enter email"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="txtPassword">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Password"></asp:TextBox>
        </div>
        <div class="form-group form-check text-left">
            <input type="checkbox" class="form-check-input" id="chkRememberMe">
            <label class="form-check-label" for="chkRememberMe">Remember me</label>
            <a href="#" class="float-right">Forgot your password?</a>
        </div>
        <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn btn-primary" OnClick="btnLogin_Click" />

        <div class="mt-4">
            <p class="text-muted">Demo Accounts</p>
            <div class="autofill-buttons">
                <asp:Button ID="btnStudentAutofill" runat="server" Text="Student" CssClass="btn btn-secondary" OnClientClick="autofill('student1@campus.edu', 'pass123'); return false;" />
                <asp:Button ID="btnFacultyAutofill" runat="server" Text="Faculty" CssClass="btn btn-secondary" OnClientClick="autofill('faculty1@campus.edu', 'pass123'); return false;" />
                <asp:Button ID="btnAdminAutofill" runat="server" Text="Admin" CssClass="btn btn-secondary" OnClientClick="autofill('admin1@campus.edu', 'pass123'); return false;" />
            </div>
        </div>

        <asp:Literal ID="litMessage" runat="server" EnableViewState="false"></asp:Literal>
    </form>

    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <%-- Ensure Font Awesome 5.15.4 is correctly linked for the icon --%>
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script> 
    <script>
        // This JavaScript function remains unchanged as it only interacts with HTML elements.
        function autofill(email, password) {
            $('#<%= txtEmail.ClientID %>').val(email);
            $('#<%= txtPassword.ClientID %>').val(password);
        }
    </script>
</body>
</html>