brasaoWebApp.controller('cipController', function ($scope, $http, $filter, $ngBootbox, $window) {

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

        $scope.impSelecionada = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.promiseGetImpressoras = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetImpressorasProducao'
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

    $scope.modalAlteracao = function (imp) {
        $('#formGravarImp').validator('destroy').validator();

        $scope.impSelecionada = imp;

        if ($scope.impSelecionada.codEmpresa != null) {
            $scope.impSelecionada.codEmpresa = $scope.impSelecionada.codEmpresa.toString();
        }

        $scope.modoCadastro = 'A';
        $('#modalGravarImp').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarImp').validator('destroy').validator();

        $scope.impSelecionada = {};
        $scope.modoCadastro = 'I';
        $('#modalGravarImp').modal('show');
    }

    $scope.gravarImpressoraProducao = function () {

        var hasErrors = $('#formGravarImp').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.promiseGravarImp = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarImpressoraProducao',
            data: { imp: $scope.impSelecionada, modoCadastro: $scope.modoCadastro },
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

        $('#modalGravarImp').modal('hide');
        $scope.impSelecionada = null;

    }

    $scope.confirmaExclusaoImp = function (imp) {

        $ngBootbox.confirm('Confirma a exclusão da impressora ' + imp.codImpressora + '?')
            .then(function () {
                $scope.impSelecionada = imp;
                $scope.executaExclusaoImp();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoImp = function () {

        $scope.promiseExcluirImpressora = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiImpressoraProducao',
            data: $scope.impSelecionada,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Impressora excluída com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codImpressora == $scope.impSelecionada.codImpressora) {
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

            $scope.impSelecionada = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

    

});