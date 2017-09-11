var angularFormsApp = angular.module('angularFormsApp', ['ngBootbox', 'cgBusy', 'timer']);

angularFormsApp.config(['$httpProvider', function ($httpProvider) {
    //$httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
}]);

angularFormsApp.value('$', $);

angularFormsApp.factory('noteService', ['$', '$rootScope',
function ($, $rootScope) {
    var proxy;
    var connection;
    return {
        connect: function () {
            connection = $.hubConnection();
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

var urlBase = 'http://10.84.161.57:57919/';