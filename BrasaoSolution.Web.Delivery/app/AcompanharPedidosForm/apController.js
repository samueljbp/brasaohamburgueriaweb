brasaoWebApp.controller('apController', function ($scope, $http, $filter, $ngBootbox, $interval, $window, $timeout, noteService) {

    $scope.promisesLoader = [];

    $scope.getPedidoAberto = function (loginUsuario) {

        //var accesstoken = sessionStorage.getItem('accessToken');

        $scope.promiseGetPedidoAberto = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Pedido/GetPedidoAberto?loginUsuario=' + loginUsuario
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                var situacaoAnterior = -1;
                if ($scope.pedidoSelecionado) {
                    situacaoAnterior = $scope.pedidoSelecionado.situacao;
                }

                $scope.pedidoSelecionado = retorno.data;
                $scope.tempoMedioEspera = $scope.pedidoSelecionado.tempoMedioEspera;
                $scope.descricaoSituacaoPedido = getDescricaoSituacaoPedido($scope.pedidoSelecionado.situacao);

                if (situacaoAnterior == 1 && $scope.pedidoSelecionado.situacao == 2) {
                    $('#modalPedidoConfirmado').modal('show');
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

        $scope.promisesLoader.push($scope.promiseGetPedidoAberto);


    };

    $scope.getTimeMs = function (data) {
        var dataHora = new Date(data);
        return dataHora.getTime();
    }

    $scope.cancelaPedido = function () {


        $ngBootbox.confirm('Confirma o cancelamento definitivo do pedido?')
            .then(function () {

                $scope.pedidoSelecionado.situacao = 9;

                $scope.promiseFinalizaPedido = $http({
                    method: 'POST',
                    url: urlBase + 'Pedido/AvancarPedido',
                    data: $scope.pedidoSelecionado,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {
                        noteService.sendMessage('', $scope.pedidoSelecionado.codPedido, 9);
                        window.location.href = urlBase + '/Home/Index';

                    }
                    else {
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                    $window.scrollTo(0, 0);
                });



            }, function () {
                //console.log('Confirm dismissed!');
            });


    }

    $scope.finalizaPedido = function () {

        var hasErrors = $('#formPedido').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $ngBootbox.confirm('Confirma o recebimento da entrega?')
            .then(function () {

                $scope.pedidoSelecionado.situacao = 5;

                $scope.promiseFinalizaPedido = $http({
                    method: 'POST',
                    url: urlBase + 'Pedido/AvancarPedido',
                    data: $scope.pedidoSelecionado,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {
                        var proximaSituacao = getProximaSituacaoPedido($scope.pedidoSelecionado.situacao);

                        noteService.sendMessage('', $scope.pedidoSelecionado.codPedido, proximaSituacao);
                        window.location.href = urlBase + '/Home/Index';

                    }
                    else {
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                    $window.scrollTo(0, 0);
                });



            }, function () {
                //console.log('Confirm dismissed!');
            });



    }

    $scope.consultarPedidos = function () {
        window.location.href = urlBase + 'Pedido/ConsultarPedidos';
    }

    noteService.connect();
    $scope.messages = [];

    $scope.$on('messageAdded', function (event, codPedido, situacao) {
        
        //para ignorar mensagens de broadcast
        if ($scope.pedidoSelecionado.codPedido != codPedido) {
            return;
        }

        if ($scope.pedidoSelecionado.situacao == 1 && situacao == 2) {
            $('#modalPedidoConfirmado').modal('show');
        }

        $scope.pedidoSelecionado.situacao = parseInt(situacao);
        $scope.descricaoSituacaoPedido = getDescricaoSituacaoPedido($scope.pedidoSelecionado.situacao);

        $scope.$apply();
    });

    $scope.sendMessage = function () {
        noteService.sendMessage($scope.pedidoSelecionado.usuario, $scope.pedidoSelecionado.codPedido, $scope.pedidoSelecionado.situacao);
    };


    $scope.init = function (loginUsuario, antiForgeryToken, tempoMedioEspera) {
        $scope.tempoMedioEspera = parseInt(tempoMedioEspera);
        $scope.loginUsuario = loginUsuario;
        $scope.antiForgeryToken = antiForgeryToken;
        $scope.getPedidoAberto(loginUsuario);

        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.pedidoSelecionado = { usuario: '', codPedido: 1, situacao: 0 };
        $scope.acao = { ehGestao: false, ehConsulta: false };

        $interval(function () {
            $scope.getPedidoAberto($scope.loginUsuario);
        }, 60000);
    }

});