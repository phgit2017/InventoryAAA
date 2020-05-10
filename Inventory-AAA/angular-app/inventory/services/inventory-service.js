angular
    .module('InventoryApp')
    .factory('InventoryService', InventoryService);

InventoryService.$inject = ['$http', '$q'];

function InventoryService($http, $q) {
    var InventoryServiceFactory = {},
        baseUrl = '/Stocks'

    InventoryServiceFactory.GetInventorySummary = _getInventorySummary;
    InventoryServiceFactory.GetProductDetails = _getProductDetails;
    InventoryServiceFactory.SaveOrderRequest = _saveOrderRequest;

    return InventoryServiceFactory;

    function _getInventorySummary() {
        var defer = $q.defer(),
            url = baseUrl + '/InventorySummary';

        $http.get(url)
            .then(function (response) {
                defer.resolve(response.data.result);
            }, function (err) {
                defer.reject(err);
            });
        return defer.promise;
    }

    function _getProductDetails() {
        var defer = $q.defer(),
            url = baseUrl + '/InventoryDetails';

        $http.get(url)
            .then(function (response) {
                defer.resolve(response.data.result);
            }, function (err) {
                    defer.reject(err);
            });
        return defer.promise;
    }

    function _getProductDetails() {
        var defer = $q.defer(),
            url = baseUrl + '/InventoryDetails';

        $http.get(url)
            .then(function (response) {
                defer.resolve(response.data.result);
            }, function (err) {
                defer.reject(err);
            });
        return defer.promise;
    }

    function _saveOrderRequest(data) {
        var defer = $q.defer(),
            url = baseUrl + '/UpdateInventoryOrder';

        $http.post(url, data)
            .then(function (response) {
                debugger;
                defer.resolve(response.data.result);
            }, function (err) {
                defer.reject(err);
            });
        return defer.promise;
    }
}