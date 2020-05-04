app.controller("HomeController", function ($scope) {
    var vm = this;

    vm.Test = "test";

    vm.ProductList = [
        {
            Id: 1,
            Code: "MP01",
            Descripton: "Men's Cotton Pants",
            CurrentStock: 30,
            Sold: 10000
        },
        {
            Id: 2,
            Code: "MP02",
            Descripton: "Men's Silk Pants",
            CurrentStock: 20,
            Sold: 20000
        },
        {
            Id: 1,
            Code: "MP03",
            Descripton: "Men's Denim Pants",
            CurrentStock: 10,
            Sold: 30000
        },
    ]
});