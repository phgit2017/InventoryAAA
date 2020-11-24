angular
    .module('InventoryApp')
    .factory('SalesOrderService', SalesOrderService)

SalesOrderService.$inject = ['$q', '$http', '$location', 'globalBaseUrl'];

function SalesOrderService($q, $http, $location, globalBaseUrl) {
    var SalesOrderServiceFactory = {},
        baseUrl = globalBaseUrl + "/Stocks";

    SalesOrderServiceFactory.GetSalesOrders = _getSalesOrders;
    SalesOrderServiceFactory.GetAllSalesOrders = _getAllSalesOrders;
    SalesOrderServiceFactory.SalesOrderDetails = _salesOrderDetails;
    SalesOrderServiceFactory.SubmitOrder = _submitOrder;
    SalesOrderServiceFactory.GetOrderReceipt = _getOrderReceipt;

    return SalesOrderServiceFactory;

    function _getSalesOrders() {
        var defer = $q.defer(),
            url = baseUrl + '/SalesOrders';
        $http.post(url)
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

    function _getAllSalesOrders(data) {
        var defer = $q.defer(),
            url = baseUrl + '/SalesOrdersTransactionHistory';
        $http.post(url, data)
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

    function _submitOrder(data) {
        var defer = $q.defer(),
            url = baseUrl + '/UpdateInventorySalesOrder';
        $http.post(url, data)
            .then(function(response) {
                defer.resolve(response.data)
            }),
            function(err) {
                defer.reject(err);
            }
        return defer.promise;
    }

    function _salesOrderDetails(salesOrderId) {
        var defer = $q.defer(),
            url = baseUrl + '/SalesOrderDetails?salesOrderId=' + salesOrderId;
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

    function _getOrderReceipt(salesOrderId) {
        var defer = $q.defer(),
            url = baseUrl + '/GetSalesOrderReceipt?salesOrderId=' + salesOrderId;
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