angular
    .module('InventoryApp')
    .controller('InventoryController', InventoryController);

InventoryController.$inject = ['InventoryService', 'DTOptionsBuilder', 'DTDefaultOptions', '$scope', 'DTColumnDefBuilder', '$rootScope', 'QuickAlert'];

function InventoryController(InventoryService, DTOptionsBuilder, DTDefaultOptions, $scope, DTColumnDefBuilder, $rootScope, QuickAlert ) {
    var vm = this, controllerName = 'inventoryCtrl';

    // View Items
    vm.InventorySummary = [];
    vm.SelectedProduct = {
        ProductId: 0,
        ProductCode: "",
        ProductDescription: "",
        Quantity: 0,
        UnitPrice: "",
        IsActive: true,
        CreatedBy: 0,
        CreatedTime: "",
    };
    vm.OrderRequestQuantity = 0;
    vm.OrderRequestTransactionType = 0;
    vm.OrderRequest = {};
    vm.ProductDetailRequest = {};
    vm.ProductHistory = {};

    // Misc Items
    vm.dtInventorySummaryOptions = "";
    vm.dtInventorySummaryColumnDefs = "";
    vm.dtProductHistoryOptions = "";
    vm.IsLoading = true;
    vm.LoaderErrorMessage = '';
    vm.ManageModalShown = false;

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

    //Watches

    //Implementations

    function _initialize() {
        _resetManageFields();
        _getInventorySummary();
    }

    vm.dtInventorySummaryOptions = DTOptionsBuilder.newOptions()
        .withPaginationType('simple_numbers')
        .withDisplayLength(10)
        .withDOM("<'row'<'col-sm-12 col-md-6'f><'col-sm-12 col-md-6'p><'table-details't>>");

    vm.dtInventorySummaryColumnDefs = [
        DTColumnDefBuilder.newColumnDef(0),
        DTColumnDefBuilder.newColumnDef(6).notSortable()
    ];

    vm.dtProductHistoryOptions = DTOptionsBuilder.newOptions()
        .withPaginationType('simple_numbers')
        .withDisplayLength(10)
        .withDOM("<'row'<'col-sm-12 col-md-6'f><'col-sm-12 col-md-6'p><'table-details't>>")
        .withOption('order', [0, 'desc']);

    DTDefaultOptions.setLoadingTemplate(
        '<div class="loader-design"><img src="Content/assets/crescent-loader.gif" ></div>'
    );

    function _toggleManageModal() {
        vm.ManageModalShown = !vm.ManageModalShown
    }

    function _getInventorySummary() {
        InventoryService.GetInventorySummary().then(
            function (data) {
                vm.InventorySummary = data;
                vm.IsLoading = false;
            }, function (error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
            });
    }

    function _getProductDetails(productId, showManageModal = false) {
        vm.ProductDetailRequest["ProductId"] = productId;
        $rootScope.IsLoading = true;
        InventoryService.GetProductDetails(vm.ProductDetailRequest).then(
            
            function (data) {
                setProduct(data.ProductResult);
                vm.ProductHistory = data.InventoryDetailsResult;
                if (showManageModal) {
                    _toggleManageModal();
                }
                $rootScope.IsLoading = false;
            }, function (error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
            });
    }

    function _getProductDetailsBasic(productId) {
        $rootScope.IsLoading = true;
        InventoryService.GetProductDetailsBasic(productId).then(
            function (data) {
                setProduct(data.ProductResult);
                $rootScope.IsLoading = false;
            }, function (error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
            });
    }

    function _saveOrderRequest(isAddNew = false) {
        // On Purchase/Sale Order, check if Quantity != 0
        if (!isAddNew) {
            if (vm.OrderRequestQuantity == 0) {
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
            vm.OrderRequest["UnitPrice"] = parseInt(vm.SelectedProduct.UnitPrice);
            vm.OrderRequest["Stocks"] = parseInt(vm.OrderRequestQuantity !== 0 ? vm.OrderRequestQuantity : vm.SelectedProduct.Quantity);
            vm.OrderRequest["OrderTransactionType"] = vm.OrderRequestTransactionType;
            vm.OrderRequest["IsActive"] = 1;
            vm.OrderRequest["CreatedBy"] = 1;
            vm.OrderRequest["CreatedDate"] = new Date();

            InventoryService.SaveOrderRequest(vm.OrderRequest).then(
                function (data) {
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
                    } else {
                        QuickAlert.Show({
                            type: 'error',
                            message: data.messageAlert
                        })
                    }
                }, function (error) {
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

    function _updateProductDetails() {
        $rootScope.IsLoading = true;
        if (validProductDetails()) {
            InventoryService.UpdateProductDetails(vm.SelectedProduct).then(
                function (data) {
                    if (data.isSucess) {
                        QuickAlert.Show({
                            type: 'success',
                            message: 'The product has been successfully edited.'
                        })
                        _getInventorySummary();
                        _resetFields();
                        $rootScope.IsLoading = false;
                    }
                }, function (error) {
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

    function _resetManageFields() {
        vm.OrderRequest = {};
        vm.OrderRequestTransactionType = 0;
        vm.OrderRequestQuantity = 0;
        _resetFields();
    }

    function _resetFields() {
        vm.SelectedProduct = {
            ProductId: 0,
            ProductCode: "",
            ProductDescription: "",
            Quantity: 0,
            UnitPrice: "",
            IsActive: true
        };
    }

    // Private Methods

    function setProduct(data) {
        vm.SelectedProduct.ProductId = data.ProductId;
        vm.SelectedProduct.ProductCode = data.ProductCode;
        vm.SelectedProduct.ProductDescription = data.ProductDescription;
        vm.SelectedProduct.UnitPrice = data.UnitPrice;
        vm.SelectedProduct.Quantity = data.Quantity;
        vm.SelectedProduct.CreatedBy = data.CreatedBy,
        vm.SelectedProduct.CreatedTime = data.CreatedDateTimeFormat
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
        if (!(isNullOrEmpty(vm.SelectedProduct.ProductCode))
            && !(isNullOrEmpty(vm.SelectedProduct.ProductDescription))
            && validUnitPrice()) {
            return true;
        } else {
            return false;
        }
    }

    function isNullOrEmpty(data) {
        if (data.trim() === "" || data.trim() === null) {
            return true;
        }
        return false;
    }
}