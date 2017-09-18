brasaoWebApp.controller('pedRegistradoController', function ($scope, $http, $filter) {
    $scope.pedido = { codPedido: sessionStorage.codPedido };
});