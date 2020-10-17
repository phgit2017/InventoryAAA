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
        getCustomerList();
    }

    vm.ResetFields = function() {
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
    }

    vm.SelectCustomer = function(data) {
        vm.SelectedCustomer = {
            CustomerId: data.CustomerId,
            CustomerCode: data.CustomerCode,
            FirstName: data.FirstName,
            LastName: data.LastName,
            FullAddress: data.FullAddress,
            IsActive: data.IsActive,
            CreatedBy: data.CreatedBy,
            CreatedTime: data.CreatedTime
        };
    }

    vm.SaveCustomer = function() {
        var mode = vm.SelectedCustomer.CustomerId === 0 ? 'Add' : 'Update';

        if (validCustomerDetails()) {
            CustomerService.SaveCustomer(vm.SelectedCustomer, mode).then(
                function(data) {
                    QuickAlert.Show({
                        type: 'success',
                        message: data.messageAlert
                    });
                    getCustomerList();
                    $scope.customerForm.$setPristine();
                    vm.ResetFields();
                },
                function(error) {
                    QuickAlert.Show({
                        type: 'error',
                        message: error
                    });
                }
            )
        } else {
            QuickAlert.Show({
                type: 'error',
                message: 'Please fill up all the fields!'
            });
        }
    }

    validCustomerDetails = function() {
        if (isNullOrEmpty(vm.SelectedCustomer.FirstName) ||
            isNullOrEmpty(vm.SelectedCustomer.LastName) ||
            isNullOrEmpty(vm.SelectedCustomer.CustomerCode) ||
            isNullOrEmpty(vm.SelectedCustomer.FullAddress)) {
            return false
        } else {
            return true;
        }
    }

    getCustomerList = function() {
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

    function isNullOrEmpty(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }

}