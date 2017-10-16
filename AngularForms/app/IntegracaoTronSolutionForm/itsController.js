brasaoWebApp.controller('itsController', function ($scope, $http, $filter) {

    $scope.solicitarSincronizacao = function () {

        $scope.promiseSolicitarSincronizacao = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Integracoes/SolicitarSincronismoTronSolution'
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.listaAlteracoes = retorno.data;

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }



        }, function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

    }


    $scope.init = function (loginUsuario, antiForgeryToken) {

        $scope.listaAlteracoes = [];

    }


});