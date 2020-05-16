angular
    .module('InventoryApp')
    .controller('UserController', UserController);

UserController.$inject = ['UserService', 'DTOptionsBuilder', 'DTDefaultOptions' ,'$scope'];

function UserController(UserService, DTOptionsBuilder, DTDefaultOptions, $scope) {

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
    vm.ConfirmPassword = "";

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
        UserService.GetUserList().then(
            function (data) {
                vm.UserList = data.UserDetailsResult;
                console.log(vm.UserList);
                vm.UserListLoading = false;
            }, function (error) {
                console.log(error);
            }
        );
    }

    function _saveUser() {
        if (ValidUserDetails()) {
            if (vm.ConfirmPassword != vm.SelectedUser.Password) {
                if (vm.SelectedUser.UserId === 0) {
                    _addNewUser();
                } else {
                    _updateUser();
                }
            } else {
                alert("Passwords do not match!");
            }
        } else {
            alert("Please fill up all fields!");
        }
    }

    function _addNewUser() {
        UserService.AddNewUser(vm.SelectedUser).then(
            function (data) {
                if (data.isSuccess) {
                    alert("User has been successfully added.");
                    vm.UserListLoading = true;
                    _initialize();
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
                    alert("User has been successfully updated.");
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