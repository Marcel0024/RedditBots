(function abc() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/loghub")
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.start().then(function () {
        console.log("connected");
    });

    connection.onreconnecting(() => {
        console.assert(connection.state === signalR.HubConnectionState.Reconnecting);

        document.getElementById('alert').classList.remove('alert-success');
        document.getElementById('alert').classList.add('alert-warning');

        document.getElementById('flikker').innerHTML = 'Reconnecting';
    });

    connection.onreconnected(() => {
        console.assert(connection.state === signalR.HubConnectionState.Connected);

        document.getElementById('alert').classList.remove('alert-warning');
        document.getElementById('alert').classList.add('alert-success');

        document.getElementById('flikker').innerHTML = 'Connected.. Streaming logs LIVE';
    });

    connection.onclose(() => {
        document.getElementById('alert').classList.remove('alert-warning');
        document.getElementById('alert').classList.remove('alert-success');
        document.getElementById('alert').classList.add('alert-danger');
        document.getElementById('flikker').innerHTML = 'Disconnected..';

        setTimeout(() => {
            location.reload();
        }, 4000);
    });

    if (Notification.permission !== "denied") {
        Notification.requestPermission();
    }

    connection.on("UpdateLastDateTime", (time) => {
        document.getElementById('lastUpdate').innerHTML = 'Last log: ' + time;
    });

    connection.on("UpdateViewers", (viewers) => {
        document.getElementById('viewers').innerHTML = 'Viewers: ' + viewers;
    });

    connection.on("Log", (log) => {
        var topdiv = document.createElement('div');
        topdiv.className = 'p-2';

        var borderClass = '';
        if (log.logLevel === 'Information') {
            borderClass = 'border-success';
        } else if (log.logLevel === 'Warning') {
            borderClass = 'border-warning';
        }

        var mainDiv = document.createElement('div');
        mainDiv.className = 'card ' + borderClass;
        mainDiv.style.border = '3px solid black';

        setTimeout(() => {
            mainDiv.style.border = '';
        }, 3000);

        var headerDiv = document.createElement('div');
        headerDiv.className = 'card-header d-flex justify-content-between';

        var firstP = document.createElement('span');
        var namearray = log.logName.split('.');

        firstP.innerHTML = namearray[namearray.length - 1];
        headerDiv.appendChild(firstP);

        var secondP = document.createElement('div');
        secondP.className = 'badge ' + (log.logLevel === "Information" ? 'badge-success' : log.logLevel === "Warning" ? 'badge-warning' : 'badge-light');
        secondP.innerHTML = log.logLevel;
        headerDiv.appendChild(secondP);

        var bodyDiv = document.createElement('div');
        bodyDiv.className = 'card-body';

        var thirdP = document.createElement('span');
        thirdP.innerHTML = log.message;
        bodyDiv.appendChild(thirdP);

        mainDiv.appendChild(headerDiv);
        mainDiv.appendChild(bodyDiv);

        topdiv.appendChild(mainDiv);

        var messages = document.getElementById('messages');
        messages.prepend(topdiv);

        if (log.logLevel === 'Information' || log.logLevel === 'Warning') {
            notifyMe(log);
        }
    });

    function notifyMe(log) {
        var namearray = log.logName.split('.');

        new Notification(namearray[namearray.length - 1], {
            body: log.message,
            icon: '/bot.png',
            tag: 'renotify',
            renotify: true
        });
    }
})();