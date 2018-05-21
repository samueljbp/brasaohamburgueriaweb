var brasaoWebApp = angular.module('brasaoWebApp', ['ngBootbox', 'cgBusy', 'timer', 'smart-table', 'angularFileUpload', 'autoCompleteModule', 'dndLists', 'ngCpfCnpj', 'ngLocalizacao', 'ui.mask', 'color.picker']);

brasaoWebApp.config(['$httpProvider', function ($httpProvider) {
    //$httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
}]);

brasaoWebApp.value('$', $);

var urlBase = 'http://10.84.124.128:57919/';
var urlWebAPIBase = 'http://localhost:62993/api/';

brasaoWebApp.factory('noteService', ['$', '$rootScope',
    function ($, $rootScope) {
        var proxy;
        var connection;
        return {
            connect: function () {
                connection = $.hubConnection(urlBase + 'signalr');
                proxy = connection.createHubProxy('HubMessage');
                connection.start();
                proxy.on('messageAdded', function (codPedido, situacao) {
                    $rootScope.$broadcast('messageAdded', codPedido, situacao);
                });
            },
            isConnecting: function () {
                return connection.state === 0;
            },
            isConnected: function () {
                return connection.state === 1;
            },
            connectionState: function () {
                return connection.state;
            },
            sendMessage: function (usuario, codPedido, situacao) {
                proxy.invoke('SendMessage', usuario, codPedido, situacao);
            },
        }
    }]);



brasaoWebApp.value('cgBusyDefaults', {
    templateUrl: urlBase + 'Content/templates/_Loading.html'
});


brasaoWebApp.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            var format = {
                prefix: '',
                centsSeparator: '.',
                thousandsSeparator: ''
            };

            ctrl.$parsers.unshift(function (value) {
                elem.priceFormat(format);
                console.log('parsers', elem[0].value);
                return elem[0].value;
            });

            ctrl.$formatters.unshift(function (value) {
                elem[0].value = ctrl.$modelValue * 100;
                elem.priceFormat(format);
                console.log('formatters', elem[0].value);
                return elem[0].value;
            })
        }
    };
}]);

brasaoWebApp.directive('ngThumb', ['$window', function ($window) {
    var helper = {
        support: !!($window.FileReader && $window.CanvasRenderingContext2D),
        isFile: function (item) {
            return angular.isObject(item) && item instanceof $window.File;
        },
        isImage: function (file) {
            var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
        }
    };

    return {
        restrict: 'A',
        template: '<canvas/>',
        link: function (scope, element, attributes) {
            if (!helper.support) return;

            var params = scope.$eval(attributes.ngThumb);

            if (!helper.isFile(params.file)) return;
            if (!helper.isImage(params.file)) return;

            var canvas = element.find('canvas');
            var reader = new FileReader();

            reader.onload = onLoadFile;
            reader.readAsDataURL(params.file);

            function onLoadFile(event) {
                var img = new Image();
                img.onload = onLoadImage;
                img.src = event.target.result;
            }

            function onLoadImage() {
                var width = params.width || this.width / this.height * params.height;
                var height = params.height || this.height / this.width * params.width;
                canvas.attr({ width: width, height: height });
                canvas[0].getContext('2d').drawImage(this, 0, 0, width, height);
            }
        }
    };
}]);


app.directive('tooltip', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).hover(function () {
                // on mouseenter
                $(element).tooltip('show');
            }, function () {
                // on mouseleave
                $(element).tooltip('hide');
            });
        }
    };
});