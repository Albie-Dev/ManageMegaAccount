window.sidebarManager = {
    init: function () {
        const sidebar = document.getElementById('sidebar');
        const toggleBtn = document.getElementById('toggleSidebar');

        toggleBtn?.addEventListener('click', () => {
            const isCollapsed = sidebar.classList.toggle('collapsed');

            if (isCollapsed) {
                toggleBtn.classList.remove('bi-chevron-left');
                toggleBtn.classList.add('bi-chevron-right');

                document.querySelectorAll('.collapse.show').forEach(c => {
                    new bootstrap.Collapse(c, { toggle: true });
                });

                document.querySelectorAll('.nav-link[data-bs-toggle]').forEach(link => {
                    link.classList.add('collapsed');
                });

                document.querySelectorAll('.toggle-icon').forEach(icon => {
                    icon.classList.add('d-none');
                });

            } else {
                toggleBtn.classList.remove('bi-chevron-right');
                toggleBtn.classList.add('bi-chevron-left');

                document.querySelectorAll('.toggle-icon').forEach(icon => {
                    icon.classList.remove('d-none');
                });
            }
        });

        const navLinks = document.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.addEventListener('click', function (e) {
                navLinks.forEach(l => l.classList.remove('active'));
                this.classList.add('active');

                if (sidebar.classList.contains('collapsed')) {
                    sidebar.classList.remove('collapsed');
                    document.querySelectorAll('.toggle-icon').forEach(icon => {
                        icon.classList.remove('d-none');
                    });
                    toggleBtn.classList.remove('bi-chevron-right');
                    toggleBtn.classList.add('bi-chevron-left');
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