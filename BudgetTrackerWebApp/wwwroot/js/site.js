document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggler = document.querySelector('.sidebar-toggler');

    if (sidebarToggler) { // Check if the toggle button exists
        sidebarToggler.addEventListener('click', function () {
            sidebar.classList.toggle('collapsed'); // Toggle 'collapsed' class on sidebar
            if (sidebar.classList.contains('collapsed')) {
                sidebarToggler.innerHTML = '<i class="bi bi-chevron-right"></i>'; // Change icon to right chevron when collapsed
            } else {
                sidebarToggler.innerHTML = '<i class="bi bi-chevron-left"></i>';  // Change icon back to left chevron when expanded
            }
        });
    }
});