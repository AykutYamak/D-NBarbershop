document.addEventListener('DOMContentLoaded', () => {
    // Intersection Observer
    const observer = new IntersectionObserver(
        (entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate');
                    observer.unobserve(entry.target);
                }
            });
        },
        {
            root: null,
            threshold: 0.15,
            rootMargin: '0px' // Remove negative margin
        }
    );

    // Observe main sections
    const sections = [
        '.introduction',
        '.whyUsSection',
        '.home-sections-container',
        '.barbers-section-container',
        '.feedback-section-container',
    ].map(selector => document.querySelector(selector));
    sections.forEach(section => {
        if (section) observer.observe(section);
    });

    // Observe staggered elements within sections
    const staggerElements = [
        { selector: '.feature-item', delayMultiplier: 0.2 },
        { selector: '.card', delayMultiplier: 0.2 },
        { selector: '.image-container img', delayMultiplier: 0.15 },
        { selector: '.left-home-section-worktime h3', delayMultiplier: 0.1 },
    ];
    staggerElements.forEach(({ selector, delayMultiplier }) => {
        const items = document.querySelectorAll(selector);
        items.forEach((item, index) => {
            if (item) {
                item.style.setProperty('--anim-index', index);
                observer.observe(item);
            }
        });
    });

    // Throttled Parallax Effect
    let lastScroll = 0;
    const throttle = (func, limit) => {
        let inThrottle;
        return (...args) => {
            if (!inThrottle) {
                func.apply(this, args);
                inThrottle = true;
                setTimeout(() => (inThrottle = false), limit);
            }
        };
    };
    const handleParallax = throttle(() => {
        const scrollPosition = window.scrollY;
        const homePageBody = document.querySelector('.HomePageBody');
        if (homePageBody) {
            homePageBody.style.backgroundPosition = `center ${scrollPosition * 0.4}px`;
        }
    }, 16); // ~60fps
    window.addEventListener('scroll', handleParallax);
});