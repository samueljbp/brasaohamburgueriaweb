brasaoWebApp.controller('pcController', function ($scope, $http, $filter, $ngBootbox, $window, noteService, $interval) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.pesquisaItem = { chave: '' };

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.pedidos = [];

        configuraDndLists();

        $scope.getPedidosPendentes();

        $interval(function () {
            $scope.getPedidosPendentes(true, false);
        }, 60000);
    }

    $scope.getPedidosPendentes = function (confereAlerta, mostraErro) {

        //var accesstoken = sessionStorage.getItem('accessToken');

        $scope.promiseGetPedidos = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': $scope.antiForgeryToken,
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Pedido/GetPedidosAbertos?somenteProducao=true'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.pedidos = retorno.data;

                    var pedidosConfirmadosNovo = $filter('filter')(retorno.data, { situacao: 2 });
                    var pedidosEmPreparacao = $filter('filter')(retorno.data, { situacao: 3 });
                    var pedidosAguardandoEntrega = $filter('filter')(retorno.data, { situacao: 4 });

                    if (confereAlerta) {
                        if (pedidosConfirmadosNovo.length != $scope.models[0].items.length && pedidosConfirmadosNovo.length > 0) {
                            playSound();
                        }
                    }

                    $scope.models[0].items = [];
                    $scope.models[0].items = $scope.models[0].items.concat(pedidosConfirmadosNovo);

                    $scope.models[1].items = [];
                    $scope.models[1].items = $scope.models[1].items.concat(pedidosEmPreparacao);

                    $scope.models[2].items = [];
                    $scope.models[2].items = $scope.models[2].items.concat(pedidosAguardandoEntrega);

                    angular.forEach($scope.models[0].items, function (item) { item.selected = false; });
                    angular.forEach($scope.models[1].items, function (item) { item.selected = false; });
                    angular.forEach($scope.models[2].items, function (item) { item.selected = false; });

                    //verificaTemposPedidos();

                }
                else {
                    if (mostraErro) {
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }
                }



            }, function (error) {
                if (mostraErro) {
                    $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                    $window.scrollTo(0, 0);
                }
            });


    };

    function playSound() {
        document.getElementById('play').play();
    }

    $scope.getTimeMs = function (data) {
        var dataHora = new Date(data);
        return dataHora.getTime();
    }

    $scope.determinaEstiloLinha = function (index) {
        var resto = index % 2;
        if (resto == 0) {
            return "row bg-info";
        }

        return "row bg-success";
    }

    function configuraDndLists() {

        $scope.models = [
            { listName: "Confirmados", items: [], dragging: false },
            { listName: "Em preparação", items: [], dragging: false },
            { listName: "Aguardando entrega", items: [], dragging: false }
        ];

        $scope.models[0].items = [];
        $scope.models[1].items = [];
        $scope.models[2].items = [];

        angular.forEach($scope.models[0].items, function (item) { item.selected = false; });
        angular.forEach($scope.models[1].items, function (item) { item.selected = false; });
        angular.forEach($scope.models[2].items, function (item) { item.selected = false; });

        $scope.onDrop = function (list, items, index) {
            angular.forEach(items, function (item) { item.selected = false; });
            list.items = list.items.slice(0, index)
                .concat(items)
                .concat(list.items.slice(index));

            $scope.avancarPedido(items[0]);

            return true;
        }

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

    $scope.avancarPedido = function (pedido) {

        var proximaSituacao = getProximaSituacaoPedido(pedido.situacao);
        var descricaoProximaSituacao = getDescricaoSituacaoPedido(proximaSituacao);



        pedido.situacao = proximaSituacao;

        $scope.promiseAvancaPedido = $http({
            method: 'POST',
            url: urlBase + 'Pedido/AvancarPedido',
            data: pedido,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                noteService.sendMessage(pedido.usuario, pedido.codPedido, proximaSituacao);

                if (proximaSituacao == 3) {

                    for (i = 0; i < $scope.models[0].items.length; i++) {
                        if ($scope.models[0].items[i].codPedido == pedido.codPedido) {

                            $scope.models[0].items.splice(i, 1);
                            $scope.models[1].items.push(pedido);

                            break;
                        }
                    }

                } else if (proximaSituacao == 4) {

                    for (i = 0; i < $scope.models[1].items.length; i++) {
                        if ($scope.models[1].items[i].codPedido == pedido.codPedido) {

                            $scope.models[1].items.splice(i, 1);
                            $scope.models[2].items.push(pedido);

                            break;
                        }
                    }

                }


            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.mensagem.erro = error.statusText;
            $window.scrollTo(0, 0);
        });

    }

    function addPedido(lista, pedido) {
        lista.items.push(pedido);
        angular.forEach(lista.items, function (item) { item.selected = false; });
        $scope.$apply();
    }

    $scope.getPedido = function (codPedido, situacao) {

        $scope.promiseGetPedido = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': $scope.antiForgeryToken,
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Pedido/GetPedido?CodPedido=' + codPedido + '&paraConsulta=false'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.pedidos.push(retorno.data);

                    if (situacao == 2) {
                        addPedido($scope.models[0], retorno.data);
                    }

                }
                else {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                }



            }, function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });

        $scope.promisesLoader.push($scope.promiseGetPedido);


    }

    noteService.connect();
    $scope.messages = [];

    $scope.$on('messageAdded', function (event, codPedido, situacao) {

        //mensagens de pedidos feito pelo usuario logado.
        if (situacao == 2) {
            var existente = $filter('filter')($scope.pedidos, { codPedido: codPedido })[0];
            if (existente == null) {
                $scope.getPedido(codPedido, situacao);
                playSound();
            } else if (existente.situacao == 1) {
                existente.situacao = 2;
                addPedido($scope.models[0], existente);
                playSound();
            }
        } else if (situacao == 9) {

            for (i = 0; i < $scope.models[0].items.length; i++) {
                if ($scope.models[0].items[i].codPedido == codPedido) {

                    $scope.models[0].items.splice(i, 1);

                    $scope.$apply();
                    return;
                }
            }

            for (i = 0; i < $scope.models[1].items.length; i++) {
                if ($scope.models[1].items[i].codPedido == codPedido) {

                    $scope.models[1].items.splice(i, 1);

                    $scope.$apply();
                    return;
                }
            }

            for (i = 0; i < $scope.models[2].items.length; i++) {
                if ($scope.models[2].items[i].codPedido == codPedido) {

                    $scope.models[2].items.splice(i, 1);

                    $scope.$apply();
                    return;
                }
            }

        }


    });

    $scope.sendMessage = function () {
        noteService.sendMessage($scope.pedido.usuario, $scope.pedido.codPedido, $scope.pedido.situacao);
    };


});