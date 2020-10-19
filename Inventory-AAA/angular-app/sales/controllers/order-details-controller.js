angular
    .module('InventoryApp')
    .controller('OrderDetailsController', OrderDetailsController);

OrderDetailsController.$inject = ['$scope', '$rootScope', '$routeParams', 'SalesOrderService', 'QuickAlert'];

function OrderDetailsController($scope, $rootScope, $routeParams, SalesOrderService, QuickAlert) {

    var vm = this,
        controllerName = 'orderDetailsCtrl';

    vm.SalesOrderId = 0;
    vm.OrderDetails = {};
    vm.SalesOrder = {};
    vm.Products = {};
    vm.OrderDetailsLoading = false;


    vm.Initialize = function() {
        vm.SalesOrderId = parseFloat($routeParams.salesOrderId);

        if (vm.SalesOrderId !== 0) {
            getOrderDetails();
        }

        function isNullOrEmpty(data) {
            if (data === "" || data === undefined || data === null) {
                return true;
            } else {
                return false;
            }
        }
    }

    getOrderDetails = function() {
        vm.OrderDetailsLoading = true;
        SalesOrderService.SalesOrderDetails(vm.SalesOrderId).then(
            function(data) {
                vm.OrderDetailsLoading = false;
                vm.OrderDetails = data.result
                vm.SalesOrder = vm.OrderDetails[0].SalesOrders;
                vm.Products = vm.OrderDetails[0].ProductDetails;
                debugger;
            },
            function(error) {
                QuickAlert.Show({
                    type: 'error',
                    message: error
                });
            }
        );
    }
}