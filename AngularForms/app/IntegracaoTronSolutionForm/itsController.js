brasaoWebApp.controller('itsController', function ($scope, $http, $filter, $window) {

    $scope.executaSincronizacao = function () {

        $scope.promiseExecutarSincronizacao = $http({
            method: 'POST',
            url: urlBase + 'Integracoes/SolicitarSincronismoTronSolution',
            data: { itensTron: $scope.dadosTron.itens, classesTron: $scope.dadosTron.classes },
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                $scope.listaAlteracoes = retorno.data;

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.erro.mensagem = error.statusText;
            $window.scrollTo(0, 0);
        });

    }

    $scope.solicitarSincronizacao = function () {


        $scope.promiseSolicitarSincronizacao = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlWebAPIBase + 'TronSolutionData/GetDadosTron'
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.dadosTron = retorno.data;
                $scope.executaSincronizacao();

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

        $scope.antiForgeryToken = antiForgeryToken;
        $scope.listaAlteracoes = [];
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

    }


});