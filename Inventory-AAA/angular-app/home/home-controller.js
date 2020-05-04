app.controller("HomeController", function ($scope) {
    var vm = this;

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