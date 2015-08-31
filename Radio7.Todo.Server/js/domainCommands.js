todo.commands.authenticateCommand = function (data) {
    var raw = $(todo.views.tokenInput).val();

    if (String.IsNullOrWhiteSpace(raw)) {
        return todo.notify('Authenticate missing token.');
    }

    return todo.commands.ajaxPost(
        '',
        todo.baseUrl + '/todo/authenticate?token=' + encodeURIComponent(raw))
        .done(todo.commands.refreshCommand);
};

todo.commands.refreshCommand = function (data) {
    var onDone = function (response) {
        var output = $(todo.views.itemTemplate)
            .tmpl(response);

        $(todo.views.list).html(output);
    };

    return todo.commands
        .ajaxGet(todo.baseUrl + '/todo')
        .done(onDone)
        .fail(todo.errorNotifier);
};

todo.commands.createCommand = function (data) {
    var raw = $(todo.views.todoInput).val();

    if (String.IsNullOrWhiteSpace(raw)) {
        return todo.notify('You did not enter a task.');
    }

    var onDone = function (response) {
        todo.commands.refreshCommand();
        $(todo.views.todoInput).val('');
    };

    return todo.commands.ajaxPost(
        '',
        todo.baseUrl + '/todo?raw=' + encodeURIComponent(raw))
        .done(onDone);
};

todo.commands.doneCommand = function (data) {
    todo.commands.ajaxPost(
        '',
        todo.baseUrl + '/todo/done?id=' + encodeURIComponent(data.commandargument))
        .done(todo.commands.refreshCommand);

    return todo.notify('Job done.');
};

todo.errorNotifier = function (xhr, b, c) {
    if (xhr.status == 401) {
        todo.notify('You are not authenticated.');
        return;
    }

    if (xhr.status == 0) {
        todo.notify('Possibly a x-domain request has been denied. baseUrl is ' + todo.baseUrl);
        return;
    }

    if (xhr.status != 200) todo.notify(c);
};

todo.notify = function (message) {
    $(todo.views.notify).hide();
    $(todo.views.notify)
        .text(message)
        .show()
        .delay(3000)
        .fadeOut('slow');
};
