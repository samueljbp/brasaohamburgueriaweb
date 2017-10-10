brasaoWebApp.controller('prController', function ($scope, $http, $filter) {

    function calculaTotais() {
        $scope.totais = {
            debito: 0.0,
            credito: 0.0,
            dinheiro: 0.0,
            ticket: 0.0
        }

        if ($scope.rowCollection != null && $scope.rowCollection.length > 0) {

            for (i = 0; i < $scope.rowCollection.length; i++) {

                switch ($scope.rowCollection[i].formaPagamento) {
                    case 'D':
                        $scope.totais.dinheiro = $scope.totais.dinheiro + $scope.rowCollection[i].valorTotal;
                        break;

                    case 'C':
                        $scope.totais.credito = $scope.totais.credito + $scope.rowCollection[i].valorTotal;
                        break;

                    case 'B':
                        $scope.totais.debito = $scope.totais.debito + $scope.rowCollection[i].valorTotal;
                        break;

                    case 'A':
                        $scope.totais.ticket = $scope.totais.ticket + $scope.rowCollection[i].valorTotal;
                        break;
                }

            }
        }
    }

    $scope.getPedidosRealizados = function () {

        var inicio = '';
        var fim = '';

        var dataObj;

        if ($scope.filtros.dataInicio != '') {
            dataObj = quebraData($scope.filtros.dataInicio);
            inicio = dataObj.mes + '/' + dataObj.dia + '/' + dataObj.ano;

            if ($scope.filtros.horaInicio != '') {
                inicio = inicio + ' ' + $scope.filtros.horaInicio + ':00';
            } else {
                inicio = inicio + ' 00:00:00';
            }
        }

        if ($scope.filtros.dataFim != '') {
            dataObj = quebraData($scope.filtros.dataFim);
            fim = dataObj.mes + '/' + dataObj.dia + '/' + dataObj.ano;

            if ($scope.filtros.horaFim != '') {
                fim = fim + ' ' + $scope.filtros.horaFim + ':00';
            } else {
                fim = fim + ' 00:00:00';
            }
        }

        var params = '?';

        if (inicio != '') {
            params = params + 'dataInicio=' + encodeURIComponent(inicio.toString());
        }

        if (fim != '') {
            if (params != '?') {
                params = params + '&';
            }
            params = params + 'dataFim=' + encodeURIComponent(fim.toString());
        }

        if (params == '?') {
            params = '';
        }

        $scope.promiseGetPedidosRealizados = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Consultas/GetPedido' + params
        })
        .then(function (response) {

            var retorno = genericSuccess(response);

            if (retorno.succeeded) {

                $scope.rowCollection = retorno.data;
                calculaTotais();

            }
            else {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);
            }



        }, function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });


    };

    $scope.init = function (loginUsuario, antiForgeryToken) {
        
        $scope.rowCollection = [];
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.totais = {
            debito: 0.0,
            credito: 0.0,
            dinheiro: 0.0,
            ticket: 0.0
        }

        $scope.itemsByPage = 10;

        $scope.filtros = {
            dataInicio: '',
            horaInicio: '',
            dataFim: '',
            horaFim: ''
        }

        $(function () {
            $('#compDataInicio').datetimepicker({
                locale: 'pt-BR',
                format: 'DD/MM/YYYY',
                useCurrent: false
            });

            $('#compHoraInicio').datetimepicker({
                locale: 'pt-BR',
                format: 'LT'
            });

            $('#compDataFim').datetimepicker({
                locale: 'pt-BR',
                format: 'DD/MM/YYYY',
                useCurrent: false
            });

            $('#compHoraFim').datetimepicker({
                locale: 'pt-BR',
                format: 'LT'
            });

            $('#compDataFim').datetimepicker({
                useCurrent: false //Important! See issue #1075
            });

            $("#compDataInicio").on("dp.change", function (e) {

                $('#compDataFim').data("DateTimePicker").minDate(e.date);
                $scope.filtros.dataInicio = $("#txtDataInicio").val();

            });

            $("#compDataFim").on("dp.change", function (e) {

                $('#compDataInicio').data("DateTimePicker").maxDate(e.date);
                $scope.filtros.dataFim = $("#txtDataFim").val();

            });

            $("#compHoraInicio").on("dp.change", function () {

                $scope.selecteddate = $("#datetimepicker").val();
                $scope.filtros.horaInicio = $("#txtHoraInicio").val();

            });

            $("#compHoraFim").on("dp.change", function () {

                $scope.selecteddate = $("#datetimepicker").val();
                $scope.filtros.horaFim = $("#txtHoraFim").val();

            });

        });

    }

});