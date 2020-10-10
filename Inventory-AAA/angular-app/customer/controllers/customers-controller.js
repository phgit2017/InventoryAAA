angular
    .module('InventoryApp')
    .controller('CustomersController', CustomersController);

CustomersController.$inject = ['$scope', '$rootScope', 'CustomerService', 'DTOptionsBuilder', 'DTColumnDefBuilder', 'QuickAlert'];

function CustomersController($scope, $rootScope, CustomerService, DTOptionsBuilder, DTColumnDefBuilder, QuickAlert) {

    var vm = this,
        controllerName = 'customerCtrl';

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
                console.log(vm.CustomerList);
                vm.CustomerListLoading = false;
                $rootScope.IsLoading = false;
            },
            function(error) {
                alert(error);
                $rootScope.IsLoading = false;
            }
        );
    }

    vm.dtUserListOptions = DTOptionsBuilder.newOptions()
        .withPaginationType('simple_numbers')
        .withDisplayLength(10)
        .withDOM("<'row'<'col-sm-12 col-md-6'f><'col-sm-12 col-md-6'p><'table-details't>>");

    vm.dtUserListColumnDefs = [
        DTColumnDefBuilder.newColumnDef(5).notSortable(),
    ];
}