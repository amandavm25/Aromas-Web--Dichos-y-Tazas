// ============================================
// LOADER
// ============================================
window.addEventListener('load', function () {
    const loader = document.querySelector('.loader-wrapper');
    setTimeout(() => {
        loader.classList.add('hidden');
    }, 1500);
});

// ============================================
// CURSOR PERSONALIZADO (DESACTIVADO POR EL MOMENTO)
// ============================================
const cursor = document.querySelector('.custom-cursor');
const cursorFollower = document.querySelector('.custom-cursor-follower');

document.addEventListener('mousemove', (e) => {
    if (cursor) {
        cursor.style.left = e.clientX + 'px';
        cursor.style.top = e.clientY + 'px';
    }

    if (cursorFollower) {
        setTimeout(() => {
            cursorFollower.style.left = e.clientX + 'px';
            cursorFollower.style.top = e.clientY + 'px';
        }, 100);
    }
});

// Efecto hover en elementos interactivos
const interactiveElements = document.querySelectorAll('a, button, .menu-card, .gallery-item');
interactiveElements.forEach(el => {
    el.addEventListener('mouseenter', () => {
        if (cursorFollower) cursorFollower.classList.add('active');
    });
    el.addEventListener('mouseleave', () => {
        if (cursorFollower) cursorFollower.classList.remove('active');
    });
});

// ============================================
// NAVBAR
// ============================================
const navbar = document.getElementById('navbar');
const hamburger = document.getElementById('hamburger');
const navMenu = document.getElementById('navMenu');
const navLinks = document.querySelectorAll('.nav-link');

// Scroll effect
window.addEventListener('scroll', () => {
    if (navbar) {
        if (window.scrollY > 100) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
    }
});

// Menu mobile toggle
if (hamburger) {
    hamburger.addEventListener('click', () => {
        hamburger.classList.toggle('active');
        navMenu.classList.toggle('active');
    });
}

// Cerrar menu al hacer click en link
navLinks.forEach(link => {
    link.addEventListener('click', () => {
        hamburger.classList.remove('active');
        navMenu.classList.remove('active');
    });
});

// Active link on scroll
window.addEventListener('scroll', () => {
    let current = '';
    const sections = document.querySelectorAll('section[id]');

    sections.forEach(section => {
        const sectionTop = section.offsetTop;
        const sectionHeight = section.clientHeight;

        if (scrollY >= sectionTop - 200) {
            current = section.getAttribute('id');
        }
    });

    navLinks.forEach(link => {
        link.classList.remove('active');
        const href = link.getAttribute('href');
        if (href && href.includes(current)) {
            link.classList.add('active');
        }
    });
});

// Smooth scroll para anchors
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// ============================================
// HERO PARTICLES
// ============================================
function createParticles() {
    const particlesContainer = document.getElementById('heroParticles');
    const particleCount = 30;

    for (let i = 0; i < particleCount; i++) {
        const particle = document.createElement('div');
        particle.style.position = 'absolute';
        particle.style.width = Math.random() * 5 + 2 + 'px';
        particle.style.height = particle.style.width;
        particle.style.background = i % 2 === 0 ? '#207476' : '#E06E49';
        particle.style.borderRadius = '50%';
        particle.style.opacity = Math.random() * 0.3 + 0.1;
        particle.style.left = Math.random() * 100 + '%';
        particle.style.top = Math.random() * 100 + '%';
        particle.style.animation = `floatParticle ${Math.random() * 10 + 10}s infinite ease-in-out`;
        particle.style.animationDelay = Math.random() * 5 + 's';

        particlesContainer.appendChild(particle);
    }
}

// Crear animación CSS para partículas
const style = document.createElement('style');
style.innerHTML = `
    @keyframes floatParticle {
        0%, 100% {
            transform: translate(0, 0);
        }
        25% {
            transform: translate(20px, -20px);
        }
        50% {
            transform: translate(-10px, 10px);
        }
        75% {
            transform: translate(10px, 20px);
        }
    }
`;
document.head.appendChild(style);

