/**
 * Habitia · Interactions Engine
 * Efectos tech-modern: scroll reveals, parallax, cursor glow,
 * header inteligente, magnetic buttons, y más.
 */

(function() {
    'use strict';

    // ============================================================
    // CONFIGURACIÓN
    // ============================================================
    const CONFIG = {
        scrollOffset: 100,
        headerHideThreshold: 80,
        parallaxStrength: 0.3,
        cursorGlow: true,
        reducedMotion: window.matchMedia('(prefers-reduced-motion: reduce)').matches
    };

    // ============================================================
    // UTILIDADES
    // ============================================================
    const $ = (selector, context = document) => context.querySelector(selector);
    const $$ = (selector, context = document) => Array.from(context.querySelectorAll(selector));
    const debounce = (fn, ms = 100) => {
        let t;
        return (...args) => { clearTimeout(t); t = setTimeout(() => fn(...args), ms); };
    };
    const throttle = (fn, ms = 16) => {
        let last = 0;
        return (...args) => {
            const now = Date.now();
            if (now - last >= ms) { last = now; fn(...args); }
        };
    };

    // ============================================================
    // 1. SCROLL REVEAL (Intersection Observer)
    // ============================================================
    function initScrollReveal() {
        const revealElements = $$('.reveal-init');
        if (!revealElements.length) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    // Una vez visible, dejar de observar (animación única)
                    // Descomentar la siguiente línea si quieres que solo se anime una vez:
                    // observer.unobserve(entry.target);
                }
            });
        }, {
            root: null,
            rootMargin: '0px 0px -60px 0px',
            threshold: 0.1
        });

        revealElements.forEach(el => observer.observe(el));
    }

    // ============================================================
    // 2. HEADER INTELIGENTE (hide/show + shrink on scroll)
    // ============================================================
    function initSmartHeader() {
        const header = $('.public-header');
        if (!header) return;

        let lastScrollY = 0;
        let ticking = false;

        const updateHeader = () => {
            const scrollY = window.scrollY;

            // Shrink effect
            if (scrollY > 50) {
                header.classList.add('header--scrolled');
            } else {
                header.classList.remove('header--scrolled');
            }

            // Hide/show on scroll direction
            if (scrollY > CONFIG.headerHideThreshold) {
                if (scrollY > lastScrollY) {
                    // Scrolling down → hide
                    header.classList.add('header--hidden');
                } else {
                    // Scrolling up → show
                    header.classList.remove('header--hidden');
                }
            } else {
                header.classList.remove('header--hidden');
            }

            lastScrollY = scrollY;
            ticking = false;
        };

        window.addEventListener('scroll', () => {
            if (!ticking) {
                requestAnimationFrame(updateHeader);
                ticking = true;
            }
        }, { passive: true });
    }

    // ============================================================
    // 3. SMOOTH SCROLL para nav links
    // ============================================================
    function initSmoothScroll() {
        $$('a[href^="#"]').forEach(link => {
            link.addEventListener('click', (e) => {
                const targetId = link.getAttribute('href');
                if (targetId === '#') return;

                const target = $(targetId);
                if (target) {
                    e.preventDefault();
                    const headerHeight = $('.public-header')?.offsetHeight || 80;
                    const targetPosition = target.getBoundingClientRect().top + window.scrollY - headerHeight - 20;

                    window.scrollTo({
                        top: targetPosition,
                        behavior: CONFIG.reducedMotion ? 'auto' : 'smooth'
                    });
                }
            });
        });
    }

    // ============================================================
    // 4. ACTIVE NAV LINK (highlight según sección visible)
    // ============================================================
    function initActiveNav() {
        const navLinks = $$('.public-header__nav a[href^="#"]');
        const sections = navLinks.map(link => $(link.getAttribute('href'))).filter(Boolean);
        if (!sections.length) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    navLinks.forEach(link => link.classList.remove('active'));
                    const activeLink = navLinks.find(l => l.getAttribute('href') === '#' + entry.target.id);
                    if (activeLink) activeLink.classList.add('active');
                }
            });
        }, {
            rootMargin: '-40% 0px -55% 0px',
            threshold: 0
        });

        sections.forEach(section => observer.observe(section));
    }

    // ============================================================
    // 5. CURSOR GLOW (efecto tech sutil)
    // ============================================================
    function initCursorGlow() {
        if (CONFIG.reducedMotion || window.matchMedia('(pointer: coarse)').matches) return;
        if (!CONFIG.cursorGlow) return;

        const glow = document.createElement('div');
        glow.className = 'cursor-glow';
        document.body.appendChild(glow);

        let mouseX = 0, mouseY = 0;
        let glowX = 0, glowY = 0;
        let isActive = false;
        let rafId = null;

        const animate = () => {
            if (!isActive) return;
            glowX += (mouseX - glowX) * 0.12;
            glowY += (mouseY - glowY) * 0.12;
            glow.style.left = glowX + 'px';
            glow.style.top = glowY + 'px';
            rafId = requestAnimationFrame(animate);
        };

        document.addEventListener('mousemove', (e) => {
            mouseX = e.clientX;
            mouseY = e.clientY;
            if (!isActive) {
                isActive = true;
                glow.style.opacity = '1';
                animate();
            }
        });

        document.addEventListener('mouseleave', () => {
            isActive = false;
            glow.style.opacity = '0';
            if (rafId) cancelAnimationFrame(rafId);
        });
    }

    // ============================================================
    // 6. PARALLAX HERO (sutil)
    // ============================================================
    function initParallax() {
        if (CONFIG.reducedMotion) return;

        const hero = $('.hero');
        const heroTitle = $('.hero-title');
        if (!hero) return;

        let ticking = false;

        const updateParallax = () => {
            const scrollY = window.scrollY;
            const heroHeight = hero.offsetHeight;

            if (scrollY < heroHeight) {
                const progress = scrollY / heroHeight;
                // Background parallax
                hero.style.backgroundPositionY = (scrollY * CONFIG.parallaxStrength) + 'px';
                // Title fade + translate
                if (heroTitle) {
                    heroTitle.style.transform = `translateY(${scrollY * 0.15}px)`;
                    heroTitle.style.opacity = 1 - (progress * 1.2);
                }
            }
            ticking = false;
        };

        window.addEventListener('scroll', () => {
            if (!ticking) {
                requestAnimationFrame(updateParallax);
                ticking = true;
            }
        }, { passive: true });
    }

    // ============================================================
    // 7. FLOW CONNECTOR ANIMATION
    // ============================================================
    function initFlowConnectors() {
        const connectors = $$('.flow-connector');
        if (!connectors.length) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                }
            });
        }, { threshold: 0.5 });

        connectors.forEach(c => observer.observe(c));
    }

    // ============================================================
    // 8. MAGNETIC BUTTONS
    // ============================================================
    function initMagneticButtons() {
        if (CONFIG.reducedMotion || window.matchMedia('(pointer: coarse)').matches) return;

        const buttons = $$('.btn-registrarse, .public-header__nav a');

        buttons.forEach(btn => {
            btn.addEventListener('mousemove', (e) => {
                const rect = btn.getBoundingClientRect();
                const x = e.clientX - rect.left - rect.width / 2;
                const y = e.clientY - rect.top - rect.height / 2;
                btn.style.transform = `translate(${x * 0.15}px, ${y * 0.15}px)`;
            });

            btn.addEventListener('mouseleave', () => {
                btn.style.transform = '';
            });
        });
    }

    // ============================================================
    // 9. COUNTER ANIMATION (para números si los hay)
    // ============================================================
    function initCounters() {
        const counters = $$('[data-counter]');
        if (!counters.length) return;

        const animateCounter = (el) => {
            const target = parseInt(el.dataset.counter, 10);
            const duration = parseInt(el.dataset.duration, 10) || 2000;
            const start = performance.now();

            const update = (now) => {
                const elapsed = now - start;
                const progress = Math.min(elapsed / duration, 1);
                // Ease out expo
                const eased = 1 - Math.pow(1 - progress, 3);
                el.textContent = Math.floor(eased * target).toLocaleString();

                if (progress < 1) {
                    requestAnimationFrame(update);
                } else {
                    el.textContent = target.toLocaleString();
                }
            };

            requestAnimationFrame(update);
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !entry.target.dataset.animated) {
                    entry.target.dataset.animated = 'true';
                    animateCounter(entry.target);
                }
            });
        }, { threshold: 0.5 });

        counters.forEach(c => observer.observe(c));
    }

    // ============================================================
    // 10. MOBILE MENU TOGGLE
    // ============================================================
    function initMobileMenu() {
        // Si en el futuro agregas un botón hamburguesa, esto funcionará automáticamente
        const toggle = $('[data-mobile-toggle]');
        const nav = $('.public-header__nav');
        if (!toggle || !nav) return;

        toggle.addEventListener('click', () => {
            const isOpen = nav.classList.toggle('is-open');
            toggle.setAttribute('aria-expanded', isOpen);
            toggle.classList.toggle('is-active', isOpen);
        });
    }

    // ============================================================
    // 11. SCROLL INDICATOR CLICK
    // ============================================================
    function initScrollIndicator() {
        const indicator = $('.hero-scroll-indicator');
        if (!indicator) return;

        indicator.addEventListener('click', () => {
            const nextSection = indicator.closest('.hero-section')?.nextElementSibling;
            if (nextSection) {
                const headerHeight = $('.public-header')?.offsetHeight || 80;
                window.scrollTo({
                    top: nextSection.offsetTop - headerHeight - 20,
                    behavior: CONFIG.reducedMotion ? 'auto' : 'smooth'
                });
            }
        });
    }

    // ============================================================
    // 12. TILT EFFECT en cards (3D sutil)
    // ============================================================
    function initTiltCards() {
        if (CONFIG.reducedMotion || window.matchMedia('(pointer: coarse)').matches) return;

        const cards = $$('.pillar, .feature-card, .benefit-card, .flow-step, .directory-board__row');

        cards.forEach(card => {
            card.addEventListener('mousemove', (e) => {
                const rect = card.getBoundingClientRect();
                const x = (e.clientX - rect.left) / rect.width;
                const y = (e.clientY - rect.top) / rect.height;
                const tiltX = (y - 0.5) * 6;  // -3 to 3 deg
                const tiltY = (x - 0.5) * -6; // -3 to 3 deg

                card.style.transform = `perspective(800px) rotateX(${tiltX}deg) rotateY(${tiltY}deg) translateY(-4px)`;
            });

            card.addEventListener('mouseleave', () => {
                card.style.transform = '';
            });
        });
    }

    // ============================================================
    // 13. GLOW FOLLOW en cards tech (secciones oscuras)
    // ============================================================
    function initCardGlow() {
        if (CONFIG.reducedMotion || window.matchMedia('(pointer: coarse)').matches) return;

        const darkCards = $$('.directory-board__row, .flow-step');

        darkCards.forEach(card => {
            card.addEventListener('mousemove', (e) => {
                const rect = card.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;
                card.style.setProperty('--glow-x', x + 'px');
                card.style.setProperty('--glow-y', y + 'px');
                card.style.background = `radial-gradient(circle 120px at ${x}px ${y}px, rgba(110, 231, 135, .08), rgba(255, 255, 255, .04))`;
            });

            card.addEventListener('mouseleave', () => {
                card.style.background = '';
            });
        });
    }

    // ============================================================
    // 14. LOADING STATE para imágenes
    // ============================================================
    function initImageLoad() {
        $$('img').forEach(img => {
            if (!img.complete) {
                img.classList.add('is-loading');
                img.addEventListener('load', () => img.classList.remove('is-loading'));
                img.addEventListener('error', () => img.classList.remove('is-loading'));
            }
        });
    }

    // ============================================================
    // 15. TEXT SCRAMBLE EFFECT (opcional, para títulos)
    // ============================================================
    function initTextScramble() {
        if (CONFIG.reducedMotion) return;

        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*';

        const scramble = (el, finalText, duration = 1200) => {
            const start = performance.now();
            const length = finalText.length;

            const update = (now) => {
                const elapsed = now - start;
                const progress = Math.min(elapsed / duration, 1);
                const revealed = Math.floor(progress * length);

                let text = '';
                for (let i = 0; i < length; i++) {
                    if (i < revealed) {
                        text += finalText[i];
                    } else if (finalText[i] === ' ') {
                        text += ' ';
                    } else {
                        text += chars[Math.floor(Math.random() * chars.length)];
                    }
                }
                el.textContent = text;

                if (progress < 1) {
                    requestAnimationFrame(update);
                } else {
                    el.textContent = finalText;
                }
            };

            requestAnimationFrame(update);
        };

        // Aplicar a elementos con data-scramble
        $$('[data-scramble]').forEach(el => {
            const finalText = el.textContent;
            el.textContent = '';

            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting && !el.dataset.scrambled) {
                        el.dataset.scrambled = 'true';
                        scramble(el, finalText);
                        observer.unobserve(el);
                    }
                });
            }, { threshold: 0.5 });

            observer.observe(el);
        });
    }

    // ============================================================
    // INICIALIZACIÓN
    // ============================================================
    function init() {
        // Esperar a que el DOM esté listo
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', runInit);
        } else {
            runInit();
        }
    }

    function runInit() {
        initScrollReveal();
        initSmartHeader();
        initSmoothScroll();
        initActiveNav();
        initCursorGlow();
        initParallax();
        initFlowConnectors();
        initMagneticButtons();
        initCounters();
        initMobileMenu();
        initScrollIndicator();
        initTiltCards();
        initCardGlow();
        initImageLoad();
        initTextScramble();

        console.log('🌿 Habitia interactions loaded');
    }

    // Exponer API global por si necesitas control manual
    window.Habitia = {
        refresh: runInit,
        config: CONFIG
    };

    init();
})();