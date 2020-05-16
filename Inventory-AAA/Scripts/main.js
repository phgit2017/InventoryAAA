var app =
    angular
        .module("InventoryApp", ["ngRoute", "datatables", "datatables.options", "ngCookies"])
        .config(function ($routeProvider, $locationProvider) {
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
                .otherwise({
                    redirectTo: "/"
                })
        })
        .run(['$rootScope', '$location', '$cookies', function ($rootScope, $location, $cookies) {
            $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
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

app.directive('loading', function () {
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

app.directive('pageLoading', function () {
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

app.directive('quickAlert', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            showme: '=',
            message: '='
        },
        replace: true, // Replace with template
        transclude: true, // To use custom content
        link: function (scope, element, attrs) {
            scope.showQuickAlert = false;
            scope.message = "";
        },
        templateUrl: 'angular-app/shared/views/quick-alert-directive.html',
    }
});


// Modal Directive

app.directive('modalWindow', function () {
    return {
        restrict: 'E',
        scope: {
            show: '='
        },
        replace: true, // Replace with template
        transclude: true, // To use custom content
        link: function (scope, element, attrs) {

            scope.windowStyle = {};

            if (attrs.width) {
                scope.windowStyle.width = attrs.width;
            }
            if (attrs.height) {
                scope.windowStyle.height = attrs.height;
            }

            scope.hideModal = function () {
                scope.show = false;
            };
        },
        template: "<div ng-show='show'><div class='modal-overlay' ng-click='hideModal()'></div><div class='modal-container'><div class='modal-box' ng-transclude></div></div></div>"
    };
});