brasaoWebApp.controller('cccController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: '',
            erroCampoPreco: false,
            erroListasDragDrop: false
        }

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';
        $scope.pesquisaItem = { chave: '' };

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.comboSelecionado = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.itensCardapio = [];





        





        $scope.promiseGetCombos = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetCombos'
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

        $scope.promiseGetItens = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetItensCardapio'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.itensCardapio = retorno.data;
                    configuraDndLists();

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

    $scope.pesquisaItens = function () {

        if ($scope.pesquisaItem.chave == '') {
            $scope.models[0].items = $scope.itensCardapio;
            return;
        }

        $scope.models[0].items = $filter('filter')($scope.itensCardapio, { nome: $scope.pesquisaItem.chave});

    }

    function configuraDndLists() {

        $scope.models = [
            { listName: "Itens disponíveis", items: [], dragging: false },
            { listName: "Itens incluídos", items: [], dragging: false }
        ];

        $scope.models[0].items = [];
        $scope.models[0].items = $scope.models[0].items.concat($scope.itensCardapio);

        $scope.onDrop = function (list, items, index) {
            angular.forEach(items, function (item) { item.selected = false; item.quantidade = 1 });
            list.items = list.items.slice(0, index)
                .concat(items)
                .concat(list.items.slice(index));
            //list.items = removeDuplicates(list.items, 'codItemCardapio');
            return true;
        }


        angular.forEach($scope.models[0].items, function (item) { item.selected = false; });
        angular.forEach($scope.models[1].items, function (item) { item.selected = false; });

        $scope.getSelectedItemsIncluding = function (list, item) {
            item.selected = true;
            return list.items.filter(function (item) { return item.selected; });
        };

        $scope.onDragstart = function (list, event) {
            list.dragging = true;
            if (event.dataTransfer.setDragImage) {
                var img = new Image();
                img.src = urlBase + 'Content/icons/ic_drag_drop.png';
                event.dataTransfer.setDragImage(img, 0, 0);
            }
        };

        $scope.onMoved = function (list) {
            list.items = list.items.filter(function (item) { return !item.selected; });
        };

    }

    $scope.modalAlteracao = function (combo) {
        $('#formGravarCombo').validator('destroy').validator();

        $scope.comboSelecionado = combo;

        $scope.models[1].items = $scope.models[1].items.concat($scope.comboSelecionado.itens);
        angular.forEach($scope.models[1].items, function (item) { item.selected = false; });

        $scope.modoCadastro = 'A';
        $('#modalGravarCombo').modal('show');
    }

    function montaArrayDias() {

        $scope.diasSelecionados = [];
        angular.forEach($scope.comboSelecionado.diasAssociados, function (item) { $scope.diasSelecionados.push(item.numDiaSemana); });


    }

    $scope.modalInclusao = function () {
        $('#formGravarCombo').validator('destroy').validator();

        $scope.comboSelecionado = {};
        $scope.comboSelecionado.ativo = true;
        $scope.comboSelecionado.itens = [];
        $scope.models[1].items = [];
        angular.forEach($scope.models[1].items, function (item) { item.selected = false; });

        $scope.modoCadastro = 'I';
        $('#modalGravarCombo').modal('show');
    }

    $scope.gravarCombo = function () {

        $('#formGravarCombo').validator('destroy').validator();

        $scope.mensagem.erroCampoPreco = false;
        if ($scope.comboSelecionado.preco <= 0.0 || isNaN($scope.comboSelecionado.preco)) {

            $scope.mensagem.erroCampoPreco = true;
            return;

        } else { $scope.comboSelecionado.preco = parseFloat($scope.comboSelecionado.preco); }

        var hasErrors = $('#formGravarCombo').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.mensagem.erroListasDragDrop = false;
        if ($scope.models[1].items.length == 0) {

            $scope.mensagem.erroListasDragDrop = true;
            return;

        }

        $scope.comboSelecionado.itens = [];
        $scope.comboSelecionado.itens = $scope.models[1].items;

        //angular.forEach($scope.comboSelecionado.itens, function (item) { if (!item.quantidade > 0) { item.quantidade = 1; } });

        $scope.promiseGravarCombo = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarCombo',
            data: { combo: $scope.comboSelecionado, modoCadastro: $scope.modoCadastro },
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

                $('#modalGravarCombo').modal('hide');

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

    $scope.confirmaExclusaoCombo = function (combo) {

        $ngBootbox.confirm('Confirma a exclusão do combo ' + combo.codCombo + '?')
            .then(function () {
                $scope.comboSelecionado = combo;
                $scope.executaExclusaoCombo();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoCombo = function () {

        $scope.promiseExcluirCombo = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiCombo',
            data: $scope.comboSelecionado,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Combo excluído com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codCombo == $scope.comboSelecionado.codCombo) {
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

});