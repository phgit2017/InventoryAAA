angular
    .module('InventoryApp')
    .controller('ReportController', ReportController)

ReportController.$inject = ['ReportService', 'CommonService', 'MaintenanceService', '$scope', '$window', '$http', 'QuickAlert', 'globalBaseUrl'];

function ReportController(ReportService, CommonService, MaintenanceService, $scope, $window, $http, QuickAlert, globalBaseUrl) {
    var vm = this,
        controllerName = "reportCtrl";

    vm.StartDate = "";
    vm.EndDate = "";
    vm.ReportType = 0; // 0 - SalesReport; 1 - InventoryReport; 2 - SalesInventoryReport;
    vm.Report = {};
    vm.ReportFilterLabel = "";
    vm.ReportFilter = [];
    vm.SelectedReportFilter = 0;
    vm.SalesNo = 0;
    vm.CategoryList = [];
    vm.CategoryFilter = "";
    vm.SelectedCategory = {
        CategoryId : 0,
        CategoryName : ""
    };
    vm.SelectedCategoryInv = {
        CategoryId : 0,
        CategoryName : ""
    };
    vm.CustomerList = [];
    vm.SelectedCustomer = {
        CustomerId : 0,
        CustomerCode : "",
        FullName : ""
    };
    vm.CustomerFilter = "";
    vm.StatusList = [];
    vm.SelectedStatus = 0;

    $scope.$watch(
        function() {
            return vm.SelectedReportFilter;
        },
        function(oldValue, newValue) {
            if (oldValue != newValue) {
                vm.SalesNo = 0;
                vm.SelectedCustomer = {
                    CustomerId : 0,
                    CustomerCode : "",
                    FullName : ""
                };
                vm.SelectedCategory = {
                    CategoryId : 0,
                    CategoryName : ""
                };
                vm.SelectedCategoryInv = {
                    CategoryId : 0,
                    CategoryName : ""
                };
                vm.StartDate = "";
                vm.EndDate = "";
            }
        }
    );

    vm.Initialize = function() {
        ReportService.InitializeReportPage().then(
            function(data) {
                vm.ReportFilterLabel = "--Select A Report Filter--";

                vm.ReportFilter = [{
                        FilterId: 1,
                        FilterLabel: "By Sales No."
                    },
                    {
                        FilterId: 2,
                        FilterLabel: "By Customer and Date"
                    },
                    {
                        FilterId: 3,
                        FilterLabel: "By Date"
                    },
                    {
                        FilterId: 4,
                        FilterLabel: "By Category and Date"
                    },
                ];

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

                getCustomerList();
                getCategoryList();
            },
            function(err) {
                QuickAlert.Show({
                    type: 'error',
                    message: err
                })
            });
    }

    vm.GenerateReport = function() {
        var startDateString = vm.StartDate,
            endDateString = vm.EndDate,
            startDate = new Date(startDateString),
            endDate = new Date(endDateString);
        if (vm.ReportType !== 2 || (vm.ReportType === 2 && (vm.SelectedReportFilter === 3 || vm.SelectedReportFilter === 4))) {
            if (dateDiffDays(startDate, endDate) > 30) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'Date range can only be a maximum of 30 days.'
                });
                return;
            } else if ((startDate - endDate) > 0) {
                QuickAlert.Show({
                    type: 'error',
                    message: 'End date should be after Start Date.'
                });
                return;
            }
        }

        sendResponse();
    }

    vm.ResetFields = function() {
        vm.StartDate = "";
        vm.EndDate = "";
        vm.SelectedCategory = {
            CategoryId : 0,
            CategoryName : ""
        };
        vm.SelectedCategoryInv = {
            CategoryId : 0,
            CategoryName : ""
        };
    }

    vm.ValidReportParameters = function() {
        switch (vm.ReportType) {
            case 0:
            case 1:
                return vm.StartDate === "" || vm.EndDate === "" ? false : true
            case 2:
                switch (vm.SelectedReportFilter) {
                    case 1:
                        return vm.SalesNo === 0 || vm.SalesNo === '' ? false : true;
                    case 2:
                        return vm.StartDate === "" || vm.EndDate === "" || vm.SelectedCustomer.CustomerId === 0 || vm.SelectedStatus === 0 ? false : true;
                    case 3:
                        return vm.StartDate === "" || vm.EndDate === "" || vm.SelectedStatus === 0 ? false : true;
                    case 4:
                        return vm.StartDate === "" || vm.EndDate === "" || vm.SelectedCategory.CategoryId === 0 || vm.SelectedStatus === 0 ? false : true
                }
        }
    }

    // Private Functions
    sendResponse = function() {
        switch (vm.ReportType) {
            case 0:
                url = globalBaseUrl + '/Report/GenerateSalesReport' +
                    '?startDate=' + vm.StartDate +
                    '&endDate=' + vm.EndDate;
                break;
            case 1:
                url = globalBaseUrl + '/Report/GeneratePurchaseAndSalesReport' +
                    '?startDate=' + vm.StartDate +
                    '&endDate=' + vm.EndDate +
                    '&categoryId=' + vm.SelectedCategoryInv.CategoryId;
                break;
            case 2:
                url = globalBaseUrl + '/Report/GenerateSalesOrder?' +
                    '&reportSalesType=' + vm.SelectedReportFilter +
                    '&salesNo=' + vm.SalesNo +
                    '&customerId=' + vm.SelectedCustomer.CustomerId +
                    '&startDate=' + vm.StartDate +
                    '&endDate=' + vm.EndDate +
                    '&categoryId=' + vm.SelectedCategory.CategoryId +
                    '&salesOrderStatusId=' + vm.SelectedStatus;
                break;
        }
        downloadFile(url);
    }

    getCustomerList = function() {
        CommonService.GetCustomerList().then(
            function(data) {
                vm.CustomerList = data.result;
            },
            function(error) {
                alert(error);
                QuickAlert.Show({
                    type: 'error',
                    message: 'Error fetching Customer List from Server.'
                });
            }
        );
    }

    getCategoryList = function() {
        MaintenanceService.GetCategoryList().then(
            function(data) {
                vm.CategoryList = data.CategoryDetailsResult;
            },
            function(error) {
                vm.LoaderErrorMessage = error;
            }
        )
    }

    dateDiffDays = function(dt1, dt2) {
        return Math.floor((Date.UTC(dt2.getFullYear(), dt2.getMonth(), dt2.getDate()) - Date.UTC(dt1.getFullYear(), dt1.getMonth(), dt1.getDate())) / (1000 * 60 * 60 * 24));
    }

    downloadFile = function(url) {
        var a = document.createElement('a');
        a.href = url;
        a.target = '_blank';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }

}