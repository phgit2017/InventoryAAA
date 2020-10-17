angular
    .module('InventoryApp')
    .factory('SalesOrderService', SalesOrderService)

SalesOrderService.$inject = ['$q', '$http', '$location', 'globalBaseUrl'];

function SalesOrderService($q, $http, $location, globalBaseUrl) {
    var SalesOrderServiceFactory = {},
        baseUrl = globalBaseUrl + "/Maintenance";

    SalesOrderServiceFactory.GetCustomerList = _getCustomerList;

    return SalesOrderServiceFactory;

    function _getCustomerList() {
        var defer = $q.defer(),
            url = baseUrl + '/CustomerList';
        $http.get(url)
            .then(function(response) {
                if (!response.data.isSuccess) {
                    $location.url('/Unauthorized');
                }
                defer.resolve(response.data)
            }),
            function(err) {

                defer.reject(err);
            }
        return defer.promise;
    }
}