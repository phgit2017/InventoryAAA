angular
    .module('InventoryApp')
    .factory('ReportService', ReportService)

ReportService.$inject = ['$http', '$q', '$location', 'globalBaseUrl'];

function ReportService($http, $q, $location, globalBaseUrl) {
    var ReportServiceFactory = {},
        baseUrl = globalBaseUrl + "/Report"

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