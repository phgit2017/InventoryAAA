angular
    .module('InventoryApp')
    .controller('CustomersController', CustomersController);

CustomersController.$inject = ['$filter', '$scope', '$rootScope', 'CustomerService', 'CommonService', 'QuickAlert'];

function CustomersController($filter, $scope, $rootScope, CustomerService, CommonService, QuickAlert) {

    var vm = this,
        controllerName = 'customersCtrl';

    vm.CustomerListLoading = true;

    vm.CustomerList = [];
    vm.filteredCustomerList = [];
    vm.CustomerStatusList = [];
    vm.SelectedCustomer = {
        CustomerId: 0,
        CustomerCode: "",
        FirstName: "",
        LastName: "",
        FullAddress: "",
        MobileNumber: "",
        IsActive: true,
        CreatedBy: 0,
        CreatedTime: "",
        CustomerStatusId: 1
    };
    vm.SearchCustomerInput = '';
    vm.currentPage = 1;
    vm.numPerPage = 10;
    vm.maxSize = 5;

    vm.ShowConfirmAlert = false;
    vm.ManageBarShown = false;

    vm.Initialize = _initialize;

    $scope.$watch(
        function() {
            return vm.SearchCustomerInput;
        },
        function(newValue,oldValue){                
            if(oldValue!=newValue){
                vm.filteredCustomerList = $filter('filter')(vm.CustomerList, vm.SearchCustomerInput);
                vm.currentPage = 1;
            }
        }
    );

    function _initialize() {
        getCustomerStatusList();
        getCustomerList();
    }

    vm.ResetFields = function() {
        vm.SelectedCustomer = {
            CustomerId: 0,
            CustomerCode: "",
            FirstName: "",
            LastName: "",
            FullAddress: "",
            MobileNumber: "",
            IsActive: true,
            CreatedBy: 0,
            CreatedTime: "",
            CustomerStatusId: 1
        };
    }

    vm.SelectCustomer = function(data) {
        
        vm.SelectedCustomer = {
            CustomerId: data.CustomerId,
            CustomerCode: data.CustomerCode,
            FirstName: data.FirstName,
            LastName: data.LastName,
            FullAddress: data.FullAddress,
            MobileNumber: data.MobileNumber,
            IsActive: data.IsActive,
            CreatedBy: data.CreatedBy,
            CreatedTime: data.CreatedTime,
            CustomerStatusId: data.CustomerStatusId
        };

        vm.ManageBarShown = true;
    }

    vm.SaveCustomer = function() {
        var mode = vm.SelectedCustomer.CustomerId === 0 ? 'Add' : 'Update';

        if (validCustomerDetails()) {
            CustomerService.SaveCustomer(vm.SelectedCustomer, mode).then(
                function(data) {
                    if (data.isSuccess) {
                        QuickAlert.Show({
                            type: 'success',
                            message: 'Customer has been added.'
                        });
                        getCustomerList();
                        $scope.customerForm.$setPristine();
                        vm.ResetFields();
                        vm.ManageBarShown = false;
                    } else {
                        QuickAlert.Show({
                            type: 'error',
                            message: data.messageAlert
                        });
                    }

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
        debugger;
        if (isNullOrEmpty(vm.SelectedCustomer.FirstName) ||
            isNullOrEmpty(vm.SelectedCustomer.LastName) ||
            isNullOrEmpty(vm.SelectedCustomer.CustomerCode) ||
            isNullOrEmpty(vm.SelectedCustomer.FullAddress) ||
            isNullOrEmpty(vm.SelectedCustomer.MobileNumber) || 
            vm.SelectedCustomer.MobileNumber.length < 11) {
            return false
        } else {
            return true;
        }
    }

    getCustomerList = function() {
        vm.CustomerListLoading = true;
        CustomerService.GetCustomerList().then(
            function(data) {
                vm.CustomerList = data.CustomerDetailsResult;
                vm.filteredCustomerList = data.CustomerDetailsResult;
                vm.CustomerListLoading = false;
                $rootScope.IsLoading = false;
            },
            function(error) {
                alert(error);
                $rootScope.IsLoading = false;
            }
        );
    }

    getCustomerStatusList = function() {
        CommonService.GetCustomerStatusList().then(
            function(data) {
                vm.CustomerStatusList = data.result;
            },
            function(error) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'Server error.'
                });
            }
        )
    }

    function isNullOrEmpty(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }

}