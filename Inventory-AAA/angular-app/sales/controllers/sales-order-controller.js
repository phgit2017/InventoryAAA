angular
    .module('InventoryApp')
    .controller('SalesOrderController', SalesOrderController);

SalesOrderController.$inject = ['$scope', '$rootScope', '$location', 'SalesOrderService', 'QuickAlert'];

function SalesOrderController($scope, $rootScope, $location, SalesOrderService, QuickAlert) {

    var vm = this,
        controllerName = 'salesOrderCtrl';

    vm.FilteredSalesOrders = [];
    vm.SalesOrders = [];
    vm.SalesOrdersLoading = true;
    vm.SearchSalesOrdersInput = "";

    vm.Initialize = function() {
        getSalesOrders();
    }

    vm.ManageSalesOrder = function(salesOrderId) {
        $location.url('/OrderDetails/' + salesOrderId);
    }

    getSalesOrders = function() {
        vm.SalesOrdersLoading = true;
        SalesOrderService.GetSalesOrders().then(
            function(data) {
                vm.SalesOrders = data.result;
                vm.FilteredSalesOrders = vm.SalesOrders;
                vm.SalesOrdersLoading = false;
            },
            function(error) {
                vm.SalesOrdersLoading = false;
                QuickAlert.Show({
                    type: 'error',
                    message: error
                });
            },
        )
    }

    vm.GetStatusClass = function(statusId) {
        return {
            'fab-pending': statusId === 0 || statusId === 1,
            'fab-success': statusId === 2 || statusId === 3 || statusId === 4,
            'fab-danger': statusId === 5
        }
    }

    function isNullOrEmpty(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }

}