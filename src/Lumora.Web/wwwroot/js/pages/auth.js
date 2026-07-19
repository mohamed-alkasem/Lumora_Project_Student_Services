/* Auth pages motion & niceties:
   - entrance animation (adds .is-in to the shell)
   - password show/hide toggle injected next to every password input
   - brand-panel glow follows the pointer (subtle parallax) */
(function () {
    "use strict";

    var shell = document.querySelector(".auth-shell");
    if (!shell) {
        return;
    }

    requestAnimationFrame(function () {
        requestAnimationFrame(function () {
            shell.classList.add("is-in");
        });
    });

    /* ---------- password visibility toggles ---------- */
    var EYE_OPEN = '<svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17a5 5 0 1 1 5-5 5 5 0 0 1-5 5zm0-8a3 3 0 1 0 3 3 3 3 0 0 0-3-3z"/></svg>';
    var EYE_OFF = '<svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M12 7a5 5 0 0 1 5 5 4.85 4.85 0 0 1-.36 1.83l2.92 2.92A11.82 11.82 0 0 0 23 12c-1.73-4.39-6-7.5-11-7.5a11.8 11.8 0 0 0-4 .7l2.17 2.16A4.85 4.85 0 0 1 12 7zM2.71 3.16 1.29 4.57l2.4 2.4A11.79 11.79 0 0 0 1 12c1.73 4.39 6 7.5 11 7.5a11.8 11.8 0 0 0 4.38-.84l3.05 3.05 1.41-1.41zM12 17a5 5 0 0 1-5-5 4.9 4.9 0 0 1 .49-2.14l1.57 1.57A3 3 0 0 0 12 15a3 3 0 0 0 .57-.06l1.57 1.57A4.9 4.9 0 0 1 12 17z"/></svg>';

    shell.querySelectorAll('input[type="password"]').forEach(function (input) {
        var wrap = document.createElement("div");
        wrap.className = "auth-pass-wrap";
        input.parentNode.insertBefore(wrap, input);
        wrap.appendChild(input);

        var btn = document.createElement("button");
        btn.type = "button";
        btn.className = "auth-eye";
        btn.setAttribute("aria-label", "toggle password");
        btn.innerHTML = EYE_OPEN;
        wrap.appendChild(btn);

        btn.addEventListener("click", function () {
            var show = input.type === "password";
            input.type = show ? "text" : "password";
            btn.innerHTML = show ? EYE_OFF : EYE_OPEN;
            input.focus();
        });
    });

    /* ---------- glow parallax ---------- */
    var brand = shell.querySelector(".auth-brand");
    var glow = shell.querySelector(".auth-brand-glow");
    if (brand && glow && window.matchMedia("(pointer: fine)").matches) {
        brand.addEventListener("mousemove", function (e) {
            var rect = brand.getBoundingClientRect();
            var dx = (e.clientX - rect.left) / rect.width - 0.5;
            var dy = (e.clientY - rect.top) / rect.height - 0.5;
            glow.style.transform = "translate(" + dx * 30 + "px," + dy * 30 + "px)";
        });
        brand.addEventListener("mouseleave", function () {
            glow.style.transform = "";
        });
    }
})();
