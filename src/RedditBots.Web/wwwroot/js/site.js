(function abc() {
    var notify = false;
    var showDebug = false;

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

    renderBotsSettings();

    connection.on("Log", (log) => {
        if (log.notify === true) {
            logs++;
        }

        var firstP = document.createElement('span');
        var namearray = log.logName.split('.');
        var botName = namearray[namearray.length - 1];

        addBotIfNeeded(botName);
        var botSetting = getBotSetting(botName);

        if (!botSetting.displayLogs) {
            return;
        }

        if (log.logLevel === 'Debug'
            && !showDebug) {
            return;
        }

        var topdiv = document.createElement('div');
        topdiv.className = 'p-1';
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

        var topDivInBody = document.createElement('div');
        topDivInBody.className = 'row d-flex justify-content-between';

        if (botName === "AzurePipeline") {
            firstP.innerHTML = 'Azure DevOps Pipeline';
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

        var logcards = document.querySelectorAll('[data-log]');

        if (logcards.length >= 50) {
            for (var i = 50; i < logcards.length; i++) {
                logcards[0].parentNode.removeChild(logcards[i]);
            }
        }

        if (notify === true
            && log.notify === true) {
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

    function getSettings() {
        var settings = localStorage.getItem('botSettings');

        if (settings === undefined || settings === null) {
            settings = [];
            saveSettings(settings);
            return settings;
        }

        return JSON.parse(settings);
    }

    function saveSettings(settings) {
        localStorage.setItem('botSettings', JSON.stringify(settings));
    }

    function getBotSetting(botName) {
        var settings = getSettings();

        return settings.find(b => b.name === botName);
    }

    function addBotIfNeeded(botName) {
        var botSetting = getBotSetting(botName);

        if (botSetting === undefined || botSetting === null) {
            var settings = getSettings();
            var bot = { name: botName, displayLogs: true };

            settings.push(bot);
            saveSettings(settings);
            renderBotSetting(bot);
        }
    }

    function renderBotsSettings() {
        var settings = getSettings();

        var element = document.getElementById('botSettings');
        element.innerHTML = '';

        settings.sort((a, b) => (a.name > b.name) ? 1 : -1).forEach(renderBotSetting);
    }

    function renderBotSetting(bot) {
        var botRow = document.getElementById('botSettings');

        var mainDiv = document.createElement('div');
        mainDiv.className = 'col-lg-4 col-md-6 col-12';

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

        //  <div class="col-lg-4 col-md-6 col-12">
        //      <div class="custom-control custom-toggle my-2">
        //          <input type="checkbox" id="showdebug" name="showdebug" class="custom-control-input">
        //          <label class="custom-control-label" for="showdebug">HanzeMemesBot</label>
        //      </div>
        //  </div>

        secondDiv.appendChild(input);
        secondDiv.appendChild(label);

        mainDiv.appendChild(secondDiv);

        botRow.appendChild(mainDiv);
    }

    function updateBotSetting(event) {
        var botName = event.srcElement.getAttribute('data-botname');
        var settings = getSettings();

        for (var i in settings) {
            if (settings[i].name === botName) {
                settings[i].displayLogs = event.srcElement.checked;
            }
        }

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