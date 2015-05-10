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
// fix jquery textarea encoding
$.valHooks.textarea = {
    get: function (elem) {
        return elem.value.replace(/\r?\n/g, "\r\n");
    }
};

// override encode to convert \r\n to br
$.encode = function( text ) {
    // Do HTML encoding replacing < > & and ' and " by corresponding entities.
    return ("" + text).split("<").join("&lt;").split(">").join("&gt;").split('"').join("&#34;").split("'").join("&#39;").split("&#13;&#10;").join("<br/>");
}

$(function () {
    todo.commands.bind();
    todo.commands.invoke({ command: 'refresh' });
});



