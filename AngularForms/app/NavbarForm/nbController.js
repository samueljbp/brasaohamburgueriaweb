brasaoWebApp.controller('nbController', function ($scope, $http, $filter, $window, $ngBootbox) {

    $scope.init = function (casaAberta, codLojaSelecionada) {
        $scope.casaAberta = (casaAberta == 'S');
        $scope.codLojaSelecionada = codLojaSelecionada;
    }

    $scope.confirmaAlteracaoLoja = function (codLoja) {

        $ngBootbox.confirm('Confirma a alteração da loja? Se você está realizando alguma operação, finalize-a primeiro ou seus dados serão perdidos.')
            .then(function () {

                $scope.promiseGeral = $http({
                    method: 'POST',
                    url: urlBase + 'Home/TrocaLoja',
                    data: { codLoja: codLoja },
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        $window.location.reload();

                    }
                    else {
                        window.alert('Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido'));
                    }

                }).catch(function (error) {
                    window.alert('Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.'));
                });

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.checkCasaAberta = function () {
        

        $scope.promiseAbreFechaCasa = $http({
            method: 'POST',
            url: urlBase + 'Home/AbreFechaLoja',
            data: { aberta: $scope.casaAberta },
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

            }
            else {
                window.alert('Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido'));
            }

        }).catch(function (error) {
            window.alert('Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.'));
        });



    }

});