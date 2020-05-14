angular
    .module('InventoryApp')
    .factory('LoginService', LoginService)

LoginService.$inject = ['$http', '$q'];

function LoginService($http, $q) {

    var LoginServiceFactory = {},
        baseUrl = "/User"

    LoginServiceFactory.Login = _login;
    LoginServiceFactory.Logout = _logout;

    return LoginServiceFactory;

    function _login(data) {
        var defer = $q.defer(),
            url = baseUrl + "/AuthenticateLogin";
        debugger;
        $http.post(url, data)
            .then(function (response) {
                debugger;
                defer.resolve(response.data);
            }), function (err) {
                defer.reject(err);
            };
        return defer.promise;
    }

    function _logout() {

    }

}