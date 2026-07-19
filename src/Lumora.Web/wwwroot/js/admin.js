/* Admin shell behaviors (no inline JS in views).
   - [data-confirm]: asks before submitting destructive forms.
   - Language tabs: if server-side validation failed inside a hidden tab,
     mark its tab header and activate the first invalid one. */
(function () {
    "use strict";

    document.addEventListener("submit", function (e) {
        var form = e.target.closest("form[data-confirm]");
        if (form && !window.confirm(form.dataset.confirm)) {
            e.preventDefault();
        }
    });

    document.querySelectorAll(".admin-lang-tabs").forEach(function (tabs) {
        var panes = document.querySelectorAll(tabs.dataset.target + " .tab-pane");
        var firstInvalid = null;

        panes.forEach(function (pane) {
            if (pane.querySelector(".input-validation-error, .field-validation-error")) {
                var link = tabs.querySelector('[data-bs-target="#' + pane.id + '"]');
                if (link) {
                    link.classList.add("is-invalid");
                    firstInvalid = firstInvalid || link;
                }
            }
        });

        if (firstInvalid && window.bootstrap) {
            new window.bootstrap.Tab(firstInvalid).show();
        }
    });

    /* Dashboard: animate the stat numbers counting up. */
    document.querySelectorAll(".admin-stat-value[data-count]").forEach(function (el) {
        var target = parseInt(el.dataset.count, 10);
        if (isNaN(target) || target === 0) {
            return;
        }

        var start = null;
        var duration = 700;

        function tick(ts) {
            if (!start) { start = ts; }
            var progress = Math.min((ts - start) / duration, 1);
            el.textContent = Math.round(target * (0.2 + 0.8 * progress));
            if (progress < 1) {
                requestAnimationFrame(tick);
            } else {
                el.textContent = target;
            }
        }

        requestAnimationFrame(tick);
    });
})();
