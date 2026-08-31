var ProgressGateway = (function () {

    var connection = null;

    var progressUpdateCallbacks = [];

    var completedCallbacks = [];


    // ============================================
    // INITIALIZE SIGNALR CONNECTION
    // ============================================

    function initialize() {

        if (connection !== null) {

            return Promise.resolve();

        }


        var hubUrl =
            window.progressGatewayConfig.apiBaseUrl
            + "/progressHub";


        console.log(
            "Connecting to SignalR Hub:",
            hubUrl
        );


        connection =
            new signalR.HubConnectionBuilder()
                .withUrl(hubUrl)
                .withAutomaticReconnect()
                .build();


        // ============================================
        // RECEIVE PROGRESS UPDATE
        // ============================================

        connection.on(
            "ReceiveProgressUpdate",
            function (data) {

                console.log(
                    "Progress update received:",
                    data
                );


                progressUpdateCallbacks.forEach(
                    function (callback) {

                        callback(data);

                    }
                );

            }
        );


        // ============================================
        // RECEIVE COMPLETED EVENT
        // ============================================

        connection.on(
            "ReceiveProgressCompleted",
            function (data) {

                console.log(
                    "Progress completed:",
                    data
                );


                completedCallbacks.forEach(
                    function (callback) {

                        callback(data);

                    }
                );

            }
        );


        // ============================================
        // CONNECTION EVENTS
        // ============================================

        connection.onreconnecting(function (error) {

            console.warn(
                "SignalR reconnecting...",
                error
            );

        });


        connection.onreconnected(function (connectionId) {

            console.log(
                "SignalR reconnected.",
                connectionId
            );

        });


        connection.onclose(function (error) {

            console.error(
                "SignalR connection closed.",
                error
            );

        });


        // ============================================
        // START CONNECTION
        // ============================================

        return connection.start()
            .then(function () {

                console.log(
                    "Connected to ProgressGateway.Api"
                );

            })
            .catch(function (error) {

                console.error(
                    "SignalR connection failed:",
                    error
                );

                connection = null;

                throw error;

            });

    }


    // ============================================
    // REGISTER PROGRESS CALLBACK
    // ============================================

    function onProgressUpdate(callback) {

        progressUpdateCallbacks.push(callback);

    }


    // ============================================
    // REGISTER COMPLETED CALLBACK
    // ============================================

    function onCompleted(callback) {

        completedCallbacks.push(callback);

    }


    // ============================================
    // JOIN EXECUTION GROUP
    // ============================================

    function joinExecution(executionId) {

        if (!connection) {

            return Promise.reject(
                "SignalR connection is not initialized."
            );

        }


        console.log(
            "Joining execution:",
            executionId
        );


        return connection.invoke(
            "JoinExecutionGroup",
            executionId
        );

    }


    // ============================================
    // LEAVE EXECUTION GROUP
    // ============================================

    function leaveExecution(executionId) {

        if (!connection) {

            return Promise.resolve();

        }


        return connection.invoke(
            "LeaveExecutionGroup",
            executionId
        );

    }


    // ============================================
    // GENERATE EXECUTION ID
    // ============================================

    function generateExecutionId() {

        return crypto.randomUUID();

    }


    // ============================================
    // PUBLIC METHODS
    // ============================================

    return {

        initialize: initialize,

        onProgressUpdate: onProgressUpdate,

        onCompleted: onCompleted,

        joinExecution: joinExecution,

        leaveExecution: leaveExecution,

        generateExecutionId: generateExecutionId

    };

})();