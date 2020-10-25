﻿angular
    .module('InventoryApp')
    .controller('InventoryController', InventoryController);

InventoryController.$inject = ['InventoryService', 'MaintenanceService', '$scope', '$rootScope', 'QuickAlert'];

function InventoryController(InventoryService, MaintenanceService, $scope, $rootScope, QuickAlert) {
    var vm = this,
        controllerName = 'inventoryCtrl';

    // View Items
    vm.InventorySummary = [];
    vm.SelectedProduct = {
        ProductId: 0,
        ProductCode: "",
        CategoryId: 0,
        ProductDescription: "",
        Quantity: 0,
        IsActive: true,
        CreatedBy: 0,
        CreatedTime: "",
        BigBuyerPrice: 0,
        RetailerPrice: 0,
        ResellerPrice: 0,
        ProductPrices: []
    };
    vm.TotalStock = 0;

    vm.CONST_BIG_BUYER = 1;
    vm.CONST_RESELLER = 2;
    vm.CONST_RETAILER = 3;
    vm.MANAGE_MODE_DETAILS = 'Details';
    vm.MANAGE_MODE_STOCKS = 'Stocks';
    vm.STOCK_MODE_ADD = 'Add';
    vm.STOCK_MODE_CORRECTION = 'Correction';

    vm.OrderRequestQuantity = 0;
    vm.OrderRequestTransactionType = 1;
    vm.OrderRequest = {};
    vm.ProductDetailRequest = {};
    vm.ProductHistory = {};
    vm.OrderRequestRemarks = null;
    vm.CriticalStock = 100;
    vm.CategoryList = [];
    vm.NewCategoryName = "";

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
    vm.IsCategoryNew = false;
    vm.ManageMode = vm.MANAGE_MODE_DETAILS;
    vm.StockMode = '';

    // Bindable methods
    vm.Initialize = _initialize;
    vm.ToggleManageModal = _toggleManageModal;
    vm.ResetManageFields = _resetManageFields;
    vm.ResetFields = _resetFields
    vm.filteredProducts = [];
    vm.currentPage = 1;
    vm.numPerPage = 9;
    vm.maxSize = 5;

    // API methods
    vm.GetInventorySummary = _getInventorySummary;
    vm.SaveOrderRequest = _saveOrderRequest;
    vm.GetProductDetails = _getProductDetails;
    vm.UpdateProductDetails = _updateProductDetails;
    vm.DeleteProduct = _deleteProduct;
    vm.ShowManageBar = _showManageBar;

    vm.Page = 1;

    //Watches

    $scope.$watch(
        function() {
            return vm.OrderRequestQuantity;
        },
        function(oldValue, newValue) {
            if (oldValue != newValue) {
                vm.TotalStock = vm.OrderRequestQuantity !== null ? vm.SelectedProduct.Quantity + vm.OrderRequestQuantity : vm.SelectedProduct.Quantity;
            }
        }
    );

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
                getCategoryList();
                vm.IsLoading = false;
            },
            function(error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
            });
    }

    function _getProductDetails(productId, showManageModal = false) {
        vm.ProductDetailRequest["ProductId"] = productId;
        InventoryService.GetProductDetails(vm.ProductDetailRequest).then(
            function(data) {
                setProduct(data.ProductResult);
                vm.ProductHistory = data.InventoryDetailsResult;
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
            vm.OrderRequest["CategoryId"] = vm.SelectedProduct.CategoryId;
            vm.OrderRequest["Stocks"] = parseInt(vm.OrderRequestQuantity !== 0 ? vm.OrderRequestQuantity : vm.SelectedProduct.Quantity);
            vm.OrderRequest["BigBuyerPrice"] = vm.SelectedProduct.BigBuyerPrice;
            vm.OrderRequest["RetailerPrice"] = vm.SelectedProduct.RetailerPrice;
            vm.OrderRequest["ResellerPrice"] = vm.SelectedProduct.ResellerPrice;
            vm.OrderRequest["IsActive"] = 1;
            vm.OrderRequest["Remarks"] = vm.OrderRequestTransactionType === 0 ? null : vm.OrderRequestRemarks;
            vm.OrderRequest["CreatedBy"] = 1;
            vm.OrderRequest["CreatedDate"] = new Date();

            switch (vm.StockMode) {
                case 'Add':
                    vm.OrderRequest["OrderTransactionType"] = 0;
                    break;
                case 'Correction':
                    vm.OrderRequest["OrderTransactionType"] = 2;
                    break;
            }

            InventoryService.SaveOrderRequest(vm.OrderRequest).then(
                function(data) {
                    if (data.isSucess) {
                        QuickAlert.Show({
                            type: 'success',
                            message: 'Transaction has been saved.'
                        });
                        _getInventorySummary();
                        _getProductDetails(vm.OrderRequest.ProductId);
                        _resetFields();
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
        var errorMsg = '';
        errorMsg = validateUnitPrice();
        if (validProductDetails() && errorMsg === '') {
            InventoryService.UpdateProductDetails(vm.SelectedProduct).then(
                function(data) {
                    if (data.isSucess) {
                        QuickAlert.Show({
                            type: 'success',
                            message: isDelete ? 'The product has been deleted.' : 'The product has been successfully edited.'
                        })
                        _getInventorySummary();
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
                        message: 'Error while fetching data from the service.'
                    })
                });
        } else {
            QuickAlert.Show({
                type: 'error',
                message: errorMsg === '' ? 'Please fill in the required fields!' : errorMsg
            });
        }
    }

    function _deleteProduct(result) {
        vm.SelectedProduct.IsActive = false;
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
            CategoryId: 0,
            ProductDescription: "",
            Quantity: 0,
            IsActive: true,
            BigBuyerPrice: 0,
            RetailerPrice: 0,
            ResellerPrice: 0,
        };

        vm.TotalStock = 0;
        vm.OrderRequestQuantity = 0;

        if (vm.StockMode !== '') {
            vm.StockMode = '';
        }
    }

    function _showManageBar(resetFields = false) {
        if (resetFields) {
            vm.ManageMode = vm.MANAGE_MODE_DETAILS;
            _resetFields();
        }

        vm.ManageBarShown = true;
    }

    vm.FilterProducts = function() {
        var begin = ((vm.currentPage - 1) * vm.numPerPage),
            end = begin + vm.numPerPage;

        vm.filteredProducts = vm.InventorySummary.slice(begin, end);
    }

    vm.SaveNewCategory = function() {
        if (vm.NewCategoryName.trim() === '') {
            QuickAlert.Show({
                type: 'error',
                message: 'Please input a valid category name.'
            });
            return;
        }

        vm.IsLoading = true;
        var data = {
            CategoryId: 0,
            CategoryName: vm.NewCategoryName,
            IsActive: true
        }
        MaintenanceService.SaveCategory(data).then(
            function(data) {
                getCategoryList();
                vm.SelectedProduct.CategoryId = data.CategoryId;
                QuickAlert.Show({
                    type: 'success',
                    message: 'The category has been saved!'
                });
                vm.IsCategoryNew = false;
                vm.NewCategoryName = '';
                vm.IsLoading = false;
            },
            function(error) {
                QuickAlert.Show({
                    type: 'error',
                    message: data.messageAlert
                });
                vm.IsLoading = false;
            }
        )
    }

    // Private Methods

    function setProductPrices(prices) {
        for (var x = 0; x < prices.length; x++) {
            switch (prices[x].PriceTypeId) {
                case 1:
                    vm.SelectedProduct.BigBuyerPrice = prices[x].Price;
                    break;
                case 2:
                    vm.SelectedProduct.ResellerPrice = prices[x].Price;
                    break;
                case 3:
                    vm.SelectedProduct.RetailerPrice = prices[x].Price;
                    break;
            }
        }

    }

    function setProduct(data) {
        vm.SelectedProduct = {
            ProductId: data.ProductId,
            ProductCode: data.ProductCode,
            CategoryId: data.CategoryId,
            ProductDescription: data.ProductDescription,
            Quantity: data.Quantity,
            IsActive: true,
            CreatedBy: data.CreatedBy,
            CreatedTime: data.CreatedDateTimeFormat,
            BigBuyerPrice: data.BigBuyerPrice,
            RetailerPrice: data.RetailerPrice,
            ResellerPrice: data.ResellerPrice,
        };
        vm.TotalStock = data.Quantity;
        setProductPrices(data.ProductPrices);
    }

    function validateUnitPrice() {
        var bigBuyer = vm.SelectedProduct.BigBuyerPrice,
            reseller = vm.SelectedProduct.ResellerPrice,
            retailer = vm.SelectedProduct.RetailerPrice

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

    function getCategoryList() {
        vm.isLoading = true;
        MaintenanceService.GetCategoryList().then(
            function(data) {
                vm.CategoryList = data.CategoryDetailsResult;
                vm.isLoading = false;
            },
            function(error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = error;
            }
        )
    }
}