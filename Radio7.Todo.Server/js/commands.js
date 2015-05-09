/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="~/js/namespace.js" />
/// <reference path="~/typings/knockout/knockout.d.ts" />
todo.messenger = new ko.subscribable();

todo.commands.targetPanel = '#panel';

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

todo.commands.publish = function (data) {
    if (data.command) {
        todo.messenger.notifySubscribers(data.commandargument, data.command);
    }
};

todo.commands.ajaxPost = function (id, postaction, onDone) {
    $.ajax({
        type: "POST",
        url: postaction,
        data: { '': id }
    }).done(onDone);
};

todo.commands.ajaxGet = function (action, onDone) {
    if (action) {
        $.ajax({
            url: action,
            xhrFields: { withCredentials: true }
        }).done(onDone);
    }
};