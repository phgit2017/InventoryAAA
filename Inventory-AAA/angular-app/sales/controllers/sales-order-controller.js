angular
    .module('InventoryApp')
    .controller('SalesOrderController', SalesOrderController);

SalesOrderController.$inject = ['$filter', '$scope', '$rootScope', '$location', 'SalesOrderService', 'QuickAlert'];

function SalesOrderController($filter, $scope, $rootScope, $location, SalesOrderService, QuickAlert) {

    var vm = this,
        controllerName = 'salesOrderCtrl';

    vm.FilteredSalesOrders = [];
    vm.SalesOrders = [];
    vm.SalesOrdersLoading = true;
    vm.SearchSalesOrdersInput = "";
    vm.TableMode = "Undelivered";
    vm.currentPage = 1;
    vm.numPerPage = 10;
    vm.maxSize = 5;

    vm.FilterToggled = false;
    vm.FilterApplied = false;
    vm.FilterAllOrders = {
        StartDate: null,
        EndDate: null,
        SalesOrderStatusId: null
    }

    vm.StatusList = [{
        StatusId: 1,
        StatusName: "Pending"
        },
        {
            StatusId: 2,
            StatusName: "Paid"
        },
        {
            StatusId: 3,
            StatusName: "Shipped"
        },
        {
            StatusId: 4,
            StatusName: "Delivered"
        },
        {
            StatusId: 5,
            StatusName: "Cancelled"
        },
    ]

    //Watches

    $scope.$watch(
        function() {
            return vm.TableMode;
        },
        function(oldValue, newValue) {
            if (oldValue !== newValue) {
                switch(vm.TableMode) {
                    case 'Undelivered' : 
                        getSalesOrders();
                        break;
                    case 'Delivered' : 
                        getAllSalesOrders();
                }
            }
        }
    );

    $scope.$watch(
        function() {
            return vm.SearchSalesOrdersInput;
        },
        function(newValue,oldValue){                
            if(oldValue!=newValue){
                vm.FilteredSalesOrders = $filter('filter')(vm.SalesOrders, vm.SearchSalesOrdersInput);
                vm.currentPage = 1;
            }
        }
    );

    vm.Initialize = function() {
        getSalesOrders();
    }

    vm.ManageSalesOrder = function(salesOrderId) {
        $location.url('/OrderDetails/' + salesOrderId);
    }

    getSalesOrders = function() {
        vm.SalesOrdersLoading = true;
        SalesOrderService.GetSalesOrders().then(
            function(data) {
                vm.SalesOrders = data.result;
                vm.FilteredSalesOrders =  data.result;
                vm.SalesOrdersLoading = false;
            },
            function(error) {
                vm.SalesOrdersLoading = false;
                QuickAlert.Show({
                    type: 'error',
                    message: error
                });
            },
        )
    }

    getAllSalesOrders = function() {
        vm.SalesOrdersLoading = true;   
        var filter = {
            StartDate: new Date(vm.FilterAllOrders.StartDate),
            EndDate: new Date(vm.FilterAllOrders.EndDate),
            SalesOrderStatusId: vm.FilterAllOrders.SalesOrderStatusId
        }
        SalesOrderService.GetAllSalesOrders(filter).then(
            function(data) {
                vm.SalesOrders = data.result;
                vm.FilteredSalesOrders =  data.result;
                vm.SalesOrdersLoading = false;
            },
            function(error) {
                vm.SalesOrdersLoading = false;
                QuickAlert.Show({
                    type: 'error',
                    message: error
                });
            },
        )
    }

    vm.ClearFilters = function() {
        vm.FilterStartDate = null;
        vm.FilterEndDate = null;
        vm.FilterStatus = null;
    }

    vm.GetStatusClass = function(statusId) {
        return {
            'fab-pending': statusId === 0 || statusId === 1,
            'fab-success': statusId === 2 || statusId === 3 || statusId === 4,
            'fab-danger': statusId === 5
        }
    }

    vm.ApplyFilter = function() {
        var startDateString = vm.FilterStartDate,
        endDateString = vm.FilterEndDate,
        startDate = new Date(startDateString),
        endDate = new Date(endDateString);

        if (startDate > endDate) {
            QuickAlert.Show({
                type: 'error',
                message: 'End date should be after Start Date.'
            });
            return;
        } else  {
            getAllSalesOrders();
            vm.FilterToggled = false;
            vm.FilterApplied = true;
        }
    }

    vm.RemoveFilter = function() {
        vm.ClearFilters();
        getAllSalesOrders();
        vm.FilterToggled = false;
    }

    vm.SliceSalesOrders = function() {
        var begin = ((vm.currentPage - 1) * vm.numPerPage),
        end = begin + vm.numPerPage;

        vm.FilteredSalesOrders = vm.SalesOrders.slice(begin, end);
    }

    isNullOrEmpty = function(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }

}