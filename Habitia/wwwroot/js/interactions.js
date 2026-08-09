// ===================================================================
// Habitia · Home pública — animaciones al hacer scroll + nav activo
// ===================================================================
(function () {
    "use strict";

    var prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    if (!prefersReducedMotion && "IntersectionObserver" in window) {

        // ---------- Revelado de tarjetas/filas al entrar en pantalla ----------
        var revealTargets = document.querySelectorAll(
            ".pillar, .feature-card, .directory-board__row, .flow-step, .benefit-card"
        );

        revealTargets.forEach(function (el, i) {
            el.classList.add("reveal-init");
            el.style.transitionDelay = (i % 4) * 60 + "ms";
        });

        var revealObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.15, rootMargin: "0px 0px -40px 0px" });

        revealTargets.forEach(function (el) {
            revealObserver.observe(el);
        });

        // ---------- Línea del flujo: se "dibuja" al entrar en pantalla ----------
        var connectors = document.querySelectorAll(".flow-connector");

        var connectorObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.4 });

        connectors.forEach(function (el) {
            connectorObserver.observe(el);
        });
    }

    // ---------- Nav: resalta el enlace de la sección visible ----------
    var navLinks = document.querySelectorAll(".public-header__nav a");
    var sections = document.querySelectorAll("main section[id], body > section[id]");

    if (sections.length && navLinks.length && "IntersectionObserver" in window) {
        var navObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;

                var id = entry.target.getAttribute("id");

                navLinks.forEach(function (link) {
                    var href = link.getAttribute("href") || "";
                    var isMatch = href === "#" + id || href.indexOf("#" + id) !== -1;
                    link.classList.toggle("active", isMatch);
                });
            });
        }, { threshold: 0.5, rootMargin: "-35% 0px -50% 0px" });

        sections.forEach(function (section) {
            navObserver.observe(section);
        });
    }
})();