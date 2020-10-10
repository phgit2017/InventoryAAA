angular
    .module('InventoryApp')
    .factory('CustomerService', CustomerService)

CustomerService.$inject = ['$q', '$http', '$location', 'globalBaseUrl'];

function CustomerService($q, $http, $location, globalBaseUrl) {
    var CustomerServiceFactory = {},
        baseUrl = globalBaseUrl + "/Maintenance";

    CustomerServiceFactory.GetCustomerList = _getCustomerList;

    return CustomerServiceFactory;

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