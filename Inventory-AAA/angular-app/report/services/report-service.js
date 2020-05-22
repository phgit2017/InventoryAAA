angular
    .module('InventoryApp')
    .factory('ReportService', ReportService)

ReportService.$inject = ['$http', '$q', '$location'];

function ReportService($http, $q, $location) {
    var ReportServiceFactory = {},
        baseUrl = "/Report"
        //baseUrl = "/Inventory-AAA/Report"

    ReportServiceFactory.InitializeReportPage = _initializeReportPage;

    return ReportServiceFactory;

    function _initializeReportPage() {
        var url = baseUrl + "/ReportIndex",
            defer = $q.defer();

        $http.get(url).then(
            function (response) {
                if (!response.data.isSuccess) {
                    $location.url('/Unauthorized');
                }
                defer.resolve(response);
            }, function (err) {
                defer.reject(err);
            });

        return defer.promise;
    }
}