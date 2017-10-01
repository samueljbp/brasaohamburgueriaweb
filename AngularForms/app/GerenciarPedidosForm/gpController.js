brasaoWebApp.controller('gpController', function ($scope, $http, $filter, $ngBootbox, $window, $interval, $timeout, noteService) {
    $erro = { mensagem: '' };

    $scope.pedidos = [];
    $scope.promisesLoader = [];
    $scope.pedidoSelecionado = [];
    $scope.acao = { ehGestao: true, atualizacaoAutomatica: true };

    $scope.checkAtualizacaoAutomatica = function () {
        if ($scope.acao.atualizacaoAutomatica) {
            $scope.iniciarTimer();
        } else {
            $scope.pararTimer();
        }
    }


    var parar;
    $scope.iniciarTimer = function () {
        // Don't start a new fight if we are already fighting
        if (angular.isDefined(parar)) return;

        parar = $interval(function () {
            $scope.getPedidosPendentes(true);
        }, 5000);
    };

    $scope.pararTimer = function () {
        if (angular.isDefined(parar)) {
            $interval.cancel(parar);
            parar = undefined;
        }
    };

    $scope.$on('$destroy', function () {
        // Make sure that the interval is destroyed too
        $scope.pararTimer();
    });


    function playSound() {
        document.getElementById('play').play();
    }

    $scope.alteraPedido = function (pedido) {
        pedido.alterar = true;
        sessionStorage.pedido = JSON.stringify(pedido);
        sessionStorage.modoAdm = "S";
        var win = window.open(urlBase + "Pedido/Index", "_blank", "toolbar=no,scrollbars=yes,resizable=yes,top=50,left=50,height=screen.availHeight,width=screen.availWidth,menubar=no");
        win.moveTo(0, 0);
        win.resizeTo(screen.width, screen.height);
    }

    $scope.novoPedidoExterno = function () {
        sessionStorage.modoAdm = "S";
        var win = window.open(urlBase + "Pedido/Index", "_blank", "toolbar=no,scrollbars=yes,resizable=yes,top=50,left=50,height=screen.availHeight,width=screen.availWidth,menubar=no");
        win.moveTo(0, 0);
        win.resizeTo(screen.width, screen.height);
    }

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

    $scope.getPedidosPendentes = function (confereeAlerta) {

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

                if (confereeAlerta) {
                    var pedidosNovo = retorno.data;

                    if (pedidosNovo.length != $scope.pedidos.length && pedidosNovo.length > 0) {
                        playSound();
                    }
                }

                $scope.pedidos = retorno.data;
                verificaTemposPedidos();

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
            var existente = $filter('filter')($scope.pedidos, { codPedido: codPedido })[0];
            if (existente == null) {
                $scope.getPedido(codPedido);
                playSound();
            }            
            return;
        }

        //pedidos concluídos ou cancelados, retirar da lista
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

    function verificaTemposPedidos() {
        for (i = 0; i < $scope.pedidos.length; i++) {

            $scope.pedidos[i].estiloLinhaPorTempo = "";

            var dataHora = new Date($scope.pedidos[i].dataPedido);
            var agora = new Date();
            var diff = agora.getTime() - dataHora.getTime();

            //se está aguardando confirmação a mais de 5 minutos
            if ($scope.pedidos[i].situacao == 1 && diff >= 300000) {
                $scope.pedidos[i].estiloLinhaPorTempo = "corAlerta";
            } else if (diff >= 1200000) {
                if (diff < 2400000) { //mais de 40 minutos
                    $scope.pedidos[i].estiloLinhaPorTempo = "warning";
                } else { //mais de 20 minutos
                    $scope.pedidos[i].estiloLinhaPorTempo = "danger";
                }
            }
        }
    }

    $scope.sendMessage = function () {
        noteService.sendMessage($scope.pedido.usuario, $scope.pedido.codPedido, $scope.pedido.situacao);
    };


    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.erro = { mensagem: '' };
        $scope.sucesso = { mensagem: '' };

        $scope.getPedidosPendentes(false);

        $scope.antiForgeryToken = antiForgeryToken;

        if ($scope.acao.atualizacaoAutomatica) {
            $scope.checkAtualizacaoAutomatica();
        }

        $interval(function () {
            verificaTemposPedidos();
        }, 60000);
    }

    $scope.getTimeMs = function (data) {
        var dataHora = new Date(data);
        return dataHora.getTime();
    }

    $scope.finalizaPedido = function (pedido) {

        var proximaSituacao = 5;
        var descricaoProximaSituacao = getDescricaoSituacaoPedido(proximaSituacao);

        $ngBootbox.confirm('Deseja finalizar o pedido ' + pedido.codPedido + '?')
            .then(function () {

                pedido.situacao = proximaSituacao;

                $scope.promiseGravaPedido = $http({
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

                        for (i = 0; i < $scope.pedidos.length; i++) {
                            if ($scope.pedidos[i].codPedido == pedido.codPedido) {

                                $scope.pedidos.splice(i, 1);
                                break;
                            }
                        }

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

    $scope.avancarPedido = function (pedido) {

        var proximaSituacao = getProximaSituacaoPedido(pedido.situacao);
        var descricaoProximaSituacao = getDescricaoSituacaoPedido(proximaSituacao);

        $ngBootbox.confirm('Deseja avançar o pedido ' + pedido.codPedido + ' para a situação "' + descricaoProximaSituacao + '"?')
            .then(function () {

                pedido.situacao = proximaSituacao;

                $scope.promiseGravaPedido = $http({
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

                        for (i = 0; i < $scope.pedidos.length; i++) {
                            if ($scope.pedidos[i].codPedido == pedido.codPedido) {

                                if (!pedido.pedidoExterno) {
                                    noteService.sendMessage(pedido.usuario, pedido.codPedido, proximaSituacao);
                                }

                                $scope.pedidos[i].situacao = proximaSituacao;
                                $scope.pedidos[i].descricaoSituacao = descricaoProximaSituacao;
                                break;
                            }
                        }

                        if (proximaSituacao == 2) {

                            imprime(pedido);

                        }



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

    function imprime(pedido) {
        $scope.promiseImprime = $http({
            method: 'POST',
            crossDomain: true,
            dataType: 'json',
            url: urlWebAPIBase + 'Impressao/ImprimePedido',
            data: pedido
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                $scope.sucesso.mensagem = 'Impressão enviada com sucesso.';
                $window.scrollTo(0, 0);

            }
            else {
                $scope.erro.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (errorInt) {
            $scope.erro.mensagem = errorInt.statusText;
            $window.scrollTo(0, 0);
        });
    }

    $scope.imprimePedido = function (pedido) {

        $ngBootbox.confirm('Confirma a impressão do pedido ' + pedido.codPedido + ' ?')
            .then(function () {

                imprime(pedido);

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
            case 0:
                retorno.classeIcone = "glyphicon glyphicon-thumbs-up";
                retorno.classeBotao = "btn btn-success btn-sm";
                retorno.classeSituacao = "text-left text-success";
                break;
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