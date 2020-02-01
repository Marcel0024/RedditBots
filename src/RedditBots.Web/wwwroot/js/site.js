(function abc() {
    var notify = false;
    var showDebug = true;

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

        document.getElementById('alert').classList.remove('alert-light-success');
        document.getElementById('alert').classList.add('alert-light-warning');

        document.getElementById('flikker').innerHTML = 'Reconnecting';
    });

    connection.onreconnected(() => {
        console.assert(connection.state === signalR.HubConnectionState.Connected);

        document.getElementById('alert').classList.remove('alert-light-warning');
        document.getElementById('alert').classList.add('alert-light-success');

        document.getElementById('flikker').innerHTML = 'Streaming logs...';
    });

    connection.onclose(() => {
        document.getElementById('alert').classList.remove('alert-light-warning');
        document.getElementById('alert').classList.remove('alert-light-success');
        document.getElementById('alert').classList.add('alert-danger');
        document.getElementById('flikker').innerHTML = 'Disconnected..';

        setTimeout(() => {
            location.reload();
        }, 4000);
    });

    connection.on("UpdateLastDateTime", (time) => {
        document.getElementById('lastUpdate').innerHTML = 'Last log: ' + time;
    });

    connection.on("UpdateViewers", (viewers) => {
        document.getElementById('viewers').innerHTML = 'Viewers: ' + viewers;
    });

    connection.on("Log", (log) => {
        if (log.logLevel === 'Debug'
            && !showDebug) {
            return;
        }

        logs++;

        var topdiv = document.createElement('div');
        topdiv.className = 'p-2';
        topdiv.setAttribute('data-log', log.logLevel);

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
        var botName = namearray[namearray.length - 1];

        firstP.innerHTML = `<a href='https://www.reddit.com/u/${botName}' target="_blank">/u/${botName}</a>`;
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

        var logcards = document.querySelectorAll('[data-log]');

        if (logcards.length >= 100) {
            for (var i = 100; i < logcards.length; i++) {
                logcards[0].parentNode.removeChild(logcards[i]);
            }
        }

        if (notify === true) {
            if (log.logLevel === 'Information' || log.logLevel === 'Warning') {
                notifyMe(log);
            }
        }
    });

    function notifyMe(log) {
        var namearray = log.logName.split('.');

        new Notification(namearray[namearray.length - 1], {
            body: log.message,
            icon: '/bot.png',
            silent: true
        });
    }

    document.getElementById("notification").addEventListener("change", (event) => {
        notify = event.srcElement.checked;

        if (Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    });

    if (Notification.permission === "granted") {
        notify = true;
        document.getElementById('notification').checked = true;
    }

    document.getElementById("showdebug").addEventListener("change", (event) => {
        showDebug = event.srcElement.checked;

        if (!showDebug) {
            document.querySelectorAll("[data-log='Debug']").forEach(e => e.parentNode.removeChild(e));
        }
    });


    var logs = 0;
    setInterval(() => {
        document.getElementById('lps').innerHTML = 'LPS: ' + logs;
        logs = 0;
    }, 1000);


})();