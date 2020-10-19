angular
    .module('InventoryApp')
    .controller('SalesOrderController', SalesOrderController);

SalesOrderController.$inject = ['$scope', '$rootScope', '$location', 'SalesOrderService', 'QuickAlert'];

function SalesOrderController($scope, $rootScope, $location, SalesOrderService, QuickAlert) {

    var vm = this,
        controllerName = 'salesOrderCtrl';

    vm.FilteredSalesOrders = [];
    vm.SalesOrders = [];
    vm.SalesOrdersLoading = false;
    vm.SearchSalesOrdersInput = "";

    vm.Initialize = function() {
        getSalesOrders();
    }

    vm.ManageSalesOrder = function(salesOrderId) {
        $location.url('/OrderDetails/' + salesOrderId);
    }

    getSalesOrders = function() {
        vm.SalesOrdersLoading = false;
        SalesOrderService.GetSalesOrders().then(
            function(data) {
                vm.SalesOrdersLoading = false;
                vm.SalesOrders = data.result;
                vm.FilteredSalesOrders = vm.SalesOrders;
                debugger;
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

    function isNullOrEmpty(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }

}