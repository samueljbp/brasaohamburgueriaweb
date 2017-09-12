angularFormsApp.controller('gpController', function ($scope, $http, $filter, $ngBootbox, noteService) {
    $erro = { mensagem: '' };

    $scope.pedidos = [];
    $scope.promisesLoader = [];
    $scope.pedidoSelecionado = [];

    $scope.getPedido = function (codPedido) {

        $scope.promiseGetPedido = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Pedido/GetPedido?CodPedido=' + codPedido
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.pedidos.push(retorno.data);

            }
            else {
                $scope.erro.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }



        }, function (error) {
            $scope.erro.mensagem = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $scope.promisesLoader.push($scope.promiseGetPedido);


    }

    $scope.getPedidosPendentes = function () {

        //var accesstoken = sessionStorage.getItem('accessToken');

        $scope.promiseGetPedidos = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Pedido/GetPedidosAbertos'
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.pedidos = retorno.data;

            }
            else {
                $scope.erro.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }



        }, function (error) {
            $scope.erro.mensagem = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $scope.promisesLoader.push($scope.promiseGetPedidos);


    };

    noteService.connect();
    $scope.messages = [];
 
    $scope.$on('messageAdded', function (event, codPedido, situacao) {
        
        //mensagens de pedidos feito pelo usuario logado.
        if (situacao > 1 && situacao < 5) {
            return;
        }

        //novos pedidos, recuperar e incluir na tela
        if (situacao == 1) {
            $scope.getPedido(codPedido);
            return;
        }
        
        for (i = 0; i < $scope.pedidos.length; i++) {
            if ($scope.pedidos[i].codPedido == codPedido) {

                //var descricaoSituacao = getDescricaoSituacaoPedido(situacao);

                //$scope.pedidos[i].situacao = situacao;
                //$scope.pedidos[i].descricaoSituacao = descricaoSituacao;

                $scope.pedidos.splice(i, 1);

                $scope.$apply();
                return;
            }
        }

    });
 
    $scope.sendMessage = function () {
        noteService.sendMessage($scope.pedido.usuario, $scope.pedido.codPedido, $scope.pedido.situacao);
    };


    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.getPedidosPendentes();

        $scope.startTime = Date.parse("2017-09-11 14:06:00");
    }

    $scope.getTimeMs = function (data) {
        var dataHora = new Date(data);
        return dataHora.getTime();
    }

    $scope.avancarPedido = function (pedido) {

        var proximaSituacao = getProximaSituacaoPedido(pedido.situacao);
        var descricaoProximaSituacao = getDescricaoSituacaoPedido(proximaSituacao);

        $ngBootbox.confirm('Deseja o pedido para a situação "' + descricaoProximaSituacao + '"?')
            .then(function () {

                for (i = 0; i < $scope.pedidos.length; i++) {
                    if ($scope.pedidos[i].codPedido == pedido.codPedido) {

                        noteService.sendMessage(pedido.usuario, pedido.codPedido, proximaSituacao);

                        $scope.pedidos[i].situacao = proximaSituacao;
                        $scope.pedidos[i].descricaoSituacao = descricaoProximaSituacao;
                        $scope.$apply();
                        return;
                    }
                }

                return;

                $scope.promiseGravaPedido = $http({
                    method: 'POST',
                    url: urlBase + 'Pedido/GravarPedido',
                    data: $scope.pedido,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        reiniciaVariaveis();
                        reiniciaVariaveisPedido();
                        reiniciaVariaveisItem();

                        sessionStorage.pedido = null;
                        sessionStorage.codPedido = retorno.data;

                        window.location.href = urlBase + '/Pedido/PedidoRegistrado';

                    }
                    else {
                        $scope.erro.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.erro.mensagem = error.statusText;
                    $window.scrollTo(0, 0);
                });



            }, function () {
                //console.log('Confirm dismissed!');
            });



    }

    $scope.visualizaPedido = function (pedido) {
        $scope.pedidoSelecionado = pedido;
        $('#modalDetalhesPedido').modal('show');
    }

    $scope.determinaEstiloBotao = function (pedido, tipo) {
        var retorno = { classeBotao: "", classeIcone: "", classeSituacao: "" }

        switch (pedido.situacao) {
            case 1:
                retorno.classeIcone = "glyphicon glyphicon-thumbs-up";
                retorno.classeBotao = "btn btn-success btn-sm";
                retorno.classeSituacao = "text-left text-success";
                break;
            case 2:
                retorno.classeIcone = "glyphicon glyphicon-cutlery";
                retorno.classeBotao = "btn btn-primary btn-sm";
                retorno.classeSituacao = "text-left text-primary";
                break;
            case 3:
                retorno.classeIcone = "glyphicon glyphicon-plane";
                retorno.classeBotao = "btn btn-warning btn-sm";
                retorno.classeSituacao = "text-left text-warning";
                break;
            default:
                retorno.classeIcone = "";
                retorno.classeBotao = "";
                retorno.classeSituacao = "";
                break;
        }

        switch (tipo) {
            case 'I':
                return retorno.classeIcone;
            case 'B':
                return retorno.classeBotao;
            case 'S':
                return retorno.classeSituacao;
            default:
                return '';
        }
    }

});