brasaoWebApp.directive('scrollToTop', function ($window, $timeout) {
    return {
        restrict: 'A',
        link: function postLink(scope, elem, attrs) {
            scope.$watch(attrs.scrollToTop, function () {
                //elem[0].scrollTop = 0;
                $timeout(function () { $("#modalIncluirItem").animate({ scrollTop: 0 }, "fast"); });
            });
        }
    };
});

brasaoWebApp.controller('pedController', function ($scope, $http, $filter, $ngBootbox, $window, $timeout, noteService) {
    //FUNÇÕES DA TELA QUE LISTA O PEDIDO
    //função que carrega o cardápio em memória assincronamente
    $scope.getCardapio = function () {

        //var accesstoken = sessionStorage.getItem('accessToken');

        $scope.promiseGetCardapio = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cardapio/GetCardapio'
        })
        .then(function (response) {
            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.classes = retorno.data;
                $scope.itensFiltrados = null;

            }
            else {
                $scope.erro.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }, function (error) {
            console.log(error);
            $scope.classes = null;
            $scope.itensFiltrados = null;

            $scope.erro.mensagem = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $scope.promisesLoader.push($scope.promiseGetCardapio);


    };



    //função que retorna pedido em aberto, caso exista
    $scope.getPedidoAberto = function (loginUsuario) {

        if (sessionStorage.modoAdm == "S") {
            $scope.pedidoAberto = null;
            return;
        }

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

                $scope.pedidoAberto = retorno.data;

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



    $scope.getDadosUsuario = function () {

        if (sessionStorage.modoAdm == "S") {
            $scope.pedido.dadosCliente = {
                nome: '',
                telefone: '',
                estado: 'MG',
                cidade: 'Cataguases',
                logradouro: '',
                numero: '',
                complemento: '',
                bairro: '',
                referencia: '',
                clienteNovo: false
            }

            return;
        }

        if ($scope.loginUsuario) {

            $scope.promiseDadosUsuario = $http({
                method: 'GET',
                url: urlBase + 'Conta/GetUsuario'
            })
            .then(function (response) {

                $scope.usuario = response.data;

                $scope.pedido.dadosCliente = {
                    nome: $scope.usuario.nome,
                    telefone: $scope.usuario.telefone,
                    estado: $scope.usuario.estado,
                    cidade: $scope.usuario.cidade,
                    logradouro: $scope.usuario.logradouro,
                    numero: $scope.usuario.numero,
                    complemento: $scope.usuario.complemento,
                    bairro: $scope.usuario.bairro,
                    referencia: $scope.usuario.referencia,
                    clienteNovo: false
                };

            }, function (error) {
                $scope.erro.mensagem = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });

            $scope.promisesLoader.push($scope.promiseDadosUsuario);
        }
    }

    $scope.getDadosUsuarioByPhone = function (telefone) {

        $scope.formataTelefone();

        if (!$scope.modoAdm.ativo) {
            return;
        }

        $scope.promiseDadosUsuarioByPhone = $http({
            method: 'GET',
            url: urlBase + 'Conta/GetUsuarioByPhone?telefone=' + telefone
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {
                $scope.erro.mensagem = '';
                $scope.sucesso.mensagem = '';
                if (retorno.data != null && retorno.data != '') {
                    $scope.usuario = retorno.data;

                    $scope.pedido.dadosCliente = {
                        nome: $scope.usuario.nome,
                        telefone: $scope.usuario.telefone,
                        estado: $scope.usuario.estado,
                        cidade: $scope.usuario.cidade,
                        logradouro: $scope.usuario.logradouro,
                        numero: $scope.usuario.numero,
                        complemento: $scope.usuario.complemento,
                        bairro: $scope.usuario.bairro,
                        referencia: $scope.usuario.referencia,
                        clienteNovo: $scope.usuario.clienteNovo,
                        salvar: $scope.usuario.salvar
                    }

                    if ($scope.usuario.id <= 0) {
                        $scope.informacao.mensagem = 'Cliente não cadastrado.';

                    }
                } else {
                    $scope.informacao.mensagem = 'Cliente não encontrado.';
                }
            } else {
                $scope.informacao.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }



        }, function (error) {
            $scope.informacao.mensagem = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $scope.promisesLoader.push($scope.promiseDadosUsuarioByPhone);
    }

    //função que abre uma imagem grande em popup a partir de um thumbnail
    $scope.exibeImagemGrande = function (codItemCardapio) {
        var imagem = $filter('filter')($scope.itensFiltrados, { codItemCardapio: codItemCardapio })[0].complemento.imagem;
        $('#imagepreview').attr('src', urlBase + '/Content/img/itens_cardapio/' + imagem);
        $('#imagemodal').modal('show');
    }

    //função de confirmação de exclusão de item do pedido
    $scope.confirmCallbackRemove = function (seqItem) {

        for (i = 0; i < $scope.pedido.itens.length; i++) {
            if ($scope.pedido.itens[i].seqItem == seqItem) {
                if ($scope.pedido.itens[i].acaoRegistro == acaoRegistro.incluir) {
                    $scope.pedido.itens.splice(i, 1);
                } else if ($scope.pedido.itens[i].acaoRegistro == acaoRegistro.nenhuma) {
                    $scope.pedido.itens[i].acaoRegistro = acaoRegistro.cancelar;
                }

                break;
            }
        }
        atualizaValorTotalPedido();
        sessionStorage.pedido = JSON.stringify($scope.pedido);
    };

    //função que atualiza o valor total do pedido
    function atualizaValorTotalPedido() {
        $scope.pedido.valorTotal = 0;

        if ($scope.pedido.itens != null && $scope.pedido.itens.length > 0) {
            $scope.pedido.valorTotal = $scope.pedido.taxaEntrega;

            for (i = 0; i < $scope.pedido.itens.length; i++) {
                if ($scope.pedido.itens[i].acaoRegistro != acaoRegistro.cancelar) {
                    $scope.pedido.valorTotal = $scope.pedido.valorTotal + $scope.pedido.itens[i].valorTotalItem;
                }

            }
        }
    }

    //função que obtem o sequencial do próximo item do pedido
    function getNextSeqItemPedido() {
        var seqItem = 0;

        if ($scope.pedido.itens != null && $scope.pedido.itens.length > 0) {
            for (i = 0; i < $scope.pedido.itens.length; i++) {
                if ($scope.pedido.itens[i].seqItem > seqItem) {
                    seqItem = $scope.pedido.itens[i].seqItem;
                }
            }
        }

        seqItem = seqItem + 1;

        return seqItem;
    }

    $scope.confirmAvancaPedido = function () {
        if ($scope.pedido.itens == null || $scope.pedido.itens.length == 0) {
            $ngBootbox.alert("Você não incluiu nenhum item no pedido!");
            return;
        }


        $ngBootbox.confirm('Deseja avançar para a finalização do pedido?')
            .then(function () {
                $scope.pedido.situacao = 1;

                sessionStorage.pedido = JSON.stringify($scope.pedido);

            }, function () {
                //console.log('Confirm dismissed!');
            });
    }

    function reiniciaVariaveisPedido() {

        $scope.pedido = {
            formaPagamento: '',
            taxaEntrega: $scope.taxaEntrega,
            trocoPara: 0.0,
            troco: 0.0,
            bandeiraCartao: '',
            valorTotal: 0.0,
            situacao: 0,
            pedidoExterno: $scope.modoAdm.ativo,
            dadosCliente: {
                salvar: false,
                nome: '',
                telefone: '',
                estado: 'MG',
                cidade: 'Cataguases',
                logradouro: '',
                numero: '',
                complemento: '',
                bairro: '',
                referencia: ''
            },
            itens: []
        };

    }
    //FIM FUNÇÕES DA TELA QUE LISTA O PEDIDO



    //FUNÇÕES DA TELA DE INCLUSÃO DE ITEM

    function reiniciaVariaveisItem() {
        $scope.itemCardapioSelecionado = null;

        $scope.dadosItemCardapio = {
            codItemCardapio: 0,
            observacoes: [],
            extras: []
        }

        $scope.novoItem = {
            seqItem: 0,
            codItem: 0,
            descricaoItem: '',
            obs: [],
            observacaoLivre: '',
            extras: [],
            quantidade: 0,
            precoUnitario: 0.0,
            valorExtras: 0.0,
            valorTotalItem: 0.0
        }

    }

    $scope.calculaTroco = function () {
        $scope.pedido.troco = $scope.pedido.trocoPara - $scope.pedido.valorTotal;
        if ($scope.pedido.troco < 0) {
            $scope.pedido.troco = 0;
        }
    }

    //função que atualiza valor total de um item que está sendo incluído
    $scope.atualizaValorTotalItem = function () {
        var valor = 0.0;

        valor = valor + ($scope.novoItem.quantidade * $scope.novoItem.precoUnitario);

        $scope.novoItem.valorExtras = calculaValorTotalExtras();

        valor = valor + $scope.novoItem.valorExtras;

        $scope.novoItem.valorTotalItem = valor;
    };

    function calculaValorTotalExtras() {
        var valorExtras = 0.0;

        $.each($scope.novoItem.extras, function (index, value) {
            if (value != null && value.preco != null) {
                valorExtras = valorExtras + value.preco;
            }
        });

        return valorExtras;
    }

    //função que filtra a lista de itens a incluir por classe
    $scope.filtraClasse = function (codClasse) {
        $scope.comboClasse.codClasse = codClasse;
        var filtro = $filter('filter')($scope.classes, { codClasse: codClasse });
        $scope.comboClasse.descricaoClasse = filtro[0].descricaoClasse;
        $scope.itensFiltrados = filtro[0].itens;
    };

    //função que confirma a inclusão de um item no pedido
    $scope.confirmaInclusaoItem = function () {

        if ($scope.novoItem.observacaoLivre != '') {
            $scope.novoItem.obs.push({ codObservacao: -1, descricaoObservacao: $scope.novoItem.observacaoLivre });
        }

        $scope.pedido.itens.push($scope.novoItem);

        $scope.atualizaValorTotalItem();
        atualizaValorTotalPedido();

        sessionStorage.pedido = JSON.stringify($scope.pedido);

        $('#modalIncluirItem').modal('hide');
    };

    //evento de clique no botão que cancela a inclusao de um item
    $scope.cancelaInclusaoItem = function () {
        reiniciaVariaveisItem();
    }

    $scope.formataTelefone = function () {
        var numero = $scope.pedido.dadosCliente.telefone;
        if (numero.length <= 11) {
            var ddd = numero.substring(0, 2);
            var resto = numero.substring(2);
            var primeiroBloco = '';
            var segundoBloco = '';
            if (resto.length > 8) {
                primeiroBloco = resto.substring(0, 5);
                segundoBloco = resto.substring(5);
            } else {
                primeiroBloco = resto.substring(0, 4);
                segundoBloco = resto.substring(4);
            }

            $scope.pedido.dadosCliente.telefone = '(' + ddd + ') ' + primeiroBloco + '-' + segundoBloco;
        }
    }

    //função de seleção de item para inclusão
    $scope.selecionaItemParaInclusao = function (codItemCardapio) {

        $scope.promiseGetDadosItemCardapio = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cardapio/GetDadosItemCardapio?codItemCardapio=' + codItemCardapio
        })
        .then(function (response) {
            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.dadosItemCardapio = retorno.data;

                $scope.itemCardapioSelecionado = $filter('filter')($scope.itensFiltrados, { codItemCardapio: codItemCardapio })[0];

                $scope.novoItem = {
                    seqItem: getNextSeqItemPedido(),
                    codItem: $scope.itemCardapioSelecionado.codItemCardapio,
                    descricaoItem: $scope.itemCardapioSelecionado.nome,
                    obs: [],
                    observacaoLivre: '',
                    extras: [],
                    quantidade: 1,
                    precoUnitario: $scope.itemCardapioSelecionado.preco,
                    valorExtras: 0.0,
                    valorTotal: 0.0,
                    acaoRegistro: acaoRegistro.incluir
                }

                $scope.atualizaValorTotalItem();

                $timeout(function () { $("#modalIncluirItem").animate({ scrollTop: 0 }, "fast"); });

            }
            else {
                $scope.erro.mensagem = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }, function (error) {
            console.log(error);

            $scope.erro.mensagem = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

    }

    //função que incrementa a quantidade de um item a incluir
    $scope.incrementaQuantidade = function () {
        $scope.novoItem.quantidade = $scope.novoItem.quantidade + 1;
        $scope.atualizaValorTotalItem();
    }

    //função que decrementa a quantidade de um item a incluir
    $scope.decrementaQuantidade = function () {
        if ($scope.novoItem.quantidade > 1) {
            $scope.novoItem.quantidade = $scope.novoItem.quantidade - 1;
            $scope.atualizaValorTotalItem();
        }
    }

    //FIM FUNÇÕES DA TELA DE INCLUSÃO DE ITEM


    //FUNÇÕES DA TELA DE FECHAMENTO DO PEDIDO

    $scope.retornarTelaPedido = function () {
        $scope.pedido.situacao = 0;

        sessionStorage.pedido = JSON.stringify($scope.pedido);
    }

    $scope.finalizarPedidoAberto = function () {

        $ngBootbox.confirm('Confirma o encerramento do pedido que está em aberto?')
            .then(function () {


                $scope.promiseFinalizaPedido = $http({
                    method: 'POST',
                    url: urlBase + 'Pedido/FinalizaPedido',
                    data: $scope.pedidoAberto,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        $scope.pedidoAberto = null;

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

    $scope.finalizaPedido = function () {

        var hasErrors = $('#formFechamento').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        if ($scope.pedido.trocoPara > 0) {
            if ($scope.pedido.trocoPara < $scope.pedido.valorTotal) {
                $scope.erro.mensagem = 'O valor informado para o pagamento em dinheiro está menor que o valor total do pedido.';
                $window.scrollTo(0, 0);
                return;
            }

            if ($scope.troco < 0) {
                $scope.troco = 0;
            }

            $scope.pedido.trocoPara = parseFloat($scope.pedido.trocoPara);
        } else if ($scope.pedido.formaPagamento == 'D' && $scope.pedido.trocoPara <= 0) {
            $scope.erro.mensagem = 'Informe como o pagamento em dinheiro será realizado.';
            $window.scrollTo(0, 0);
            return;
        }

        $ngBootbox.confirm('Confirma a finalização do pedido? Não será possível retornar.')
            .then(function () {


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

                        var codPedido = retorno.data;

                        noteService.sendMessage('', codPedido, 1);

                        sessionStorage.removeItem('pedido');
                        sessionStorage.codPedido = codPedido;

                        if ($scope.modoAdm.ativo) {
                            sessionStorage.modoAdm = "N";
                            setTimeout(function () { window.close(); }, 2000);
                        } else {
                            window.location.href = urlBase + 'Pedido/PedidoRegistrado';
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
    //FUM FUNÇÕES DA TELA DE FECHAMENTO DO PEDIDO


    //CONFIGURAÇÃO DE CONTROLES
    //configura a popup de mensagem

    var locale = {
        OK: 'OK',
        CANCEL: 'Cancelar',
        CONFIRM: 'Confirmar'
    };

    bootbox.addLocale('pt', locale);

    $ngBootbox.setDefaults({
        animate: false,
        backdrop: false,
        title: 'ATENÇÃO!',
        locale: 'pt'
    });

    //configura a modal de inclusão de item
    $scope.modalIncluirItem = function () {
        reiniciaVariaveisItem();
        $('#modalIncluirItem').modal('show');
    }
    //FIM CONFIGURAÇÃO DE CONTROLES


    //INICIALIZAÇÃO DE VARIÁVEIS
    function reiniciaVariaveis() {
        $scope.erro = { mensagem: '' };
        $scope.sucesso = { mensagem: '' };
        $scope.informacao = { mensagem: '' };

        $scope.promisesLoader = [];

        //variável que armazena um pedido em aberto, caso ele exista
        $scope.pedidoAberto = {};

        //variável para exibir as classes de item na combo da modal de inclusão de item
        $scope.classes = null;

        //variável para exibir os itens na lista após seleção da classe
        $scope.itensFiltrados = null;

        //variável para controlar se tem um item selecionado para inclusão
        $scope.itemCardapioSelecionado = null;

        //variável para controlar se tem uma classe selecionada na combo
        $scope.comboClasse = { codClasse: 0, descricaoClasse: 'Selecione o tipo de item' }
    }
    //FIM DA DECLARAÇÃO DE VARIÁVEIS

    $scope.init = function (loginUsuario, antiForgeryToken, taxaEntrega) {
        $scope.modoAdm = { ativo: true };
        if (sessionStorage.getItem("modoAdm") == 'null' || sessionStorage.getItem("modoAdm") == null || sessionStorage.getItem("modoAdm") == 'N') {
            sessionStorage.modoAdm = "N";
            $scope.modoAdm.ativo = false;
        } else {

        }

        noteService.connect();
        $scope.messages = [];

        $scope.taxaEntrega = parseFloat(taxaEntrega);

        $scope.loginUsuario = loginUsuario;

        $scope.antiForgeryToken = antiForgeryToken;

        reiniciaVariaveis();

        $scope.getCardapio();

        $scope.getPedidoAberto(loginUsuario);

        //variável que armazena os dados do pedido que está sendo montado
        if (sessionStorage.pedido == null || sessionStorage.pedido == "null") {
            reiniciaVariaveisPedido();
            $scope.getDadosUsuario();
        } else {
            $scope.pedido = JSON.parse(sessionStorage.pedido);
            if ($scope.pedido.alterar) {
                $scope.pedido.situacao = 0;
            }
        }

        atualizaValorTotalPedido();

        reiniciaVariaveisItem();
    }
    //FIM INICIALIZAÇÃO DE VARIÁVEIS
});


brasaoWebApp.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            var format = {
                prefix: '',
                centsSeparator: '.',
                thousandsSeparator: ''
            };

            ctrl.$parsers.unshift(function (value) {
                elem.priceFormat(format);
                console.log('parsers', elem[0].value);
                return elem[0].value;
            });

            ctrl.$formatters.unshift(function (value) {
                elem[0].value = ctrl.$modelValue * 100;
                elem.priceFormat(format);
                console.log('formatters', elem[0].value);
                return elem[0].value;
            })
        }
    };
}]);