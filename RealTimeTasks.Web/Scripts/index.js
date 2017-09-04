$(function () {
    load();

    var hub = $.connection.simpleHub;
    $.connection.hub.start();

    hub.client.Reload = load;
    hub.client.BtnUpdate = load;

    $(".table").on('click', '.done', function () {
        $.post("/home/completedTask", { taskId: $(this).data('id') }, function () {
            hub.server.taskAdded();
        });
    });

    $("#new-task").on('click', function () {
        $.post("/home/addTask", { title: $("#title").val() }, function () {
            hub.server.taskAdded();
            $("#title").val('');
        });
    });

    $(".table").on('click', '.working', function () {
        $.post("/home/updateTask", { taskId: $(this).data('id') }, function (result) {
            hub.server.taskUpdated(result);
        });
    });

    function load() {
        $('tr:gt(0)').remove();
        $.get("/home/allTasks", function (result) {
            result.forEach(t => $('table').append(`<tr>
                    <td>${t.title}</td>
                    <td>${setButton(t)}</td>
                </tr>`));
        });
    };
    function setButton(task) {
        if (task.isHandled == null) {
            return `<button data-id=${task.id} id=task-${task.id} class ="working btn btn-primary">Im Working On It!</button>`
        } else if (task.working) {
            return `<button data-id=${task.id} id=task-${task.id} class ="done btn btn-success">I'm Done!</button>`
        } else {
            return `<button disabled data-id=${task.id} id=task-${task.id} class ="btn btn-warning">${task.userFirst} ${task.userLast} is Working on it</button>`
        }
    };
});