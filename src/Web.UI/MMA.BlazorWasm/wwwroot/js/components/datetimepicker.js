$(document).ready(function () {
    window.initDateTimePicker = (type, start, end, parentElementClassName, _inputId, dotNetHelper) => {
        if (type === "DateOnly") {
            $(`#${_inputId}`).daterangepicker({
                timePicker: false,
                timePickerSeconds: false,
                autoApply: false,
                showDropdowns: true,
                autoUpdateInput: (start && end) ? true : false,
                locale: {
                    format: 'DD/MM/YYYY',
                    separator: ' - ',
                    applyLabel: 'Apply',
                    cancelLabel: 'Cancel',
                    fromLabel: 'From',
                    toLabel: 'To',
                    customRangeLabel: 'Custom',
                    weekLabel: 'W',
                    daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
                    monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                    firstDay: 1
                },
                linkedCalendars: false,
                showCustomRangeLabel: false,
                startDate: start ? moment(start, 'DD/MM/YYYY') : undefined,
                endDate: end ? moment(end, 'DD/MM/YYYY') : undefined,
                parentEl: parentElementClassName,
            }).on('apply.daterangepicker', function (ev, picker) {
                $(`#${_inputId}`).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
                dotNetHelper.invokeMethodAsync('UpdateDate', picker.startDate.format('DD/MM/YYYY'), picker.endDate.format('DD/MM/YYYY'));
            }).on('show.daterangepicker', function (ev, picker) {
                picker.container.find('.drp-calendar.right').hide();

                picker.container.find('.drp-calendar.left').css({
                    'width': '100%',
                    'max-width': '100%',
                    'flex': '1',
                });

                const inputWidth = $(".date-time-range").outerWidth();
                picker.container.css({
                    'width': inputWidth * 0.9 + 'px',
                    'min-width': inputWidth * 0.8 + 'px'
                });
            });
        }
        else if (type === "TimeOnly") {
            $(`#${_inputId}`).daterangepicker({
                timePicker: true,
                timePicker24Hour: true,
                timePickerIncrement: 1,
                timePickerSeconds: false,
                locale: { format: 'HH:mm' }
            }).on('show.daterangepicker', function () {
                $('.calendar-table').hide();
            }).on('hide.daterangepicker', function () {
                $('.calendar-table').show();
            }).on('apply.daterangepicker', function (ev, picker) {
                dotNetHelper.invokeMethodAsync('UpdateTime', picker.startDate.format('HH:mm'), picker.endDate.format('HH:mm'));
            });
        }
        else if (type === "DateTime") {
            $(`#${_inputId}`).daterangepicker({
                timePicker: true,
                timePickerSeconds: true,
                autoApply: false,
                showDropdowns: true,
                autoUpdateInput: (start && end) ? true : false,
                locale: {
                    format: 'DD/MM/YYYY HH:mm',
                    separator: ' - ',
                    applyLabel: 'Apply',
                    cancelLabel: 'Cancel',
                    fromLabel: 'From',
                    toLabel: 'To',
                    customRangeLabel: 'Custom',
                    weekLabel: 'W',
                    daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
                    monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                    firstDay: 1
                },
                linkedCalendars: false,
                showCustomRangeLabel: false,
                startDate: start ? moment(start, 'DD/MM/YYYY HH:mm') : undefined,
                endDate: end ? moment(end, 'DD/MM/YYYY HH:mm') : undefined,
                parentEl: parentElementClassName,
            }, function (start, end, label) {
                $(`#${_inputId}`).val(start.format('DD/MM/YYYY HH:mm') + ' - ' + end.format('DD/MM/YYYY HH:mm'));
                dotNetHelper.invokeMethodAsync('UpdateDateTime', start.format('DD/MM/YYYY HH:mm'), end.format('DD/MM/YYYY HH:mm'));
            }).on('show.daterangepicker', function (ev, picker) {
                const container = picker.container;
                container.find('.drp-calendar.right .calendar-table').hide();

                // Adjust the left calendar to take full width
                picker.container.find('.drp-calendar.left').css({
                    'width': '100%',
                    'max-width': '100%',
                    'flex': '1',
                });

                picker.container.find('.drp-calendar.right').css({
                    'width': '100%',
                    'max-width': '100%',
                    'flex': '1',
                });

                // Ensure the time picker is centered and properly spaced
                picker.container.find('.calendar-time').css({
                    'display': 'flex',
                    'justify-content': 'center',
                    'align-items': 'center',
                    'gap': '8px',
                    'margin': '10px 0',
                });

                // Adjust the overall container width based on input
                const inputWidth = $(".date-time-range").outerWidth();
                picker.container.css({
                    'width': inputWidth * 0.9 + 'px',
                    'min-width': inputWidth * 0.8 + 'px'
                });
            });
        }
    };

    // Các hàm clear
    window.clearDatePicker = (_inputId) => {
        $(`#${_inputId}`).val('');
    };

    window.clearTimePicker = (_inputId) => {
        $(`#${_inputId}`).val('');
    };

    window.clearDateTimePicker = (_inputId) => {
        $(`#${_inputId}`).val('');
    };
});
