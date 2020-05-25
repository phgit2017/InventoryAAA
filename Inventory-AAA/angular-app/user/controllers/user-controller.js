angular
    .module('InventoryApp')
    .controller('UserController', UserController);

UserController.$inject = ['UserService', 'DTOptionsBuilder', 'DTColumnDefBuilder' ,'$scope', '$rootScope', 'QuickAlert'];

function UserController(UserService, DTOptionsBuilder, DTColumnDefBuilder, $scope, $rootScope, QuickAlert) {

    var vm = this, controllerName = 'userCtrl';

    vm.dtUserListOptions = "";
    vm.UserListLoading = true;
    vm.dtUserListColumnDefs = "";

    vm.UserList = [];
    vm.SelectedUser = {
        UserId: 0,
        FirstName: "",
        LastName: "",
        UserName: "",
        Password: "",
        UserRoleId: 1,
        IsActive: true,
        CreatedBy: 0,
        CreatedTime: ""
    };
    vm.ShowConfirmAlert = false;

    vm.Initialize = _initialize;
    vm.ClearUserDetails = _clearUserDetails;
    vm.SelectUser = _selectUser;
    vm.SaveUser = _saveUser;
    vm.DeleteUser = _deleteUser;

    vm.dtUserListOptions = DTOptionsBuilder.newOptions()
        .withPaginationType('simple_numbers')
        .withDisplayLength(10)
        .withDOM("<'row'<'col-sm-12 col-md-6'f><'col-sm-12 col-md-6'p><'table-details't>>");

    vm.dtUserListColumnDefs = [
        DTColumnDefBuilder.newColumnDef(5).notSortable(),
    ];

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

            $scope.userForm.$setPristine();
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
                } else {
                    QuickAlert.Show({
                        type: 'error',
                        message: data.messageAlert
                    });
                    $rootScope.IsLoading = false;
                }
            }, function (err) {
                QuickAlert.Show({
                    type: 'error',
                    message: err
                });
            }
        );
    }

    function _updateUser(isDelete = false) {
        UserService.UpdateUser(vm.SelectedUser).then(
            function (data) {
                if (data.isSuccess) {
                    QuickAlert.Show({
                        type: 'success',
                        message: isDelete ? 'User has been deleted.' : 'User has been successfully updated.'
                    });
                    vm.UserListLoading = true;
                    _initialize();
                } else {
                    QuickAlert.Show({
                        type: 'error',
                        message: data.messageAlert
                    });
                }
            }, function (err) {
                QuickAlert.Show({
                    type: 'error',
                    message: err
                });
            }
        );
    }

    function _selectUser(data) {
        debugger;
        vm.SelectedUser = {
            UserId: data.UserId,
            FirstName: data.FirstName,
            LastName: data.LastName,
            UserName: data.UserName,
            Password: data.Password,
            UserRoleId: data.UserRoleId,
            IsActive: data.IsActive,
            CreatedBy: data.CreatedBy,
            CreatedTime: data.CreatedDateTimeFormat
        };
    }

    function _deleteUser(result) {
        vm.SelectedUser.IsActive = false;
        _updateUser(true);
        $rootScope.IsLoading = false;
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
            IsActive: true,
            CreatedBy: 0,
            CreatedTime: ""
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