if (document.getElementById('heroParticles')) {
    createParticles();
}

// ============================================
// STATS COUNTER
// ============================================
function animateCounter(element) {
    const target = parseInt(element.getAttribute('data-count'));
    const duration = 2000;
    const step = target / (duration / 16);
    let current = 0;

    const timer = setInterval(() => {
        current += step;
        if (current >= target) {
            element.textContent = target.toLocaleString() + '+';
            clearInterval(timer);
        } else {
            element.textContent = Math.floor(current).toLocaleString();
        }
    }, 16);
}

// Observador para animar cuando sea visible
const statsObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            const counters = entry.target.querySelectorAll('[data-count]');
            counters.forEach(counter => {
                if (!counter.classList.contains('counted')) {
                    counter.classList.add('counted');
                    animateCounter(counter);
                }
            });
        }
    });
}, { threshold: 0.5 });

const heroStats = document.querySelector('.hero-stats');
if (heroStats) {
    statsObserver.observe(heroStats);
}

// ============================================
// MENU TABS
// ============================================
const menuTabs = document.querySelectorAll('.menu-tab');
const menuContents = document.querySelectorAll('.menu-content');

menuTabs.forEach(tab => {
    tab.addEventListener('click', () => {
        const category = tab.getAttribute('data-category');

        // Remover active de todos
        menuTabs.forEach(t => t.classList.remove('active'));
        menuContents.forEach(c => c.classList.remove('active'));

        // Agregar active al seleccionado
        tab.classList.add('active');
        document.querySelector(`[data-category="${category}"].menu-content`).classList.add('active');
    });
});

// ============================================
// ADD TO CART
// ============================================
let cartCount = 0;
const cartBadge = document.querySelector('.cart-badge');
const addToCartButtons = document.querySelectorAll('.btn-add-cart');

addToCartButtons.forEach(button => {
    button.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        cartCount++;
        cartBadge.textContent = cartCount;

        // Animación del botón
        this.style.transform = 'scale(1.2) rotate(360deg)';
        this.style.background = 'linear-gradient(135deg, #62AA76, #207476)';

        setTimeout(() => {
            this.style.transform = '';
            this.style.background = '';
        }, 600);

        // Animación del badge
        cartBadge.style.transform = 'scale(1.5)';
        setTimeout(() => {
            cartBadge.style.transform = '';
        }, 300);

        // Mostrar notificación
        showNotification('Producto agregado al carrito');
    });
});

function showNotification(message) {
    const notification = document.createElement('div');
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 100px;
        right: 20px;
        background: linear-gradient(135deg, #62AA76, #207476);
        color: white;
        padding: 1rem 2rem;
        border-radius: 50px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001;
        animation: slideIn 0.3s ease;
        font-weight: 600;
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 2000);
}

// Animaciones para notificaciones
const siteNotifStyle = document.createElement('style');
siteNotifStyle.innerHTML = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(siteNotifStyle);

// ============================================
// SCROLL TO TOP BUTTON
// ============================================
const scrollTopBtn = document.getElementById('scrollToTop');

if (scrollTopBtn) {
    window.addEventListener('scroll', () => {
        if (window.scrollY > 500) {
            scrollTopBtn.classList.add('visible');
        } else {
            scrollTopBtn.classList.remove('visible');
        }
    });

    scrollTopBtn.addEventListener('click', () => {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });
}

// ============================================
// FORM VALIDATION
// ============================================
const contactForm = document.querySelector('.contact-form');
const newsletterForms = document.querySelectorAll('.newsletter-form, .newsletter-form-inline');

if (contactForm) {
    contactForm.addEventListener('submit', function (e) {
        e.preventDefault();

        // Animación de envío
        const submitBtn = this.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;

        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Enviando...';
        submitBtn.disabled = true;

        // Simular envío
        setTimeout(() => {
            submitBtn.innerHTML = '<i class="fas fa-check"></i> ¡Enviado!';
            showNotification('¡Reserva enviada exitosamente!');

            setTimeout(() => {
                submitBtn.innerHTML = originalText;
                submitBtn.disabled = false;
                this.reset();
            }, 2000);
        }, 1500);
    });
}

