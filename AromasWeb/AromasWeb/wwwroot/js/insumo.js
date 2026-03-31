// insumo.js - Gestión de insumos

document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTableHoverEffects();
    initializeModals();
    initStockBajoPagination();

    // ============================================
    // PAGINACIÓN (usando función general de site.js)
    // ============================================
    if (document.getElementById('laTablaDeInsumos')) {
        initTablePagination({
            tableId: 'laTablaDeInsumos',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // ============================================
    // ANIMACIÓN DE ENTRADA (feature-cards)
    // ============================================
    const cards = document.querySelectorAll('.feature-card, .admin-form-wrapper');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

    cards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.6s ease';
        observer.observe(card);
    });

    // ============================================
    // VALIDACIÓN DE FORMULARIOS
    // ============================================
    const formsInsumo = document.querySelectorAll('.contact-form');

    formsInsumo.forEach(form => {
        const nombreInput = form.querySelector('input[name="NombreInsumo"]');

        if (nombreInput) {
            nombreInput.addEventListener('input', function () {
                if (this.value.length > 100) {
                    this.value = this.value.substring(0, 100);
                }
            });
        }
    });

});

// ============================================
// ANIMACIONES AL CARGAR
// ============================================
function animateOnLoad() {
    const statCards = document.querySelectorAll('[style*="linear-gradient(135deg,"]');
    statCards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px) scale(0.9)';
        setTimeout(() => {
            card.style.transition = 'all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0) scale(1)';
        }, index * 100);
    });
}

// ============================================
// EFECTOS HOVER EN TABLA
// ============================================
function initializeTableHoverEffects() {
    const tables = document.querySelectorAll('table');
    tables.forEach(table => {
        const tbody = table.querySelector('tbody');
        if (!tbody) return;
        tbody.querySelectorAll('tr').forEach(row => {
            row.addEventListener('mouseenter', function () {
                this.style.background = 'linear-gradient(90deg, rgba(143, 142, 106, 0.05) 0%, transparent 100%)';
                this.style.transform = 'translateX(5px)';
                this.style.boxShadow = '0 4px 12px rgba(0,0,0,0.08)';
            });
            row.addEventListener('mouseleave', function () {
                this.style.background = '';
                this.style.transform = '';
                this.style.boxShadow = '';
            });
        });
    });
}



// ============================================
// MODALES
// ============================================
function initializeModals() {
    // Modal de detalles de insumo
    document.querySelectorAll('.btn-detalles').forEach(btn => {
        btn.addEventListener('click', function () {
            const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };

            set('detalles-id-insumo', this.dataset.id);
            set('detalles-nombre-insumo', this.dataset.nombre);
            set('detalles-categoria-insumo', this.dataset.categoria);

            const estadoBadge = document.getElementById('detalles-estado-badge-insumo');
            if (estadoBadge) {
                const estado = this.dataset.estado;
                const config = {
                    'Activo': { bg: 'var(--green)' },
                    'Inactivo': { bg: 'var(--red)' }
                };
                const c = config[estado] || { bg: 'var(--charcoal)' };
                estadoBadge.textContent = estado;
                estadoBadge.style.background = c.bg;
                estadoBadge.style.color = 'white';
            }
        });
    });
}

// ============================================
// PAGINACIÓN PANEL STOCK BAJO
// ============================================
function initStockBajoPagination() {
    const items = document.querySelectorAll('.stock-alerta-item');
    if (!items.length) return;

    const itemsPerPage = 6;
    let currentPage = 0;
    const totalPages = Math.ceil(items.length / itemsPerPage);

    const btnAnterior = document.getElementById('btnStockAnterior');
    const btnSiguiente = document.getElementById('btnStockSiguiente');
    const elStart = document.getElementById('stockStart');
    const elEnd = document.getElementById('stockEnd');
    const elTotal = document.getElementById('stockTotal');

    function render() {
        const from = currentPage * itemsPerPage;
        const to = Math.min(from + itemsPerPage, items.length);

        items.forEach((item, i) => {
            item.style.display = (i >= from && i < to) ? '' : 'none';
        });

        if (elStart) elStart.textContent = from + 1;
        if (elEnd) elEnd.textContent = to;
        if (elTotal) elTotal.textContent = items.length;

        if (btnAnterior) btnAnterior.disabled = currentPage === 0;
        if (btnSiguiente) btnSiguiente.disabled = currentPage >= totalPages - 1;
    }

    if (btnAnterior) btnAnterior.addEventListener('click', () => { if (currentPage > 0) { currentPage--; render(); } });
    if (btnSiguiente) btnSiguiente.addEventListener('click', () => { if (currentPage < totalPages - 1) { currentPage++; render(); } });

    render();
}

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = { success: 'fa-check-circle', error: 'fa-exclamation-triangle', warning: 'fa-exclamation-triangle', info: 'fa-info-circle' };
    const colores = { success: 'var(--green)', error: 'var(--red)', warning: 'var(--yellow)', info: 'var(--gold)' };

    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed; top: 100px; right: 20px;
        background: ${colores[tipo]}; color: white;
        padding: 1.5rem 2rem; border-radius: 10px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001; animation: slideInNotif 0.3s ease;
        font-weight: 600; display: flex; align-items: center;
        gap: 1rem; min-width: 300px;
    `;
    notification.innerHTML = `<i class="fas ${iconos[tipo]}" style="font-size:1.5rem;"></i><span>${mensaje}</span>`;
    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOutNotif 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// ============================================
// ESTILOS DE ANIMACIÓN
// ============================================
if (!document.getElementById('insumo-styles')) {
    const s = document.createElement('style');
    s.id = 'insumo-styles';
    s.innerHTML = `
        @keyframes slideInNotif  { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
        @keyframes slideOutNotif { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
        @keyframes tooltipFadeIn  { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes tooltipFadeOut { from { opacity: 1; transform: translateY(0); } to { opacity: 0; transform: translateY(10px); } }
    `;
    document.head.appendChild(s);
}