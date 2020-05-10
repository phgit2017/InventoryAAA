var app =
    angular
        .module("InventoryApp", ["ngRoute", "datatables"])
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

app.controller('WithOptionsCtrl', WithOptionsCtrl);

function WithOptionsCtrl(DTOptionsBuilder, DTColumnDefBuilder) {
    var vm = this;
    vm.dtOptions = DTOptionsBuilder.newOptions()
        .withPaginationType('simple_numbers')
        .withDisplayLength(10)
        .withDOM("<'row'<'col-sm-12 col-md-6'f><'col-sm-12 col-md-6'l>t>p");
    vm.dtColumnDefs = [
        DTColumnDefBuilder.newColumnDef(0),
        //DTColumnDefBuilder.newColumnDef(1).notVisible(),
        DTColumnDefBuilder.newColumnDef(1).notSortable()
    ];
}