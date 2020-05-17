angular
    .module('InventoryApp')
    .controller('LoginController', LoginController)

LoginController.$inject = ['LoginService', '$scope', '$location', '$rootScope', '$cookies', 'QuickAlert']

function LoginController(LoginService, $scope, $location, $rootScope, $cookies, QuickAlert) {
    
    let vm = this, controllerName = 'loginCtrl';

    vm.LoginDetails = {
        UserName: "",
        Password: ""
    }

    vm.Login = _login;
    vm.Logout = _logout;
    vm.ChangeRoute = _changeRoute;

    function _login() {
        $rootScope.IsLoading = true;
        LoginService.Login(vm.LoginDetails).then(
            function (data) {
                if (data.isSuccess) {
                    $rootScope.IsLoading = false;
                    $location.url('/Inventory');
                } else {
                    QuickAlert.Show({
                        type: "error",
                        message: "Invalid User Details"
                    })
                    $rootScope.IsLoading = false;
                }
            },
            function (err) {
                QuickAlert.Show({
                    type: 'error',
                    message: err
                });
            });
    };

    function _logout() {
        LoginService.Logout().then(
            function (data) {
                $location.url("/Login");
            }, function (err) {
                QuickAlert.Show({
                    type: 'error',
                    message: err
                });
            }
        );
    }

    function _changeRoute(route) {
        $location.url('/' + route);
    }
}