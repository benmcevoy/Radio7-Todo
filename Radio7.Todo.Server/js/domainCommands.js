todo.commands.authenticateCommand = function (data) {
    var raw = $(todo.views.tokenInput).val();

    if (String.IsNullOrWhiteSpace(raw)) {
        return todo.notify('Authenticate missing token.');
    }

    return todo.commands.ajaxPost(
        '',
        todo.baseUrl + '/todo/authenticate?token=' + encodeURIComponent(raw),
        todo.commands.refreshCommand);
};

todo.commands.refreshCommand = function (data) {
    var bindList = function (response) {
        var output = $(todo.views.itemTemplate)
            .tmpl(response);

        $(todo.views.list).html(output);
    };

    return todo.commands.ajaxGet(todo.baseUrl + '/todo', bindList);
};

todo.commands.createCommand = function (data) {
    var raw = $(todo.views.todoInput).val();

    if (String.IsNullOrWhiteSpace(raw)) {
        return todo.notify('You did not enter a task.');
    }

    return todo.commands.ajaxPost(
        '',
        todo.baseUrl + '/todo?raw=' + encodeURIComponent(raw),
        todo.commands.refreshCommand);
};

todo.commands.doneCommand = function (data) {
    todo.commands.ajaxPost(
        '',
        todo.baseUrl + '/todo/done?id=' + encodeURIComponent(data.commandargument),
        todo.commands.refreshCommand);

    return todo.notify('Job done.');
};

todo.notify = function (message) {
    $(todo.views.notify).hide();
    $(todo.views.notify)
        .text(message)
        .show()
        .delay(3000)
        .fadeOut('slow');
};
