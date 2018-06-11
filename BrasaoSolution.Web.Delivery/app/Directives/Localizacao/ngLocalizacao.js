var module = angular.module('ngLocalizacao', ['cgBusy']);
var bairroBefore = null;

module.directive('ngLocalizacao', ['$http', '$window', function ($http) {
    return {

        restrict: 'E',
        transclude: true,
        scope: {
            estadoModel: '=',
            cidadeModel: '=',
            bairroModel: '=',
            estadosModel: '=',
            cidadesModel: '=',
            bairrosModel: '=',
            mensagemModel: '=',
            gatilhoModel: '='
        },
        templateUrl: urlBase + "/app/Directives/Localizacao/template.html",
        link: function (scope, element, attrs) {


            scope.$watch(
                function () { return scope.estadoModel; },
                function (value) {
                    bairroBefore = null;
                    if (scope.bairroModel != null) {
                        bairroBefore = scope.bairroModel;
                    }
                    scope.selecionaEstado();
                },
                true);

            scope.$watch(
                function () { return scope.cidadeModel; },
                function (value) {
                    bairroBefore = null;
                    if (scope.bairroModel != null) {
                        bairroBefore = scope.bairroModel;
                    }

                    scope.selecionaCidade();
                },
                true);

            scope.promiseGetEstados = $http({
                method: 'GET',
                headers: {
                    //'Authorization': 'Bearer ' + accesstoken,
                    'RequestVerificationToken': 'XMLHttpRequest',
                    'X-Requested-With': 'XMLHttpRequest',
                },
                url: urlBase + 'Cadastros/GetEstados'
            })
                .then(function (response) {

                    var retorno = genericSuccess(response);

                    if (retorno.succeeded) {

                        scope.estadosModel = retorno.data;

                    }
                    else {
                        scope.mensagemModel.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }



                }, function (error) {
                    scope.mensagemModel.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                    $window.scrollTo(0, 0);
                });

            scope.selecionaEstado = function () {


                scope.cidadesModel = {};
                scope.bairrosModel = {};

                scope.promiseGetCidades = $http({
                    method: 'GET',
                    headers: {
                        //'Authorization': 'Bearer ' + accesstoken,
                        'RequestVerificationToken': 'XMLHttpRequest',
                        'X-Requested-With': 'XMLHttpRequest',
                    },
                    url: urlBase + "Cadastros/GetCidades?siglaEstado=" + scope.estadoModel
                })
                    .then(function (response) {

                        var retorno = genericSuccess(response);

                        if (retorno.succeeded) {

                            scope.cidadesModel = retorno.data;

                            if (scope.cidadeModel != null) {
                                scope.cidadeModel = scope.cidadeModel.toString();
                            }

                        }
                        else {
                            scope.mensagemModel.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                            $window.scrollTo(0, 0);
                        }



                    }, function (error) {
                        scope.mensagemModel.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                        $window.scrollTo(0, 0);
                    });


            }

            scope.selecionaCidade = function () {


                scope.bairrosModel = {};

                scope.promiseGetBairros = $http({
                    method: 'GET',
                    headers: {
                        //'Authorization': 'Bearer ' + accesstoken,
                        'RequestVerificationToken': 'XMLHttpRequest',
                        'X-Requested-With': 'XMLHttpRequest',
                    },
                    url: urlBase + "Cadastros/GetBairros?codCidade=" + scope.cidadeModel
                })
                    .then(function (response) {

                        var retorno = genericSuccess(response);

                        if (retorno.succeeded) {

                            scope.bairrosModel = retorno.data;

                            if (bairroBefore != null) {
                                scope.bairroModel = bairroBefore.toString();
                            }

                        }
                        else {
                            scope.mensagemModel.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                            //$window.scrollTo(0, 0);
                        }



                    }, function (error) {
                        scope.mensagemModel.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                        //$window.scrollTo(0, 0);
                    });



            }

        }

    }
}]);