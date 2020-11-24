angular
    .module('InventoryApp')
    .controller('SalesOrderController', SalesOrderController);

SalesOrderController.$inject = ['$scope', '$rootScope', '$location', 'SalesOrderService', 'QuickAlert'];

function SalesOrderController($scope, $rootScope, $location, SalesOrderService, QuickAlert) {

    var vm = this,
        controllerName = 'salesOrderCtrl';

    vm.FilteredSalesOrders = [];
    vm.SalesOrders = [];
    vm.SalesOrdersLoading = true;
    vm.SearchSalesOrdersInput = "";
    vm.TableMode = "Undelivered";

    vm.filteredUsers = [];
    vm.currentPage = 1;
    vm.numPerPage = 10;
    vm.maxSize = 5;

    vm.FilterToggled = false;
    vm.FilterStartDate = null;
    vm.FilterEndDate = null;
    vm.FilterStatus = null;
    vm.FilterApplied = false;

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
                vm.FilteredSalesOrders = vm.SalesOrders;
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
            StartDate: vm.FilterStartDate,
            EndDate: vm.FilterEndDate,
            SalesOrderStatusId: vm.FilterStatus
        }
        SalesOrderService.GetAllSalesOrders(filter).then(
            function(data) {
                vm.SalesOrders = data.result;
                vm.FilteredSalesOrders = vm.SalesOrders;
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

    isNullOrEmpty = function(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }

}