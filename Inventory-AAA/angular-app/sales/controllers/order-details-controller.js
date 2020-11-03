angular
    .module('InventoryApp')
    .controller('OrderDetailsController', OrderDetailsController);

OrderDetailsController.$inject = ['$scope', '$route', '$location', '$routeParams', 'MaintenanceService', 'SalesOrderService', 'CustomerService', 'InventoryService', 'QuickAlert'];

function OrderDetailsController($scope, $route, $location, $routeParams, MaintenanceService, SalesOrderService, CustomerService, InventoryService, QuickAlert) {

    var vm = this,
        controllerName = 'orderDetailsCtrl';

    vm.SalesOrderId = 0;
    vm.OrderDetails = {};
    vm.SalesDetails = '';
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
    vm.ModeOfPayment = 'N/A';
    vm.ShippingFee = 0;
    vm.SalesOrderStatusId = 0;
    vm.FilterCategoryId = '';
    vm.ShowConfirmAlert = false;
    vm.AlertMessage = '';
    vm.ReceiptShown = false;
    vm.ReceiptDetails;


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
        let priceTypeId;

        if (vm.SelectedPriceType === '') {
            QuickAlert.Show({
                type: 'error',
                message: 'Please select a Price Type before adding a product.'
            });
            return;
        }

        product.Quantity = 0;
        switch (vm.SelectedPriceType) {
            case "Big Buyer":
                product.UnitPrice = product.BigBuyerPrice;
                product.PriceTypeId = 1;
                break;
            case "Reseller":
                product.UnitPrice = product.ResellerPrice;
                product.PriceTypeId = 2;
                break;
            case "Retailer":
                product.UnitPrice = product.RetailerPrice;
                product.PriceTypeId = 3;
                break;
        }
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
            saveOrder();
        } else {
            QuickAlert.Show({
                type: 'error',
                message: errorMsg
            });
        }
    }

    vm.CancelOrder = function() {
        saveOrder('Cancel');
    }

    vm.CancelAction = function() {
        return;
    }

    vm.GetOrderReceipt = function() {
        vm.ReceiptShown = true;
        SalesOrderService.GetOrderReceipt(vm.SalesOrderId).then(
            function(data) {
                vm.ReceiptDetails = data.result;
            },
            function(error) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'Error fetching Receipt from Server.'
                });
            }
        )
    }

    vm.GetStatusClass = function() {
        return {
            'fab-pending': vm.SalesOrderStatusId === 0 || vm.SalesOrderStatusId === 1,
            'fab-success': vm.SalesOrderStatusId === 2 || vm.SalesOrderStatusId === 3 || vm.SalesOrderStatusId === 4,
            'fab-danger': vm.SalesOrderStatusId === 5
        }
    }

    vm.SetQuantity = function(i) {
        if (i.Quantity) {
            i.Quantity = null
        } else {
            if (i.UnfinishedQuantity < 0) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'Please input a valid Quantity.'
                });
            } else {
                i.Quantity = i.UnfinishedQuantity;
            }
        }
    }

    saveOrder = function(status = null) {
        let statusId, alertMessage, priceTypeId;
        if (isNullOrEmpty(status)) {
            switch (vm.SalesOrderStatusId) {
                case 0:
                    statusId = 1; // Pending
                    alertMessage = "Saved";
                    break;
                case 1:
                    statusId = 2; // Paid
                    alertMessage = "Paid";
                    break;
                case 2:
                    statusId = 3; // Shipped
                    alertMessage = "Shipped";
                    break;
                case 3:
                    statusId = 4; // Delivered
                    alertMessage = "Delivered";
                    break;
            }
        } else {
            statusId = 5; // Cancelled
            alertMessage = "Cancelled";
        }

        if (vm.ShippingFee < 0) {
            QuickAlert.Show({
                type: 'error',
                message: "Please input a valid Shipping Fee."
            });
            return;
        }

        var salesOrderRequest = {
            OrderTransactionType: 1,
            CustomerId: vm.SelectedCustomer.CustomerId,
            SalesOrderId: vm.SalesOrderId,
            SalesOrderStatusId: statusId,
            SalesNo: vm.SalesDetails.SalesNo,
            ModeOfPayment: vm.ModeOfPayment,
            ShippingFee: vm.ShippingFee,
            SalesOrderProductDetailRequest: vm.ProductsInOrder,
        }
        SalesOrderService.SubmitOrder(salesOrderRequest).then(
            function(data) {
                if (vm.SalesOrderId === 0) {
                    $location.url('/OrderDetails/' + data.salesOrderIdResult);
                } else {
                    $route.reload();
                }

                QuickAlert.Show({
                    type: 'success',
                    message: 'Order has been ' + alertMessage
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
                vm.OrderDetails = data.result
                vm.SalesDetails = vm.OrderDetails.SalesDetails;
                vm.ModeOfPayment = vm.SalesDetails.ModeOfPayment ? vm.SalesDetails.ModeOfPayment : 'N/A';
                vm.ShippingFee = vm.SalesDetails.ShippingFee ? vm.SalesDetails.ShippingFee : 0;
                vm.SalesOrderStatusId = vm.SalesDetails.SalesOrderStatusId
                vm.SelectCustomer(vm.OrderDetails.CustomerDetails);
                vm.ProductsInOrder = vm.OrderDetails.ProductList;
                switch (vm.ProductsInOrder[0].PriceTypeId) {
                    case 1:
                        vm.SelectedPriceType = 'Big Buyer';
                        break;
                    case 2:
                        vm.SelectedPriceType = 'Reseller';
                        break;
                    case 3:
                        vm.SelectedPriceType = 'Retailer';
                        break;
                }
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
                getCategoryList();
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

    getCategoryList = function() {
        vm.isLoading = true;
        MaintenanceService.GetCategoryList().then(
            function(data) {
                vm.CategoryList = data.CategoryDetailsResult;
                vm.CategoryList.unshift({ CategoryId: '', CategoryName: 'All Categories' })
                vm.isLoading = false;
            },
            function(error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = error;
            }
        )
    }

    isNullOrEmpty = function(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }
}