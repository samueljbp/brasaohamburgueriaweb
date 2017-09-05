angularFormsApp.controller('gpController', function ($scope, $http, $filter, noteService) {
    $erro = { mensagem: '' };

    $scope.pedido = { usuario: '', codPedido: 0, $situacao: 0 };

    noteService.connect();
    $scope.messages = [];
 
    $scope.$on('messageAdded', function (event, codPedido, situacao) {
        
    });
 
    $scope.sendMessage = function () {
        noteService.sendMessage($scope.pedido.usuario, $scope.pedido.codPedido, $scope.pedido.situacao);
    };


});