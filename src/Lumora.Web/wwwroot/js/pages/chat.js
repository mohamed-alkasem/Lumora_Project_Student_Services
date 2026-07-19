/* Live chat (student page + admin conversation view — same script).
   data-viewer tells us which side we are; "mine" bubbles are the viewer's own.
   Messages are appended only when the hub broadcasts them back, so what you
   see is exactly what was persisted. */
(function () {
    "use strict";

    var page = document.getElementById("chatPage");
    if (!page || typeof signalR === "undefined") {
        return;
    }

    var messagesBox = document.getElementById("chatMessages");
    var form = document.getElementById("chatForm");
    var input = document.getElementById("chatText");
    var empty = document.getElementById("chatEmpty");

    var viewer = page.dataset.viewer; // "student" | "admin"
    var conversationId = parseInt(page.dataset.conversationId, 10);

    function scrollToBottom() {
        messagesBox.scrollTop = messagesBox.scrollHeight;
    }

    function appendMessage(m) {
        if (empty) {
            empty.classList.add("d-none");
        }

        var mine = (viewer === "admin") === m.isFromAdmin;
        var bubble = document.createElement("div");
        bubble.className = "chat-bubble " + (mine ? "mine" : "theirs");

        var text = document.createElement("span");
        text.className = "chat-text";
        text.textContent = m.text;

        var time = document.createElement("span");
        time.className = "chat-time";
        time.textContent = m.sentAt;

        bubble.appendChild(text);
        bubble.appendChild(time);
        messagesBox.appendChild(bubble);
        scrollToBottom();
    }

    var connection = new signalR.HubConnectionBuilder()
        .withUrl(page.dataset.hubUrl)
        .withAutomaticReconnect()
        .build();

    connection.on("message", appendMessage);

    var ready = connection.start().then(function () {
        // Student joins (and may lazily create) their own conversation;
        // admin joins the one this page shows.
        if (viewer === "admin") {
            return connection.invoke("JoinConversation", conversationId);
        }
        return connection.invoke("JoinMyConversation").then(function (id) {
            conversationId = id;
        });
    });

    form.addEventListener("submit", function (e) {
        e.preventDefault();
        var text = input.value.trim();
        if (!text) {
            return;
        }

        input.value = "";
        ready.then(function () {
            return connection.invoke("SendMessage", conversationId, text);
        }).catch(function () {
            input.value = text; // let the user retry on failure
        });
    });

    scrollToBottom();
})();
