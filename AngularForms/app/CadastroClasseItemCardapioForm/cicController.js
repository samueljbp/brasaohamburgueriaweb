brasaoWebApp.controller('cicController', function ($scope, $http, $filter, $ngBootbox, $window, FileUploader) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        var uploader = $scope.uploader = new FileUploader({
            url: urlBase + 'Cadastros/UploadFile',
            removeAfterUpload: true,
            queueLimit: 1
        });

        uploader.filters.push({
            name: 'imageFilter',
            fn: function (item /*{File|FileLikeObject}*/, options) {
                var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                var size = item.size;
                return ('|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1) && size < 500000;
            }
        });

        // CALLBACKS

        uploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
            console.info('onWhenAddingFileFailed', item, filter, options);
            $scope.mensagem.informacao = "Arquivo não permitido. Permitidos arquivos |jpg|png|jpeg|bmp|gif| e com tamanho de até 500kb.";
        };
        uploader.onAfterAddingFile = function (fileItem) {
            console.info('onAfterAddingFile', fileItem);
            fileItem.formData.push($scope.classeSelecionada);
        };
        uploader.onAfterAddingAll = function (addedFileItems) {
            console.info('onAfterAddingAll', addedFileItems);
        };
        uploader.onBeforeUploadItem = function (item) {
            console.info('onBeforeUploadItem', item);
        };
        uploader.onProgressItem = function (fileItem, progress) {
            console.info('onProgressItem', fileItem, progress);
        };
        uploader.onProgressAll = function (progress) {
            console.info('onProgressAll', progress);
        };
        uploader.onSuccessItem = function (fileItem, response, status, headers) {
            console.info('onSuccessItem', fileItem, response, status, headers);
        };
        uploader.onErrorItem = function (fileItem, response, status, headers) {
            console.info('onErrorItem', fileItem, response, status, headers);

            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (status != '' ? status : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        };
        uploader.onCancelItem = function (fileItem, response, status, headers) {
            console.info('onCancelItem', fileItem, response, status, headers);
        };
        uploader.onCompleteItem = function (fileItem, response, status, headers) {
            console.info('onCompleteItem', fileItem, response, status, headers);

            if (response.succeeded) {

                $scope.mensagem.sucesso = 'Imagem enviada com sucesso.';
                $window.scrollTo(0, 0);
                $scope.classeSelecionada.imagem = response.data;
                $scope.classeSelecionada.imagemMini = $scope.classeSelecionada.imagem.replace("img_classe", "mini-img_classe");

            } else {

                $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (response.errors[0] ? response.errors[0] : 'erro desconhecido');
                $window.scrollTo(0, 0);

            }

        };
        uploader.onCompleteAll = function () {
            console.info('onCompleteAll');
        };

        console.info('uploader', uploader);

        // I: inclusão, A: alteração
        $scope.modoCadastro = '';

        $scope.antiForgeryToken = antiForgeryToken;

        $scope.classeSelecionada = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.impressorasProducao = [];

        $scope.promiseGetClasses();
    }

    $scope.selecionaImpressora = function () {
        
        $scope.classeSelecionada.descricaoImpressoraPadrao = $filter('filter')($scope.impressorasProducao, { codImpressora: $scope.classeSelecionada.codImpressoraPadrao })[0].descricao;

    }

    $scope.removerImagem = function () {

        $ngBootbox.confirm('Confirma a exclusão da imagem?')
            .then(function () {


                $scope.promiseGravarClasse = $http({
                    method: 'POST',
                    url: urlBase + 'Cadastros/RemoverImagem',
                    data: { classe: $scope.classeSelecionada },
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        $scope.mensagem.sucesso = 'Imagem removida com sucesso.';
                        $scope.classeSelecionada.imagem = null;
                        $scope.classeSelecionada.imagemMini = null;

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

    $scope.modalAlteracao = function (classe) {
        $('#formGravarClasse').validator('destroy').validator();
        $scope.uploader.clearQueue();

        $scope.classeSelecionada = classe;
        $scope.modoCadastro = 'A';
        $('#modalGravarClasse').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarClasse').validator('destroy').validator();
        $scope.uploader.clearQueue();

        $scope.classeSelecionada = {};
        $scope.classeSelecionada.sincronizar = true;
        $scope.modoCadastro = 'I';
        $('#modalGravarClasse').modal('show');
    }

    $scope.gravarClasse = function () {

        var hasErrors = $('#formGravarClasse').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        var fechaPopup = true;

        $scope.promiseGravarClasse = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarClasseItemCardapio',
            data: { classe: $scope.classeSelecionada, modoCadastro: $scope.modoCadastro },
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


                    $ngBootbox.confirm('Deseja carregar uma imagem para esta classe?')
                        .then(function () {

                            fechaPopup = false;

                        }, function () {
                            //console.log('Confirm dismissed!');
                        });

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

        if (fechaPopup) {
            $('#modalGravarClasse').modal('hide');
            $scope.classeSelecionada = null;
        }

    }

    $scope.confirmaExclusaoClasse = function (classe) {

        $ngBootbox.confirm('Confirma a exclusão da classe ' + classe.codClasse + '?')
            .then(function () {
                $scope.classeSelecionada = classe;
                $scope.executaExclusaoClasse();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoClasse = function () {

        $scope.promiseExcluirClasse = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiClasseItemCardapio',
            data: $scope.classeSelecionada,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Classe excluída com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codClasse == $scope.classeSelecionada.codClasse) {
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

            $scope.classeSelecionada = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

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


    $scope.promiseGetImpressorasProducao = $http({
        method: 'GET',
        headers: {
            //'Authorization': 'Bearer ' + accesstoken,
            'RequestVerificationToken': 'XMLHttpRequest',
            'X-Requested-With': 'XMLHttpRequest',
        },
        url: urlBase + 'Cadastros/GetImpressorasProducao'
    })
    .then(function (response) {

        var retorno = genericSuccess(response);

        if (retorno.succeeded) {

            $scope.impressorasProducao = retorno.data;

        }
        else {
            $scope.mensagem.erro = 'Ocorreu uma falha durante a execução da operação com a seguinte mensagem: ' + (retorno.errors[0] ? retorno.errors[0] : 'erro desconhecido');
            $window.scrollTo(0, 0);
        }



    }, function (error) {
        $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
        $window.scrollTo(0, 0);
    });

});