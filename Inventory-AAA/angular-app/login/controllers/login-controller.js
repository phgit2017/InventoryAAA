angular
    .module('InventoryApp')
    .controller('LoginController', LoginController)

LoginController.$inject = ['LoginService', '$scope', '$location', '$rootScope']

function LoginController(LoginService, $scope, $location, $rootScope) {
    
    let vm = this, controllerName = 'loginCtrl';

    vm.LoginDetails = {
        UserName: "",
        Password: ""
    }

    vm.Login = _login;
    vm.Logout = _logout;
    vm.ChangeRoute = _changeRoute;

    function _login() {
        LoginService.Login(vm.LoginDetails).then(
            function (data) {
                if (data.isSuccess) {
                    $location.url('/Users');
                } else {
                    alert(data.messageAlert);
                }
            },
            function (err) {
                alert(err);
            });
    };

    function _logout() {
        LoginService.Logout().then(
            function (data) {
                $location.url("/Login");
            }, function (err) {
                alert(err);
            }
        );
    }

    function _changeRoute(route) {
        $location.url('/' + route);
    }
}