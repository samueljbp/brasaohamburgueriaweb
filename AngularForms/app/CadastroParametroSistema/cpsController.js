brasaoWebApp.controller('cpsController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.parSelecionado = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.promiseGetParametros = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetParametrosSistema'
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

    }

    $scope.modalAlteracao = function (par) {
        $('#formGravarPar').validator('destroy').validator();

        $scope.parSelecionado = par;
        $scope.modoCadastro = 'A';
        $('#modalGravarPar').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarPar').validator('destroy').validator();

        $scope.parSelecionado = {};
        $scope.modoCadastro = 'I';
        $('#modalGravarPar').modal('show');
    }

    $scope.gravarParametroSistema = function () {

        var hasErrors = $('#formGravarPar').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.promiseGravarPar = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarParametroSistema',
            data: { par: $scope.parSelecionado, modoCadastro: $scope.modoCadastro },
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                $scope.mensagem.sucesso = 'Dados gravados com sucesso.';

                if ($scope.modoCadastro == 'I') {
                    $scope.rowCollection.push(retorno.data);
                }

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $('#modalGravarPar').modal('hide');
        $scope.parSelecionado = null;

    }


});