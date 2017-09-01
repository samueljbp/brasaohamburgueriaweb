﻿angularFormsApp.controller('pedController', function ($scope, $http, $filter, $ngBootbox) {
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
            url: urlBase + 'Pedido/GetCardapio'
        })
        .then(function (response) {
            $scope.classes = response.data.classes;
            $scope.itensFiltrados = null;

        }, function (error) {
            console.log(error);
            $scope.classes = null;
            $scope.itensFiltrados = null;

            $('#mensagemErroFormulario').removeClass('hidden');
            $('#mensagemErroFormulario').text('Ocorreu uma falha no processamento da requisição. ' + error.data);
        });

        $scope.promisesLoader.push($scope.promiseGetCardapio);


    };

    $scope.getDadosUsuario = function () {

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
                    referencia: $scope.usuario.referencia
                };

            }, function (error) {
                alert(error);
            });

            $scope.promisesLoader.push($scope.promiseDadosUsuario);
        }
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
                $scope.pedido.itens.splice(i, 1);
                break;
            }
        }
        atualizaValorTotalPedido();

    };

    //função que atualiza o valor total do pedido
    function atualizaValorTotalPedido() {
        $scope.pedido.valorTotal = 0;

        if ($scope.pedido.itens != null && $scope.pedido.itens.length > 0) {
            for (i = 0; i < $scope.pedido.itens.length; i++) {
                $scope.pedido.valorTotal = $scope.pedido.valorTotal + $scope.pedido.itens[i].valorTotalItem;
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



                //configura validador
                $('#formFechamento').validator({
                    custom: {
                        //função de validação customizada para validar formato de e-mail
                        'email': function ($el) {
                            var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                            if ($el.val() != '' && !re.test($el.val())) {
                                return 'E-mail inválido.';
                            }
                        }
                    }
                });



            }, function () {
                //console.log('Confirm dismissed!');
            });
    }

    function reiniciaVariaveisPedido() {

        $scope.pedido = {
            formaPagamento: '',
            trocoPara: 0.0,
            troco: 0.0,
            bandeiraCartao: '',
            valorTotal: 0.0,
            situacao: 0,
            dadosCliente: {
                nome: '',
                telefone: '',
                estado: '',
                cidade: '',
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
            valorTotal: 0.0
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

        sessionStorage.pedido = JSON.stringify($scope.pedido);

        $scope.atualizaValorTotalItem();
        atualizaValorTotalPedido();

        $('#modalIncluirItem').modal('hide');
    };

    //evento de fechamento da modal de inclusão de item
    $('#modalIncluirItem').on('hide.bs.modal', function (e) {
        reiniciaVariaveisItem();
    });

    //evento de clique no botão que cancela a inclusao de um item
    $scope.cancelaInclusaoItem = function () {
        reiniciaVariaveisItem();
    }

    //função de seleção de item para inclusão
    $scope.selecionaItemParaInclusao = function (codItemCardapio) {
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
            valorTotal: 0.0
        }

        $scope.atualizaValorTotalItem();
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

    $scope.finalizaPedido = function () {

        var hasErrors = $('#formFechamento').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }


        $ngBootbox.confirm('Confirma a finalização do pedido? Não será possível retornar.')
            .then(function () {


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
        $('#modalIncluirItem').modal('show');
    }

    //FIM CONFIGURAÇÃO DE CONTROLES




    //INICIALIZAÇÃO DE VARIÁVEIS
    $scope.init = function (loginUsuario) {
        $scope.promisesLoader = [];

        $scope.loginUsuario = loginUsuario;

        //variável para exibir as classes de item na combo da modal de inclusão de item
        $scope.classes = null;

        //variável para exibir os itens na lista após seleção da classe
        $scope.itensFiltrados = null;

        //variável para controlar se tem um item selecionado para inclusão
        $scope.itemCardapioSelecionado = null;

        //variável para controlar se tem uma classe selecionada na combo
        $scope.comboClasse = { codClasse: 0, descricaoClasse: 'Selecione o tipo de item' }
        //FIM DA DECLARAÇÃO DE VARIÁVEIS

        $scope.getCardapio();

        //variável que armazena os dados do pedido que está sendo montado
        if (!sessionStorage.pedido) {
            reiniciaVariaveisPedido();
            $scope.getDadosUsuario();
        } else {
            $scope.pedido = JSON.parse(sessionStorage.pedido);
        }

        atualizaValorTotalPedido();

        reiniciaVariaveisItem();
    }
    //FIM INICIALIZAÇÃO DE VARIÁVEIS
});


angularFormsApp.directive('format', ['$filter', function ($filter) {
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