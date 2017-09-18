brasaoWebApp.controller('apController', function ($scope, $http, $filter, $ngBootbox, noteService) {
    $scope.erro = { mensagem: '' };
    $scope.pedidoSelecionado = { usuario: '', codPedido: 1, situacao: 0 };
    $scope.promisesLoader = [];
    $scope.acao = { ehGestao: false };

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

                $scope.pedidoSelecionado = retorno.data;
                $scope.descricaoSituacaoPedido = getDescricaoSituacaoPedido($scope.pedidoSelecionado.situacao);
                $scope.descricaoFormaPagamentoPedido = getDescricaoFormaPagamentoPedido($scope.pedidoSelecionado.formaPagamento);

            }
            else {
                $scope.erro.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }, function (error) {
            $scope.erro.mensagem = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $scope.promisesLoader.push($scope.promiseGetPedidoAberto);


    };

    $scope.finalizaPedido = function () {



        $ngBootbox.confirm('Confirma o recebimento da entrega?')
            .then(function () {


                $scope.promiseFinalizaPedido = $http({
                    method: 'POST',
                    url: urlBase + 'Pedido/FinalizaPedido',
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


    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.antiForgeryToken = antiForgeryToken;
        $scope.getPedidoAberto(loginUsuario);
    }

});