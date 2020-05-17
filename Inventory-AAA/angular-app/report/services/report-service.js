angular
    .module('InventoryApp')
    .factory('ReportService', ReportService)

ReportService.$inject = ['$http', '$q'];

function ReportService($http, $q) {
    var ReportServiceFactory = {},
        baseUrl = "/Report"
        //baseUrl = "/Inventory-AAA/Report"

    ReportServiceFactory.GenerateSalesReport = _generateSalesReport;

    return ReportServiceFactory;

    function _generateSalesReport(startDate, endDate) {
        var url = "/GenerateSalesReport",
            defer = $q.defer;

        $http.post(url, startDate, endDate).then(
            function (result) {
                defer.resolve(result);
            }, function (err) {
                defer.reject(err);
            });

        return defer.promise;
    }
}