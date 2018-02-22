brasaoWebApp.controller('cpfController', function ($scope, $http, $filter, $ngBootbox, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.programaSelecionado = {
            pontosGanhosPorUnidadeMonetariaGasta: 0.0,
            valorDinheiroPorPontoParaResgate: 0.0,
            quantidadeMinimaPontosParaResgate: 0.0
        };

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.impressorasProducao = [];


        $('#compInicioVigencia').datetimepicker({
            locale: 'pt-BR',
            format: 'DD/MM/YYYY',
            useCurrent: false
        });

        $('#compTerminoVigencia').datetimepicker({
            locale: 'pt-BR',
            format: 'DD/MM/YYYY',
            useCurrent: false
        });

        $('#compTerminoVigencia').datetimepicker({
            useCurrent: false //Important! See issue #1075
        });

        $("#compInicioVigencia").on("dp.change", function (e) {

            $('#compTerminoVigencia').data("DateTimePicker").minDate(e.date);
            $scope.programaSelecionado.inicioVigencia = $("#txtInicioVigencia").val();

        });

        $("#compTerminoVigencia").on("dp.change", function (e) {

            $('#compInicioVigencia').data("DateTimePicker").maxDate(e.date);
            $scope.programaSelecionado.terminoVigencia = $("#txtTerminoVigencia").val();

        });


        $scope.promiseGetProgramas = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'ProgramaFidelidade/GetProgramasFidelidade'
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

    $scope.modalAlteracao = function (programa) {
        $('#formGravarPrograma').validator('destroy').validator();

        $scope.programaSelecionado = programa;
        $scope.modoCadastro = 'A';
        $('#modalGravarPrograma').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarPrograma').validator('destroy').validator();

        $scope.programaSelecionado = {};
        $scope.programaSelecionado.programaAtivo = true;
        $scope.programaSelecionado.pontosGanhosPorUnidadeMonetariaGasta = 0.0;
        $scope.programaSelecionado.valorDinheiroPorPontoParaResgate = 0.0;
        $scope.programaSelecionado.quantidadeMinimaPontosParaResgate = 0.0;
        $scope.modoCadastro = 'I';
        $('#modalGravarPrograma').modal('show');
    }

    $scope.gravarPrograma = function () {

        var hasErrors = $('#formGravarPrograma').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        if ($scope.programaSelecionado.pontosGanhosPorUnidadeMonetariaGasta <= 0.0) {

            $scope.mensagem.erro = 'Informe a quantidade de pontos ganhos por unidade monetária gasta.';
            return;

        } else { $scope.programaSelecionado.pontosGanhosPorUnidadeMonetariaGasta = parseFloat($scope.programaSelecionado.pontosGanhosPorUnidadeMonetariaGasta); }

        if ($scope.programaSelecionado.valorDinheiroPorPontoParaResgate <= 0.0) {

            $scope.mensagem.erro = 'Informe o valor em dinheiro por ponto para resgate.';
            return;

        } else { $scope.programaSelecionado.valorDinheiroPorPontoParaResgate = parseFloat($scope.programaSelecionado.valorDinheiroPorPontoParaResgate); }

        if ($scope.programaSelecionado.quantidadeMinimaPontosParaResgate <= 0.0) {

            $scope.mensagem.erro = 'Informe a quantidade mínima de pontos para resgate.';
            return;

        } else { $scope.programaSelecionado.quantidadeMinimaPontosParaResgate = parseFloat($scope.programaSelecionado.quantidadeMinimaPontosParaResgate); }

        $scope.promiseGravarPrograma = $http({
            method: 'POST',
            url: urlBase + 'ProgramaFidelidade/GravarProgramaFidelidade',
            data: { prog: $scope.programaSelecionado, modoCadastro: $scope.modoCadastro },
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


                    $('#modalGravarPrograma').modal('hide');

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

    $scope.confirmaExclusaoPrograma = function (programa) {

        $ngBootbox.confirm('Confirma a exclusão do programa de fidelidade ' + programa.codProgramaFidelidade + '?')
            .then(function () {
                $scope.programaSelecionado = programa;
                $scope.executaExclusaoPrograma();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoPrograma = function () {

        $scope.promiseExcluirPrograma = $http({
            method: 'POST',
            url: urlBase + 'ProgramaFidelidade/ExcluiProgramaFidelidade',
            data: $scope.programaSelecionado,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Programa de fidelidade excluído com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codProgramaFidelidade == $scope.programaSelecionado.codProgramaFidelidade) {
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

            $scope.programaSelecionado = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

});