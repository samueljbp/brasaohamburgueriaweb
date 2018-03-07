brasaoWebApp.controller('cpvController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: '',
            erroCampoDesconto: false,
            erroListasDragDrop: false,
            erroDiasSemana: false
        }

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.promocaoSelecionada = {};
        $scope.promocaoSelecionada.diasAssociados = [];

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.tiposDesconto = [];
        $scope.classes = [];
        $scope.itensCardapio = [];
        $scope.diasSemana = [];

        $scope.diasSelecionados = [];




        $('#compInicioVigencia').datetimepicker({
            locale: 'pt-BR',
            format: 'DD/MM/YYYY',
            useCurrent: false
        });

        $('#compHoraInicio').datetimepicker({
            locale: 'pt-BR',
            format: 'LT'
        });

        $('#compTerminoVigencia').datetimepicker({
            locale: 'pt-BR',
            format: 'DD/MM/YYYY',
            useCurrent: false
        });

        $('#compHoraFim').datetimepicker({
            locale: 'pt-BR',
            format: 'LT'
        });

        $('#compTerminoVigencia').datetimepicker({
            useCurrent: false //Important! See issue #1075
        });

        $("#compInicioVigencia").on("dp.change", function (e) {

            $('#compTerminoVigencia').data("DateTimePicker").minDate(e.date);
            $scope.promocaoSelecionada.dataInicio = $("#txtInicioVigencia").val();

        });

        $("#compTerminoVigencia").on("dp.change", function (e) {

            $('#compInicioVigencia').data("DateTimePicker").maxDate(e.date);
            $scope.promocaoSelecionada.dataFim = $("#txtTerminoVigencia").val();

        });

        $("#compHoraInicio").on("dp.change", function () {

            $scope.selecteddate = $("#datetimepicker").val();
            $scope.promocaoSelecionada.horaInicio = $("#txtHoraInicio").val();

        });

        $("#compHoraFim").on("dp.change", function () {

            $scope.selecteddate = $("#datetimepicker").val();
            $scope.promocaoSelecionada.horaFim = $("#txtHoraFim").val();

        });







        $scope.promiseGetPromocoes = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetPromocoesVenda'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.rowCollection = retorno.data;

                }
                else {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                }



            }, function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });


        $scope.promiseGetTiposDesconto = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetTiposAplicacaoDesconto'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.tiposDesconto = retorno.data;

                }
                else {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                }



            }, function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });



        $scope.promiseGetClasses = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetClassesItemCardapio'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.classes = retorno.data;

                }
                else {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                }



            }, function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });


        $scope.promiseGetTiposDesconto = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetDiasSemana'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.diasSemana = retorno.data;

                }
                else {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                }



            }, function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });


        $scope.promiseGetItens = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetItensCardapio'
        })
            .then(function (response) {

                var retorno = genericSuccess(response);

                if (retorno.succeeded) {

                    $scope.itensCardapio = retorno.data;

                }
                else {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                }



            }, function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });




    }

    $scope.toggleSelection = function toggleSelection(numDia) {
        var idx = $scope.diasSelecionados.indexOf(numDia);

        // Is currently selected
        if (idx > -1) {
            $scope.diasSelecionados.splice(idx, 1);
        }

        // Is newly selected
        else {
            $scope.diasSelecionados.push(numDia);
        }
    };

    $scope.selecionaTipoDesconto = function () {

        $scope.promocaoSelecionada.descricaoTipoAplicacaoDesconto = $filter('filter')($scope.tiposDesconto, { codTipoAplicacaoDesconto: $scope.promocaoSelecionada.codTipoAplicacaoDesconto })[0].descricao;

        if ($scope.promocaoSelecionada.codTipoAplicacaoDesconto == 2) {

            $scope.models = [
                { listName: "Classes disponíveis", items: [], dragging: false },
                { listName: "Classes incluídas", items: [], dragging: false }
            ];

            $scope.models[0].items = [];
            $scope.models[0].items = $scope.models[0].items.concat($scope.classes);

            $scope.onDrop = function (list, items, index) {
                angular.forEach(items, function (item) { item.selected = false; });
                list.items = list.items.slice(0, index)
                    .concat(items)
                    .concat(list.items.slice(index));
                list.items = removeDuplicates(list.items, 'codClasse');
                return true;
            }

        } else if ($scope.promocaoSelecionada.codTipoAplicacaoDesconto == 1) {

            $scope.models = [
                { listName: "Itens disponíveis", items: [], dragging: false },
                { listName: "Itens incluídos", items: [], dragging: false }
            ];

            $scope.models[0].items = [];
            $scope.models[0].items = $scope.models[0].items.concat($scope.itensCardapio);

            $scope.onDrop = function (list, items, index) {
                angular.forEach(items, function (item) { item.selected = false; });
                list.items = list.items.slice(0, index)
                    .concat(items)
                    .concat(list.items.slice(index));
                list.items = removeDuplicates(list.items, 'codItemCardapio');
                return true;
            }

        }

        angular.forEach($scope.models[0].items, function (item) { item.selected = false; });
        angular.forEach($scope.models[1].items, function (item) { item.selected = false; });

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

    $scope.modalAlteracao = function (promo) {
        $('#formGravarPromocao').validator('destroy').validator();

        $scope.promocaoSelecionada = promo;
        montaArrayDias();
        $scope.selecionaTipoDesconto();

        $scope.models[1].items = [];
        if ($scope.promocaoSelecionada.codTipoAplicacaoDesconto == 2) {
            $scope.models[1].items = $scope.models[1].items.concat($scope.promocaoSelecionada.classesAssociadas);
        } else {
            $scope.models[1].items = $scope.models[1].items.concat($scope.promocaoSelecionada.itensAssociados);
        }


        $scope.modoCadastro = 'A';
        $('#modalGravarPromocao').modal('show');
    }

    function montaArrayDias() {

        $scope.diasSelecionados = [];
        angular.forEach($scope.promocaoSelecionada.diasAssociados, function (item) { $scope.diasSelecionados.push(item.numDiaSemana); });


    }

    $scope.modalInclusao = function () {
        $('#formGravarPromocao').validator('destroy').validator();

        $scope.promocaoSelecionada = {};
        $scope.promocaoSelecionada.promocaoAtiva = true;
        $scope.promocaoSelecionada.diasAssociados = [];
        $scope.promocaoSelecionada.itensAssociados = [];
        $scope.promocaoSelecionada.classesAssociadas = [];

        $scope.modoCadastro = 'I';
        $('#modalGravarPromocao').modal('show');
    }

    $scope.gravarPromocao = function () {

        $('#formGravarPromocao').validator('destroy').validator();

        $scope.mensagem.erroCampoDesconto = false;
        if ($scope.promocaoSelecionada.percentualDesconto <= 0.0 || isNaN($scope.promocaoSelecionada.percentualDesconto)) {

            $scope.mensagem.erroCampoDesconto = true;
            return;

        } else { $scope.promocaoSelecionada.percentualDesconto = parseFloat($scope.promocaoSelecionada.percentualDesconto); }

        var hasErrors = $('#formGravarPromocao').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.mensagem.erroDiasSemana = false;
        if ($scope.diasSelecionados.length <= 0) {

            $scope.mensagem.erroDiasSemana = true;
            return;

        }

        $scope.promocaoSelecionada.diasAssociados = [];
        for (i = 0; i < $scope.diasSelecionados.length; i++) {

            var numDia = $scope.diasSelecionados[i];
            $scope.promocaoSelecionada.diasAssociados.push({ numDiaSemana: numDia });

        }

        $scope.mensagem.erroListasDragDrop = false;
        if ($scope.models[1].items.length == 0) {

            $scope.mensagem.erroListasDragDrop = true;
            return;

        }

        $scope.promocaoSelecionada.itensAssociados = [];
        $scope.promocaoSelecionada.classesAssociadas = [];
        if ($scope.promocaoSelecionada.codTipoAplicacaoDesconto == 1) {

            $scope.promocaoSelecionada.itensAssociados = $scope.models[1].items;

        } else {

            $scope.promocaoSelecionada.classesAssociadas = $scope.models[1].items;

        }

        var fechaPopup = true;

        $scope.promiseGravarPromocao = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarPromocaoVenda',
            data: { promocao: $scope.promocaoSelecionada, modoCadastro: $scope.modoCadastro },
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                $scope.mensagem.sucesso = 'Dados gravados com sucesso.';

                if ($scope.modoCadastro == 'I') {
                    $scope.rowCollection.push(retorno.data);

                }

                $('#modalGravarPromocao').modal('hide');

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

    }

    $scope.confirmaExclusaoPromocao = function (promo) {

        $ngBootbox.confirm('Confirma a exclusão da promoção ' + promo.codPromocaoVenda + '?')
            .then(function () {
                $scope.promocaoSelecionada = promo;
                $scope.executaExclusaoPromocao();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoPromocao = function () {

        $scope.promiseExcluirPromocao = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiPromocaoVenda',
            data: $scope.promocaoSelecionada,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Promoção excluída com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codPromocaoVenda == $scope.promocaoSelecionada.codPromocaoVenda) {
                            $scope.rowCollection.splice(i, 1);

                            return;
                        }
                    }
                } else {

                    $scope.mensagem.erro = retorno.data;
                    $window.scrollTo(0, 0);

                }

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

            $scope.promocaoSelecionada = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

});