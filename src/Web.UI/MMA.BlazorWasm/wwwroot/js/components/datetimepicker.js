$(document).ready(function () {
    window.initDateTimePicker = (type, dotNetHelper) => {
        console.log(type);

        if (type === "DateOnly") {
            $('#dateRange').daterangepicker({
                opens: 'left',
                locale: { format: 'YYYY/MM/DD' }
            }).on('apply.daterangepicker', function (ev, picker) {
                dotNetHelper.invokeMethodAsync('UpdateDate', picker.startDate.format('YYYY/MM/DD'), picker.endDate.format('YYYY/MM/DD'));
            });
        }
        else if (type === "TimeOnly") {
            $('#timeRange').daterangepicker({
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
            $('#dateTimeRange').daterangepicker({
                timePicker: true,
                timePicker24Hour: true,
                timePickerIncrement: 1,
                timePickerSeconds: false,
                showDropdowns: true,
                opens: 'left',
                ranges: {
                    'Today': [moment().startOf('day'), moment().endOf('day')],
                    'Yesterday': [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days').endOf('day')],
                    'Last 7 Days': [moment().subtract(6, 'days').startOf('day'), moment().endOf('day')],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                },
                locale: {
                    format: 'YYYY/MM/DD HH:mm',
                    separator: ' - ',
                    applyLabel: 'Apply',
                    cancelLabel: 'Cancel',
                    fromLabel: 'From',
                    toLabel: 'To',
                    customRangeLabel: 'Custom Range',
                    weekLabel: 'W',
                    daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
                    monthNames: [
                        'January', 'February', 'March', 'April', 'May', 'June',
                        'July', 'August', 'September', 'October', 'November', 'December'
                    ],
                    firstDay: 1
                },
                startDate: moment().startOf('day'),
                endDate: moment().endOf('day'),
                minDate: moment().subtract(2, 'years'),
                maxDate: moment().add(2, 'years')
            }).on('apply.daterangepicker', function (ev, picker) {
                dotNetHelper.invokeMethodAsync('UpdateDateTime', picker.startDate.format('YYYY/MM/DD HH:mm'), picker.endDate.format('YYYY/MM/DD HH:mm'));
            });
        }
    };

    // Các hàm clear
    window.clearDatePicker = () => {
        $('#dateRange').val('');
        $('#dateRange').data('daterangepicker').setStartDate(moment().startOf('day'));
        $('#dateRange').data('daterangepicker').setEndDate(moment().startOf('day'));
    };

    window.clearTimePicker = () => {
        $('#timeRange').val('');
        $('#timeRange').data('daterangepicker').setStartDate(moment().startOf('day'));
        $('#timeRange').data('daterangepicker').setEndDate(moment().startOf('day'));
    };

    window.clearDateTimePicker = () => {
        $('#dateTimeRange').val('');
        $('#dateTimeRange').data('daterangepicker').setStartDate(moment().startOf('day'));
        $('#dateTimeRange').data('daterangepicker').setEndDate(moment().startOf('day'));
    };
});
