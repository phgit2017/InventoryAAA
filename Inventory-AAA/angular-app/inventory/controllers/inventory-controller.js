angular
    .module('InventoryApp')
    .controller('InventoryController', InventoryController);

InventoryController.$inject = ['$filter', '$scope', 'InventoryService', 'MaintenanceService', '$scope', '$rootScope', 'QuickAlert'];

function InventoryController($filter, $scope, InventoryService, MaintenanceService, $scope, $rootScope, QuickAlert) {
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
    vm.OrderRequestRemarks = "";
    vm.CriticalStock = 100;
    vm.CategoryList = [];
    vm.CategoryFilter = "";
    vm.SelectedCategory = {
        CategoryId: 0,
        CategoryName: ""
    }
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
    vm.numPerPage = 10;
    vm.maxSize = 5;

    // API methods
    vm.GetInventorySummary = _getInventorySummary;
    vm.GetProductDetails = _getProductDetails;
    vm.UpdateProductDetails = _updateProductDetails;
    vm.DeleteProduct = _deleteProduct;

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

    $scope.$watch(
        function() {
            return vm.SearchProductInput;
        },
        function(newValue,oldValue){                
            if(oldValue!=newValue){
                vm.filteredProducts = $filter('filter')(vm.InventorySummary, vm.SearchProductInput);
                vm.currentPage = 1;
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
                vm.filteredProducts = data;
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

    vm.SaveOrderRequest = function(isAddNew = false) {
        if ((isAddNew && (vm.SelectedProduct.Quantity === 0 || vm.SelectedProduct.Quantity < 0)) ||
            !isAddNew && (vm.OrderRequestQuantity === 0 || vm.OrderRequestQuantity < 0)) {
            QuickAlert.Show({
                type: 'error',
                message: 'Please input a valid Quantity.'
            });
            return;
        }

        if (vm.SelectedProduct.CategoryId === 0) {
            QuickAlert.Show({
                type: 'error',
                message: 'Please input a Category.'
            });
            return;
        }
        if (!isAddNew && (vm.OrderRequestRemarks === '' || vm.OrderRequestRemarks === null)) {
            QuickAlert.Show({
                type: 'error',
                message: 'Please input Remarks.'
            });
            return;
        }

        if (validateUnitPrice() !== '') {
            QuickAlert.Show({
                type: 'error',
                message: 'Please input at least one price type.'
            });
            return;
        }

        if (validProductDetails()) {
            vm.OrderRequest["ProductId"] = vm.SelectedProduct.ProductId;
            vm.OrderRequest["ProductCode"] = vm.SelectedProduct.ProductCode;
            vm.OrderRequest["ProductDescription"] = vm.SelectedProduct.ProductDescription;
            vm.OrderRequest["CategoryId"] = vm.SelectedProduct.CategoryId;
            vm.OrderRequest["Stocks"] = isAddNew ? vm.SelectedProduct.Quantity : vm.OrderRequestQuantity;
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
                        vm.ManageBarShown = false;
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
                        vm.ManageBarShown = false;
                        _getInventorySummary();
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
        vm.OrderRequestRemarks = "";
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
        vm.SelectedCategory = {
            CategoryId: 0,
            CategoryName: ""
        }
        vm.OrderRequestRemarks = "";
        vm.TotalStock = 0;
        vm.OrderRequestQuantity = 0;

        if (vm.StockMode !== '') {
            vm.StockMode = '';
        }
    }

    vm.ShowManageBar = function(resetFields = false) {
        if (resetFields) {
            vm.ManageMode = vm.MANAGE_MODE_DETAILS;
            _resetFields();
        }

        vm.ManageBarShown = true;
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
                if (data.isSuccess) {
                    getCategoryList();
                    vm.SelectedProduct.CategoryId = data.CategoryId;
                    vm.SelectedCategory = {
                        CategoryId: data.CategoryId,
                        CategoryName: data.CategoryName
                    }
                    QuickAlert.Show({
                        type: 'success',
                        message: 'The category has been saved!'
                    });
                    vm.IsCategoryNew = false;
                    vm.NewCategoryName = '';
                } else {
                    QuickAlert.Show({
                        type: 'error',
                        message: data.messageAlert
                    });
                }

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
        vm.SelectedCategory = {
            CategoryId: data.CategoryId,
            CategoryName: data.CategoryName
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