newsletterForms.forEach(form => {
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        const email = this.querySelector('input[type="email"]').value;

        if (email) {
            showNotification('¡Suscripción exitosa!');
            this.reset();
        }
    });
});

// ============================================
// PARALLAX EFFECT
// ============================================
window.addEventListener('scroll', () => {
    const scrolled = window.pageYOffset;

    // Hero parallax
    const hero = document.querySelector('.hero');
    if (hero) {
        hero.style.transform = `translateY(${scrolled * 0.3}px)`;
    }

    // Floating elements parallax
    const floatingElements = document.querySelectorAll('.floating-element');
    floatingElements.forEach((el, index) => {
        const speed = (index + 1) * 0.05;
        el.style.transform = `translateY(${scrolled * speed}px)`;
    });
});

// ============================================
// INTERSECTION OBSERVER ANIMATIONS
// ============================================
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -100px 0px'
};

const fadeInObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.style.opacity = '1';
            entry.target.style.transform = 'translateY(0)';
        }
    });
}, observerOptions);

// Observar elementos con data-aos
document.addEventListener('DOMContentLoaded', () => {
    const animatedElements = document.querySelectorAll('[data-aos]');

    animatedElements.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(50px)';
        el.style.transition = 'opacity 0.8s ease, transform 0.8s ease';

        const delay = el.getAttribute('data-aos-delay');
        if (delay) {
            el.style.transitionDelay = delay + 'ms';
        }

        fadeInObserver.observe(el);
    });
});

// ============================================
// GALLERY LIGHTBOX
// ============================================
const galleryItems = document.querySelectorAll('.gallery-item');

galleryItems.forEach(item => {
    item.addEventListener('click', function () {
        // Aquí puedes agregar un lightbox más completo
        showNotification('Lightbox - Próximamente');
    });
});

// ============================================
// 3D CARD EFFECT
// ============================================
const cards3D = document.querySelectorAll('.card-3d');

