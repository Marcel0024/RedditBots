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

    setUserSettings();
    renderBotsSettings();

    // removeOldBots();

    connection.on("Log", (log) => {
        if (log.notify === true) {
            logs++;
        }

        var firstP = document.createElement('span');
        var namearray = log.logName.split('.');
        var botName = namearray[namearray.length - 1];
        var isVisible = true;

        addBotIfNeeded(botName);
        var settings = getOrCreateUserSettings();
        var botSetting = getBotSetting(botName);

        if (!botSetting.displayLogs) {
            isVisible = false;
        }

        if (log.logLevel === 'Debug'
            && !settings.showDebugLogs) {
            isVisible = false;
        }

        var topdiv = document.createElement('div');
        topdiv.className = 'p-1';
        topdiv.setAttribute('data-log', log.logLevel);
        topdiv.setAttribute('data-botname', botName);

        if (!isVisible) {
            topdiv.className = topdiv.className + ' d-none';
        }

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

        var topDivInBody = document.createElement('div');
        topDivInBody.className = 'row d-flex justify-content-between';

        if (botName === "Azure (Not a bot)") {
            firstP.innerHTML = 'Azure (Not a bot) Pipeline';
        } else {
            firstP.innerHTML = `<a href='https://www.reddit.com/u/${botName}' target="_blank">/u/${botName}</a>`;
        }

        topDivInBody.appendChild(firstP);

        var secondP = document.createElement('div');
        secondP.className = 'badge ' + (log.logLevel === "Information" ? 'badge-success' : log.logLevel === "Warning" ? 'badge-warning' : 'badge-light');
        secondP.innerHTML = log.logLevel;
        topDivInBody.appendChild(secondP);

        var bodyDiv = document.createElement('div');
        bodyDiv.className = 'card-body';

        var contentDiv = document.createElement('div');
        contentDiv.classList = 'row pt-2';
        contentDiv.innerHTML = log.message;

        bodyDiv.appendChild(topDivInBody);
        bodyDiv.appendChild(contentDiv);

        mainDiv.appendChild(bodyDiv);

        topdiv.appendChild(mainDiv);

        var messages = document.getElementById('messages');
        messages.prepend(topdiv);

        var logcards = document.querySelectorAll("[data-log='Debug']");

        if (logcards.length >= 200) {
            for (var i = 200; i < logcards.length; i++) {
                logcards[0].parentNode.removeChild(logcards[i]);
            }
        }

        if (settings.receiveDesktopNotification === true
            && botSetting.displayLogs
            && log.notify === true) {
            if (log.logLevel !== 'Debug') {
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
        var onoff = event.srcElement.checked;

        if (Notification.permission !== "denied") {
            Notification.requestPermission().then(function (permission) {
                if (permission === "denied") {
                    var settings = getOrCreateUserSettings();
                    settings.receiveDesktopNotification = false;
                    saveSettings(settings);

                    document.getElementById('notification').checked = false;
                }
            });
        }
        else if (Notification.permission === "denied" && event.srcElement.checked) {
            alert("Allow Notifications on this site to activate this function");
            onoff = false;
        }

        var settings = getOrCreateUserSettings();
        settings.receiveDesktopNotification = onoff;
        saveSettings(settings);

        document.getElementById('notification').checked = onoff;
    });

    document.getElementById("showdebug").addEventListener("change", (event) => {
        var settings = getOrCreateUserSettings();
        settings.showDebugLogs = event.srcElement.checked;
        saveSettings(settings);

        if (!settings.showDebugLogs) {
            document.querySelectorAll("[data-log='Debug']").forEach(e => e.className = e.className + " d-none");
        }
        else {
            document.querySelectorAll("[data-log='Debug']").forEach(e => e.className = e.className.replace("d-none", ""));
        }
    });

    $('#settingsRow').on("show.bs.collapse", (event) => {
        var settings = getOrCreateUserSettings();
        settings.showSettings = true;
        saveSettings(settings);

        document.getElementById('togglesettingsbutton').innerHTML = 'Hide advanced settings';
    });

    $('#settingsRow').on("hide.bs.collapse", (event) => {
        var settings = getOrCreateUserSettings();
        settings.showSettings = false;
        saveSettings(settings);

        document.getElementById('togglesettingsbutton').innerHTML = 'Show advanced settings';
    });

    function setUserSettings() {
        var settings = getOrCreateUserSettings();

        document.getElementById('showdebug').checked = settings.showDebugLogs;
        document.getElementById('notification').checked = settings.receiveDesktopNotification;

        if (settings.showSettings) {
            $('#settingsRow').collapse('show');
            document.getElementById('togglesettingsbutton').innerHTML = 'Hide advanced settings';
        } else {
            $('#settingsRow').collapse('hide');
            document.getElementById('togglesettingsbutton').innerHTML = 'Show advanced settings';
        }
    }

    function getOrCreateUserSettings() {
        var settings = localStorage.getItem('usersettings');

        if (settings === undefined || settings === null) {
            settings = { receiveDesktopNotification: false, showDebugLogs: false, showSettings: false, bots: [] };
            saveSettings(settings);
            return settings;
        }

        return JSON.parse(settings);
    }

    function saveSettings(settings) {
        localStorage.setItem('usersettings', JSON.stringify(settings));
    }

    function getBotSetting(botName) {
        var settings = getOrCreateUserSettings();
        var botSetting = null;

        for (var i in settings.bots) {
            if (settings.bots[i].name === botName) {
                settings.bots[i].dateLastSeen = new Date();
                botSetting = settings.bots[i];
                saveSettings(settings);

                return botSetting;
            }
        }
    }

    function addBotIfNeeded(botName) {
        var botSetting = getBotSetting(botName);

        if (botSetting === undefined || botSetting === null) {
            var settings = getOrCreateUserSettings();
            var bot = { name: botName, displayLogs: true, dateLastSeen: new Date() };

            settings.bots.push(bot);
            saveSettings(settings);
            renderBotSetting(bot);
        }
    }

    function renderBotsSettings() {
        var settings = getOrCreateUserSettings();

        var element = document.getElementById('botSettings');
        element.innerHTML = '';

        settings.bots.sort((a, b) => (a.color > b.color) ? 1 : -1).forEach(renderBotSetting);
    }

    function renderBotSetting(bot) {
        var botRow = document.getElementById('botSettings');

        var mainDiv = document.createElement('div');
        mainDiv.className = 'col-xl-4 col-6';

        var secondDiv = document.createElement('div');
        secondDiv.className = 'custom-control custom-toggle my-2';

        var id = bot.name + 'settings';

        var input = document.createElement('input');
        input.type = 'checkbox';
        input.checked = bot.displayLogs;
        input.id = id;
        input.name = id;
        input.setAttribute('data-botname', bot.name);
        input.classList = 'custom-control-input';

        input.addEventListener('change', updateBotSetting);

        var label = document.createElement('label');
        label.classList = 'custom-control-label';
        label.setAttribute('for', id);
        label.innerHTML = bot.name;

        secondDiv.appendChild(input);
        secondDiv.appendChild(label);

        mainDiv.appendChild(secondDiv);

        botRow.appendChild(mainDiv);
    }

    function updateBotSetting(event) {
        var botName = event.srcElement.getAttribute('data-botname');
        var settings = getOrCreateUserSettings();

        for (var i in settings.bots) {
            if (settings.bots[i].name === botName) {
                settings.bots[i].displayLogs = event.srcElement.checked;
            }
        }

        saveSettings(settings);

        if (!event.srcElement.checked) {
            document.querySelectorAll(`[data-botname='${botName}']`).forEach(e => e.className = e.className + " d-none");
        }
        else {
            document.querySelectorAll(`[data-botname='${botName}']`).forEach(e => e.className = e.className.replace("d-none", ""));
        }
    }

    function removeOldBots() {
        var settings = getOrCreateUserSettings();
        var dateNow = new Date();

        settings.bots = settings.bots.filter(function (bot) { return dateNow.getTime() - new Date(bot.dateLastSeen).getTime() < 1000 * 60 * 60 * 24 * 30; });  // older than 30 days

        saveSettings(settings);
    }

    var logs = 0;
    var history = [];
    setInterval(() => {
        document.getElementById('lps').innerHTML = 'LPS: ' + logs;
        history.push({ x: Date.now(), y: logs });

        logs = 0;
    }, 1000);

    var ctx = document.getElementById('chart');
    var myLineChart = new Chart(ctx, {
        type: 'line',
        data: {
            datasets: [{
                backgroundColor: '#f0fff1',
                borderColor: '#aeff7b',
                data: [],
                label: 'Logs processed'
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    type: 'realtime',
                    realtime: {
                        duration: 30000,
                        refresh: 1000,
                        delay: 1000,
                        pause: false,
                        ttl: undefined,

                        onRefresh: function (chart) {
                            var data = history.shift();

                            if (data) {
                                Array.prototype.push.apply(chart.data.datasets[0].data, [data]);
                            }
                        }
                    }
                }],
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        stepSize: 5
                    }
                }]
            },
            plugins: {
                streaming: {
                    frameRate: 30
                }
            }
        }
    });
})();