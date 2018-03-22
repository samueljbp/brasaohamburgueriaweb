brasaoWebApp.controller('gpController', function ($scope, $http, $filter, $ngBootbox, $window, $interval, $timeout, noteService) {
    $scope.mensagem = {
        erro: '',
        sucesso: '',
        informacao: ''
    }

    $scope.pedidos = [];
    $scope.promisesLoader = [];
    $scope.pedidoSelecionado = [];
    $scope.acao = { ehGestao: true, ehConsulta: false, atualizacaoAutomatica: true, alertaSonoro: true, pedidoSelecionado: null };

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
            $scope.getPedidosPendentes(true, false);
        }, 10000);
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
        if ($scope.acao.alertaSonoro) {
            document.getElementById('play').play();
        }
    }

    $scope.alteraTempoMedioEspera = function () {

        if (typeof $scope.tempoMedioEspera == 'undefined') {
            $scope.mensagem.erro = 'Informe um valor entre 20 e 180 minutos.';
            $window.scrollTo(0, 0);
            return;
        }

        $scope.promiseAlteraTempoMedioEspera = $http({
            method: 'POST',
            url: urlBase + 'Pedido/AlteraTempoMedioEspera',
            data: { tempo: $scope.tempoMedioEspera },
            headers: {
                'RequestVerificationToken': $scope.antiForgeryToken,
                'X-Requested-With': 'XMLHttpRequest'
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {
                $scope.mensagem.sucesso = 'Tempo de espera alterado com sucesso.';
            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $scope.promisesLoader.push($scope.promiseAlteraTempoMedioEspera);

    }

    $scope.alteraPedido = function (pedido) {
        pedido.alterar = true;
        sessionStorage.pedido = JSON.stringify(pedido);
        sessionStorage.modoAdm = "S";
        var win = window.open(urlBase + "Pedido/Index?ModoAdm=S", "_blank", "toolbar=no,scrollbars=yes,resizable=yes,top=50,left=50,height=screen.availHeight,width=screen.availWidth,menubar=no");
        win.moveTo(0, 0);
        win.resizeTo(screen.width, screen.height);
    }

    $scope.novoPedidoExterno = function () {
        sessionStorage.modoAdm = "S";
        sessionStorage.removeItem('pedido');
        var win = window.open(urlBase + "Pedido/Index?ModoAdm=S", "_blank", "toolbar=no,scrollbars=yes,resizable=yes,top=50,left=50,height=screen.availHeight,width=screen.availWidth,menubar=no");
        win.moveTo(0, 0);
        win.resizeTo(screen.width, screen.height);
    }

    $scope.getPedido = function (codPedido) {

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

    $scope.getPedidosPendentes = function (confereAlerta, mostraErro) {

        //var accesstoken = sessionStorage.getItem('accessToken');

        $scope.promiseGetPedidos = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': $scope.antiForgeryToken,
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Pedido/GetPedidosAbertos'
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                if (confereAlerta) {
                    var pedidosNovo = retorno.data;

                    if (pedidosNovo.length != $scope.pedidos.length && pedidosNovo.length > 0) {
                        playSound();
                    }
                }

                $scope.pedidos = retorno.data;
                verificaTemposPedidos();

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


    $scope.init = function (loginUsuario, antiForgeryToken, tempoMedioEspera) {
        $scope.erro = { mensagem: '' };
        $scope.sucesso = { mensagem: '' };

        $scope.tempoMedioEspera = parseInt(tempoMedioEspera);

        $scope.getPedidosPendentes(false, true);

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

    $scope.cancelaPedido = function (pedido) {
        $scope.acao.pedidoSelecionado = pedido;
        $('#modalCancelamentoPedido').modal('show');
    }

    $scope.descontoPedido = function (pedido) {
        $scope.acao.pedidoSelecionado = pedido;
        $('#modalDescontoPedido').modal('show');
    }

    $scope.calculaDesconto = function (tipo) {
        if (tipo == 'V') {
            $scope.acao.pedidoSelecionado.percentualDesconto = ($scope.acao.pedidoSelecionado.valorDesconto / $scope.acao.pedidoSelecionado.valorTotal) * 100;
            $scope.acao.pedidoSelecionado.percentualDesconto = $scope.acao.pedidoSelecionado.percentualDesconto.toFixed(2);
        } else if (tipo == 'P') {
            $scope.acao.pedidoSelecionado.valorDesconto = (($scope.acao.pedidoSelecionado.percentualDesconto / 100) * $scope.acao.pedidoSelecionado.valorTotal);
            $scope.acao.pedidoSelecionado.valorDesconto = $scope.acao.pedidoSelecionado.valorDesconto.toFixed(2);
        }
    }

    $scope.aplicaDescontoPedido = function () {

        //configura validador
        $('#formDesconto').validator({
            custom: {
                'valorDesconto': function ($el) {
                    if ($el.val() <= 0) {
                        return 'Valor de desconto inválido.';
                    }

                    if ($el.val() >= $scope.acao.pedidoSelecionado.valorTotal) {
                        return 'Valor de desconto deve ser inferior ao valor do pedido.';
                    }

                },
                'percentualDesconto': function ($el) {
                    if ($el.val() <= 0) {
                        return 'Percentual de desconto inválido.';
                    }

                    if ($el.val() > 99) {
                        return 'Percentual de desconto não pode ser superior a 99%.';
                    }
                }
            }
        });

        var hasErrors = $('#formDesconto').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.acao.pedidoSelecionado.valorDesconto = parseFloat($scope.acao.pedidoSelecionado.valorDesconto);
        $scope.acao.pedidoSelecionado.percentualDesconto = parseFloat($scope.acao.pedidoSelecionado.percentualDesconto);

        $scope.promiseGravaPedido = $http({
            method: 'POST',
            url: urlBase + 'Pedido/AplicaDesconto',
            data: $scope.acao.pedidoSelecionado,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                $scope.mensagem.sucesso = 'Desconto aplicado com sucesso.';

                $scope.acao.pedidoSelecionado = null;

                $('#modalDescontoPedido').modal('hide');

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

    $scope.executaCancelamentoPedido = function () {

        var hasErrors = $('#formCancelamento').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.acao.pedidoSelecionado.situacao = 9;

        $scope.promiseGravaPedido = $http({
            method: 'POST',
            url: urlBase + 'Pedido/AvancarPedido',
            data: $scope.acao.pedidoSelecionado,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                for (i = 0; i < $scope.pedidos.length; i++) {
                    if ($scope.pedidos[i].codPedido == $scope.acao.pedidoSelecionado.codPedido) {

                        $scope.pedidos.splice(i, 1);
                        break;
                    }
                }

                $scope.mensagem.sucesso = 'Pedido cancelado com sucesso';

                $scope.acao.pedidoSelecionado = null;

                $('#modalCancelamentoPedido').modal('hide');

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
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.mensagem.erro = error.statusText;
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
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.mensagem.erro = error.statusText;
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

                $scope.mensagem.sucesso = 'Impressão enviada com sucesso.';
                $window.scrollTo(0, 0);

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (errorInt) {
            $scope.mensagem.erro = errorInt.statusText;
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
        $scope.descricaoFormaPagamentoPedido = getDescricaoFormaPagamentoPedido($scope.pedidoSelecionado.formaPagamento);
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