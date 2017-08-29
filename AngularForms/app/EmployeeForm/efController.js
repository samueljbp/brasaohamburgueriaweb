angularFormsApp.controller('efController', function ($scope, efService) {
    init();

    function init() {
        efService.getEmployee(1).then(function (data)
        {
            $scope.employee = data.data;
        });

        $scope.departments = [
        "Engeneering",
        "Marketing",
        "Finance",
        "Administration"
        ];

        $scope.submitForm = function () {

        };
    }
});