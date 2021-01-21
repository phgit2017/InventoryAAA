angular
    .module('InventoryApp')
    .controller('OrderDetailsController', OrderDetailsController);

OrderDetailsController.$inject = ['$scope', '$route', '$location', '$routeParams', 'MaintenanceService', 'SalesOrderService', 'CustomerService', 'CommonService', 'InventoryService', 'QuickAlert', 'CommonService'];

function OrderDetailsController($scope, $route, $location, $routeParams, MaintenanceService, SalesOrderService, CustomerService, CommonService, InventoryService, QuickAlert, CommonService) {

    var vm = this,
        controllerName = 'orderDetailsCtrl';

    vm.SalesOrderId = 0;
    vm.OrderDetails = {};
    vm.SalesDetails = '';
    vm.ProductList = [];
    vm.FilteredProductList = [];
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
    vm.SelectedCustomerDetail = '';
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
    vm.NewCustomerToggled = false;
    vm.SelectedCustomer = {
        CustomerId: 0,
        CustomerCode: "",
        FirstName: "",
        LastName: "",
        FullAddress: "",
        IsActive: true,
        CreatedBy: 0,
        CreatedTime: "",
        CustomerStatusId: 1
    };
    vm.TotalAmount = 0;

    $scope.$watch(
        function() {
            return vm.FilterCategoryId;
        },
        function(oldValue, newValue) {
            if (oldValue !== newValue) {
                vm.FilterProducts();
            }
        }
    );

    $scope.$watch(
        function() {
            return vm.SearchProductList;
        },
        function(oldValue, newValue) {
            if (oldValue !== newValue) {
                vm.FilterProducts();
            }
        }
    );


    vm.Initialize = function() {
        getCustomerList();
        vm.SalesOrderId = parseFloat($routeParams.salesOrderId);

        if (vm.SalesOrderId !== 0) {
            getOrderDetails();
        } else {
            getProductList();
        }
    }

    vm.FilterProducts = function() {
        if (!isNullOrEmpty(vm.FilterCategoryId)) {
            vm.FilteredProductList = vm.ProductList.filter((data) => {return data.CategoryId === vm.FilterCategoryId});
        } else {
            vm.FilteredProductList = vm.ProductList;
        }

        if (!isNullOrEmpty(vm.SearchProductList)) {
            vm.FilteredProductList = vm.FilteredProductList.filter((data) => 
                data.ProductDescription.toLowerCase().indexOf(vm.SearchProductList) !== -1 ||
                data.ProductCode.toLowerCase().indexOf(vm.SearchProductList) !== -1 
                );
        }

        
    }





    vm.SelectCustomer = function(customer) {
        vm.SelectedCustomer = customer;
        vm.SelectedCustomerLabel = customer.CustomerCode + ' - ' + customer.FirstName + ' ' + customer.LastName;
        vm.SelectedCustomerDetail = (isNullOrEmpty(customer.MobileNumber) ? 'No Number' : customer.MobileNumber)  + ' - ' + customer.FullAddress;
        vm.CustomerListShown = false;
    }

    vm.NewCustomer = function() {
        vm.SelectedCustomer = {
            CustomerId: 0,
            CustomerCode: "",
            FirstName: "",
            LastName: "",
            FullAddress: "",
            IsActive: true,
            CreatedBy: 0,
            CreatedTime: "",
            CustomerStatusId: 1
        };
        CommonService.GetCustomerStatusList().then(
            function(data) {
                vm.CustomerStatusList = data.result;
            },
            function(error) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'Server error.'
                });
            }
        )
    }

    vm.SaveCustomer = function() {
        CustomerService.SaveCustomer(vm.SelectedCustomer, 'Add').then(
            function(data) {
                if (data.isSuccess) {
                    QuickAlert.Show({
                        type: 'success',
                        message: 'Customer has been added and set.'
                    });
                    vm.SelectCustomer(vm.SelectedCustomer);
                    vm.NewCustomerToggled = false;
                    vm.CustomerListShown = false;
                    getCustomerList();
                    vm.SelectedCustomer.CustomerId = data.CustomerId
                } else {
                    QuickAlert.Show({
                        type: 'error',
                        message: data.messageAlert
                    });
                }

            },
            function(error) {
                QuickAlert.Show({
                    type: 'error',
                    message: error
                });
            }
        )
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
        selectedProductIndex = vm.FilteredProductList.indexOf(product);
        vm.FilteredProductList.splice(selectedProductIndex, 1);
    }

    vm.RemoveProductFromOrder = function(product) {
        selectedProductIndex = vm.ProductsInOrder.indexOf(product);
        vm.ProductsInOrder.splice(selectedProductIndex, 1);

        computeTotalAmount();
        getProductList();
    }

    vm.SubmitOrder = function() {
        let errorMsg = '',
            isBreak = false;

        if (vm.SelectedCustomer.CustomerId === 0) {
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
        let tempQuantity = i.Quantity;

        if (i.Quantity) {
            i.Quantity = null
        } else {
            if (i.UnfinishedQuantity < 0) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'Please input a valid quantity.'
                });
            } else if (i.UnfinishedQuantity > i.StocksAvailable) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'Insufficient available stocks.'
                });
            }
            else {
                i.Quantity = parseInt(i.UnfinishedQuantity);
                computeTotalAmount();
            }
        }
    }


    vm.printDiv = function(divName) {
        var printContents = document.getElementById(divName).innerHTML;
        var popupWin = window.open('', '_blank', 'width=300,height=300');
        popupWin.document.open();
        popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="../../../Content/Site.css" medi /></head><body onload="window.print()">' + printContents + '</body></html>');
        popupWin.document.close();
    } 

    computeTotalAmount = function(){
        vm.TotalAmount = 0;
        vm.ProductsInOrder.forEach(function
            (product, index) {
                vm.TotalAmount += (product.Quantity * product.UnitPrice); 
            }
        );
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
                vm.TotalAmount = vm.SalesDetails.TotalAmount;
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
        CommonService.GetCustomerList().then(
            function(data) {
                vm.CustomerList = data.result;
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
        InventoryService.GetInventorySummaryForSalesOrders().then(
            function(data) {
                getCategoryList();
                vm.ProductList = data.result;
                vm.FilteredProductList = data.result;
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