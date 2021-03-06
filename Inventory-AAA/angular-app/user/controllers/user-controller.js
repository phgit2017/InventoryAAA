﻿angular
    .module('InventoryApp')
    .controller('UserController', UserController);

UserController.$inject = ['$filter', 'UserService', 'DTOptionsBuilder', 'DTColumnDefBuilder', '$scope', '$rootScope', 'QuickAlert'];

function UserController($filter, UserService, DTOptionsBuilder, DTColumnDefBuilder, $scope, $rootScope, QuickAlert) {

    var vm = this,
        controllerName = 'userCtrl';

    vm.UserListLoading = true;

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
    vm.SearchUserInput = "";
    vm.ManageBarShown = false;

    vm.filteredUsers = [];
    vm.currentPage = 1;
    vm.numPerPage = 10;
    vm.maxSize = 5;

    vm.Initialize = _initialize;
    vm.ClearUserDetails = _clearUserDetails;
    vm.SelectUser = _selectUser;
    vm.SaveUser = _saveUser;
    vm.DeleteUser = _deleteUser;

    $scope.$watch(
        function() {
            return vm.SearchUserInput;
        },
        function(newValue,oldValue){                
            if(oldValue!=newValue){
                vm.filteredUsers = $filter('filter')(vm.UserList, vm.SearchUserInput);
                vm.currentPage = 1;
            }
        }
    );

    function _initialize() {
        _getUserList();
        _clearUserDetails();
    }

    function _getUserList() {
        UserService.GetUserList().then(
            function(data) {
                vm.UserList = data.UserDetailsResult;
                vm.filteredUsers = data.UserDetailsResult;
                vm.UserListLoading = false;
            },
            function(error) {
                alert(error);
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
            function(data) {
                if (data.isSuccess) {
                    QuickAlert.Show({
                        type: 'success',
                        message: 'User has been successfully added.'
                    });
                    vm.UserListLoading = true;
                    vm.ManageBarShown = false;
                    _initialize();
                    $rootScope.IsLoading = false;
                } else {
                    QuickAlert.Show({
                        type: 'error',
                        message: data.messageAlert
                    });
                    $rootScope.IsLoading = false;
                }
            },
            function(err) {
                QuickAlert.Show({
                    type: 'error',
                    message: err
                });
            }
        );
    }

    function _updateUser(isDelete = false) {
        UserService.UpdateUser(vm.SelectedUser).then(
            function(data) {
                if (data.isSuccess) {
                    QuickAlert.Show({
                        type: 'success',
                        message: isDelete ? 'User has been deleted.' : 'User has been successfully updated.'
                    });
                    vm.UserListLoading = true;
                    vm.ManageBarShown = false;
                    _initialize();
                } else {
                    QuickAlert.Show({
                        type: 'error',
                        message: data.messageAlert
                    });
                }
            },
            function(err) {
                QuickAlert.Show({
                    type: 'error',
                    message: err
                });
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

    vm.ResetFields = function() {
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
}