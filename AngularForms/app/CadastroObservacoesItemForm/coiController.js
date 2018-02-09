﻿brasaoWebApp.controller('coiController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.associacao = { codItemCardapio: 0, nome: '' };

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.models = [
            { listName: "Observações disponíveis", items: [], dragging: false },
            { listName: "Observações incluídas", items: [], dragging: false }
        ];

        $scope.promiseGetObservacoesProducao = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetObservacoes'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.observacoesProducao = retorno.data;
                    $scope.models[0].items = $scope.models[0].items.concat(retorno.data);
                }
                else {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                }



            }, function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });








        

        /**
         * dnd-dragging determines what data gets serialized and send to the receiver
         * of the drop. While we usually just send a single object, we send the array
         * of all selected items here.
         */
        $scope.getSelectedItemsIncluding = function (list, item) {
            item.selected = true;
            return list.items.filter(function (item) { return item.selected; });
        };

        /**
         * We set the list into dragging state, meaning the items that are being
         * dragged are hidden. We also use the HTML5 API directly to set a custom
         * image, since otherwise only the one item that the user actually dragged
         * would be shown as drag image.
         */
        $scope.onDragstart = function (list, event) {
            list.dragging = true;
            if (event.dataTransfer.setDragImage) {
                var img = new Image();
                img.src = urlBase + 'Content/icons/ic_drag_drop.png';
                event.dataTransfer.setDragImage(img, 0, 0);
            }
        };

        /**
         * In the dnd-drop callback, we now have to handle the data array that we
         * sent above. We handle the insertion into the list ourselves. By returning
         * true, the dnd-list directive won't do the insertion itself.
         */
        $scope.onDrop = function (list, items, index) {
            angular.forEach(items, function (item) { item.selected = false; });
            list.items = list.items.slice(0, index)
                .concat(items)
                .concat(list.items.slice(index));
            return true;
        }

        /**
         * Last but not least, we have to remove the previously dragged items in the
         * dnd-moved callback.
         */
        $scope.onMoved = function (list) {
            list.items = list.items.filter(function (item) { return !item.selected; });
        };

        // Generate the initial model
        //angular.forEach($scope.models, function (list) {
        //    for (var i = 1; i <= 4; ++i) {
        //        list.items.push({ label: "Item " + list.listName + i, value: i });
        //    }
        //});

        // Model to JSON for demo purpose
        $scope.$watch('models', function (model) {
            $scope.modelAsJson = angular.toJson(model, true);
        }, true);















    }

    $scope.autoCompleteOptions = {
        minimumChars: 2,
        selectedTextAttr: 'nome',
        data: function (searchTerm) {


            searchTerm = searchTerm.toUpperCase();

            return $http.get(urlBase + 'Cadastros/GetItensCardapioByNome?chave=' + searchTerm)
                .then(function (response) {

                    var retorno = genericSuccess(response);

                    if (retorno.succeeded) {

                        return retorno.data;

                    }

                });

        },
        renderItem: function (item) {
            return {
                value: item.codItemCardapio,
                label: item.nome
            };
        },
        itemSelected: function (e) {
            $scope.associacao = e.item;
            $scope.models[1].items = [];
            $scope.models[1].items = $scope.models[1].items.concat(e.item.observacoes);

        }
    };

    $scope.salvarObservacoes = function () {

        $scope.promiseSalvarObservacoes = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarObservacoesItens',
            data: { obs: $scope.models[1].items, codItemCardapio: $scope.associacao.codItemCardapio },
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                $scope.mensagem.sucesso = 'Dados gravados com sucesso.';
                $scope.associacao = [];

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


});