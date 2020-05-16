angular
    .module('InventoryApp')
    .controller('LoginController', LoginController)

LoginController.$inject = ['LoginService', '$scope', '$location', '$rootScope', '$cookies']

function LoginController(LoginService, $scope, $location, $rootScope, $cookies) {
    
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
                    $rootScope.FirstName = data.userDetailResult.FirstName;
                    $rootScope.LastName = data.userDetailResult.LastName;
                    $rootScope.RoleName = data.userDetailResult.UserRoleDetails.UserRoleName;
                    $rootScope.Cookie = $cookies.get('AuthenticateLoginDetails');
                    $rootScope.IsLoading = false;
                    debugger;
                    $location.url('/Inventory');
                } else {
                    alert(data.messageAlert);
                    $rootScope.IsLoading = false;
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