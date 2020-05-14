angular
    .module('InventoryApp')
    .controller('InventoryController', InventoryController);

InventoryController.$inject = ['InventoryService', 'DTOptionsBuilder', 'DTDefaultOptions', '$scope'];

function InventoryController(InventoryService, DTOptionsBuilder, DTDefaultOptions, $scope) {
    var vm = this, controllerName = 'inventoryCtrl';

    // View Items
    vm.InventorySummary = [];
    vm.SelectedProduct = [];
    vm.OrderRequestQuantity = 0;
    vm.OrderRequestTransactionType = 0;


    // Object Declarations
    vm.OrderRequest = {};
    vm.ProductDetailRequest = {};
    vm.SelectedProductEdit = {
        ProductId: 0,
        ProductCode: "",
        ProductDescription: "",
        Quantity: 0,
        UnitPrice: "",
        IsActive: true
    };

    // Misc Items
    vm.dtInventorySummaryOptions = "";
    vm.dtProductDetailsOptions = "";
    vm.IsLoading = true;
    vm.LoaderErrorMessage = '';
    vm.EditDetailsShown = false;
    vm.ManageModalShown = false;
    vm.AddStockModalShown = false;

    // Bindable methods
    vm.Initialize = _initialize;
    vm.ToggleManageModal = _toggleManageModal;
    vm.ToggleAddStockModal = _toggleAddStockModal;
    vm.ToggleEditDetails = _toggleEditDetails;
    vm.ResetManageFields = _resetManageFields;
    vm.ResetAddFields = _resetAddFields

    // API methods
    vm.GetInventorySummary = _getInventorySummary;
    vm.SaveOrderRequest = _saveOrderRequest;
    vm.GetProductDetails = _getProductDetails; 
    vm.UpdateProductDetails = _updateProductDetails;

    //Watches

    //Implementations

    function _initialize() {
        _resetManageFields();
        _getInventorySummary();
        vm.EditDetailsShown = false;
    }

    vm.dtInventorySummaryOptions = DTOptionsBuilder.newOptions()
        .withPaginationType('simple_numbers')
        .withDisplayLength(10)
        .withDOM("<'row'<'col-sm-12 col-md-6'f><'col-sm-12 col-md-6'l><'table-details't>>p");

    vm.dtProductDetailsOptions = DTOptionsBuilder.newOptions()
        .withPaginationType('simple_numbers')
        .withDisplayLength(10)
        .withDOM("<'table-details't>");

    DTDefaultOptions.setLoadingTemplate(
        '<div class="loader-design"><img src="Content/assets/crescent-loader.gif" ></div>'
    );

    function _toggleManageModal() {
        vm.ManageModalShown = !vm.ManageModalShown
    }

    function _toggleAddStockModal() {
        _resetAddFields();
        vm.AddStockModalShown = !vm.AddStockModalShown;
    }

    function _toggleEditDetails() {
        vm.EditDetailsShown = !vm.EditDetailsShown;
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
        
        InventoryService.GetProductDetails(vm.ProductDetailRequest).then(
            function (data) {
                vm.SelectedProduct = data.ProductResult;

                if (showManageModal) {
                    _toggleManageModal();
                }
            }, function (error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
            });
    }

    function _saveOrderRequest(isAddNew = false) {
        vm.OrderRequest["ProductId"] = isAddNew ? 0 : vm.SelectedProduct.ProductId;
        vm.OrderRequest["ProductCode"] = vm.SelectedProduct.ProductCode;
        vm.OrderRequest["ProductDescription"] = vm.SelectedProduct.ProductDescription;
        vm.OrderRequest["UnitPrice"] = parseInt(vm.SelectedProduct.UnitPrice);
        vm.OrderRequest["Stocks"] = parseInt(vm.OrderRequestQuantity);
        vm.OrderRequest["OrderTransactionType"] = vm.OrderRequestTransactionType;
        vm.OrderRequest["IsActive"] = 1;
        vm.OrderRequest["CreatedBy"] = 1;
        vm.OrderRequest["CreatedDate"] = new Date();

        InventoryService.SaveOrderRequest(vm.OrderRequest).then(
            function (data) {
                if (data.isSucess) {
                    alert("Transaction has been saved!");
                    if (isAddNew) {
                        _resetAddFields()
                    }
                    else {
                        _getProductDetails(vm.OrderRequest.ProductId);
                        _resetManageFields();
                    }
                }
            }, function (error) {
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
                alert(error);
        });
    }

    function _updateProductDetails() {
        if (validUnitPrice()) {
            vm.SelectedProductEdit["ProductId"] = vm.SelectedProduct.ProductId;
            vm.SelectedProductEdit["ProductCode"] = vm.SelectedProductEdit.ProductCode === "" ? vm.SelectedProduct.ProductCode : vm.SelectedProductEdit.ProductCode;
            vm.SelectedProductEdit["ProductDescription"] = vm.SelectedProductEdit.ProductDescription === "" ? vm.SelectedProduct.ProductDescription : vm.SelectedProductEdit.ProductDescription;
            vm.SelectedProductEdit["UnitPrice"] = vm.SelectedProductEdit.UnitPrice === "" ? parseInt(vm.SelectedProduct.UnitPrice) : parseInt(vm.SelectedProductEdit.UnitPrice);
            vm.SelectedProductEdit["Quantity"] = vm.SelectedProduct.Quantity;
            vm.SelectedProductEdit["IsActive"] = 1;

            InventoryService.UpdateProductDetails(vm.SelectedProductEdit).then(
                function (data) {
                    if (data.isSucess) {
                        alert("The product has been successfully edited!");
                        _getProductDetails(vm.SelectedProductEdit.ProductId);
                        _resetEditDetails();
                    }
                }, function (error) {
                    vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
                    alert(error);
                });
        } else {
            alert("Please enter a valid Unit Price value.")
        }
    }

    function _resetManageFields() {
        vm.OrderRequest = {};
        vm.OrderRequestTransactionType = 0;
        vm.OrderRequestQuantity = 0;
    }

    function _resetEditDetails() {
        vm.EditDetailsShown = false;
        vm.SelectedProductEdit = {
            ProductId: 0,
            ProductCode: "",
            ProductDescription: "",
            Quantity: 0,
            UnitPrice: "",
            IsActive: true
        };
    }

    function _resetAddFields() {
        vm.SelectedProduct = {};
        vm.OrderRequestQuantity = 0;
        vm.OrderTransactionType = 0;
    }

    function validUnitPrice() {
        var unitPrice = vm.SelectedProductEdit.UnitPrice === "" ? vm.SelectedProduct.UnitPrice : vm.SelectedProductEdit.UnitPrice;

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
}