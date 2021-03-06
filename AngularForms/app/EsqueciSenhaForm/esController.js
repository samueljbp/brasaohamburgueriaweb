﻿brasaoWebApp.controller('esController', function ($scope, $http, $window) {
    $scope.init = function (loginUsuario, antiForgeryToken) {

        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.esqueciMinhaSenhaViewModel = { email: "" }
        $scope.antiForgeryToken = antiForgeryToken;

        $('#formEsqueciSenha').validator({
            custom: {
                'email': function ($el) {
                    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                    if ($el.val() != '' && !re.test($el.val())) {
                        return 'E-mail inválido.';
                    }
                }
            }
        });
    }

    $scope.confirmar = function () {

        var hasErrors = $('#formEsqueciSenha').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.promiseEsqueciSenha = $http({
            method: 'POST',
            url: urlBase + 'Conta/EsqueciMinhaSenha',
            data: $scope.esqueciMinhaSenhaViewModel,
            headers: {
                'RequestVerificationToken': $scope.antiForgeryToken,
                'X-Requested-With': 'XMLHttpRequest',
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.Succeeded) {
                window.location.href = urlBase + 'Conta/EsqueciMinhaSenhaConfirmacao';
            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });


    };
});