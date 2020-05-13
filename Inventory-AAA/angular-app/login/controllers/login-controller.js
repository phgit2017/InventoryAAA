angular
    .module('InventoryApp')
    .controller('LoginController', LoginController)

LoginController.$inject = ['$scope', '$location']

function LoginController($scope, $location) {
    
    let vm = this, controllerName = 'loginCtrl';

    vm.Username = "Test";
    vm.Password = "TestPW";

    vm.Login = _login;
    vm.Logout = _logout;

    function _login() {
        $location.url('/Users')
    };

    function _logout() {
        $location.url('/Login')
    }
}