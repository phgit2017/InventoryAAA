angular
    .module('InventoryApp')
    .controller('InventoryController', InventoryController);

InventoryController.$inject = ['InventoryService', 'DTOptionsBuilder', 'DTDefaultOptions', '$http', '$q'];

function InventoryController(InventoryService, DTOptionsBuilder, DTDefaultOptions, $http, $q) {
    var vm = this, controllerName = 'InventoryCtrl';

    // View Items
    vm.InventorySummary = [];
    vm.SelectedProduct = [];
    vm.OrderRequestQuantity = 0;
    vm.OrderRequestTransactionType = 1;


    // Object Declarations
    vm.OrderRequest = {
        ProductId: 0,
        ProductCode: '',
        ProductDescription: '',
        Stocks: 0,
        UnitPrice: 0,
        OrderTransactionType: 0,
        IsActive: 1
    };

    vm.ProductDetailRequest = {
        ProductId: 0
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

    // API methods
    vm.GetInventorySummary = _getInventorySummary;
    vm.SaveOrderRequest = _saveOrderRequest;
    vm.GetProductDetails = _getProductDetails; 
    vm.ResetManageFields = _resetManageFields;

    function _initialize() {
        _getInventorySummary();
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
        console.log(vm.SelectedProduct);
        vm.ManageModalShown = !vm.ManageModalShown
    }

    function _toggleAddStockModal() {
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
                console.log(error);
            });
    }

    function _getProductDetails(productId, showManageModal = false) {
        vm.ProductDetailRequest.ProductId = productId;
        InventoryService.GetProductDetails(vm.ProductDetailRequest).then(
            function (data) {
                vm.SelectedProduct = data.ProductResult;

                if (showManageModal) {
                    _toggleManageModal();
                }
            }, function (error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
                console.log(error);
            });
    }

    function _saveOrderRequest() {
        vm.OrderRequest["ProductId"] = vm.SelectedProduct.ProductId;
        vm.OrderRequest["ProductCode"] = vm.SelectedProduct.ProductCode;
        vm.OrderRequest["ProductDescription"] = vm.SelectedProduct.ProductDescription;
        vm.OrderRequest["UnitPrice"] = vm.SelectedProduct.UnitPrice;
        vm.OrderRequest["Stocks"] = parseInt(vm.OrderRequestQuantity);
        vm.OrderRequest["OrderTransactionType"] = vm.OrderRequestTransactionType;
        vm.OrderRequest["IsActive"] = 1;

        InventoryService.SaveOrderRequest(vm.OrderRequest).then(
            function (data) {
                if (data.isSucess) {
                    alert("Data has been saved!");
                    vm.ResetManageFields();
                }
            }, function (error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
                alert(error);
        });
    }

    function _resetManageFields() {
        _getProductDetails(vm.OrderRequest.ProductId);
        vm.OrderRequest = {};
        vm.OrderRequestTransactionType = 1;
        vm.OrderRequestQuantity = 0;
        
    }
}