window.setupDropdownCloseHandler = (dotNetHelper) => {
    document.addEventListener('click', function (e) {
        const dropdowns = document.querySelectorAll('.dropdown-container');
        let isClickInside = false;

        dropdowns.forEach(dropdown => {
            if (dropdown.contains(e.target)) {
                isClickInside = true;
            }
        });

        if (!isClickInside) {
            dotNetHelper.invokeMethodAsync('CloseDropdown');
        }
    });
}