todo.commands.authenticateCommand = function (data) {
    var raw = $('#token').val();

    if (String.IsNullOrWhiteSpace(raw)) {
        return todo.commands.notify('Authenticate missing token.');
    }

    return todo.commands.ajaxPost(
        '',
        todo.baseUrl + '/todo/authenticate?token=' + raw,
        todo.commands.refreshCommand);
};

todo.commands.refreshCommand = function (data) {
    var bindList = function (response) {
        console.log('render the todo items');
        console.log(response);
    };

    return todo.commands.ajaxGet(todo.baseUrl + '/todo', bindList);
};

todo.commands.createCommand = function (data) {
    var raw = $('#todo').val();

    if (String.IsNullOrWhiteSpace(raw)) {
        return todo.commands.notify('You did not enter a task.');
    }

    return todo.commands.ajaxPost(
        '',
        baseUrl + '/todo?raw=' + raw,
        todo.commands.refreshCommand);
};

todo.commands.doneCommand = function (data) {
    return todo.commands.ajaxPost(
        '',
        baseUrl + '/todo/done' + data.commandargument,
        todo.commands.refreshCommand);
};

todo.commands.notify = function(message) {
    return todo.commands.publish({ command: 'notify', commandargument: message });
};
