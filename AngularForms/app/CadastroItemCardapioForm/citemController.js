brasaoWebApp.controller('citemController', function ($scope, $http, $filter, $ngBootbox, $window, FileUploader) {

    $scope.init = function (loginUsuario, antiForgeryToken) {
        $scope.mensagem = {
            erro: '',
            sucesso: '',
            informacao: ''
        }

        var uploader = $scope.uploader = new FileUploader({
            url: urlBase + 'Cadastros/UploadImagemItemCardapio',
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
            fileItem.formData.push($scope.itemSelecionado);
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
                $scope.itemSelecionado.complemento.imagem = response.data;
                $scope.itemSelecionado.complemento.imagemMini = $scope.itemSelecionado.complemento.imagem.replace("img_item", "mini-img_item");

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

        $scope.itemSelecionado = {};

        $scope.rowCollection = [];
        $scope.itemsByPage = 10;

        $scope.classes = [];
        $scope.impressorasProducao = [];

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

    }

    $scope.selecionaClasse = function () {

        $scope.itemSelecionado.descricaoClasse = $filter('filter')($scope.classes, { codClasse: $scope.itemSelecionado.codClasse })[0].descricao;

    }

    $scope.selecionaImpressora = function () {

        $scope.itemSelecionado.descricaoImpressoraProducao = $filter('filter')($scope.impressorasProducao, { codImpressora: $scope.itemSelecionado.codImpressoraProducao })[0].descricao;

    }

    $scope.removerImagem = function () {

        $ngBootbox.confirm('Confirma a exclusão da imagem?')
            .then(function () {


                $scope.promiseRemoverImagem = $http({
                    method: 'POST',
                    url: urlBase + 'Cadastros/RemoverImagemItemCardapio',
                    data: { item: $scope.itemSelecionado },
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).then(function (success) {
                    var retorno = genericSuccess(success);

                    if (retorno.succeeded) {

                        $scope.mensagem.sucesso = 'Imagem removida com sucesso.';
                        $scope.itemSelecionado.complemento.imagem = null;
                        $scope.itemSelecionado.complemento.imagemMini = null;

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

    $scope.modalAlteracao = function (item) {
        $('#formGravarItem').validator('destroy').validator();
        $scope.uploader.clearQueue();

        $scope.itemSelecionado = item;
        $scope.modoCadastro = 'A';
        $('#modalGravarItem').modal('show');
    }

    $scope.modalInclusao = function () {
        $('#formGravarItem').validator('destroy').validator();
        $scope.uploader.clearQueue();

        $scope.itemSelecionado = {};
        $scope.itemSelecionado.sincronizar = true;
        $scope.itemSelecionado.ativo = true;
        $scope.modoCadastro = 'I';
        $('#modalGravarItem').modal('show');
    }

    $scope.gravarItem = function () {

        var hasErrors = $('#formGravarItem').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        if ($scope.itemSelecionado.preco) {
            preco = parseFloat($scope.itemSelecionado.preco);
        }

        if (!isNaN(preco) || preco > 0) {
            $scope.itemSelecionado.preco = preco;
        }
        else {
            $scope.itemSelecionado.preco = 0.0;
        }

        

        $scope.promiseGravarItem = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/GravarItemCardapio',
            data: { item: $scope.itemSelecionado, modoCadastro: $scope.modoCadastro },
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


                    $ngBootbox.confirm('Deseja carregar uma imagem para este item de cardápio?')
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
        $('#modalGravarItem').modal('hide');
        $scope.itemSelecionado = null;
    }

    $scope.confirmaExclusaoItem = function (item) {

        $ngBootbox.confirm('Confirma a exclusão do item de cardápio ' + item.codItemCardapio + '?')
            .then(function () {
                $scope.itemSelecionado = item;
                $scope.executaExclusaoItem();

            }, function () {
                //console.log('Confirm dismissed!');
            });

    }

    $scope.executaExclusaoItem = function () {

        $scope.promiseExcluirItem = $http({
            method: 'POST',
            url: urlBase + 'Cadastros/ExcluiItemCardapio',
            data: $scope.itemSelecionado,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': $scope.antiForgeryToken
            }
        }).then(function (success) {
            var retorno = genericSuccess(success);

            if (retorno.succeeded) {

                if (retorno.data == '') {
                    $scope.mensagem.sucesso = 'Item excluído com sucesso.';

                    for (i = 0; i < $scope.rowCollection.length; i++) {
                        if ($scope.rowCollection[i].codItemCardapio == $scope.itemSelecionado.codItemCardapio) {
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

            $scope.itemSelecionado = null;

        }).catch(function (error) {
            $scope.mensagem.erro = 'Ocorreu uma falha no processamento da requisição. ' + (error.statusText != '' ? error.statusText : 'Erro desconhecido.');
            $window.scrollTo(0, 0);
        });



    }

});