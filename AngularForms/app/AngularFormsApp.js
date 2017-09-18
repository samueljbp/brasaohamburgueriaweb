var angularFormsApp = angular.module('angularFormsApp', ['ngBootbox', 'cgBusy', 'timer']);

angularFormsApp.config(['$httpProvider', function ($httpProvider) {
    //$httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
}]);

angularFormsApp.value('$', $);

var urlBase = 'http://localhost:57919/';

angularFormsApp.factory('noteService', ['$', '$rootScope',
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



angularFormsApp.value('cgBusyDefaults', {
    templateUrl: urlBase + 'Content/templates/_Loading.html'
});