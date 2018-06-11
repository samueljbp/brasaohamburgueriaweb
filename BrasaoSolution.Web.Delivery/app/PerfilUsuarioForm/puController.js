brasaoWebApp.controller('puController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.usuario = {};
        $scope.roles = {};
        $scope.perfilExcluir = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.antiForgeryToken = antiForgeryToken;


        $scope.promiseGetRoles = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Conta/GetRoles'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.roles = retorno.data;

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

            preencheDescricaoPerfis($scope.usuario.perfis);

            $scope.rowCollection = $scope.usuario.perfis;
        }
    };

    function preencheDescricaoPerfis (listaPerfis) {

        $.each(listaPerfis, function (index, value) {

            value.nome = $filter('filter')($scope.roles, { id: value.id })[0].name;

        });

    }

    $scope.incluirPerfil = function () {

        var hasErrors = $('#formPerfil').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        var fechaPopup = true;

        $scope.promiseIncluirPerfil = $http({
            method: 'POST',
            url: urlBase + 'Conta/IncluirPerfilUsuario',
            data: { idPerfil: $scope.usuario.roleid, idUsuario: $scope.usuario.id },
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                $scope.mensagem.sucesso = 'Dados gravados com sucesso.';
                preencheDescricaoPerfis(retorno.data);
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

        $scope.perfilExcluir = null;

    }



    $scope.confirmaExclusaoPerfil = function (perfil) {

        $ngBootbox.confirm('Confirma a exclusão do perfil ' + perfil.id + '?')
            .then(function () {
                $scope.perfilExcluir = perfil;
                $scope.executaExclusaoPerfil();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoPerfil = function () {

        $scope.promiseExcluirPerfil = $http({
            method: 'POST',
            url: urlBase + 'Conta/ExcluirPerfilUsuario',
            data: { idPerfil: $scope.perfilExcluir.id, idUsuario: $scope.usuario.id },
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {


                $scope.mensagem.sucesso = 'Perfil excluído com sucesso.';
                preencheDescricaoPerfis(retorno.data);
                $scope.rowCollection = retorno.data;

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

            $scope.perfilExcluir = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }



});