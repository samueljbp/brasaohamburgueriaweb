brasaoWebApp.controller('euController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken, empresasJson) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.usuario = {};
        $scope.empresas = {};
        if (empresasJson != '') {
            $scope.empresas = JSON.parse(empresasJson);
        }
        $scope.empresaExcluir = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.antiForgeryToken = antiForgeryToken;
    }

    $scope.autoCompleteOptions = {
        minimumChars: 1,
        data: function (searchTerm) {


            searchTerm = searchTerm.toUpperCase();

            return $http.get(urlBase + 'Conta/GetUsuarioByNome?nome=' + searchTerm)
                .then(function (response) {

                    var retorno = genericSuccess(response);

                    if (retorno.succeeded) {

                        return retorno.data;

                    }

                });

        },
        renderItem: function (item) {
            return {
                value: item.id,
                label: item.nome + ' - ' + item.email
            };
        },
        itemSelected: function (e) {
            $scope.usuario = e.item;

            $scope.rowCollection = $scope.usuario.empresas;
        }
    };

    $scope.incluirEmpresa = function () {

        var hasErrors = $('#formEmpresa').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        var fechaPopup = true;

        $scope.promiseIncluirEmpresa = $http({
            method: 'POST',
            url: urlBase + 'Conta/IncluirEmpresaUsuario',
            data: { codEmpresa: $scope.usuario.codEmpresa, idUsuario: $scope.usuario.id },
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                $scope.mensagem.sucesso = 'Dados gravados com sucesso.';
                $scope.rowCollection = retorno.data;

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

    }



    $scope.confirmaExclusaoEmpresa = function (empresa) {

        $ngBootbox.confirm('Confirma a exclusão da empresa ' + empresa.codEmpresa + '?')
            .then(function () {
                $scope.empresaExcluir = empresa;
                $scope.executaExclusaoEmpresa();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoEmpresa = function () {

        $scope.promiseExcluirEmpresa = $http({
            method: 'POST',
            url: urlBase + 'Conta/ExcluirEmpresaUsuario',
            data: { codEmpresa: $scope.empresaExcluir.codEmpresa, idUsuario: $scope.usuario.id },
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {


                $scope.mensagem.sucesso = 'Empresa excluída com sucesso.';
                $scope.rowCollection = retorno.data;

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

            $scope.empresaExcluir = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }



});