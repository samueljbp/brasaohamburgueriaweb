brasaoWebApp.controller('copController', function ($scope, $http, $filter) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.promiseGetObservacoes();
    }

    $scope.promiseGetObservacoes = $http({
        method: 'GET',
        headers: {
            //'Authorization': 'Bearer ' + accesstoken,
            'RequestVerificationToken': 'XMLHttpRequest',
            'X-Requested-With': 'XMLHttpRequest',
        },
        url: urlBase + 'Cadastros/GetObservacoes'
    })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.rowCollection = retorno.data;

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }



        }, function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

});