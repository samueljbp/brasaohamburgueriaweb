brasaoWebApp.controller('cuController', function ($scope, $http, $window) {

    $scope.init = function (loginUsuario, antiForgeryToken, email) {

        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.novo = !(email != '');
        $scope.usuario = { estado: 'MG', cidade: 'Cataguases', email: '' };

        if (email != '') {
            $scope.usuario.email = email;
        }

        $scope.GetData();


        //variável de apoio para validação de e-mail
        $scope.emailJaUtilizado = false;

        //configura máscara para telefone
        id('telefone').onkeypress = function () {
            mascara(this, mtel);
        }

        //configura calendário
        $(function () {
            $('#datetimepicker1').datetimepicker({
                locale: 'pt-BR',
                format: 'DD/MM/YYYY',
                allowInputToggle: true,
                useCurrent: false
            });
        });

        //variável para desabilitar o botão de salvar somente quando estiver cadastrando
        var disable = ($scope.usuario.email == '');

        //quando o campo e-mail recebe o foco, limpa erro
        $("#email").focus(function () {
            if (!$('#mensagemEmailJaUtilizado').hasClass('hidden')) {
                $('#mensagemEmailJaUtilizado').addClass('hidden');
            }
        });

        if ($scope.usuario.email == '') { //se estiver cadastrando, esconde as opções de mudar senha e configura validação de e-mail
            $("#email").focusout(function () {
                if ($(this).val() == '') {
                    return;
                }

                if (!$('#mensagemEmailJaUtilizado').hasClass('hidden')) {
                    $('#mensagemEmailJaUtilizado').addClass('hidden');
                }

                var postData = { 'email': $scope.usuario.email };

                $scope.promiseEmail = $http.post(urlBase + 'Conta/EmailExiste', postData).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno) {
                        $('#mensagemEmailJaUtilizado').removeClass('hidden');
                        var texto = $('#mensagemEmailJaUtilizado').text();
                        $('#mensagemEmailJaUtilizado').text(texto.replace('EMAIL', $('#email').val()));
                        $('#email').val('');
                        return;
                    }
                }).catch(function (error) {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (error.statusText ? error.statusText : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                });
            });

            $("#divCheckMudarSenha").addClass('hidden');
            $("#divMudarSenha").addClass('hidden');

            $('#senha').attr('required', true);
            $('#confirmeSenha').attr('required', true);

            $('#senhaAtual').attr('required', false);
            $('#novaSenha').attr('required', false);
            $('#confirmeNovaSenha').attr('required', false);
        }
        else { //se estiver alterando, configura campos que não podem ser editados e exibe opção de alterar senha
            $('#email').attr('readonly', 'readonly');

            $("#btnEnviar").removeAttr("readonly");

            $("#divSenha").addClass('hidden');
            $("#divMudarSenha").addClass('hidden');

            $('#senha').attr('required', false);
            $('#confirmeSenha').attr('required', false);

            $("#checkAlterarSenha").change(function () {

                var checked = $(this).is(":checked");

                if (checked) {
                    $("#divMudarSenha").removeClass('hidden');

                    $('#senhaAtual').attr('required', true);
                    $('#novaSenha').attr('required', true);
                    $('#confirmeNovaSenha').attr('required', true);

                } else {
                    if (!$("#divMudarSenha").hasClass('hidden')) {
                        $("#divMudarSenha").addClass('hidden');

                        $('#senhaAtual').attr('required', false);
                        $('#novaSenha').attr('required', false);
                        $('#confirmeNovaSenha').attr('required', false);
                    }
                }
            });
        }

        //configura validador
        $('#formCadastro').validator({
            custom: {
                //função de validação customizada para validar formato de e-mail
                'email': function ($el) {
                    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                    if ($el.val() != '' && !re.test($el.val())) {
                        return 'E-mail inválido.';
                    }
                },
                //função de validação customizada para validar confirmação de senha
                'senha': function ($el) {
                    if ($el.val().length < 6) {
                        return 'A senha deve ter no mínimo 6 cacateres.';
                    }
                },
                //função de validação customizada para validar confirmação de senha
                'confirmaSenha': function ($el) {
                    if ($el.val() != $('#senha').val()) {
                        return 'A confirmação da senha não está correta.';
                    }
                },
                //função de validação customizada para validar confirmação de nova senha
                'confirmaNovaSenha': function ($el) {
                    if ($el.val() != $('#novaSenha').val()) {
                        return 'A confirmação da senha não está correta.';
                    }
                }
            },
            disable: disable
        });








    }

    //função que recupera dados do usuário quando é alteração
    $scope.GetData = function () {
        if ($scope.usuario.email != null && $scope.usuario.email != '') {

            $scope.promiseGetUsuario = $http({
                method: 'GET',
                url: urlBase + 'Conta/GetUsuario'
            })
            .then(function (response) {

                $scope.usuario = response.data;

            }, function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (error.statusText ? error.statusText : 'erro desconhecido');
                $window.scrollTo(0, 0);
            });

        }
    }



    //submete dados ao servidor para gravação
    $scope.submitForm = function () {

        var hasErrors = $('#formCadastro').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        var alteraSenha = $('#checkAlterarSenha').is(":checked");

        var senhaAtual = '';
        var novaSenha = '';
        if ($scope.usuario.email != '' && alteraSenha) {
            senhaAtual = $('#senhaAtual').val();
            novaSenha = $('#novaSenha').val();
        }
        else {
            novaSenha = $('#senha').val();
        }

        var postData = { 'usuario': $scope.usuario, 'novo': $scope.novo, 'senhaAtual': senhaAtual, 'novaSenha': novaSenha };

        $scope.promiseCadastrar = $http.post(urlBase + 'Conta/Cadastrar', postData).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.Succeeded) {
                if ($scope.novo) {
                    window.location.href = urlBase + 'Home/Index';
                } else {
                    $scope.mensagem.sucesso = 'Os dados foram atualizados com sucesso!';
                    window.scrollTo(0, 0);
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

    };
});