brasaoWebApp.controller('pedRegistradoController', function ($scope, $http, $filter, $window) {
    $scope.pedido = { codPedido: sessionStorage.codPedido };
});