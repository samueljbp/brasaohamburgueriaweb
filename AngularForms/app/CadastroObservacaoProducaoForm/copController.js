brasaoWebApp.controller('copController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.obsSelecionada = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.promiseGetObservacoes();
    }

    $scope.modalAlteracao = function (obs) {
        $('#formGravarObs')[0].reset();
        $('#formGravarObs').validator('destroy').validator();

        $scope.obsSelecionada = obs;
        $scope.modoCadastro = 'A';
        $('#modalGravarObs').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarObs')[0].reset();
        $('#formGravarObs').validator('destroy').validator();

        $scope.obsSelecionada = {};
        $scope.modoCadastro = 'I';
        $('#modalGravarObs').modal('show');
    }

    $scope.gravarObservacao = function () {

        var hasErrors = $('#formGravarObs').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.promiseGravarObs = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarObservacao',
            data: { obs: $scope.obsSelecionada, modoCadastro: $scope.modoCadastro },
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

        $('#modalGravarObs').modal('hide');
        $scope.obsSelecionada = null;

    }

    $scope.confirmaExclusaoObs = function (obs) {

        $ngBootbox.confirm('Confirma a exclusão da observação ' + obs.codObservacao + '?')
            .then(function () {
                $scope.obsSelecionada = obs;
                $scope.executaExclusaoObs();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoObs = function () {

        $scope.promiseExcluirObs = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiObservacao',
            data: $scope.obsSelecionada,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Observação excluída com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codObservacao == $scope.obsSelecionada.codObservacao) {
                            $scope.rowCollection.splice(i, 1);

                            return;
                        }
                    }
                } else {

                    $scope.mensagem.erro = retorno.data;
                    $window.scrollTo(0, 0);

                }

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

            $scope.obsSelecionada = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        

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