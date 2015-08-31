/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="~/js/namespace.js" />
todo.commands.invoke = function (data) {
    if (!data.command) return;

    console.log('invoked:' + data.command);

    var commandToInvoke = todo.commands[data.command + 'Command'];

    if (commandToInvoke) {
        commandToInvoke(data);
    }
};

todo.commands.bind = function () {
    $('body').on('click', 'input.command', function () {
        todo.commands.invoke($(this).data());
    });

    $('body').on('click', 'a.command', function (e) {
        e.preventDefault();
        todo.commands.invoke($(this).data());
    });
};

todo.commands.ajaxPost = function (id, postaction, data) {
    return $.ajax({
        type: "POST",
        url: postaction,
        data: { '': id }
    });
};

todo.commands.ajaxGet = function (action) {
    if (action) {
        return $.ajax({
            url: action,
            xhrFields: { withCredentials: true }
        });
    }
};