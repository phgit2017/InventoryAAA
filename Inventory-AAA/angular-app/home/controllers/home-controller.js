app.controller("HomeController", function ($http ,$scope) {
    var vm = this;
    var url = 'http://localhost:37600/Stocks/InventorySummary';

    //vm.getTest = function () {
    //    return $http.get(url)
    //        .then(function (response) {
    //            vm.test = response;
    //            console.log('tae');
    //        });
    //}

    //vm.getTest2 = getTest2();

    //function getTest2() {
    //    console.log('tae');

    //    return $http.get(url)
    //        .then(function (response) {
    //            vm.test = response;
    //            console.log(response.data.result);
    //        });
    //}

    vm.ProductList = [
        {
            Id: 1,
            Code: "MP01",
            Description: "Men's Cotton Pants",
            CurrentStock: 30,
            Sold: 10000
        },
        {
            Id: 2,
            Code: "MP02",
            Description: "Men's Silk Pants",
            CurrentStock: 20,
            Sold: 20000
        },
        {
            Id: 3,
            Code: "MP03",
            Description: "Men's Denim Pants",
            CurrentStock: 10,
            Sold: 30000
        },
    ]
});