brasaoWebApp.controller('nbController', function ($scope, $http, $filter, $window) {

    $scope.init = function (casaAberta) {
        $scope.casaAberta = (casaAberta == 'S');
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