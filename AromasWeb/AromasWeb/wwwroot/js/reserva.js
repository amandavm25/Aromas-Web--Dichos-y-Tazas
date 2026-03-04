// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTooltips();
    initializeTableHoverEffects();
    initializeModals();

    // Paginación de tabla (ListadoReservas, HistorialReservas)
    if (document.getElementById('laTablaDeReservas')) {
        initTablePagination({
            tableId: 'laTablaDeReservas',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    if (document.getElementById('laTablaDeHistorialReservas')) {
        initTablePagination({
            tableId: 'laTablaDeHistorialReservas',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Paginación de cards (MisReservas)
    if (document.getElementById('historial-reservas-timeline')) {
        initCardsPagination({
            containerId: 'historial-reservas-timeline',
            cardsPerPage: 3,
            prevButtonId: 'btnAnteriorCards',
            nextButtonId: 'btnSiguienteCards',
            startCardId: 'startCard',
            endCardId: 'endCard',
            totalCardsId: 'totalCards'
        });
    }
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
// TOOLTIPS PERSONALIZADOS
// ============================================
function initializeTooltips() {
    document.querySelectorAll('[title]').forEach(element => {
        const title = element.getAttribute('title');
        element.removeAttribute('title');
        element.setAttribute('data-tooltip', title);
        element.addEventListener('mouseenter', (e) => showTooltip(e, title));
        element.addEventListener('mouseleave', hideTooltip);
    });
}

let tooltipElement = null;
function showTooltip(event, text) {
    hideTooltip();
    tooltipElement = document.createElement('div');
    tooltipElement.textContent = text;
    tooltipElement.style.cssText = `
        position: fixed;
        background: linear-gradient(135deg, var(--dark-green), var(--olive-green));
        color: white;
        padding: 0.7rem 1.2rem;
        border-radius: 12px;
        font-size: 0.9rem;
        z-index: 10000;
        box-shadow: 0 8px 24px rgba(0,0,0,0.2);
        pointer-events: none;
        white-space: nowrap;
        animation: tooltipFadeIn 0.3s ease;
    `;
    document.body.appendChild(tooltipElement);
    const rect = tooltipElement.getBoundingClientRect();
    tooltipElement.style.left = (event.clientX - rect.width / 2) + 'px';
    tooltipElement.style.top = (event.clientY - rect.height - 10) + 'px';
}

function hideTooltip() {
    if (tooltipElement) {
        tooltipElement.style.animation = 'tooltipFadeOut 0.2s ease';
        setTimeout(() => {
            if (tooltipElement && tooltipElement.parentNode) tooltipElement.remove();
            tooltipElement = null;
        }, 200);
    }
}

// ============================================
// MODALES
// ============================================
function initializeModals() {
    // Modal de detalles
    document.querySelectorAll('.btn-detalles-reserva').forEach(btn => {
        btn.addEventListener('click', function () {
            const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };
            set('detalles-cliente-reserva', this.dataset.cliente);
            set('detalles-telefono-text-reserva', this.dataset.telefono);
            set('detalles-id-reserva', '#' + this.dataset.id);
            set('detalles-fecha-reserva', this.dataset.fecha);
            set('detalles-hora-reserva', this.dataset.hora);
            set('detalles-personas-reserva', this.dataset.personas + ' personas');

            // Badge de estado
            const estadoBadge = document.getElementById('detalles-estado-badge-reserva');
            if (estadoBadge) {
                const estado = this.dataset.estado;
                const config = {
                    'Confirmada': { icon: 'fa-check-circle', bg: 'var(--green)' },
                    'Pendiente': { icon: 'fa-clock', bg: 'var(--yellow)' },
                    'Completada': { icon: 'fa-check-double', bg: 'var(--gold)' },
                    'Cancelada': { icon: 'fa-times-circle', bg: 'var(--red)' }
                };
                const c = config[estado] || { icon: 'fa-question-circle', bg: 'var(--charcoal)' };
                estadoBadge.innerHTML = `<i class="fas ${c.icon}"></i> ${estado}`;
                estadoBadge.className = 'badge';
                estadoBadge.style.cssText = `
                    background: ${c.bg};
                    color: white;
                    font-size: 1rem;
                    padding: 0.6rem 1.5rem;
                    border-radius: 50px;
                    display: inline-flex;
                    align-items: center;
                    gap: 0.5rem;
                    font-weight: 600;
                `;
            }

            // Observaciones
            const obsContainer = document.getElementById('detalles-observaciones-container');
            const obsTexto = document.getElementById('detalles-observaciones-reserva');
            const obs = this.dataset.observaciones;
            if (obsContainer && obsTexto) {
                if (obs && obs.trim() !== '') {
                    obsTexto.textContent = obs;
                    obsContainer.style.display = 'block';
                } else {
                    obsContainer.style.display = 'none';
                }
            }
        });
    });

    // Modal de cancelar
    document.querySelectorAll('.btn-cancelar-reserva').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.dataset.id;
            const set = (id2, val) => { const el = document.getElementById(id2); if (el) el.textContent = val; };
            set('cancelar-cliente-reserva', this.dataset.cliente);
            set('cancelar-fecha-reserva', this.dataset.fecha);
            set('cancelar-hora-reserva', this.dataset.hora);
            set('cancelar-personas-reserva', (this.dataset.personas || '') + ' personas');
            set('cancelar-id-display-reserva', '#' + id);

            const inputId = document.getElementById('cancelar-id-reserva');
            if (inputId) inputId.value = id;

            const form = document.getElementById('formCancelarReserva');
            if (form) form.action = '/Reserva/CancelarReserva/' + id;
        });
    });
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
if (!document.getElementById('reserva-styles')) {
    const s = document.createElement('style');
    s.id = 'reserva-styles';
    s.innerHTML = `
        @keyframes slideInNotif  { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
        @keyframes slideOutNotif { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
        @keyframes tooltipFadeIn  { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes tooltipFadeOut { from { opacity: 1; transform: translateY(0); } to { opacity: 0; transform: translateY(10px); } }
    `;
    document.head.appendChild(s);
}