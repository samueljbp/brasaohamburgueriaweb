angularFormsApp.controller('pedController', function ($scope, $http, $filter, $ngBootbox) {

    //DECLARAÇÃO DE VARIÁVEIS
    //variável que armazena os dados do pedido que está sendo montado
    $scope.pedido = null;

    //variável para controlar se tem um item selecionado para inclusão
    $scope.itemCardapioSelecionado = null;

    //variável para controlar se tem uma classe selecionada na combo
    $scope.comboClasse = { codClasse: 0, descricaoClasse: 'Selecione o tipo de item' }
    //FIM DA DECLARAÇÃO DE VARIÁVEIS

    
    //FUNÇÕES DA TELA QUE LISTA O PEDIDO
    //função que carrega o cardápio em memória assincronamente
    $scope.getCardapio = function () {

        //var accesstoken = sessionStorage.getItem('accessToken');

        $http({
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


    };

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

    $scope.confirmCallbackAvancaPedido = function () {
        $scope.pedido.situacao = 1;
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
    

    
    //CONFIGURAÇÃO DE CONTROLES
    //configura a popup de mensagem
    $ngBootbox.setDefaults({
        animate: false,
        backdrop: false,
        title: 'ATENÇÃO!'
        //locale: $scope.selectedLocale
    });

    //configura a modal de inclusão de item
    $scope.modalIncluirItem = function () {
        $('#modalIncluirItem').modal('show');
    }

    //FIM CONFIGURAÇÃO DE CONTROLES
    



    //INICIALIZAÇÃO DE VARIÁVEIS
    $scope.getCardapio();

    $scope.pedido = {
        codFormaPagamento: 1,
        codSituacao: 1,
        valorTotal: 0,
        situacao: 1,
        itens: []
    };

    atualizaValorTotalPedido();

    reiniciaVariaveisItem();
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