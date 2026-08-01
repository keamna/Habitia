// ===================================================================
// Habitia · Home pública — animaciones al hacer scroll + nav activo
// ===================================================================
(function () {
    "use strict";

    var prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    /* ── Carrusel: Qué es Habitia (pilares) ── */
    (function () {
        const track = document.getElementById('pillarsTrack');
        if (!track || prefersReducedMotion) return;

        const originals = Array.from(track.querySelectorAll('.pillar'));
        const GAP = 22;

        for (let i = 0; i < 2; i++) {
            originals.forEach(card => {
                const clone = card.cloneNode(true);
                clone.setAttribute('aria-hidden', 'true');
                track.appendChild(clone);
            });
        }

        requestAnimationFrame(() => {
            const cardWidth = originals[0].offsetWidth + GAP;
            const loopWidth = originals.length * cardWidth;

            let scrollX = 0;
            let paused = false;
            const speed = 0.5;

            function animate() {
                if (!paused) {
                    scrollX += speed;
                    if (scrollX >= loopWidth) scrollX -= loopWidth;
                    track.style.transform = `translateX(-${scrollX}px)`;
                }
                requestAnimationFrame(animate);
            }

            track.addEventListener('mouseenter', () => paused = true);
            track.addEventListener('mouseleave', () => paused = false);

            animate();
        });
    })();

    /* ── Carrusel: Funcionalidades ── */
    (function () {
        const track = document.getElementById('featuresTrack');
        if (!track || prefersReducedMotion) return;

        const originals = Array.from(track.querySelectorAll('.feature-card'));
        const GAP = 20;

        for (let i = 0; i < 2; i++) {
            originals.forEach(card => {
                const clone = card.cloneNode(true);
                clone.setAttribute('aria-hidden', 'true');
                track.appendChild(clone);
            });
        }

        requestAnimationFrame(() => {
            const cardWidth = originals[0].offsetWidth + GAP;
            const loopWidth = originals.length * cardWidth;

            let scrollX = 0;
            let paused = false;
            const speed = 0.5;

            function animate() {
                if (!paused) {
                    scrollX += speed;
                    if (scrollX >= loopWidth) scrollX -= loopWidth;
                    track.style.transform = `translateX(-${scrollX}px)`;
                }
                requestAnimationFrame(animate);
            }

            track.addEventListener('mouseenter', () => paused = true);
            track.addEventListener('mouseleave', () => paused = false);

            animate();
        });
    })();

    if (!prefersReducedMotion && "IntersectionObserver" in window) {

        // ---------- Revelado de tarjetas/filas al entrar en pantalla ----------
        var revealTargets = document.querySelectorAll(
            ".directory-board__row, .flow-step, .benefit-card"
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