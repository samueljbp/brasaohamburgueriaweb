brasaoWebApp.controller('cicController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.classeSelecionada = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.impressorasProducao = [];

        $scope.promiseGetClasses();
    }

    $scope.selecionaImpressora = function () {
        
        $scope.classeSelecionada.descricaoImpressoraPadrao = $filter('filter')($scope.impressorasProducao, { codImpressora: $scope.classeSelecionada.codImpressoraPadrao })[0].descricao;

    }

    $scope.modalAlteracao = function (classe) {
        $('#formGravarClasse').validator('destroy').validator();

        $scope.classeSelecionada = classe;
        $scope.modoCadastro = 'A';
        $('#modalGravarClasse').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarClasse').validator('destroy').validator();

        $scope.classeSelecionada = {};
        $scope.classeSelecionada.sincronizar = true;
        $scope.modoCadastro = 'I';
        $('#modalGravarClasse').modal('show');
    }

    $scope.gravarClasse = function () {

        var hasErrors = $('#formGravarClasse').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.promiseGravarClasse = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarClasseItemCardapio',
            data: { classe: $scope.classeSelecionada, modoCadastro: $scope.modoCadastro },
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

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $('#modalGravarClasse').modal('hide');
        $scope.classeSelecionada = null;

    }

    $scope.confirmaExclusaoClasse = function (classe) {

        $ngBootbox.confirm('Confirma a exclusão da classe ' + classe.codClasse + '?')
            .then(function () {
                $scope.classeSelecionada = classe;
                $scope.executaExclusaoClasse();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoClasse = function () {

        $scope.promiseExcluirClasse = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiClasseItemCardapio',
            data: $scope.classeSelecionada,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Classe excluída com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codClasse == $scope.classeSelecionada.codClasse) {
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

            $scope.classeSelecionada = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

    $scope.promiseGetClasses = $http({
        method: 'GET',
        headers: {
            //'Authorization': 'Bearer ' + accesstoken,
            'RequestVerificationToken': 'XMLHttpRequest',
            'X-Requested-With': 'XMLHttpRequest',
        },
        url: urlBase + 'Cadastros/GetClassesItemCardapio'
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


    $scope.promiseGetImpressorasProducao = $http({
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

            $scope.impressorasProducao = retorno.data;

        }
        else {
            $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
            $window.scrollTo(0, 0);
        }



    }, function (error) {
        $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
        $window.scrollTo(0, 0);
    });

});