cards3D.forEach(card => {
    card.addEventListener('mousemove', function (e) {
        const rect = this.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;

        const centerX = rect.width / 2;
        const centerY = rect.height / 2;

        const rotateX = (y - centerY) / 10;
        const rotateY = (centerX - x) / 10;

        this.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg)`;
    });

    card.addEventListener('mouseleave', function () {
        this.style.transform = 'perspective(1000px) rotateX(0) rotateY(0)';
    });
});

// ============================================
// CONSOLE MESSAGE
// ============================================
console.log('%c¡Bienvenido a Entre Dichos y Tazas! ☕',
    'color: #207476; font-size: 20px; font-weight: bold; text-shadow: 2px 2px 4px rgba(0,0,0,0.2);');
console.log('%cDesarrollado con ❤️ por AromasWeb',
    'color: #E06E49; font-size: 14px;');

// ===================================
// DROPDOWN MENU MOBILE
// ===================================
document.addEventListener('DOMContentLoaded', function () {
    // Manejar dropdowns en móvil
    const navDropdowns = document.querySelectorAll('.nav-dropdown');

    navDropdowns.forEach(dropdown => {
        const link = dropdown.querySelector('.nav-link');

        if (link) {
            link.addEventListener('click', function (e) {
                // Solo prevenir en móvil
                if (window.innerWidth <= 968) {
                    e.preventDefault();

                    // Cerrar otros dropdowns
                    navDropdowns.forEach(other => {
                        if (other !== dropdown) {
                            other.classList.remove('active');
                        }
                    });

                    // Toggle del dropdown actual
                    dropdown.classList.toggle('active');
                }
            });
        }
    });

    // Cerrar dropdown al hacer clic en un item del menú
    const dropdownItems = document.querySelectorAll('.dropdown-menu a');
    dropdownItems.forEach(item => {
        item.addEventListener('click', function () {
            if (window.innerWidth <= 968) {
                const dropdown = this.closest('.nav-dropdown');
                if (dropdown) {
                    dropdown.classList.remove('active');
                }

                // Cerrar menú hamburguesa
                const navMenu = document.getElementById('navMenu');
                const hamburger = document.getElementById('hamburger');
                if (navMenu && hamburger) {
                    navMenu.classList.remove('active');
                    hamburger.classList.remove('active');
                }
            }
        });
    });
});

// ===================================
// DROPDOWN PERFIL DE USUARIO
// ===================================
document.addEventListener('DOMContentLoaded', function () {
    const navDropdowns = document.querySelectorAll('.nav-dropdown');

    navDropdowns.forEach(dropdown => {
        const link = dropdown.querySelector('.nav-link');

        if (link) {
            // Para móvil
            link.addEventListener('click', function (e) {
                if (window.innerWidth <= 968) {
                    e.preventDefault();
                    navDropdowns.forEach(other => {
                        if (other !== dropdown) {
                            other.classList.remove('active');
                        }
                    });
                    dropdown.classList.toggle('active');
                }
            });
        }

        // Para desktop - hover
        if (window.innerWidth > 968) {
            dropdown.addEventListener('mouseenter', function () {
                this.querySelector('.dropdown-menu').style.display = 'block';
            });

            dropdown.addEventListener('mouseleave', function () {
                this.querySelector('.dropdown-menu').style.display = 'none';
            });
        }
    });
});

// ============================================
// PAGINACIÓN GENERAL PARA TABLAS
// ============================================
function initTablePagination(config = {}) {
    // Configuración por defecto
    const settings = {
        tableId: config.tableId,
        recordsPerPage: config.recordsPerPage || 5,
        prevButtonId: config.prevButtonId || 'btnAnterior',
        nextButtonId: config.nextButtonId || 'btnSiguiente',
        startRecordId: config.startRecordId || 'startRecord',
        endRecordId: config.endRecordId || 'endRecord',
        totalRecordsId: config.totalRecordsId || 'totalRecords'
    };

    let currentPage = 1;

    // Función para mostrar página
    function showPage() {
        const table = document.getElementById(settings.tableId);
        const tbody = table ? table.querySelector('tbody') : null;
        if (!tbody) return;

        const allRows = Array.from(tbody.children).filter(row => row.tagName === 'TR' && !row.querySelector('td[colspan]'));
        const totalRecords = allRows.length;

        const start = (currentPage - 1) * settings.recordsPerPage;
        const end = start + settings.recordsPerPage;

        // Primero ocultar todos los TR del tbody
        Array.from(tbody.children).forEach(row => { if (row.tagName === 'TR') row.style.display = 'none'; });
        // Mostrar solo los de la página actual
        allRows.forEach((row, index) => { if (index >= start && index < end) row.style.display = ''; });
        // Si no hay datos, mostrar la fila de empty state
        if (totalRecords === 0) {
            Array.from(tbody.children).forEach(row => {
                if (row.tagName === 'TR' && row.querySelector('td[colspan]')) row.style.display = '';
            });
        }

        const totalPages = Math.ceil(totalRecords / settings.recordsPerPage);

        // Actualizar textos
        const startRecord = document.getElementById(settings.startRecordId);
        const endRecord = document.getElementById(settings.endRecordId);
        const totalRecordsEl = document.getElementById(settings.totalRecordsId);

        if (startRecord) startRecord.textContent = totalRecords > 0 ? start + 1 : 0;
        if (endRecord) endRecord.textContent = Math.min(end, totalRecords);
        if (totalRecordsEl) totalRecordsEl.textContent = totalRecords;

        // Actualizar botones
        const btnPrev = document.getElementById(settings.prevButtonId);
        const btnNext = document.getElementById(settings.nextButtonId);

        if (btnPrev) {
            btnPrev.disabled = currentPage === 1;
        }

        if (btnNext) {
            btnNext.disabled = currentPage === totalPages || totalRecords === 0;
        }
    }

    // Función anterior
    function prevPage() {
        if (currentPage > 1) {
            currentPage--;
            showPage();
        }
    }

    // Función siguiente
    function nextPage() {
        const table = document.getElementById(settings.tableId);
        const tbody = table ? table.querySelector('tbody') : null;
        if (!tbody) return;

        const allRows = Array.from(tbody.children).filter(row => row.tagName === 'TR' && !row.querySelector('td[colspan]'));
        const totalPages = Math.ceil(allRows.length / settings.recordsPerPage);

        if (currentPage < totalPages) {
            currentPage++;
            showPage();
        }
    }

    // Configurar botones
    const btnPrev = document.getElementById(settings.prevButtonId);
    const btnNext = document.getElementById(settings.nextButtonId);

    if (btnPrev) {
        btnPrev.onclick = prevPage;
    }

    if (btnNext) {
        btnNext.onclick = nextPage;
    }

    // Mostrar primera página
    showPage();

    // Retornar funciones para uso externo si es necesario
    return {
        showPage,
        prevPage,
        nextPage,
        getCurrentPage: () => currentPage,
        setPage: (page) => {
            currentPage = page;
            showPage();
        }
    };
}

// ============================================
// PAGINACIÓN GENERAL PARA CARDS
// ============================================
function initCardsPagination(config = {}) {
    // Configuración por defecto
    const settings = {
        containerId: config.containerId,
        cardsPerPage: config.cardsPerPage || 3,
        prevButtonId: config.prevButtonId || 'btnAnteriorCards',
        nextButtonId: config.nextButtonId || 'btnSiguienteCards',
        startCardId: config.startCardId || 'startCard',
        endCardId: config.endCardId || 'endCard',
        totalCardsId: config.totalCardsId || 'totalCards'
    };

    let currentPage = 1;

    // Función para mostrar página
    function showPage() {
        const container = document.getElementById(settings.containerId);
        if (!container) return;

        const allCards = Array.from(container.children);
        const totalCards = allCards.length;

        const start = (currentPage - 1) * settings.cardsPerPage;
        const end = start + settings.cardsPerPage;

        allCards.forEach((card, index) => {
            card.style.display = (index >= start && index < end) ? '' : 'none';
        });

        const totalPages = Math.ceil(totalCards / settings.cardsPerPage);

        // Actualizar textos
        const startCard = document.getElementById(settings.startCardId);
        const endCard = document.getElementById(settings.endCardId);
        const totalCardsEl = document.getElementById(settings.totalCardsId);

        if (startCard) startCard.textContent = totalCards > 0 ? start + 1 : 0;
        if (endCard) endCard.textContent = Math.min(end, totalCards);
        if (totalCardsEl) totalCardsEl.textContent = totalCards;

        // Actualizar botones
        const btnPrev = document.getElementById(settings.prevButtonId);
        const btnNext = document.getElementById(settings.nextButtonId);

        if (btnPrev) {
            btnPrev.disabled = currentPage === 1;
        }

        if (btnNext) {
            btnNext.disabled = currentPage === totalPages || totalCards === 0;
        }
    }

    // Función anterior
    function prevPage() {
        if (currentPage > 1) {
            currentPage--;
            showPage();
        }
    }

    // Función siguiente
    function nextPage() {
        const container = document.getElementById(settings.containerId);
        if (!container) return;

        const allCards = Array.from(container.children);
        const totalPages = Math.ceil(allCards.length / settings.cardsPerPage);

        if (currentPage < totalPages) {
            currentPage++;
            showPage();
        }
    }

    // Configurar botones
    const btnPrev = document.getElementById(settings.prevButtonId);
    const btnNext = document.getElementById(settings.nextButtonId);

    if (btnPrev) {
        btnPrev.onclick = prevPage;
    }

    if (btnNext) {
        btnNext.onclick = nextPage;
    }

    // Mostrar primera página
    showPage();

    // Retornar funciones para uso externo
    return {
        showPage,
        prevPage,
        nextPage,
        getCurrentPage: () => currentPage,
        setPage: (page) => {
            currentPage = page;
            showPage();
        }
    };
}