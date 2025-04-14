function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const sidebarDiv = document.getElementById('sidebarDiv');
    const toggleButton = document.getElementById('sidebarToggleBtn');
    const toggleIcon = document.getElementById('sidebarToggleIcon');
    sidebar.classList.toggle('collapsed');
    sidebarDiv.classList.toggle('sidebar-collapsed');

    if (sidebar.classList.contains('collapsed')) {
        toggleIcon.classList.remove('bi-chevron-left');
        toggleIcon.classList.add('bi-chevron-right');
    } else {
        toggleIcon.classList.remove('bi-chevron-right');
        toggleIcon.classList.add('bi-chevron-left');
    }
    toggleButton.style.left = sidebar.classList.contains('collapsed') ? '60px' : '250px';
}

function toggleSubMenu(submenuId) {
    const submenu = document.getElementById(submenuId);
    submenu.classList.toggle('show');
}

function setActiveSidebarItem() {
    const sidebarItems = document.querySelectorAll('.sidebar-item');
    sidebarItems.forEach(item => {
        item.addEventListener('click', function () {
            sidebarItems.forEach(i => i.classList.remove('active'));
            this.classList.add('active');
        });
    });
}

window.addStar = () => {
    var myModal = new bootstrap.Modal(document.getElementById('loadingModal'), {
        backdrop: 'static',
        keyboard: false
    });
    myModal.show();
};

$(document).ready(function () {
    setTimeout(function () {
        $(".error-message-display").fadeOut("slow", function () {
            $(this).remove();
        });
    }, 8000);
});