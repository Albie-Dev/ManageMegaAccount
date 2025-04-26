window.sidebarManager = {
    init: function () {
        const sidebar = document.getElementById('sidebar');
        const toggleBtn = document.getElementById('toggleSidebar');

        toggleBtn?.addEventListener('click', () => {
            sidebar.classList.toggle('collapsed');

            if (sidebar.classList.contains('collapsed')) {
                document.querySelectorAll('.collapse.show').forEach(c => new bootstrap.Collapse(c, { toggle: true }));
                document.querySelectorAll('.nav-link[data-bs-toggle]').forEach(link => link.classList.add('collapsed'));
            }
        });
        
        const navLinks = document.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.addEventListener('click', function (e) {
                navLinks.forEach(l => l.classList.remove('active'));
                this.classList.add('active');

                if (sidebar.classList.contains('collapsed')) {
                    sidebar.classList.remove('collapsed');
                }
            });
        });

        document.querySelectorAll('.nav-link[data-bs-toggle="collapse"]').forEach(link => {
            const collapseTarget = document.querySelector(link.getAttribute('href'));
            collapseTarget?.addEventListener('show.bs.collapse', () => link.classList.remove('collapsed'));
            collapseTarget?.addEventListener('hide.bs.collapse', () => link.classList.add('collapsed'));
        });
    }
};