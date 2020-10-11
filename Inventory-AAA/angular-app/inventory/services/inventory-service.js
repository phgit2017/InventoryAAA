angular
    .module('InventoryApp')
    .factory('InventoryService', InventoryService);

InventoryService.$inject = ['$http', '$q', '$location', 'globalBaseUrl'];

function InventoryService($http, $q, $location, globalBaseUrl) {
    var InventoryServiceFactory = {},
        baseUrl = globalBaseUrl + '/Stocks'

    InventoryServiceFactory.GetInventorySummary = _getInventorySummary;
    InventoryServiceFactory.GetProductDetails = _getProductDetails;
    InventoryServiceFactory.GetProductDetailsBasic = _getProductDetailsBasic;
    InventoryServiceFactory.SaveOrderRequest = _saveOrderRequest;
    InventoryServiceFactory.UpdateProductDetails = _updateProductDetails;

    return InventoryServiceFactory;

    function _getInventorySummary() {
        var defer = $q.defer(),
            url = baseUrl + '/InventorySummary';

        $http.get(url)
            .then(function(response) {
                if (!response.data.isSuccess) {
                    $location.url('/Unauthorized');
                }
                defer.resolve(response.data.result);
            }, function(err) {
                defer.reject(err);
            });
        return defer.promise;
    }

    function _getProductDetails(data) {
        var defer = $q.defer(),
            url = baseUrl + '/InventoryDetails';
        $http.post(url, data)
            .then(function(response) {
                defer.resolve(response.data);
            }, function(err) {
                defer.reject(err);
            });
        return defer.promise;
    }

    function _getProductDetailsBasic(productId) {
        var defer = $q.defer(),
            url = baseUrl + '/ProductDetails?productId=' + productId;
        $http.get(url)
            .then(function(response) {
                defer.resolve(response.data);
            }, function(err) {
                defer.reject(err);
            });
        return defer.promise;
    }

    function _saveOrderRequest(data) {
        var defer = $q.defer(),
            url = baseUrl + '/UpdateInventoryOrder';
        $http.post(url, data)
            .then(function(response) {
                defer.resolve(response.data);
            }, function(err) {
                defer.reject(err);
            });
        return defer.promise;
    }

    function _updateProductDetails(data) {
        var defer = $q.defer(),
            url = baseUrl + '/UpdateProductDetails';
        $http.post(url, data)
            .then(function(response) {
                defer.resolve(response.data);
            }, function(err) {
                defer.reject(err);
            });
        return defer.promise;
    }
}