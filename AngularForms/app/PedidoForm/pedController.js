angularFormsApp.controller('pedController', function ($scope, $http, $filter, $ngBootbox) {

    $scope.codClasse = 0;

    $scope.itemCardapioSelecionado = null;

    $scope.quantidadeItemIncluir = 0;

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

    $scope.exibeImagemGrande = function (codItemCardapio) {
        var imagem = $filter('filter')($scope.itensFiltrados, { codItemCardapio: codItemCardapio })[0].complemento.imagem;
        $('#imagepreview').attr('src', urlBase + '/Content/img/itens_cardapio/' + imagem);
        $('#imagemodal').modal('show');
    }

    $scope.filtraClasse = function (codClasse) {
        $scope.codClasse = codClasse;
        var filtro = $filter('filter')($scope.classes, { codClasse: codClasse });
        $('#dropdownText').text(filtro[0].descricaoClasse);
        $scope.itensFiltrados = filtro[0].itens;
    };

    $scope.getCardapio();

    $scope.pedido = {
        codFormaPagamento: 1,
        codSituacao: 1,
        valorTotal: 0,
        itens: [
                    { seqItem: 1, codItem: 1, descricaoItem: 'AC/DC', obs: [{ codObs: 1, descricao: 'Bem passado' }], extras: [{ codExtra: 1, descricao: 'Bacon extra', valorExtra: 4.0 }], quantidade: 2, precoUnitario: 22.0, valorExtras: 4.0, valorTotal: 48.0 },
                    { seqItem: 2, codItem: 2, descricaoItem: 'Pearl Jam', obs: [{ codObs: 2, descricao: 'Ao ponto' }], extras: null, quantidade: 1, precoUnitario: 23.5, valorExtras: 0.0, valorTotal: 23.5 },
                    { seqItem: 3, codItem: 3, descricaoItem: 'Coca cola lata', obs: [{ codObs: 3, descricao: 'Com gelo' }, { codObs: 4, descricao: 'Com limão' }], extras: null, quantidade: 2, precoUnitario: 4.5, valorExtras: 0.0, valorTotal: 9.0 }
        ]
    };

    atualizaValorTotalPedido();

    $scope.novoItem = {
        seqItem: 0,
        codItem: 0,
        descricaoItem: '',
        obs: null,
        extras: null,
        quantidade: 0,
        precoUnitario: 0.0,
        valorExtras: 0.0,
        valorTotal: 0.0
    }

    $ngBootbox.setDefaults({
        animate: false,
        backdrop: false,
        title: 'ATENÇÃO!'
        //locale: $scope.selectedLocale
    });

    $scope.confirmaInclusaoItem = function () {

        $scope.novoItem = {
            seqItem: getMextSeqItemPedido(),
            codItem: 4,
            descricaoItem: 'Fanta laranja lata',
            obs: [{ codObs: 3, descricao: 'Com gelo' }],
            extras: null,
            quantidade: 1,
            precoUnitario: 4.5,
            valorExtras: 0.0,
            valorTotal: 4.5
        };

        $scope.pedido.itens.push($scope.novoItem);

        atualizaValorTotalPedido();

        $scope.itemCardapioSelecionado = 0;

        $('#modalIncluirItem').modal('hide');
    };

    $scope.cancelaInclusaoItem = function () {
        $scope.itemCardapioSelecionado = null;
    }

    $scope.selecionaItemParaInclusao = function (codItemCardapio) {
        $scope.itemCardapioSelecionado = $filter('filter')($scope.itensFiltrados, { codItemCardapio: codItemCardapio })[0];
    }

    $scope.incrementaQuantidade = function () {
        $scope.quantidadeItemIncluir = $scope.quantidadeItemIncluir + 1;
    }

    $scope.decrementaQuantidade = function () {
        if ($scope.quantidadeItemIncluir > 0) {
            $scope.quantidadeItemIncluir = $scope.quantidadeItemIncluir - 1;
        }
    }

    $scope.confirmCallbackRemove = function (seqItem) {

        for (i = 0; i < $scope.pedido.itens.length; i++) {
            if ($scope.pedido.itens[i].seqItem == seqItem) {
                $scope.pedido.itens.splice(i, 1);
                break;
            }
        }
        atualizaValorTotalPedido();

    };

    function atualizaValorTotalPedido() {
        $scope.pedido.valorTotal = 0;

        for (i = 0; i < $scope.pedido.itens.length; i++) {
            $scope.pedido.valorTotal = $scope.pedido.valorTotal + $scope.pedido.itens[i].valorTotal;
        }
    }

    function getMextSeqItemPedido() {
        var seqItem = 0;

        for (i = 0; i < $scope.pedido.itens.length; i++) {
            if ($scope.pedido.itens[i].seqItem > seqItem) {
                seqItem = $scope.pedido.itens[i].seqItem;
            }
        }

        seqItem = seqItem + 1;

        return seqItem;
    }
});