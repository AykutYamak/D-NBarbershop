document.addEventListener('DOMContentLoaded', () => {
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
            threshold: 0.10,
            rootMargin: '0px'
        }
    );

    document.querySelectorAll('.introduction, .whyUsSection, .home-sections-container, .barbers-section-container, .feedback-section-container').forEach(section => {
        observer.observe(section);
    });

    document.querySelectorAll('.feature-item, .card, .image-container img, .left-home-section-worktime h3').forEach((item, index) => {
        item.style.setProperty('--anim-index', index);
        observer.observe(item);
    });

   
});