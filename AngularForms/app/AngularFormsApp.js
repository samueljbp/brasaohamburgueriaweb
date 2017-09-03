var angularFormsApp = angular.module('angularFormsApp', ['ngBootbox', 'cgBusy']);

angularFormsApp.config(['$httpProvider', function ($httpProvider) {
    //$httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
}]);

var urlBase = 'http://192.168.0.132:57919/';