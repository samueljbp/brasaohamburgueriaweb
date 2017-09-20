brasaoWebApp.controller('mpController', function ($scope, $http, noteService) {
    $scope.init = function () {
        
    }

    $scope.iniciaPedidoModoNormal = function () {
        sessionStorage.modoAdm = "N";
        window.location = urlBase + "Pedido/Index";
    }
});