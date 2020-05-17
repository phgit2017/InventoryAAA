angular
    .module('InventoryApp')
    .controller('UserController', UserController);

UserController.$inject = ['UserService', 'DTOptionsBuilder' ,'$scope', '$rootScope', 'QuickAlert'];

function UserController(UserService, DTOptionsBuilder, $scope, $rootScope, QuickAlert) {

    var vm = this, controllerName = 'userCtrl';

    vm.dtUserListOptions = "";
    vm.UserListLoading = true;

    vm.UserList = [];
    vm.SelectedUser = {
        UserId: 0,
        FirstName: "",
        LastName: "",
        UserName: "",
        Password: "",
        UserRoleId: 1,
        IsActive: true
    };

    vm.Initialize = _initialize;
    vm.ClearUserDetails = _clearUserDetails;
    vm.SelectUser = _selectUser;
    vm.SaveUser = _saveUser;

    vm.dtUserListOptions = DTOptionsBuilder.newOptions()
        .withPaginationType('simple_numbers')
        .withDisplayLength(10)
        .withDOM("<'row'<'col-sm-12 col-md-6'f><'col-sm-12 col-md-6'p><'table-details't>>");

    function _initialize() {
        _getUserList();
        _clearUserDetails();
    }

    function _getUserList() {
        $rootScope.IsLoading = true;
        UserService.GetUserList().then(
            function (data) {
                vm.UserList = data.UserDetailsResult;
                vm.UserListLoading = false;
                $rootScope.IsLoading = false;
            }, function (error) {
                alert(error);
                $rootScope.IsLoading = false;
            }
        );
    }

    function _saveUser() {
        if (ValidUserDetails()) {
            if (vm.SelectedUser.UserId === 0) {
                _addNewUser();
            } else {
                _updateUser();
            }
        } else {
            QuickAlert.Show({
                type: 'error',
                message: 'Please fill up all the fields!'
            });
        }
    }

    function _addNewUser() {
        $rootScope.IsLoading = true;
        UserService.AddNewUser(vm.SelectedUser).then(
            function (data) {
                if (data.isSuccess) {
                    QuickAlert.Show({
                        type: 'success',
                        message: 'User has been successfully added.'
                    });
                    vm.UserListLoading = true;
                    _initialize();
                    $rootScope.IsLoading = false;
                }
            }, function (err) {
                console.log(err);
            }
        );
    }

    function _updateUser() {
        UserService.UpdateUser(vm.SelectedUser).then(
            function (data) {
                if (data.isSuccess) {
                    QuickAlert.Show({
                        type: 'success',
                        message: 'User has been successfully updated.'
                    });
                    vm.UserListLoading = true;
                    _initialize();
                }
            }, function (err) {
                console.log(err);
            }
        );
    }

    function _selectUser(data) {
        vm.SelectedUser = {
            UserId: data.UserId,
            FirstName: data.FirstName,
            LastName: data.LastName,
            UserName: data.UserName,
            Password: data.Password,
            UserRoleId: data.UserRoleId,
            IsActive: data.IsActive,
        };
    }

    function ValidUserDetails() {
        if (isNullOrEmpty(vm.SelectedUser.FirstName) || 
            isNullOrEmpty(vm.SelectedUser.LastName) ||
            isNullOrEmpty(vm.SelectedUser.UserName) ||
            isNullOrEmpty(vm.SelectedUser.Password)) {
            return false
        } else {
            return true;
        }
    }

    function _clearUserDetails() {
        vm.SelectedUser = {
            UserId: 0,
            FirstName: "",
            LastName: "",
            UserName: "",
            Password: "",
            UserRoleId: 1,
            IsActive: true
        };
    }

    function isNullOrEmpty(data) {
        if (data === "" || data === undefined || data === null) {
            return true;
        } else {
            return false;
        }
    }
}