angular
    .module('InventoryApp')
    .factory('UserService', UserService)

UserService.$inject = ['$q', '$http'];

function UserService($q, $http) {
    var UserServiceFactory = {},
        baseUrl = "/Inventory-AAA/User";

    UserServiceFactory.GetUserList = _getUserList;
    UserServiceFactory.AddNewUser = _addNewUser;
    UserServiceFactory.UpdateUser = _updateUser;

    return UserServiceFactory;

    function _getUserList() {
        var defer = $q.defer(),
            url = baseUrl + '/UserList';

        $http.post(url)
            .then(function (response) {
                defer.resolve(response.data)
            }), function (err) {
                defer.reject(err);
            }
        return defer.promise;
    }

    function _addNewUser(data) {
        var defer = $q.defer(),
            url = baseUrl + '/AddNewUserDetails';

        $http.post(url, data)
            .then(function (response) {
                defer.resolve(response.data)
            }), function (err) {
                defer.reject(err);
            }
        return defer.promise;
    }

    function _updateUser(data) {
        var defer = $q.defer(),
            url = baseUrl + '/UpdateUserDetails';

        $http.post(url, data)
            .then(function (response) {
                defer.resolve(response.data)
            }), function (err) {
                defer.reject(err);
            }
        return defer.promise;
    }
}