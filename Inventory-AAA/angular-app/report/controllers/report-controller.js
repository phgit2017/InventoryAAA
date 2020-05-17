angular
    .module('InventoryApp')
    .controller('ReportController', ReportController)

ReportController.$inject = ['ReportService', '$scope'];

function ReportController(ReportService, $scope) {
    var vm = this, controllerName = "reportCtrl";

    vm.StartDate = "";
    vm.EndDate = "";
    vm.ReportType = 0; // 0 - SalesReport; 1 - InventorySummary;
    vm.Report = {};

    vm.GenerateSalesReport = _generateSalesReport;

    function _generateSalesReport() {
        ReportService.GenerateSalesReport(vm.StartDate, vm.EndDate).then(
            function (data) {
                vm.Report = data;
            }, function (err) {
                alert(err);
            }
        );
    }
}