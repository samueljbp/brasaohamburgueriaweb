brasaoWebApp.controller('rdSenhaController', function ($scope, $http) {

    $scope.init = function (loginUsuario, antiForgeryToken, code) {
        $scope.antiForgeryToken = antiForgeryToken;

        $scope.redefinirSenhaViewModel = {
            email: "", senha: "", senhaConfirmada: "", code: code
        }

        $('#formRedefinirSenha').validator({
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

    $scope.submitForm = function () {

        var hasErrors = $('#formRedefinirSenha').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.promisesLoader = $http({
            method: 'POST',
            url: urlBase + 'Conta/RedefinirSenha',
            data: $scope.redefinirSenhaViewModel,
            headers: {
                'RequestVerificationToken': $scope.antiForgeryToken,
                'X-Requested-With': 'XMLHttpRequest',
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.Succeeded) {
                window.location.href = urlBase + 'Conta/RedefinirSenhaConfirmacao';
            }
            else {
                $('#mensagemErroFormulario').removeClass('hidden');
                $('#mensagemErroFormulario').text(retorno.errors[0]);
            }

        }).catch(function (error) {
            $('#mensagemErroFormulario').removeClass('hidden');
            $('#mensagemErroFormulario').text(error.statusText);
        });


    };
});