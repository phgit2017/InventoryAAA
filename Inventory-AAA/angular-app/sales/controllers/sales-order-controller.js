angular
    .module('InventoryApp')
    .controller('SalesOrderController', SalesOrderController);

SalesOrderController.$inject = ['$scope', '$rootScope', 'SalesOrderService', 'QuickAlert'];

function SalesOrderController($scope, $rootScope, SalesOrderService, QuickAlert) {

    var vm = this,
        controllerName = 'salesOrderCtrl';

    function isNullOrEmpty(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }

}