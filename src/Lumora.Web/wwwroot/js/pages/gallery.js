/* Photo gallery: thumbnail switcher + arrows + keyboard, with fade transition.
   Works on any page containing a .lgal block. */
(function () {
    "use strict";

    var gallery = document.querySelector(".lgal");
    if (!gallery) {
        return;
    }

    var main = gallery.querySelector(".lgal-main img");
    var counter = gallery.querySelector(".lgal-count");
    var thumbs = Array.prototype.slice.call(gallery.querySelectorAll(".lgal-thumb"));
    if (!main || thumbs.length === 0) {
        return;
    }

    var index = 0;

    function show(nextIndex) {
        index = (nextIndex + thumbs.length) % thumbs.length;

        main.classList.add("is-fading");
        setTimeout(function () {
            main.src = thumbs[index].dataset.src;
            main.onload = function () {
                main.classList.remove("is-fading");
            };
        }, 180);

        thumbs.forEach(function (t, i) {
            t.classList.toggle("is-active", i === index);
        });

        if (counter) {
            counter.textContent = (index + 1) + " / " + thumbs.length;
        }
    }

    thumbs.forEach(function (thumb, i) {
        thumb.addEventListener("click", function () {
            show(i);
        });
    });

    var prev = gallery.querySelector(".lgal-prev");
    var next = gallery.querySelector(".lgal-next");
    if (prev) { prev.addEventListener("click", function () { show(index - 1); }); }
    if (next) { next.addEventListener("click", function () { show(index + 1); }); }

    document.addEventListener("keydown", function (e) {
        if (e.key === "ArrowLeft") { show(index - 1); }
        if (e.key === "ArrowRight") { show(index + 1); }
    });
})();
