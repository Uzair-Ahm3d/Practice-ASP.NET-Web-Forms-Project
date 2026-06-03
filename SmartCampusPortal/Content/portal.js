/* Smart Campus Portal — shared UI behaviour
   Adds a mobile hamburger to open/close the sidebar drawer and
   small professional touches. Safe no-op on pages without a sidebar. */
(function () {
    function init() {
        var sidebar = document.querySelector('.sidebar');
        var navbar = document.querySelector('.navbar');

        if (sidebar && navbar) {
            var toggle = document.createElement('button');
            toggle.type = 'button';
            toggle.className = 'scp-sidebar-toggle';
            toggle.setAttribute('aria-label', 'Toggle menu');
            toggle.innerHTML = '<i class="fas fa-bars"></i>';
            toggle.addEventListener('click', function (e) {
                e.preventDefault();
                document.body.classList.toggle('sidebar-open');
            });
            // place it at the very start of the navbar
            navbar.insertBefore(toggle, navbar.firstChild);

            // close the drawer after choosing a link, or when tapping the backdrop
            sidebar.querySelectorAll('.nav-link').forEach(function (a) {
                a.addEventListener('click', function () { document.body.classList.remove('sidebar-open'); });
            });
            document.addEventListener('click', function (e) {
                if (document.body.classList.contains('sidebar-open') &&
                    !sidebar.contains(e.target) && !toggle.contains(e.target)) {
                    document.body.classList.remove('sidebar-open');
                }
            });
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
