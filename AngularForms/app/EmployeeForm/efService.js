angularFormsApp.factory('efService', function ($http) {
    var getEmployee = function (id) {
        return $http.get('/api/employee/' + id);
    };

    return { getEmployee: getEmployee };
});