angular
    .module('InventoryApp')
    .controller('OrderDetailsController', OrderDetailsController);

OrderDetailsController.$inject = ['$scope', '$rootScope', '$routeParams', 'SalesOrderService', 'CustomerService', 'InventoryService', 'QuickAlert'];

function OrderDetailsController($scope, $rootScope, $routeParams, SalesOrderService, CustomerService, InventoryService, QuickAlert) {

    var vm = this,
        controllerName = 'orderDetailsCtrl';

    vm.SalesOrderId = 0;
    vm.OrderDetails = {};
    vm.SalesDetails = {};
    vm.ProductList = [];
    vm.ProductsInOrder = [];
    vm.OrderDetailsLoading = false;
    vm.ProductListLoading = false;
    vm.CustomerFilter = '';
    vm.CustomerListShown = false;
    vm.CustomerList = [];
    vm.CustomerSearchInput = '';
    vm.SelectedCustomer = {};
    vm.SelectedCustomerLabel = '';
    vm.PriceTypes = ['Big Buyer', 'Reseller', 'Retailer']
    vm.PriceTypesShown = false;
    vm.SelectedPriceType = '';

    vm.Initialize = function() {
        getCustomerList();
        getProductList();
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

    vm.SelectCustomer = function(customer) {
        vm.SelectedCustomer = customer;
        vm.SelectedCustomerLabel = customer.CustomerCode + ' - ' + customer.FullName;
        vm.CustomerListShown = false;
    }

    vm.SelectPriceType = function(priceType) {
        vm.SelectedPriceType = priceType;
        vm.PriceTypesShown = false;
    }

    vm.AddProductToOrder = function(product) {
        var selectedProduct;
        vm.ProductsInOrder.push(product);
        selectedProductIndex = vm.ProductList.indexOf(product);
        vm.ProductList.splice(selectedProductIndex, 1);
    }

    vm.SubmitOrder = function() {
        debugger;
        var salesOrderRequest = {
            OrderTransactionType: 1,
            CustomerId: vm.SelectedCustomer.CustomerId,
            SalesOrderId: vm.SalesOrderId,
            SalesOrderStatusId: 1,
            SalesNo: 0,
            SalesOrderProductDetailRequest: vm.ProductsInOrder
        }
        SalesOrderService.SubmitOrder(salesOrderRequest).then(
            function(data) {
                QuickAlert.Show({
                    type: 'success',
                    message: 'Order has been placed'
                });
            },
            function(error) {
                QuickAlert.Show({
                    type: 'error',
                    message: error
                });
            }
        )
    }

    getOrderDetails = function() {
        vm.OrderDetailsLoading = true;
        SalesOrderService.SalesOrderDetails(vm.SalesOrderId).then(
            function(data) {
                vm.OrderDetailsLoading = false;
                vm.OrderDetails = data.result
                vm.SalesDetails = vm.OrderDetails.SalesDetails;
                vm.SelectCustomer(vm.OrderDetails.CustomerDetails);
                vm.ProductsInOrder = vm.OrderDetails.ProductList;
            },
            function(error) {
                QuickAlert.Show({
                    type: 'error',
                    message: error
                });
            }
        );
    }

    getCustomerList = function() {
        vm.OrderDetailsLoading = true;
        CustomerService.GetCustomerList().then(
            function(data) {
                vm.CustomerList = data.CustomerDetailsResult;
                vm.OrderDetailsLoading = false;
            },
            function(error) {
                alert(error);
                QuickAlert.Show({
                    type: 'error',
                    message: 'Error fetching Customer List from Server.'
                });
            }
        );
    }

    getProductList = function() {
        vm.ProductListLoading = true;
        InventoryService.GetInventorySummary().then(
            function(data) {
                vm.ProductList = data;
                vm.ProductListLoading = false;
            },
            function(error) {
                vm.ProductListLoading = false;
                QuickAlert.Show({
                    type: 'error',
                    message: 'Error fetching Product List from Server.'
                });
            });
    }
}