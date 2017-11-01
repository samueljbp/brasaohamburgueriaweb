brasaoWebApp.controller('coeController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.opcaoSelecionada = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.promiseGetOpcoesExtra();
    }

    $scope.modalAlteracao = function (opcao) {
        $('#formGravarOpcaoExtra').validator('destroy').validator();

        $scope.opcaoSelecionada = opcao;
        $scope.modoCadastro = 'A';
        $('#modalGravarOpcaoExtra').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarOpcaoExtra').validator('destroy').validator();

        $scope.opcaoSelecionada = {};
        $scope.modoCadastro = 'I';
        $('#modalGravarOpcaoExtra').modal('show');
    }

    $scope.gravarOpcaoExtra = function () {

        var hasErrors = $('#formGravarOpcaoExtra').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.promiseGravarOpcaoExtra = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarOpcaoExtra',
            data: { opcao: $scope.opcaoSelecionada, modoCadastro: $scope.modoCadastro },
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

        $('#modalGravarOpcaoExtra').modal('hide');
        $scope.opcaoSelecionada = null;

    }

    $scope.confirmaExclusaoOpcaoExtra = function (opcao) {

        $ngBootbox.confirm('Confirma a exclusão da opção extra ' + opcao.codOpcaoExtra + '?')
            .then(function () {
                $scope.opcaoSelecionada = opcao;
                $scope.executaExclusaoOpcaoExtra();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoOpcaoExtra = function () {

        $scope.promiseExcluirOpcaoExtra = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiOpcaoExtra',
            data: $scope.opcaoSelecionada,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Opção extra excluída com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codOpcaoExtra == $scope.opcaoSelecionada.codOpcaoExtra) {
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

            $scope.opcaoSelecionada = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

    $scope.promiseGetOpcoesExtra = $http({
        method: 'GET',
        headers: {
            //'Authorization': 'Bearer ' + accesstoken,
            'RequestVerificationToken': 'XMLHttpRequest',
            'X-Requested-With': 'XMLHttpRequest',
        },
        url: urlBase + 'Cadastros/GetOpcoesExtra'
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