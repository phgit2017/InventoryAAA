angular
    .module('InventoryApp')
    .controller('CustomersController', CustomersController);

CustomersController.$inject = ['$scope', '$rootScope', 'CustomerService', 'DTOptionsBuilder', 'DTColumnDefBuilder', 'QuickAlert'];

function CustomersController($scope, $rootScope, CustomerService, DTOptionsBuilder, DTColumnDefBuilder, QuickAlert) {

    var vm = this,
        controllerName = 'customersCtrl';

    vm.dtUserListOptions = "";
    vm.dtUserListColumnDefs = "";

    vm.CustomerListLoading = true;

    vm.CustomerList = [];
    vm.SelectedCustomer = {
        CustomerId: 0,
        CustomerCode: "",
        FirstName: "",
        LastName: "",
        FullAddress: "",
        IsActive: true,
        CreatedBy: 0,
        CreatedTime: ""
    };

    vm.ShowConfirmAlert = false;

    vm.Initialize = _initialize;

    function _initialize() {
        _getCustomerList();
    }

    function _getCustomerList() {
        $rootScope.IsLoading = true;
        CustomerService.GetCustomerList().then(
            function(data) {
                vm.CustomerList = data.CustomerDetailsResult;
                vm.CustomerListLoading = false;
                $rootScope.IsLoading = false;
            },
            function(error) {
                alert(error);
                $rootScope.IsLoading = false;
            }
        );
    }
}