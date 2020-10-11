angular
    .module('InventoryApp')
    .factory('MaintenanceService', MaintenanceService)

MaintenanceService.$inject = ['$q', '$http', '$location', 'globalBaseUrl'];

function MaintenanceService($q, $http, $location, globalBaseUrl) {
    var MaintenanceServiceFactory = {},
        baseUrl = globalBaseUrl + "/Maintenance";

    MaintenanceServiceFactory.GetCategoryList = _getCategoryList;
    MaintenanceServiceFactory.SaveCategory = _saveCategory;

    return MaintenanceServiceFactory;

    function _getCategoryList() {
        var defer = $q.defer(),
            url = baseUrl + '/CategoryList';
        $http.get(url)
            .then(function(response) {
                if (!response.data.isSuccess) {
                    $location.url('/Unauthorized');
                }
                defer.resolve(response.data)
            }),
            function(err) {

                defer.reject(err);
            }
        return defer.promise;
    }

    function _saveCategory(data) {
        var defer = $q.defer(),
            url = baseUrl + '/AddNewCategoryDetails';
        $http.post(url, data)
            .then(function(response) {
                if (!response.data.isSuccess) {
                    $location.url('/Unauthorized');
                }
                defer.resolve(response.data)
            }),
            function(err) {

                defer.reject(err);
            }
        return defer.promise;
    }
}