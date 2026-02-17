// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTooltips();
    initializeTableHoverEffects();
    initializeModals();

    // Paginación de tabla (ListadoSolicitudes)
    if (document.getElementById('laTablaDeSolicitudes')) {
        initTablePagination({
            tableId: 'laTablaDeSolicitudes',
            recordsPerPage: 5,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Paginación de timeline/cards (MisSolicitudes y VerSolicitudesEmpleado)
    if (document.getElementById('historial-solicitudes-timeline')) {
        initCardsPagination({
            containerId: 'historial-solicitudes-timeline',
            cardsPerPage: 3,
            prevButtonId: 'btnAnteriorCards',
            nextButtonId: 'btnSiguienteCards',
            startCardId: 'startCard',
            endCardId: 'endCard',
            totalCardsId: 'totalCards'
        });
    }

    // Cálculo de días en formularios
    initializeDaysCalculation();
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
    document.querySelectorAll('.btn-detalles-solicitud').forEach(btn => {
        btn.addEventListener('click', function () {
            const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };

            set('detalles-nombre-empleado', this.dataset.empleado);
            set('detalles-identificacion-empleado', this.dataset.identificacion);
            set('detalles-cargo-empleado', this.dataset.cargo);
            set('detalles-fechasolicitud', this.dataset.fechaSolicitud);
            set('detalles-fechainicio', this.dataset.fechaInicio);
            set('detalles-fechafin', this.dataset.fechaFin);
            set('detalles-diassolicitados', this.dataset.diasSolicitados);
            set('detalles-antiguedad', this.dataset.antiguedad);

            // Días disponibles con color
            const elDias = document.getElementById('detalles-diasdisponibles');
            if (elDias) {
                const disponibles = parseFloat(this.dataset.diasDisponibles);
                const solicitados = parseFloat(this.dataset.diasSolicitados);
                elDias.textContent = this.dataset.diasDisponibles;
                elDias.style.color = disponibles >= solicitados ? '#27ae60' : '#e74c3c';
            }

            // Badge de estado
            const estadoBadge = document.getElementById('detalles-estado-badge');
            if (estadoBadge) {
                const estado = this.dataset.estado;
                const colores = { 'Aprobada': '#27ae60', 'Rechazada': '#e74c3c', 'Pendiente': '#f39c12' };
                estadoBadge.textContent = estado;
                estadoBadge.style.background = colores[estado] || '#95a5a6';
                estadoBadge.style.color = 'white';
            }
        });
    });

    // Modal de aprobar
    document.querySelectorAll('.btn-aprobar-solicitud').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.dataset.id;
            const set = (id2, val) => { const el = document.getElementById(id2); if (el) el.textContent = val; };

            set('aprobar-nombre-empleado', this.dataset.empleado);
            set('aprobar-periodo', this.dataset.periodo);
            set('aprobar-diassolicitados', this.dataset.diasSolicitados + ' días laborables');

            const inputId = document.getElementById('aprobar-id-solicitud');
            if (inputId) inputId.value = id;

            const form = document.getElementById('formAprobarSolicitud');
            if (form) form.action = '/SolicitudVacaciones/AprobarSolicitud/' + id;
        });
    });

    // Modal de rechazar
    document.querySelectorAll('.btn-rechazar-solicitud').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.dataset.id;
            const set = (id2, val) => { const el = document.getElementById(id2); if (el) el.textContent = val; };

            set('rechazar-nombre-empleado', this.dataset.empleado);
            set('rechazar-periodo', this.dataset.periodo);

            const inputId = document.getElementById('rechazar-id-solicitud');
            if (inputId) inputId.value = id;

            const form = document.getElementById('formRechazarSolicitud');
            if (form) form.action = '/SolicitudVacaciones/RechazarSolicitud/' + id;
        });
    });

    // Modal de eliminar
    document.querySelectorAll('.btn-eliminar-solicitud').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.dataset.id;
            const set = (id2, val) => { const el = document.getElementById(id2); if (el) el.textContent = val; };

            set('eliminar-nombre-empleado', this.dataset.empleado);
            set('eliminar-periodo', this.dataset.periodo);

            const form = document.getElementById('formEliminarSolicitud');
            if (form) form.action = '/SolicitudVacaciones/EliminarSolicitud?id=' + id;
        });
    });
}

// ============================================
// CÁLCULO DE DÍAS LABORABLES
// ============================================
function initializeDaysCalculation() {
    const fechaInicio = document.getElementById('FechaInicio');
    const fechaFin = document.getElementById('FechaFin');
    const diasCalculadosEl = document.getElementById('diasCalculados');
    const hiddenDias = document.querySelector('input[name="DiasSolicitados"]');

    if (!fechaInicio || !fechaFin || !diasCalculadosEl) return;

    function calcularDias() {
        const inicio = new Date(fechaInicio.value);
        const fin = new Date(fechaFin.value);

        if (!fechaInicio.value || !fechaFin.value || inicio > fin) {
            diasCalculadosEl.textContent = '0';
            if (hiddenDias) hiddenDias.value = '0';
            return;
        }

        let dias = 0;
        const current = new Date(inicio);
        while (current <= fin) {
            if (current.getDay() !== 0) dias++; // excluye domingos
            current.setDate(current.getDate() + 1);
        }

        diasCalculadosEl.textContent = dias;
        if (hiddenDias) hiddenDias.value = dias;
    }

    fechaInicio.addEventListener('change', calcularDias);
    fechaFin.addEventListener('change', calcularDias);

    // Ejecutar al cargar si ya hay valores
    if (fechaInicio.value && fechaFin.value) calcularDias();
}

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = { success: 'fa-check-circle', error: 'fa-exclamation-triangle', warning: 'fa-exclamation-triangle', info: 'fa-info-circle' };
    const colores = { success: '#27ae60', error: '#e74c3c', warning: '#f39c12', info: '#3498db' };

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
if (!document.getElementById('solicitud-styles')) {
    const s = document.createElement('style');
    s.id = 'solicitud-styles';
    s.innerHTML = `
        @keyframes slideInNotif { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
        @keyframes slideOutNotif { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
        @keyframes tooltipFadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes tooltipFadeOut { from { opacity: 1; transform: translateY(0); } to { opacity: 0; transform: translateY(10px); } }
    `;
    document.head.appendChild(s);
}