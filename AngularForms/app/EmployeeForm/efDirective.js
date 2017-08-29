angularFormsApp.directive('employeeForm', //apesar de aqui estar como Camel Case, essa string employeeForm será traduzida para employee-form, que é o padrão HTML, por isso ao chamar deve-se utilizar essa string, conforme está no Index.html
    function () {
        return {
            restrict: 'E', //indica que este objeto será utilizado apenas como elemento
            templateUrl: 'app/EmployeeForm/efTemplate.html'
        }
    });