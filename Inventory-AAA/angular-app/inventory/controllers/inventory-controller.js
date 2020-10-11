angular
    .module('InventoryApp')
    .controller('InventoryController', InventoryController);

InventoryController.$inject = ['InventoryService', 'DTOptionsBuilder', 'DTDefaultOptions', '$scope', 'DTColumnDefBuilder', '$rootScope', 'QuickAlert'];

function InventoryController(InventoryService, DTOptionsBuilder, DTDefaultOptions, $scope, DTColumnDefBuilder, $rootScope, QuickAlert) {
    var vm = this,
        controllerName = 'inventoryCtrl';

    // View Items
    vm.InventorySummary = [];
    vm.SelectedProduct = {
        ProductId: 0,
        ProductCode: "",
        CategoryId: 1,
        ProductDescription: "",
        Quantity: 0,
        IsActive: true,
        CreatedBy: 0,
        CreatedTime: "",
        Price: {
            BigBuyer: 0,
            Retailer: 0,
            Reseller: 0,
        },
        ProductPrices: []
    };

    vm.CONST_BIG_BUYER = 1;
    vm.CONST_RESELLER = 2;
    vm.CONST_RETAILER = 3;

    vm.OrderRequestQuantity = 0;
    vm.OrderRequestTransactionType = 1;
    vm.OrderRequest = {};
    vm.ProductDetailRequest = {};
    vm.ProductHistory = {};
    vm.OrderRequestRemarks = null;

    vm.CriticalStock = 100;

    // Misc Items
    vm.dtInventorySummaryOptions = "";
    vm.dtInventorySummaryColumnDefs = "";
    vm.dtProductHistoryOptions = "";
    vm.IsLoading = true;
    vm.LoaderErrorMessage = '';
    vm.ManageModalShown = false;
    vm.ShowConfirmAlert = false;
    vm.ManageBarShown = false;
    vm.SearchProductInput = ""

    // Bindable methods
    vm.Initialize = _initialize;
    vm.ToggleManageModal = _toggleManageModal;
    vm.ResetManageFields = _resetManageFields;
    vm.ResetFields = _resetFields
    vm.filteredProducts = [];
    vm.currentPage = 1;
    vm.numPerPage = 5;
    vm.maxSize = 5;

    // API methods
    vm.GetInventorySummary = _getInventorySummary;
    vm.SaveOrderRequest = _saveOrderRequest;
    vm.GetProductDetails = _getProductDetails;
    vm.GetProductDetailsBasic = _getProductDetailsBasic;
    vm.UpdateProductDetails = _updateProductDetails;
    vm.DeleteProduct = _deleteProduct;
    vm.ShowManageBar = _showManageBar;

    vm.Page = 1;

    //Watches

    $scope.$watch(vm.currentPage, function() {
        vm.FilterProducts();
    });


    //Implementations

    function _initialize() {
        _resetManageFields();
        _getInventorySummary();
    }

    function _toggleManageModal() {
        vm.ManageModalShown = !vm.ManageModalShown
    }

    function _getInventorySummary() {
        InventoryService.GetInventorySummary().then(
            function(data) {
                vm.InventorySummary = data;
                vm.FilterProducts();
                vm.IsLoading = false;
            },
            function(error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
            });
    }

    function _getProductDetails(productId, showManageModal = false) {
        vm.ProductDetailRequest["ProductId"] = productId;
        $rootScope.IsLoading = true;
        InventoryService.GetProductDetails(vm.ProductDetailRequest).then(
            function(data) {
                setProduct(data.ProductResult);
                vm.ProductHistory = data.InventoryDetailsResult;
                $rootScope.IsLoading = false;
                vm.ManageBarShown = true;
            },
            function(error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
            });
    }

    function _getProductDetailsBasic(productId) {
        $rootScope.IsLoading = true;
        InventoryService.GetProductDetailsBasic(productId).then(
            function(data) {
                setProduct(data.ProductResult);
                $rootScope.IsLoading = false;
                vm.ManageBarShown = true;
            },
            function(error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
            });
    }

    function _saveOrderRequest(isAddNew = false) {
        if (!isAddNew) {
            // On Purchase/Sale Order, check if Quantity != 0
            if (vm.OrderRequestQuantity === 0 || vm.OrderRequestQuantity < 0) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'Please input a valid Quantity.'
                });

                return;
            }
        }

        if (validProductDetails()) {
            vm.OrderRequest["ProductId"] = vm.SelectedProduct.ProductId;
            vm.OrderRequest["ProductCode"] = vm.SelectedProduct.ProductCode;
            vm.OrderRequest["ProductDescription"] = vm.SelectedProduct.ProductDescription;
            vm.OrderRequest["Stocks"] = parseInt(vm.OrderRequestQuantity !== 0 ? vm.OrderRequestQuantity : vm.SelectedProduct.Quantity);
            vm.OrderRequest["OrderTransactionType"] = isAddNew ? 0 : vm.OrderRequestTransactionType;
            vm.OrderRequest["IsActive"] = 1;
            vm.OrderRequest["Remarks"] = vm.OrderRequestTransactionType === 0 ? null : vm.OrderRequestRemarks;
            vm.OrderRequest["CreatedBy"] = 1;
            vm.OrderRequest["CreatedDate"] = new Date();

            InventoryService.SaveOrderRequest(vm.OrderRequest).then(
                function(data) {
                    if (data.isSucess) {
                        QuickAlert.Show({
                            type: 'success',
                            message: 'Transaction has been saved.'
                        });
                        if (isAddNew) {
                            _getInventorySummary();
                            _resetFields();
                        } else {
                            _getProductDetails(vm.OrderRequest.ProductId);
                            _resetManageFields();
                        }
                        $scope.inventoryForm.$setPristine();
                    } else {
                        QuickAlert.Show({
                            type: 'error',
                            message: data.messageAlert
                        })
                    }
                },
                function(error) {
                    QuickAlert.Show({
                        type: 'error',
                        message: 'Error while fetching data from server.'
                    })
                });
        } else {
            QuickAlert.Show({
                type: 'error',
                message: 'Please fill in the required fields!'
            });
        }

    }

    function _updateProductDetails(isDelete = false) {
        $rootScope.IsLoading = true;
        var errorMsg = '';
        errorMsg = validateUnitPrice();
        if (validProductDetails() && errorMsg === '') {
            var productPrices = [{
                    PriceTypeId: vm.CONST_BIG_BUYER,
                    ProductId: vm.SelectedProduct,
                    Price: vm.SelectedProduct.Price.BigBuyer
                },
                {
                    PriceTypeId: vm.CONST_RESELLER,
                    ProductId: vm.SelectedProduct,
                    Price: vm.SelectedProduct.Price.Reseller
                },
                {
                    PriceTypeId: vm.CONST_RETAILER,
                    ProductId: vm.SelectedProduct,
                    Price: vm.SelectedProduct.Price.Retailer
                }
            ]
            InventoryService.UpdateProductDetails(vm.SelectedProduct, productPrices).then(
                function(data) {
                    if (data.isSucess) {
                        QuickAlert.Show({
                            type: 'success',
                            message: isDelete ? 'The product has been deleted.' : 'The product has been successfully edited.'
                        })
                        _getInventorySummary();
                        _resetFields();
                        $scope.inventoryForm.$setPristine();
                        $rootScope.IsLoading = false;
                    } else {
                        QuickAlert.Show({
                            type: 'error',
                            message: data.messageAlert
                        })
                        $rootScope.IsLoading = false;
                    }
                },
                function(error) {
                    QuickAlert.Show({
                        type: 'error',
                        message: 'Error while fetching data from the service.'
                    })
                    $rootScope.IsLoading = false;
                });
        } else {
            QuickAlert.Show({
                type: 'error',
                message: errorMsg === '' ? 'Please fill in the required fields!' : errorMsg
            });
            $rootScope.IsLoading = false;
        }
    }

    function _deleteProduct(result) {
        vm.SelectedProduct.IsActive = false;
        $rootScope.IsLoading = false;
        _updateProductDetails(true);
        vm.ShowConfirmAlert = false;
    }

    function _resetManageFields() {
        vm.OrderRequest = {};
        vm.OrderRequestTransactionType = 1;
        vm.OrderRequestQuantity = 0;
        vm.OrderRequestRemarks = null;
        _resetFields();
    }

    function _resetFields() {
        vm.SelectedProduct = {
            ProductId: 0,
            ProductCode: "",
            CategoryId: 1,
            ProductDescription: "",
            Quantity: 0,
            IsActive: true,
            Price: {
                BigBuyer: 0,
                Retailer: 0,
                Reseller: 0,
            }
        };
    }

    function _showManageBar(resetFields = false) {
        if (resetFields) {
            _resetFields();
        }

        vm.ManageBarShown = true;
    }

    // Private Methods

    function setProduct(data) {

        vm.SelectedProduct = {
            ProductId: data.ProductId,
            ProductCode: data.ProductCode,
            CategoryId: 1,
            ProductDescription: data.ProductDescription,
            Quantity: data.Quantity,
            IsActive: true,
            CreatedBy: data.CreatedBy,
            CreatedTime: data.CreatedDateTimeFormat,
            Price: {
                BigBuyer: 0,
                Retailer: 0,
                Reseller: 0,
            }
        };

        setProductPrices(data.ProductPrices);
    }

    function setProductPrices(prices) {
        for (var x = 0; x < prices.length; x++) {
            switch (prices[x].PriceTypeId) {
                case 1:
                    vm.SelectedProduct.Price.BigBuyer = prices[x].Price;
                    break;
                case 2:
                    vm.SelectedProduct.Price.Reseller = prices[x].Price;
                    break;
                case 3:
                    vm.SelectedProduct.Price.Retailer = prices[x].Price;
                    break;
            }
        }

    }

    function validateUnitPrice() {
        var bigBuyer = vm.SelectedProduct.Price.BigBuyer,
            reseller = vm.SelectedProduct.Price.Reseller,
            retailer = vm.SelectedProduct.Price.Retailer

        if (bigBuyer !== 0 || reseller !== 0 || retailer !== 0) {
            return '';
        }
        return 'Please input at least one price type.';
    }

    function validProductDetails() {
        if (!(isNullOrEmpty(vm.SelectedProduct.ProductCode)) &&
            !(isNullOrEmpty(vm.SelectedProduct.ProductDescription))) {
            return true;
        } else {
            return false;
        }
    }

    function isNullOrEmpty(data) {
        if (data.trim() === "" || data.trim() === null || data === undefined) {
            return true;
        }
        return false;
    }

    vm.FilterProducts = function() {
        var begin = ((vm.currentPage - 1) * vm.numPerPage),
            end = begin + vm.numPerPage;

        vm.filteredProducts = vm.InventorySummary.slice(begin, end);
    }
}