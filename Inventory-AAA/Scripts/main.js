var app =
    angular
        .module("InventoryApp", ["ngRoute"])
        .config(function ($routeProvider, $locationProvider) {
            $locationProvider.hashPrefix('');
            $routeProvider
                .when("/", {
                    title: "Login",
                    controller: "HomeController",
                    controllerAs: "homeCtrl"
                })
                .when("/Home", {
                    title: "Home",
                    templateUrl: "angular-app/home/home.html",
                    controller: "HomeController",
                    controllerAs: "homeCtrl"
                })
        })
        .run(['$rootScope', function ($rootScope) {
            $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
                $rootScope.title = current.$$route.title;
            });
        }]);