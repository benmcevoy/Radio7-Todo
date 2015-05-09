/// <reference path="../typings/knockout/knockout.d.ts"/>
/// <reference path="~/js/namespace.js" />
String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;
};

String.IsNullOrWhiteSpace = function() {
    var value = arguments[0];

    if (typeof value === 'undefined' || value == null) return true;

    return value.replace(/\s/g, '').length < 1;
};

var notifer = new todo.notification.notifier('#notification');

$(function () {
    //    ko.applyBindings(new todo.viewModels.SubscriptionsViewModel());
    todo.commands.bind();
});



