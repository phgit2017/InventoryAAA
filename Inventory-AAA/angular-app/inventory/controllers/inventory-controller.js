﻿angular
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
        }
    };
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
                if (showManageModal) {
                    _toggleManageModal();
                }
                $rootScope.IsLoading = false;
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
        if (validProductDetails()) {
            InventoryService.UpdateProductDetails(vm.SelectedProduct).then(
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
            alert('Please fill in the required fields!');
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

    function _showManageBar() {
        vm.ManageBarShown = true;
    }

    // Private Methods

    function setProduct(data) {
        debugger;
        vm.SelectedProduct.ProductId = data.ProductId;
        vm.SelectedProduct.ProductCode = data.ProductCode;
        vm.SelectedProduct.ProductDescription = data.ProductDescription;
        vm.SelectedProduct.Quantity = data.Quantity;
        vm.SelectedProduct.CreatedBy = data.CreatedBy;
        vm.SelectedProduct.CreatedTime = data.CreatedDateTimeFormat
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
                BigBuyer: data.Price.BigBuyer,
                Retailer: data.Price.Retailer,
                Reseller: data.Price.Reseller,
            }
        };
    }

    function validUnitPrice() {
        var unitPrice = vm.SelectedProduct.UnitPrice;

        unitPrice = unitPrice.toString();

        if (unitPrice !== null) {
            if (unitPrice.length > 0) {
                if (!isNaN(unitPrice)) {
                    return true;
                }
            }
        }
        return false;
    }

    function validProductDetails() {
        if (!(isNullOrEmpty(vm.SelectedProduct.ProductCode)) &&
            !(isNullOrEmpty(vm.SelectedProduct.ProductDescription)) &&
            validUnitPrice()) {
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
}