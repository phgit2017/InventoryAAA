angular
    .module('InventoryApp')
    .controller('LoginController', LoginController)

LoginController.$inject = ['LoginService','$scope', '$location']

function LoginController(LoginService, $scope, $location) {
    
    let vm = this, controllerName = 'loginCtrl';

    vm.LoginDetails = {
        UserName: "",
        Password: ""
    }

    vm.Login = _login;
    vm.Logout = _logout;

    function _login() {
        LoginService.Login(vm.LoginDetails).then(
            function (data) {
                $location.url('/Login');
            },
            function (err) {
                console.log(err);
                alert("login failed. invalid username/password.")
            });
        //$location.url('/Users')
    };

    function _logout() {
        $location.url('/Login')
    }
}