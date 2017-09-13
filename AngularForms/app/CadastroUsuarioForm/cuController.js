angularFormsApp.controller('cuController', function ($scope, $http) {
    
    var email = $('#email').val();
    if (email == null) {
        email = '';
    }

    //função que recupera dados do usuário quando é alteração
    $scope.GetData = function () {
        if (email != null && email != '') {

            $scope.promiseGetUsuario = $http({
                method: 'GET',
                url: urlBase + 'Conta/GetUsuario'
            })
            .then(function (response) {

                $scope.usuario = response.data;

            }, function (error) {
                alert(error);
            });

        }
        else {
            $scope.usuario = { estado: 'MG', cidade: 'Cataguases' };
        }
    }

    angular.element(function () {
        $scope.GetData();
    });

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
            allowInputToggle: true
        });
    });

    //variável para desabilitar o botão de salvar somente quando estiver cadastrando
    var disable = (email == '');

    //quando o campo e-mail recebe o foco, limpa erro
    $("#email").focus(function () {
        if (!$('#mensagemEmailJaUtilizado').hasClass('hidden')) {
            $('#mensagemEmailJaUtilizado').addClass('hidden');
        }
    });

    //esconde mensagem de validação do formulário no início
    if (!$('#mensagemErroFormulario').hasClass('hidden')) {
        $('#mensagemErroFormulario').addClass('hidden');
    }

    if (!$('#mensagemSucessoFormulario').hasClass('hidden')) {
        $('#mensagemSucessoFormulario').addClass('hidden');
    }

    if (email == '') { //se estiver cadastrando, esconde as opções de mudar senha e configura validação de e-mail
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
                $('#mensagemErroFormulario').removeClass('hidden');
                $('#mensagemErroFormulario').text('Ocorreu uma falha no processamento da requisição. ' + error.statusText);
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

    //submete dados ao servidor para gravação
    $scope.submitForm = function () {


        var hasErrors = $('#formCadastro').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        var alteraSenha = $('#checkAlterarSenha').is(":checked");

        var novo = (email == '');
        var senhaAtual = '';
        var novaSenha = '';
        if (email != '' && alteraSenha) {
            senhaAtual = $('#senhaAtual').val();
            novaSenha = $('#novaSenha').val();
        }
        else {
            novaSenha = $('#senha').val();
        }

        var postData = { 'usuario': $scope.usuario, 'novo': novo, 'senhaAtual': senhaAtual, 'novaSenha': novaSenha };

        $scope.promiseCadastrar = $http.post(urlBase + 'Conta/Cadastrar', postData).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.Succeeded) {
                if (email == '') {
                    window.location.href = urlBase + 'Home/Index';
                } else {
                    $('#mensagemSucessoFormulario').removeClass('hidden');
                    $('#mensagemSucessoFormulario').text('Os dados foram atualizados com sucesso!');
                    window.scrollTo(0, 0);
                }
            }
            else {
                $('#mensagemErroFormulario').removeClass('hidden');
                $('#mensagemErroFormulario').text('Ocorreu uma falha durante o cadastro com a seguinte mensagem: ' + retorno.Errors[0]);
            }

        }).catch(function (error) {
            $('#mensagemErroFormulario').removeClass('hidden');
            $('#mensagemErroFormulario').text(error.statusText);
        });

    };
});