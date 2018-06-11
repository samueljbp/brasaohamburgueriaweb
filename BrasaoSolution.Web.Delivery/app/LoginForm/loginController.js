brasaoWebApp.service('loginservice', function ($http) {

    this.login = function (userlogin) {

        var resp = $http({
            url: urlBase + "TOKEN",
            method: "POST",
            data: $.param({ grant_type: 'password', username: userlogin.username, password: userlogin.password, isPersistent: userlogin.isPersistent }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        });
        return resp;
    };
});



brasaoWebApp.controller('loginController', function ($scope, $http, loginservice, $window) {

    $scope.init = function () {

        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        $scope.loginViewModel = { usuario: "", senha: "", lembrarMe: false, returnUrl: "" };

        $('#formLogin').validator({
            custom: {
                'email': function ($el) {
                    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

                    if ($el.val() != '' && !re.test($el.val())) {
                        return 'E-mail inválido.';
                    }
                }
            }
        });

        $scope.submitForm = function () {

            var hasErrors = $('#formLogin').validator('validate').has('.has-error').length;

            if (hasErrors) {
                return;
            }

            var url = urlBase + 'Home/MenuPrincipal';

            if ($scope.loginViewModel.returnUrl != '') {
                url = $scope.loginViewModel.returnUrl;
            }

            //$scope.promise = $http.get('http://httpbin.org/delay/3');
            //return;

            $scope.promiseBusy = $http({
                method: 'POST',
                url: urlBase + 'Conta/Login',
                data: $scope.loginViewModel,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'RequestVerificationToken': $scope.antiForgeryToken
                }
            }).then(function (success) {
                var retorno = genericSuccess(success);

                if (retorno.Succeeded) {
                    window.location.href = url;

                }
                else {
                    $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                    $window.scrollTo(0, 0);
                }

            }).catch(function (error) {
                $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                $window.scrollTo(0, 0);
            });


            // Esse codigo faz o login por OWIN e armazena um token
            //var userLogin = {
            //    grant_type: 'password',
            //    username: $scope.loginViewModel.usuario,
            //    password: $scope.loginViewModel.senha,
            //    isPersistent: ($scope.loginViewModel.lembrarMe ? '1' : '0')
            //};

            //var promiselogin = loginservice.login(userLogin);



            //promiselogin.then(function (resp) {

            //    $scope.userName = resp.data.userName;
            //    //Store the token information in the SessionStorage
            //    //So that it can be accessed for other views
            //    sessionStorage.setItem('userName', resp.data.userName);
            //    sessionStorage.setItem('accessToken', resp.data.access_token);
            //    sessionStorage.setItem('refreshToken', resp.data.refresh_token);

            //    window.location.href = url;
            //}, function (error) {

            //    $('#mensagemErroFormulario').removeClass('hidden');
            //    $('#mensagemErroFormulario').text(error.statusText);

            //});


            


        };
    }
});