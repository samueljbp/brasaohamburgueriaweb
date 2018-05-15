brasaoWebApp.controller('hfController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken, empresasJson, codLojaSelecionada) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.empresas = {};
        if (empresasJson != '') {
            $scope.empresas = JSON.parse(empresasJson);
        }

        $scope.codEmpresa = codLojaSelecionada.toString();
        $scope.codLojaSelecionada = codLojaSelecionada;

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.funcionamentoSelecionado = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.getFuncionamentoEstabelecimento();

        $('#compAbertura').datetimepicker({
            locale: 'pt-BR',
            format: 'LT'
        });

        $('#compFechamento').datetimepicker({
            locale: 'pt-BR',
            format: 'LT'
        });

        $("#compAbertura").on("dp.change", function () {

            $scope.selecteddate = $("#datetimepicker").val();
            $scope.funcionamentoSelecionado.abertura = $("#txtAbertura").val();

        });

        $("#compFechamento").on("dp.change", function () {

            $scope.selecteddate = $("#datetimepicker").val();
            $scope.funcionamentoSelecionado.fechamento = $("#txtFechamento").val();

        });
    }

    $scope.modalAlteracao = function (funcionamento) {
        $('#formGravarFuncionamento').validator('destroy').validator();

        $scope.funcionamentoSelecionado = jQuery.extend({}, funcionamento);
        $scope.funcionamentoSelecionado.diaSemana = $scope.funcionamentoSelecionado.diaSemana.toString();

        var abertura = getHoras2Digitos($scope.funcionamentoSelecionado.abertura);
        var fechamento = getHoras2Digitos($scope.funcionamentoSelecionado.fechamento);

        $scope.funcionamentoSelecionado.abertura = abertura;
        $scope.funcionamentoSelecionado.fechamento = fechamento;

        if ($scope.funcionamentoSelecionado.codEmpresa != null) {
            $scope.funcionamentoSelecionado.codEmpresa = $scope.funcionamentoSelecionado.codEmpresa.toString();
        }

        $scope.modoCadastro = 'A';
        $('#modalGravarHorarioFunc').modal('show');
    }

    function getHoras2Digitos(data) {
        var date = new Date(data);

        var horas = date.getHours();
        horas = ('0' + horas).slice(-2);

        var minutos = date.getMinutes();
        minutos = ('0' + minutos).slice(-2);

        return horas + ':' + minutos;
    }

    $scope.modalInclusao = function () {
        $('#formGravarFuncionamento').validator('destroy').validator();

        $scope.funcionamentoSelecionado = {};
        $scope.modoCadastro = 'I';
        $('#modalGravarHorarioFunc').modal('show');
    }

    $scope.gravarHorarioFuncionamento = function () {

        var hasErrors = $('#formGravarFuncionamento').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        $scope.funcionamentoSelecionado.abertura = new Date('2000-01-01T' + $scope.funcionamentoSelecionado.abertura + ':00');
        $scope.funcionamentoSelecionado.fechamento = new Date('2000-01-01T' + $scope.funcionamentoSelecionado.fechamento + ':00');

        $scope.promiseGravarHorarioFuncionamento = $http({
            method: 'POST',
            url: urlBase + 'Institucional/GravarFuncionamentoEstabelecimento',
            data: { funcionamento: $scope.funcionamentoSelecionado, modoCadastro: $scope.modoCadastro },
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
                } else {

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].diaSemana == retorno.data.diaSemana &&
                            $scope.rowCollection[i].inicio == retorno.data.inicio) {

                            $scope.rowCollection[i].fechamento = retorno.data.fechamento;
                            $scope.rowCollection[i].temDelivery = retorno.data.temDelivery;

                            return;
                        }
                    }

                }

                verificaLista($scope.rowCollection, $scope.codLojaSelecionada);

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });

        $('#modalGravarHorarioFunc').modal('hide');
    }

    $scope.confirmaExclusaoFuncionamento = function (dia) {

        $ngBootbox.confirm('Confirma a exclusão do horário ' + dia.descricaoDiaSemana + ' / ' + dia.inicio + '?')
            .then(function () {
                $scope.funcionamentoSelecionado = jQuery.extend({}, dia);
                $scope.executaExclusaoFuncionamento();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoFuncionamento = function () {

        $scope.promiseExcluirFuncionamento = $http({
            method: 'POST',
            url: urlBase + 'Institucional/ExcluiFuncionamentoEstabelecimento',
            data: $scope.funcionamentoSelecionado,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Horário de funcionamento excluído com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].diaSemana == $scope.funcionamentoSelecionado.diaSemana &&
                            $scope.rowCollection[i].inicio == $scope.funcionamentoSelecionado.inicio) {
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

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

    $scope.getFuncionamentoEstabelecimento = function () {

        $scope.promiseGetFuncionamentoEstabelecimento = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Institucional/GetHorariosFuncionamento'
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

    }

});