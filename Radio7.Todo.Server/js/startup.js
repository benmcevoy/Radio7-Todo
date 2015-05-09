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

$(function () {
    todo.commands.bind();
    todo.commands.invoke({ command: 'refresh' });
});



