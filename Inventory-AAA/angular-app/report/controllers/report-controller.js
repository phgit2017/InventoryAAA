angular
    .module('InventoryApp')
    .controller('ReportController', ReportController)

ReportController.$inject = ['ReportService', '$scope', '$window'];

function ReportController(ReportService, $scope, $window) {
    var vm = this, controllerName = "reportCtrl";

    vm.StartDate = "";
    vm.EndDate = "";
    vm.ReportType = 0; // 0 - SalesReport; 1 - InventorySummary;
    vm.Report = {};

    vm.GenerateSalesReport = _generateSalesReport;

    function _generateSalesReport() {
        var test = vm.StartDate
        var date = getDateTimeFromLocal(vm.StartDate);
        //$window.location.assign("/Report/GenerateSalesReport?startDate=" + vm.StartDate + '&endDate=' + vm.EndDate);
        //ReportService.GenerateSalesReport(vm.StartDate, vm.EndDate).then(
        //    function (data) {
        //        vm.Report = data;
        //    }, function (err) {
        //        alert(err);
        //    }
        //);
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
}