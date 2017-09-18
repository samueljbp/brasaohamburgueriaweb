brasaoWebApp.controller('cpController', function ($scope, $http, $filter) {

    $scope.pedidos = [];
    $scope.promisesLoader = [];
    $scope.pedidoSelecionado = [];

    $scope.getUltimosPedidos = function (loginUsuario) {

        //var accesstoken = sessionStorage.getItem('accessToken');

        $scope.promiseGetPedidos = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Pedido/GetUltimosPedidos?loginUsuario=' + loginUsuario
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.pedidos = retorno.data;

            }
            else {
                $scope.erro.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

            

        }, function (error) {
            $scope.erro.mensagem = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $scope.promisesLoader.push($scope.promiseGetPedidos);


    };

    $scope.mostrarItensPedido = function (codPedido) {

        $scope.pedidoSelecionado = $filter('filter')($scope.pedidos, { codPedido: codPedido })[0];
        $('#modalItensPedido').modal('show');

    };

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.getUltimosPedidos(loginUsuario);
    }

});