    angular
        .module('InventoryApp')
        .factory('QuickAlert', QuickAlert);

    QuickAlert.$inject = [];

    function QuickAlert() {
        var QuickAlertFactory = {}, quickAlertClass = '.quick-alert', defaultOptions;

        defaultOptions = {
            message: "Success",
            type: "success",
            callback: null
        }

        QuickAlertFactory.Show = _show;

        return QuickAlertFactory;

        function _show(options) {
            var quickAlert, $body, hasDefault = false;

            if (typeof options === "string" && options !== "") {
                defaultOptions.message = options;
                options = defaultOptions;
                hasDefault = true;
            }
            else if (typeof options !== "object") {
                options = defaultOptions;
                hasDefault = true;
            }

            if (!hasDefault) {
                options.type = ((typeof options.type === "undefined" || options.type === "") ? defaultOptions.type : options.type)
            }

            switch (options.type) {
                case "success":
                    options.type = "quick-alert-success";
                    break;
                case "error":
                    options.type = "quick-alert-error";
                    break;
            }

            debugger;

            quickAlert = "<div class='quick-alert " + options.type + "'>";
            quickAlert += "<span>";
            quickAlert += options.message;
            quickAlert += "</span>";
            quickAlert += "</div>";

            $body = angular.element("body");

            $body.append(quickAlert);
            $body.find(quickAlertClass).addClass('fade-in');

            setTimeout(function () {
                debugger;
                $body.find(quickAlertClass).addClass('bounce-out-up').removeClass('fade-in');

                if (options.callback !== null && typeof options.callback == "function") {
                    options.callback();
                }

                setTimeout(function () {
                    $body.find(quickAlertClass).remove();
                }, 500)
            }, 4000)

        }

    }