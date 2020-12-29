angular
    .module('InventoryApp')
    .factory('CommonService', CommonService)

CommonService.$inject = ['$q', '$http', '$location', 'globalBaseUrl'];

function CommonService($q, $http, $location, globalBaseUrl) {
    var CommonServiceFactory = {},
        baseUrl = globalBaseUrl + "/Common";

    CommonServiceFactory.GetCustomerStatusList = _getCustomerStatusList;
    CommonServiceFactory.GetProductList = _getProductList;
    CommonServiceFactory.GetCustomerList = _getCustomerList;

    return CommonServiceFactory;

    function _getCustomerStatusList() {
        var defer = $q.defer(),
            url = baseUrl + '/CustomerStatusList';
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

    function _getProductList() {
        var defer = $q.defer(),
            url = baseUrl + '/ProductList';
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