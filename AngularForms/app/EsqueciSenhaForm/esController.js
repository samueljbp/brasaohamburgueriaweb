angularFormsApp.controller('esController', function ($scope, $http) {
    init();

    function init() {
        $scope.esqueciMinhaSenhaViewModel = { email: "" }

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

        $scope.submitForm = function () {

            var hasErrors = $('#formEsqueciSenha').validator('validate').has('.has-error').length;

            if (hasErrors) {
                return;
            }

            return $http({
                method: 'POST',
                url: urlBase + 'Conta/EsqueciMinhaSenha',
                data: $scope.esqueciMinhaSenhaViewModel,
                headers: {
                    'RequestVerificationToken': $scope.antiForgeryToken
                }
            }).then(function (success) {
                var retorno = genericSuccess(success);

                if (retorno.Succeeded) {
                    window.location.href = urlBase + 'Conta/EsqueciMinhaSenhaConfirmacao';
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
    }
});