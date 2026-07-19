/* Consultation form niceties: live character counter for the message box. */
(function () {
    "use strict";

    document.querySelectorAll("textarea[data-counter]").forEach(function (area) {
        var counter = document.getElementById(area.dataset.counter);
        if (!counter) {
            return;
        }

        function update() {
            counter.textContent = area.value.length;
        }

        area.addEventListener("input", update);
        update();
    });
})();
