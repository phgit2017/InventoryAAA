var app =
    angular
        .module("InventoryApp", ["ngRoute", "datatables", "datatables.options"])
        .config(function ($routeProvider, $locationProvider) {
            $locationProvider.hashPrefix('');
            $routeProvider
                .when("/", {
                    title: "Inventory Summary",
                    controller: "InventoryController",
                    controllerAs: "inventoryCtrl"
                })
                .when("/Home", {
                    title: "Home",
                    templateUrl: "angular-app/home/home.html",
                    controller: "HomeController",
                    controllerAs: "homeCtrl"
                })
                .when("/Inventory", {
                    title: "Inventory Summary",
                    templateUrl: "angular-app/inventory/views/inventory-main.html",
                    controller: "InventoryController",
                    controllerAs: "inventoryCtrl"
                })
        })
        .run(['$rootScope', function ($rootScope) {
            $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
                $rootScope.title = current.$$route.title;
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