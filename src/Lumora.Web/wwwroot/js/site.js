/* Site-wide behaviors. Page-specific logic lives in js/pages/<page>.js. */
(function () {
    "use strict";

    document.documentElement.classList.add("js-enabled");

    var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    /* ---------- reveal-on-scroll ---------- */
    var revealed = document.querySelectorAll("[data-reveal]");
    if (revealed.length > 0 && "IntersectionObserver" in window && !reduceMotion) {
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-revealed");
                    io.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });

        revealed.forEach(function (el) { io.observe(el); });
    } else {
        revealed.forEach(function (el) { el.classList.add("is-revealed"); });
    }

    /* ---------- animated counters (e.g. landing stats "5000+", "98%") ---------- */
    var counters = document.querySelectorAll("[data-count-to]");
    if (counters.length > 0 && !reduceMotion && "IntersectionObserver" in window) {
        var cio = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) { return; }
                cio.unobserve(entry.target);

                var el = entry.target;
                var target = parseInt(el.dataset.countTo, 10);
                var suffix = el.dataset.countSuffix || "";
                var duration = 1200;
                var start = null;

                function tick(ts) {
                    if (!start) { start = ts; }
                    var p = Math.min((ts - start) / duration, 1);
                    var eased = 1 - Math.pow(1 - p, 3); /* ease-out cubic */
                    el.textContent = Math.round(target * eased).toLocaleString() + suffix;
                    if (p < 1) { requestAnimationFrame(tick); }
                }

                requestAnimationFrame(tick);
            });
        }, { threshold: 0.6 });

        counters.forEach(function (el) { cio.observe(el); });
    }

    /* ---------- navbar shadow after scrolling ---------- */
    var navbar = document.querySelector(".lum-navbar");
    if (navbar) {
        var onScroll = function () {
            navbar.classList.toggle("is-scrolled", window.scrollY > 8);
        };
        window.addEventListener("scroll", onScroll, { passive: true });
        onScroll();
    }
})();
