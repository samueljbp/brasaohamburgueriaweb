var angularFormsApp = angular.module('angularFormsApp', ['ngBootbox', 'cgBusy']);

angularFormsApp.config(['$httpProvider', function ($httpProvider) {
    //$httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
}]);

var urlBase = 'http://10.84.161.57:57919/';