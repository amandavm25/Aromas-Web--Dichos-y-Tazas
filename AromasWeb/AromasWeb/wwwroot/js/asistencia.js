// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTooltips();
    initializeTableHoverEffects();

    // Inicializar paginación de tabla usando la función general de site.js
    if (document.getElementById('laTablaDeAsistencias')) {
        initTablePagination({
            tableId: 'laTablaDeAsistencias',
            recordsPerPage: 5,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Inicializar paginación de cards/timeline usando la función general de site.js
    if (document.getElementById('historial-timeline')) {
        initCardsPagination({
            containerId: 'historial-timeline',
            cardsPerPage: 3,
            prevButtonId: 'btnAnteriorCards',
            nextButtonId: 'btnSiguienteCards',
            startCardId: 'startCard',
            endCardId: 'endCard',
            totalCardsId: 'totalCards'
        });
    }

    initializeModals();
});

// ============================================
// ANIMACIONES AL CARGAR
// ============================================
function animateOnLoad() {
    const statCards = document.querySelectorAll('[style*="linear-gradient(135deg, #"]');
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
// EFECTOS HOVER MEJORADOS
// ============================================
function initializeTableHoverEffects() {
    const tables = document.querySelectorAll('table');

    tables.forEach(table => {
        const tbody = table.querySelector('tbody');
        if (!tbody) return;

        const rows = tbody.querySelectorAll('tr');

        rows.forEach(row => {
            row.addEventListener('mouseenter', function () {
                this.style.background = 'linear-gradient(90deg, rgba(32, 116, 118, 0.05) 0%, transparent 100%)';
                this.style.transform = 'translateX(5px)';
                this.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.08)';
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
    const elementsWithTitle = document.querySelectorAll('[title]');

    elementsWithTitle.forEach(element => {
        const title = element.getAttribute('title');
        element.removeAttribute('title');
        element.setAttribute('data-tooltip', title);

        element.addEventListener('mouseenter', function (e) {
            showTooltip(e, title);
        });

        element.addEventListener('mouseleave', function () {
            hideTooltip();
        });
    });
}

let tooltipElement = null;

function showTooltip(event, text) {
    hideTooltip();

    tooltipElement = document.createElement('div');
    tooltipElement.textContent = text;
    tooltipElement.style.cssText = `
        position: fixed;
        background: linear-gradient(135deg, var(--dark-teal), var(--teal));
        color: white;
        padding: 0.7rem 1.2rem;
        border-radius: 12px;
        font-size: 0.9rem;
        z-index: 10000;
        box-shadow: 0 8px 24px rgba(0, 0, 0, 0.2);
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
            if (tooltipElement && tooltipElement.parentNode) {
                tooltipElement.remove();
            }
            tooltipElement = null;
        }, 200);
    }
}

// ============================================
// MODALES
// ============================================
function initializeModals() {
    const botonesDetalles = document.querySelectorAll('.btn-detalles-asistencia');

    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            const nombre = this.getAttribute('data-nombre');
            const identificacion = this.getAttribute('data-identificacion');
            const cargo = this.getAttribute('data-cargo');
            const fecha = this.getAttribute('data-fecha');
            const dia = this.getAttribute('data-dia');
            const entrada = this.getAttribute('data-entrada');
            const salida = this.getAttribute('data-salida');
            const almuerzo = this.getAttribute('data-almuerzo');
            const horasTotales = this.getAttribute('data-horastotales');
            const horasRegulares = this.getAttribute('data-horasregulares');
            const horasExtras = this.getAttribute('data-horasextras');
            const estado = this.getAttribute('data-estado');

            const elNombre = document.getElementById('detalles-nombre-empleado');
            const elIdentificacion = document.getElementById('detalles-identificacion-empleado');
            const elCargo = document.getElementById('detalles-cargo-empleado');
            const elFecha = document.getElementById('detalles-fecha-asistencia');
            const elDia = document.getElementById('detalles-dia-asistencia');
            const elEntrada = document.getElementById('detalles-entrada-asistencia');
            const elSalida = document.getElementById('detalles-salida-asistencia');
            const elAlmuerzo = document.getElementById('detalles-almuerzo-asistencia');
            const elHorasTotales = document.getElementById('detalles-horastotales-asistencia');
            const elHorasRegulares = document.getElementById('detalles-horasregulares-asistencia');
            const elHorasExtras = document.getElementById('detalles-horasextras-asistencia');

            if (elNombre) elNombre.textContent = nombre;
            if (elIdentificacion) elIdentificacion.textContent = identificacion;
            if (elCargo) elCargo.textContent = cargo;
            if (elFecha) elFecha.textContent = fecha;
            if (elDia) elDia.textContent = dia;
            if (elEntrada) elEntrada.textContent = entrada;
            if (elSalida) elSalida.textContent = salida;
            if (elAlmuerzo) elAlmuerzo.textContent = almuerzo;
            if (elHorasTotales) elHorasTotales.textContent = horasTotales;
            if (elHorasRegulares) elHorasRegulares.textContent = horasRegulares + 'h';
            if (elHorasExtras) elHorasExtras.textContent = horasExtras + 'h';

            const estadoBadge = document.getElementById('detalles-estado-badge-asistencia');
            if (estadoBadge) {
                if (estado === 'Completo') {
                    estadoBadge.textContent = 'Completo';
                    estadoBadge.style.background = 'var(--green)';
                    estadoBadge.style.color = 'white';
                } else {
                    estadoBadge.textContent = 'En curso';
                    estadoBadge.style.background = 'var(--amber)';
                    estadoBadge.style.color = 'white';
                }
            }

            const horasExtrasSection = document.getElementById('detalles-horasextras-section');
            if (horasExtrasSection) {
                if (parseFloat(horasExtras) > 0) {
                    horasExtrasSection.style.display = 'flex';
                } else {
                    horasExtrasSection.style.display = 'none';
                }
            }
        });
    });
}

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = {
        'success': 'fa-check-circle',
        'error': 'fa-exclamation-triangle',
        'warning': 'fa-exclamation-triangle',
        'info': 'fa-info-circle'
    };

    const colores = {
        'success': 'var(--green)',
        'error': 'var(--red)',
        'warning': '#f39c12',
        'info': '#3498db'
    };

    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 100px;
        right: 20px;
        background: ${colores[tipo]};
        color: white;
        padding: 1.5rem 2rem;
        border-radius: 10px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001;
        animation: slideInNotif 0.3s ease;
        font-weight: 600;
        display: flex;
        align-items: center;
        gap: 1rem;
        min-width: 300px;
    `;

    notification.innerHTML = `
        <i class="fas ${iconos[tipo]}" style="font-size: 1.5rem;"></i>
        <span>${mensaje}</span>
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOutNotif 0.3s ease';
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 3000);
}

// Agregar estilos de animación solo si no existen
if (!document.getElementById('asistencia-styles')) {
    const asistenciaStyles = document.createElement('style');
    asistenciaStyles.id = 'asistencia-styles';
    asistenciaStyles.innerHTML = `
        @keyframes slideInNotif {
            from {
                transform: translateX(400px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        @keyframes slideOutNotif {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(400px);
                opacity: 0;
            }
        }
        @keyframes tooltipFadeIn {
            from {
                opacity: 0;
                transform: translateY(10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        @keyframes tooltipFadeOut {
            from {
                opacity: 1;
                transform: translateY(0);
            }
            to {
                opacity: 0;
                transform: translateY(10px);
            }
        }
    `;
    document.head.appendChild(asistenciaStyles);
}