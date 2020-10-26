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
    vm.OrderDetailsLoading = true;
    vm.ProductListLoading = true;
    vm.CustomerFilter = '';
    vm.CustomerListShown = false;
    vm.CustomerList = [];
    vm.CustomerSearchInput = '';
    vm.SearchProductList = '';
    vm.SearchProductsInOrder = '';
    vm.SelectedCustomer = '';
    vm.SelectedCustomerLabel = '';
    vm.PriceTypes = ['Big Buyer', 'Reseller', 'Retailer']
    vm.PriceTypesShown = false;
    vm.SelectedPriceType = '';

    vm.Initialize = function() {
        getCustomerList();
        vm.SalesOrderId = parseFloat($routeParams.salesOrderId);

        if (vm.SalesOrderId !== 0) {
            getOrderDetails();
        } else {
            getProductList();
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
        if (vm.SelectedPriceType === '') {
            QuickAlert.Show({
                type: 'error',
                message: 'Please select a Price Type before adding a product.'
            });
            return;
        }

        product.Quantity = 0;
        vm.ProductsInOrder.push(product);
        selectedProductIndex = vm.ProductList.indexOf(product);
        vm.ProductList.splice(selectedProductIndex, 1);
    }

    vm.RemoveProductFromOrder = function(product) {
        selectedProductIndex = vm.ProductsInOrder.indexOf(product);
        vm.ProductsInOrder.splice(selectedProductIndex, 1);
        getProductList();
    }

    vm.SubmitOrder = function() {
        let errorMsg = '',
            isBreak = false;

        if (vm.SelectedCustomer === '') {
            errorMsg = 'Please select a Customer.';
        }

        vm.ProductsInOrder.forEach(x => {
            if (x.Quantity === 0 && !isBreak) {
                errorMsg = 'Please input quantity for all products before saving.';
                isBreak = true;
            }
        });

        if (errorMsg === '') {
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
        } else {
            QuickAlert.Show({
                type: 'error',
                message: errorMsg
            });
        }

    }

    getOrderDetails = function() {
        vm.OrderDetailsLoading = true;
        SalesOrderService.SalesOrderDetails(vm.SalesOrderId).then(
            function(data) {
                vm.OrderDetails = data.result
                vm.SalesDetails = vm.OrderDetails.SalesDetails;
                vm.SelectCustomer(vm.OrderDetails.CustomerDetails);
                vm.ProductsInOrder = vm.OrderDetails.ProductList;
                debugger;
                getProductList();
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
        CustomerService.GetCustomerList().then(
            function(data) {
                vm.CustomerList = data.CustomerDetailsResult;
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
                let _productsInOrder = vm.ProductsInOrder.map(x => x.ProductId);
                vm.ProductList = vm.ProductList.filter(x => {
                    return _productsInOrder.includes(x.ProductId) ? false : true;
                });

                vm.ProductListLoading = false;
                vm.OrderDetailsLoading = false;
            },
            function(error) {
                vm.ProductListLoading = false;
                QuickAlert.Show({
                    type: 'error',
                    message: 'Error fetching Product List from Server.'
                });
            });
    }

    isNullOrEmpty = function(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }
}