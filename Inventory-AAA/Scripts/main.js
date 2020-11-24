var app =
    angular
    .module("InventoryApp", ["ngRoute", "datatables", "datatables.options", "ngCookies", "ui.bootstrap"])
    .value('globalBaseUrl', '')
    //.value('globalBaseUrl', '/Inventory-AAA')
    .config(function($routeProvider, $locationProvider) {
        var globalBaseUrl = '/';
        $locationProvider.hashPrefix('');
        $routeProvider
            .when("/", {
                redirectToUrl: "/Login"
            })
            .when("/Home", {
                title: "Home",
                templateUrl: "angular-app/home/home.html",
                controller: "HomeController",
                controllerAs: "homeCtrl",
                showNavbar: true
            })
            .when("/Login", {
                title: "Login",
                templateUrl: "angular-app/login/views/login.html",
                controller: "LoginController",
                controllerAs: "loginCtrl",
                showNavbar: false
            })
            .when("/Inventory", {
                title: "Inventory Summary",
                templateUrl: "angular-app/inventory/views/inventory-main.html",
                controller: "InventoryController",
                controllerAs: "inventoryCtrl",
                showNavbar: true,
                navItem: "Inventory"
            })
            .when("/Customers", {
                title: "Customers",
                templateUrl: "angular-app/customer/views/customers.html",
                controller: "CustomersController",
                controllerAs: "customersCtrl",
                showNavbar: true,
                navItem: "Customers"
            })
            .when("/SalesOrder", {
                title: "SalesOrders",
                templateUrl: "angular-app/sales/views/sales-order.html",
                controller: "SalesOrderController",
                controllerAs: "salesOrderCtrl",
                showNavbar: true,
                navItem: "SalesOrder"
            })
            .when("/OrderDetails/:salesOrderId", {
                title: "SalesOrders",
                templateUrl: "angular-app/sales/views/order-details.html",
                controller: "OrderDetailsController",
                controllerAs: "orderDetailsCtrl",
                showNavbar: true,
            })
            .when("/Users", {
                title: "Users",
                templateUrl: "angular-app/user/views/user.html",
                controller: "UserController",
                controllerAs: "userCtrl",
                showNavbar: true,
                navItem: "Users"
            })
            .when("/Reports", {
                title: "Reports",
                templateUrl: "angular-app/report/views/report.html",
                controller: "ReportController",
                controllerAs: "reportCtrl",
                showNavbar: true,
                navItem: "Reports"
            })
            .when("/Unauthorized", {
                title: "Unauthorized",
                templateUrl: "angular-app/shared/views/unauthorized.html",
                controller: "LoginController",
                controllerAs: "loginCtrl",
                showNavbar: false,
            })
            .otherwise({
                redirectTo: "/"
            })
    })
    .run(['$rootScope', '$location', '$cookies', function($rootScope, $location, $cookies) {
        $rootScope.$on('$routeChangeSuccess', function(event, current, previous) {
            $rootScope.title = current.$$route.title;
            $rootScope.showNavbar = current.$$route.showNavbar;
            $rootScope.navItem = current.$$route.navItem;
            $rootScope.IsLoading = false;

            $rootScope.FullName = $cookies.get('UserFullName');
            $rootScope.RoleName = $cookies.get('UserRoleName');

            //if (!AuthService.IsLoggedIn()) {
            //    console.log('UNAUTHORIZED');
            //    event.preventDefault();
            //    $location.url('/Login')
            //} else {
            //    console.log('AUTHORIZED');
            //    $location.url(current.$$route.originalPath)
            //}

            if (current.$$route.redirectToUrl != undefined) {
                $location.url(current.$$route.redirectToUrl);
            }
        });
    }]);

// Loader Directive

app.directive('loading', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            showme: '=',
            showerror: '=',
            errormessage: '='
        },

        templateUrl: 'angular-app/shared/views/loader-directive.html',
    }
});

app.directive('pageLoading', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            showme: '=',
        },

        templateUrl: 'angular-app/shared/views/page-loader-directive.html',
    }
});

// Quick Alerts

app.directive('quickAlert', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            showme: '=',
            message: '='
        },
        replace: true, // Replace with template
        transclude: true, // To use custom content
        link: function(scope, element, attrs) {
            scope.showQuickAlert = false;
            scope.message = "";
        },
        templateUrl: 'angular-app/shared/views/quick-alert-directive.html',
    }
});


// Modal Directive

app.directive('modalWindow', function() {
    return {
        restrict: 'E',
        scope: {
            show: '='
        },
        replace: true, // Replace with template
        transclude: true, // To use custom content
        link: function(scope, element, attrs) {

            scope.windowStyle = {};

            if (attrs.width) {
                scope.windowStyle.width = attrs.width;
            }
            if (attrs.height) {
                scope.windowStyle.height = attrs.height;
            }

            scope.hideModal = function() {
                scope.show = false;
            };
        },
        template: "<div ng-show='show'><div class='modal-overlay'></div><div class='modal-container'><div class='modal-box' ng-transclude></div></div></div>"
    };
});

// Confirm Directive 

app.directive('confirmAlert', function($rootScope) {
    return {
        restrict: 'E',
        scope: {
            show: '=',
            alertMsg: '=',
            okFunction: '&',
            cancelFunction: '&'
        },
        replace: true, // Replace with template
        transclude: true, // To use custom content
        link: function(scope, element, attrs) {

            scope.hideModal = function() {
                scope.show = false;
            };

            scope.cancelClicked = function() {
                scope.hideModal();
                scope.cancelFunction();
            }

            scope.okClicked = function() {
                $rootScope.IsLoading = true;
                scope.hideModal();
                scope.okFunction({ result: true });
            }
        },
        template: `<div ng-show='show'>
             <div class='modal-overlay'> </div>
             <div class='confirm-modal-container'>
             <div class='modal-box' ng-transclude>
              <div class='card p-2'>
                  <div class='card-body' style='color: black;'>
                      <p>{{ alertMsg }}</p>
                      <div class='d-flex justify-content-end mt-4'>
                          <button class='btn btn-alert btn-sm btn-primary mr-2' ng-click='okClicked()'>Yes</button>
                          <button class='btn btn-alert btn-sm btn-danger' ng-click='cancelClicked();'>No</button>
                      </div>
                  </div>
              </div>
             </div></div></div>`
    };
});

app.directive('datePicker', function() {
    return {
        restrict: 'E',
        scope: {
            elementId: '='
        },
        link: function(scope, element, attrs) {
            $(function() {
                $("#dp_" + scope.elementId).datepicker();
            });

            scope.show_dp = function() {
                $("#dp_" + scope.elementId).datepicker('show'); //Show on click of button
            }
        }
    };
});