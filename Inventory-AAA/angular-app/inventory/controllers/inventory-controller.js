angular
    .module('InventoryApp')
    .controller('InventoryController', InventoryController);

InventoryController.$inject = ['InventoryService', 'DTOptionsBuilder', 'DTDefaultOptions', '$http', '$q'];

function InventoryController(InventoryService, DTOptionsBuilder, DTDefaultOptions, $http, $q) {
    var vm = this, controllerName = 'InventoryCtrl';

    // View Items
    vm.InventorySummary = [];
    vm.SelectedProduct = [];
    vm.OrderRequestQuantity = '';
    vm.OrderRequestTransactionType = 0;
    vm.OrderRequestUnitPrice = 0;
    vm.OrderRequest = {
        ProductID: 0,
        ProductCode: '',
        ProductDescription: '',
        Stocks: 0,
        UnitPrice: 0,
        OrderTransactionType: 0,
        IsActive: 1
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
    vm.ToggleEditDetailsShown = _toggleEditDetails;

    // API methods
    vm.GetInventorySummary = _getInventorySummary;
    vm.SaveOrderRequest = _saveOrderRequest;
    // vm.GetProductDetails = _getProductDetails; (TODO KEIGA: For Manage Modal)

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

    function _saveOrderRequest() {
        vm.OrderRequest.ProductID = vm.SelectedProduct.ProductID;
        vm.OrderRequest.ProductCode = vm.SelectedProduct.ProductCode;
        vm.OrderRequest.ProductDescription = vm.SelectedProduct.ProductDescription;
        vm.OrderRequest.UnitPrice = vm.OrderRequestUnitPrice;
        vm.OrderRequest.Stocks = vm.OrderRequestQuantity;
        vm.OrderRequest.OrderTransactionType = vm.OrderRequestTransactionType;
        vm.IsActive = true;

        debugger;

        InventoryService.SaveOrderRequest(vm.OrderRequest).then(
            function (data) {
                debugger;
                vm.IsLoading = false;
            }, function (error) {
                vm.isLoading = false;
                vm.LoaderErrorMessage = "Error While Fetching Data from Server.";
                console.log(error);
            });
    }
}