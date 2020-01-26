const connection = new signalR.HubConnectionBuilder()
    .withUrl("/loghub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    console.log("connected");
});

if (Notification.permission !== "denied") {
    Notification.requestPermission();
}

connection.on("LogAsync", (log) => {
    if (log.logLevel === 'Information') {
        notify(log);
    }
    var topdiv = document.createElement('div');
    topdiv.className = 'p-2';

    var borderClass = '';
    if (log.logLevel === 'Information') {
        borderClass = 'border-warning';
    }

    var mainDiv = document.createElement('div');
    mainDiv.className = 'card ' + borderClass;

    var headerDiv = document.createElement('div');
    headerDiv.className = 'card-header d-flex justify-content-between';

    var firstP = document.createElement('p');
    firstP.innerHTML = log.logName;
    headerDiv.appendChild(firstP);

    var secondP = document.createElement('p');
    secondP.innerHTML = log.logLevel;
    headerDiv.appendChild(secondP);

    var bodyDiv = document.createElement('div');
    bodyDiv.className = 'card-body';

    var thirdP = document.createElement('p');
    thirdP.innerHTML = log.message;
    bodyDiv.appendChild(thirdP);

    mainDiv.appendChild(headerDiv);
    mainDiv.appendChild(bodyDiv);

    topdiv.appendChild(mainDiv);

    var messages = document.getElementById('messages');
    messages.prepend(topdiv);
});

function notifyMe(log) {
    new Notification(log.logName, {
        body: log.message
    });
}