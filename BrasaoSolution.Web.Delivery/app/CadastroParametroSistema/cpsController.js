brasaoWebApp.controller('cpsController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken, empresasJson, codLojaSelecionada) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.empresas = {};
        if (empresasJson != '') {
            $scope.empresas = JSON.parse(empresasJson);
        }

        $scope.codEmpresa = codLojaSelecionada.toString();
        $scope.codLojaSelecionada = codLojaSelecionada;

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

        if ($scope.parSelecionado.codEmpresa != null) {
            $scope.parSelecionado.codEmpresa = $scope.parSelecionado.codEmpresa.toString();
        }

        $scope.modoCadastro = 'A';
        $('#modalGravarPar').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarPar').validator('destroy').validator();

        $scope.parSelecionado = {};
        $scope.modoCadastro = 'I';
        $('#modalGravarPar').modal('show');
    }

    $scope.confirmaExclusaoParametro = function (par) {

        $ngBootbox.confirm('Confirma a exclusão do parâmetro ' + par.codParametro + '?')
            .then(function () {
                $scope.parSelecionado = par;
                $scope.executaExclusaoParametro();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoParametro = function () {

        $scope.promiseExcluirParametro = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiParametro',
            data: $scope.parSelecionado,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Parâmetro excluído com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codParametro == $scope.parSelecionado.codParametro && $scope.rowCollection[i].codEmpresa == $scope.codLojaSelecionada) {
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

            $scope.comboSelecionado = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



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

                verificaLista($scope.rowCollection, $scope.codLojaSelecionada);

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