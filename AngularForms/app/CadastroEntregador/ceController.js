brasaoWebApp.controller('ceController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.entregadorSelecionado = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.promiseGetOpcoesExtra();
    }

    $scope.modalAlteracao = function (entregador) {
        $('#formGravarEntregador').validator('destroy').validator();

        $scope.entregadorSelecionado = entregador;
        $scope.modoCadastro = 'A';
        $('#modalGravarEntregador').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarEntregador').validator('destroy').validator();

        $scope.entregadorSelecionado = {};
        $scope.modoCadastro = 'I';
        $('#modalGravarEntregador').modal('show');
    }

    $scope.gravarEntregador = function () {

        var hasErrors = $('#formGravarEntregador').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        if ($scope.entregadorSelecionado.valorPorEntrega) {
            valor = parseFloat($scope.entregadorSelecionado.valorPorEntrega);
        }

        if (!isNaN(valor) || valor > 0) {
            $scope.entregadorSelecionado.valorPorEntrega = valor;
        }
        else {
            $scope.entregadorSelecionado.valorPorEntrega = 0.0;
        }

        $scope.promiseGravarEntregador = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarEntregador',
            data: { entregador: $scope.entregadorSelecionado, modoCadastro: $scope.modoCadastro },
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

        $('#modalGravarEntregador').modal('hide');
        $scope.opcaoSelecionada = null;

    }

    $scope.confirmaExclusaoEntregador = function (entregador) {

        $ngBootbox.confirm('Confirma a exclusão do entregador ' + entregador.codEntregador + '?')
            .then(function () {
                $scope.entregadorSelecionado = entregador;
                $scope.executaExclusaoEntregador();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoEntregador = function () {

        $scope.promiseExcluirEntregador = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiEntregador',
            data: $scope.entregadorSelecionado,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Entregador excluído com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codEntregador == $scope.entregadorSelecionado.codEntregador) {
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

            $scope.entregadorSelecionado = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

    $scope.promiseGetEntregadores = $http({
        method: 'GET',
        headers: {
            //'Authorization': 'Bearer ' + accesstoken,
            'RequestVerificationToken': 'XMLHttpRequest',
            'X-Requested-With': 'XMLHttpRequest',
        },
        url: urlBase + 'Cadastros/GetEntregadores'
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