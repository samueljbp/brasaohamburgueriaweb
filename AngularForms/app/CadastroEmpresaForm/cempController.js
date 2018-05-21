brasaoWebApp.controller('cempController', function ($scope, $http, $filter, $ngBootbox, $window, FileUploader) {

    $scope.init = function (loginUsuario, antiForgeryToken, empresasJson, codLojaSelecionada) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: '',
            erroCnpj: true
        }

        $scope.gatilhoModel = 0;

        $scope.empresas = {};
        if (empresasJson != '') {
            $scope.empresas = JSON.parse(empresasJson);
        }

        $scope.codEmpresa = codLojaSelecionada.toString();
        $scope.codLojaSelecionada = codLojaSelecionada;

        inicializaUploaderLogo();
        inicializaUploaderFundoPublico();
        inicializaUploaderFundoAutenticado();
        inicializaUploaderImagensInstitucionais();

        // options - if a list is given then choose one of the items. The first item in the list will be the default
        $scope.colorPickerOptions = {
            // html attributes
            required: true,
            disabled: false,
            inputClass: '',
            // validation
            restrictToFormat: true,
            preserveInputFormat: true,
            allowEmpty: false,
            round: false,
            swatch: true,
            swatchPos: 'left',
            swatchBootstrap: true,
            // color
            format: 'hex',
            case: 'upper'
        };

        // api event handlers
        $scope.eventApi = {
            onChange: function (api, color, $event) {

                if (!$scope.empresaSelecionada.corPrincipal.startsWith("#") && $scope.empresaSelecionada.corPrincipal != "") {
                    $scope.empresaSelecionada.corPrincipal = "#" + $scope.empresaSelecionada.corPrincipal;
                }

                if (!$scope.empresaSelecionada.corSecundaria.startsWith("#") && $scope.empresaSelecionada.corSecundaria != "") {
                    $scope.empresaSelecionada.corSecundaria = "#" + $scope.empresaSelecionada.corSecundaria;
                }

                if (!$scope.empresaSelecionada.corPrincipalContraste.startsWith("#") && $scope.empresaSelecionada.corPrincipalContraste != "") {
                    $scope.empresaSelecionada.corPrincipalContraste = "#" + $scope.empresaSelecionada.corPrincipalContraste;
                }

                if (!$scope.empresaSelecionada.corDestaque.startsWith("#") && $scope.empresaSelecionada.corDestaque != "") {
                    $scope.empresaSelecionada.corDestaque = "#" + $scope.empresaSelecionada.corDestaque;
                }
            }
        };

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.empresaSelecionada = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.promiseGetEmpresas = $http({
            method: 'GET',
            headers: {
                //'Authorization': 'Bearer ' + accesstoken,
                'RequestVerificationToken': 'XMLHttpRequest',
                'X-Requested-With': 'XMLHttpRequest',
            },
            url: urlBase + 'Cadastros/GetEmpresas'
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

        $scope.$watch($scope.empresaSelecionada.cnpj, function (value) {
            console.log('Fora: ' + value);
        });

    }

    function inicializaUploaderFundoPublico() {

        var uploaderBgPublico = $scope.uploaderBgPublico = new FileUploader({
            url: urlBase + 'Cadastros/UploadImagemFundoPublico',
            removeAfterUpload: true,
            queueLimit: 1
        });

        uploaderBgPublico.filters.push({
            name: 'imageFilter',
            fn: function (item /*{File|FileLikeObject}*/, options) {
                var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                var size = item.size;
                return ('|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1) && size < 1000000;
            }
        });

        // CALLBACKS

        uploaderBgPublico.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
            console.info('onWhenAddingFileFailed', item, filter, options);
            $scope.mensagem.informacao = "Arquivo não permitido. Permitidos arquivos |jpg|png|jpeg|bmp|gif| e com tamanho de até 1000kb.";
        };
        uploaderBgPublico.onAfterAddingFile = function (fileItem) {
            console.info('onAfterAddingFile', fileItem);
            fileItem.formData.push($scope.empresaSelecionada);
        };
        uploaderBgPublico.onErrorItem = function (fileItem, response, status, headers) {
            console.info('onErrorItem', fileItem, response, status, headers);

            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (status != '' ? status : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        };
        uploaderBgPublico.onCompleteItem = function (fileItem, response, status, headers) {
            console.info('onCompleteItem', fileItem, response, status, headers);

            if (response.succeeded) {

                $scope.mensagem.sucesso = 'Imagem enviada com sucesso.';
                $window.scrollTo(0, 0);
                $scope.empresaSelecionada.imagemBackgroundPublica = response.data;
                $scope.empresaSelecionada.imagemBackgroundPublicaMini = $scope.empresaSelecionada.imagemBackgroundPublica.replace("img_bg_publica", "mini-img_bg_publica");

            } else {

                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (response.errors[0] ? response.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);

            }

        };

    }

    function inicializaUploaderFundoAutenticado() {

        var uploaderBgAutenticado = $scope.uploaderBgAutenticado = new FileUploader({
            url: urlBase + 'Cadastros/UploadImagemFundoAutenticado',
            removeAfterUpload: true,
            queueLimit: 1
        });

        uploaderBgAutenticado.filters.push({
            name: 'imageFilter',
            fn: function (item /*{File|FileLikeObject}*/, options) {
                var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                var size = item.size;
                return ('|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1) && size < 1000000;
            }
        });

        // CALLBACKS

        uploaderBgAutenticado.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
            console.info('onWhenAddingFileFailed', item, filter, options);
            $scope.mensagem.informacao = "Arquivo não permitido. Permitidos arquivos |jpg|png|jpeg|bmp|gif| e com tamanho de até 1000kb.";
        };
        uploaderBgAutenticado.onAfterAddingFile = function (fileItem) {
            console.info('onAfterAddingFile', fileItem);
            fileItem.formData.push($scope.empresaSelecionada);
        };
        uploaderBgAutenticado.onErrorItem = function (fileItem, response, status, headers) {
            console.info('onErrorItem', fileItem, response, status, headers);

            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (status != '' ? status : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        };
        uploaderBgAutenticado.onCompleteItem = function (fileItem, response, status, headers) {
            console.info('onCompleteItem', fileItem, response, status, headers);

            if (response.succeeded) {

                $scope.mensagem.sucesso = 'Imagem enviada com sucesso.';
                $window.scrollTo(0, 0);
                $scope.empresaSelecionada.imagemBackgroundAutenticada = response.data;
                $scope.empresaSelecionada.imagemBackgroundAutenticadaMini = $scope.empresaSelecionada.imagemBackgroundAutenticada.replace("img_bg_publica", "mini-img_bg_publica");

            } else {

                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (response.errors[0] ? response.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);

            }

        };

    }

    function inicializaUploaderLogo() {

        var uploaderLogo = $scope.uploaderLogo = new FileUploader({
            url: urlBase + 'Cadastros/UploadImagemLogoEmpresa',
            removeAfterUpload: true,
            queueLimit: 1
        });

        uploaderLogo.filters.push({
            name: 'imageFilter',
            fn: function (item /*{File|FileLikeObject}*/, options) {
                var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                var size = item.size;
                return ('|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1) && size < 500000;
            }
        });

        // CALLBACKS

        uploaderLogo.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
            console.info('onWhenAddingFileFailed', item, filter, options);
            $scope.mensagem.informacao = "Arquivo não permitido. Permitidos arquivos |jpg|png|jpeg|bmp|gif| e com tamanho de até 500kb.";
        };
        uploaderLogo.onAfterAddingFile = function (fileItem) {
            console.info('onAfterAddingFile', fileItem);
            fileItem.formData.push($scope.empresaSelecionada);
        };
        uploaderLogo.onErrorItem = function (fileItem, response, status, headers) {
            console.info('onErrorItem', fileItem, response, status, headers);

            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (status != '' ? status : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        };
        uploaderLogo.onCompleteItem = function (fileItem, response, status, headers) {
            console.info('onCompleteItem', fileItem, response, status, headers);

            if (response.succeeded) {

                $scope.mensagem.sucesso = 'Imagem enviada com sucesso.';
                $window.scrollTo(0, 0);
                $scope.empresaSelecionada.logomarca = response.data;
                $scope.empresaSelecionada.logomarcaMini = $scope.empresaSelecionada.logomarca.replace("img_logo", "mini-img_logo");

            } else {

                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (response.errors[0] ? response.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);

            }

        };

    }

    function inicializaUploaderImagensInstitucionais() {

        var uploaderImagensInstitucionais = $scope.uploaderImagensInstitucionais = new FileUploader({
            url: urlBase + 'Cadastros/UploadImagemInstitucional',
            removeAfterUpload: true,
            queueLimit: 10
        });

        uploaderImagensInstitucionais.filters.push({
            name: 'imageFilter',
            fn: function (item /*{File|FileLikeObject}*/, options) {
                var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                var size = item.size;
                return ('|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1) && size < 1000000;
            }
        });

        // CALLBACKS

        uploaderImagensInstitucionais.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
            console.info('onWhenAddingFileFailed', item, filter, options);
            $scope.mensagem.informacao = "Arquivo não permitido. Permitidos arquivos |jpg|png|jpeg|bmp|gif| e com tamanho de até 1000kb.";
        };
        uploaderImagensInstitucionais.onAfterAddingFile = function (fileItem) {
            console.info('onAfterAddingFile', fileItem);
            fileItem.formData.push($scope.empresaSelecionada);
        };
        uploaderImagensInstitucionais.onErrorItem = function (fileItem, response, status, headers) {
            console.info('onErrorItem', fileItem, response, status, headers);

            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (status != '' ? status : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        };
        uploaderImagensInstitucionais.onCompleteItem = function (fileItem, response, status, headers) {
            console.info('onCompleteItem', fileItem, response, status, headers);

            if (response.succeeded) {

                $scope.mensagem.sucesso = 'Imagem enviada com sucesso.';
                $window.scrollTo(0, 0);
                $scope.empresaSelecionada.imagensInstitucionais = response.data;
            } else {

                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (response.errors[0] ? response.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);

            }

        };

    }

    //$scope.selecionaEstado = function () {

    //    $scope.cidades = {};
    //    $scope.bairros = {};

    //    $scope.promiseGetCidades = $http({
    //        method: 'GET',
    //        headers: {
    //            //'Authorization': 'Bearer ' + accesstoken,
    //            'RequestVerificationToken': 'XMLHttpRequest',
    //            'X-Requested-With': 'XMLHttpRequest',
    //        },
    //        url: urlBase + "Cadastros/GetCidades?siglaEstado=" + $scope.empresaSelecionada.estado
    //    })
    //        .then(function (response) {

    //            var retorno = genericSuccess(response);

    //            if (retorno.succeeded) {

    //                $scope.cidades = retorno.data;

    //            }
    //            else {
    //                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
    //                $window.scrollTo(0, 0);
    //            }



    //        }, function (error) {
    //            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
    //            $window.scrollTo(0, 0);
    //        });

    //}

    //$scope.selecionaCidade = function () {

    //    $scope.bairros = {};

    //    $scope.promiseGetBairros = $http({
    //        method: 'GET',
    //        headers: {
    //            //'Authorization': 'Bearer ' + accesstoken,
    //            'RequestVerificationToken': 'XMLHttpRequest',
    //            'X-Requested-With': 'XMLHttpRequest',
    //        },
    //        url: urlBase + "Cadastros/GetBairros?codCidade=" + $scope.empresaSelecionada.codCidade
    //    })
    //        .then(function (response) {

    //            var retorno = genericSuccess(response);

    //            if (retorno.succeeded) {

    //                $scope.bairros = retorno.data;

    //            }
    //            else {
    //                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
    //                $window.scrollTo(0, 0);
    //            }



    //        }, function (error) {
    //            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
    //            $window.scrollTo(0, 0);
    //        });

    //}

    $scope.removerImagemLogomarca = function () {

        $ngBootbox.confirm('Confirma a exclusão da imagem?')
            .then(function () {


                $scope.promiseRemoverImagem = $http({
                    method: 'POST',
                    url: urlBase + 'Cadastros/RemoverImagemLogoEmpresa',
                    data: { empresa: $scope.empresaSelecionada },
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        $scope.mensagem.sucesso = 'Imagem removida com sucesso.';
                        $scope.empresaSelecionada.logomarca = null;
                        $scope.empresaSelecionada.logomarcaMini = null;

                    }
                    else {
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                    $window.scrollTo(0, 0);
                });


            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.removerImagemFundoPublico = function () {

        $ngBootbox.confirm('Confirma a exclusão da imagem?')
            .then(function () {


                $scope.promiseRemoverImagem = $http({
                    method: 'POST',
                    url: urlBase + 'Cadastros/RemoverImagemFundoPublico',
                    data: { empresa: $scope.empresaSelecionada },
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        $scope.mensagem.sucesso = 'Imagem removida com sucesso.';
                        $scope.empresaSelecionada.imagemBackgroundPublica = null;
                        $scope.empresaSelecionada.imagemBackgroundPublicaMini = null;

                    }
                    else {
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                    $window.scrollTo(0, 0);
                });


            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.removerImagemFundoAutenticado = function () {

        $ngBootbox.confirm('Confirma a exclusão da imagem?')
            .then(function () {


                $scope.promiseRemoverImagem = $http({
                    method: 'POST',
                    url: urlBase + 'Cadastros/RemoverImagemFundoAutenticado',
                    data: { empresa: $scope.empresaSelecionada },
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        $scope.mensagem.sucesso = 'Imagem removida com sucesso.';
                        $scope.empresaSelecionada.imagemBackgroundAutenticada = null;
                        $scope.empresaSelecionada.imagemBackgroundAutenticadaMini = null;

                    }
                    else {
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                    $window.scrollTo(0, 0);
                });


            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.removerImagemInstitucional = function (imagem) {

        $ngBootbox.confirm('Confirma a exclusão da imagem?')
            .then(function () {


                $scope.promiseRemoverImagem = $http({
                    method: 'POST',
                    url: urlBase + 'Cadastros/RemoverImagemInstitucional',
                    data: { empresa: $scope.empresaSelecionada, imagem: imagem },
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        $scope.mensagem.sucesso = 'Imagem removida com sucesso.';

                        $.each($scope.empresaSelecionada.imagensInstitucionais, function (index, value) {
                            if (value != null && value.imagem != null && value.imagem == imagem) {
                                $scope.empresaSelecionada.imagensInstitucionais.splice(index, 1);
                            }
                        });

                        $scope.empresaSelecionada.logomarca = null;
                        $scope.empresaSelecionada.logomarcaMini = null;

                    }
                    else {
                        $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
                        $window.scrollTo(0, 0);
                    }

                }).catch(function (error) {
                    $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
                    $window.scrollTo(0, 0);
                });


            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.modalAlteracao = function (empresa) {
        $('#formGravarEmpresa').validator('destroy').validator();
        $scope.uploaderLogo.clearQueue();
        $scope.uploaderBgPublico.clearQueue();
        $scope.uploaderBgAutenticado.clearQueue();
        $scope.uploaderImagensInstitucionais.clearQueue();

        $scope.empresaSelecionada = empresa;

        if ($scope.empresaSelecionada.codEmpresaMatriz != null) {
            $scope.empresaSelecionada.codEmpresaMatriz = $scope.empresaSelecionada.codEmpresaMatriz.toString();
        }

        $scope.empresaSelecionada.cnpj = $scope.empresaSelecionada.cnpj + " ";

        if ($scope.empresaSelecionada.codCidade != null) {
            $scope.empresaSelecionada.codCidade = $scope.empresaSelecionada.codCidade.toString();
        }

        if ($scope.empresaSelecionada.codBairro != null) {
            $scope.empresaSelecionada.codBairro = $scope.empresaSelecionada.codBairro.toString();
        }

        //$scope.gatilhoModel = $scope.gatilhoModel + 1;

        $scope.modoCadastro = 'A';
        $('#modalGravarEmpresa').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarEmpresa').validator('destroy').validator();
        $scope.uploaderLogo.clearQueue();
        $scope.uploaderBgPublico.clearQueue();
        $scope.uploaderBgAutenticado.clearQueue();
        $scope.uploaderImagensInstitucionais.clearQueue();

        $scope.empresaSelecionada = {};
        $scope.empresaSelecionada.cnpj = '';
        $scope.empresaSelecionada.empresaAtiva = true;
        $scope.modoCadastro = 'I';

        $('#modalGravarEmpresa').modal('show');
    }

    $scope.verificaCnpj = function (valor) {

        $scope.mensagem.erroCnpj = !valor;
        $scope.$apply();

    }

    $scope.gravarEmpresa = function () {

        var hasErrors = $('#formGravarEmpresa').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        if ($scope.mensagem.erroCnpj && $scope.empresaSelecionada.cnpj == "") {
            $scope.empresaSelecionada.cnpj = "-1"; //para forçar o erro no componente caso o CNPJ não tenha sido preenchido

            return;
        }


        $scope.promiseGravarEmpresa = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarEmpresa',
            data: { empresa: $scope.empresaSelecionada, modoCadastro: $scope.modoCadastro },
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


                    $ngBootbox.confirm('Deseja carregar imagens para esta empresa?')
                        .then(function () {

                        }, function () {

                            fechaPopup();

                        });

                } else {

                    fechaPopup();

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

    function fechaPopup() {
        $('#modalGravarEmpresa').modal('hide');
        $scope.empresaSelecionada = null;
    }

    $scope.confirmaExclusaoEmpresa = function (emp) {

        $ngBootbox.confirm('Confirma a exclusão da empresa ' + emp.codEmpresa + '?')
            .then(function () {
                $scope.empresaSelecionada = emp;
                $scope.executaExclusaoEmpresa();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoEmpresa = function () {

        $scope.promiseExcluirEmpresa = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiEmpresa',
            data: $scope.empresaSelecionada,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Empresa excluída com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codEmpresa == $scope.empresaSelecionada.codEmpresa) {
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

            $scope.empresaSelecionada = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

});