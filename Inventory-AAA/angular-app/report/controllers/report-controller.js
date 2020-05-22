angular
    .module('InventoryApp')
    .controller('ReportController', ReportController)

ReportController.$inject = ['ReportService', '$scope', '$window', '$http', 'QuickAlert'];

function ReportController(ReportService, $scope, $window, $http, QuickAlert) {
    var vm = this, controllerName = "reportCtrl";

    vm.StartDate = "";
    vm.EndDate = "";
    vm.ReportType = 0; // 0 - SalesReport; 1 - InventorySummary;
    vm.Report = {};

    vm.GenerateSalesReport = _generateSalesReport;
    vm.ResetFields = _resetFields;
    vm.Initialize = _initialize;

    function _initialize() {
        ReportService.InitializeReportPage().then(
            function (data) {
            }, function (err) {
                QuickAlert.Show({
                    type: 'error',
                    message: err
                })
            });
    }

    function _generateSalesReport() {

        var startDateString = vm.StartDate
            , endDateString = vm.EndDate
            , startDate = new Date(startDateString)
            , endDate = new Date(endDateString);

        if (dateDiffDays(startDate, endDate) > 30) {
            QuickAlert.Show({
                type: 'error',
                message: 'Date range can only be a maximum of 30 days.'
            });
        } else if ((startDate - endDate) > 0) {
            QuickAlert.Show({
                type: 'error',
                message: 'End date should be after Start Date.'
            });
        } else {
            var url = '/Report/GenerateSalesReport/?startDate=' + startDateString + '&endDate=' + endDateString;

            var a = document.createElement('a');
            a.href = url;
            a.target = '_blank';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        }
    }

    // Private Functions

    function getDateTimeFromLocal(date) {
        var roundTens = function (i) {
            return (i < 10 ? '0' : '') + i;
        }
        var YYYY = date.getFullYear(),
            MM = roundTens(date.getMonth() + 1),
            DD = roundTens(date.getDate()),
            HH = roundTens(date.getHours()),
            II = roundTens(date.getMinutes()),
            SS = roundTens(date.getSeconds());

        var test = YYYY + '-' + MM + '-' + DD + 'T' + HH + ':' + II + ':' + SS;
        return YYYY + '-' + MM + '-' + DD + 'T' + HH + ':' + II + ':' + SS;
    }

    function dateDiffDays(dt1, dt2) {
        return Math.floor((Date.UTC(dt2.getFullYear(), dt2.getMonth(), dt2.getDate()) - Date.UTC(dt1.getFullYear(), dt1.getMonth(), dt1.getDate())) / (1000 * 60 * 60 * 24));
    }

    function _resetFields() {
        vm.StartDate = "";
        vm.EndDate = "";
    }

}