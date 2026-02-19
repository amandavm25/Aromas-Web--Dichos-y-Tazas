// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTooltips();
    initializeTableHoverEffects();
    initializeModals();
    animateStatistics();

    // Inicializar paginación de tabla usando la función general de site.js
    if (document.getElementById('laTablaDePlanillas')) {
        initTablePagination({
            tableId: 'laTablaDePlanillas',
            recordsPerPage: 5,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Validación de formularios
    initializeFormValidation();
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

    const tableRows = document.querySelectorAll('tbody tr');
    tableRows.forEach((row, index) => {
        row.style.opacity = '0';
        row.style.transform = 'translateX(-20px)';

        setTimeout(() => {
            row.style.transition = 'all 0.4s ease';
            row.style.opacity = '1';
            row.style.transform = 'translateX(0)';
        }, 300 + (index * 50));
    });
}

// ============================================
// CONTADOR ANIMADO PARA ESTADÍSTICAS
// ============================================
function animateStatistics() {
    const statNumbers = document.querySelectorAll('[style*="font-size: 2.5rem"]');

    statNumbers.forEach(stat => {
        const text = stat.textContent;

        if (text.match(/^[0-9]+$/)) {
            const number = parseInt(text);
            animateNumber(stat, 0, number, 1500);
        }
        else if (text.includes('₡')) {
            const cleanNumber = text.replace(/[^\d.-]/g, '');
            const number = parseFloat(cleanNumber);
            if (!isNaN(number)) {
                animateNumberWithCurrency(stat, 0, number, 1500);
            }
        }
    });
}

function animateNumber(element, start, end, duration) {
    const range = end - start;
    const increment = range / (duration / 16);
    let current = start;

    const timer = setInterval(() => {
        current += increment;
        if (current >= end) {
            element.textContent = Math.round(end);
            clearInterval(timer);
        } else {
            element.textContent = Math.round(current);
        }
    }, 16);
}

function animateNumberWithCurrency(element, start, end, duration) {
    const range = end - start;
    const increment = range / (duration / 16);
    let current = start;

    const timer = setInterval(() => {
        current += increment;
        if (current >= end) {
            element.textContent = '₡' + Math.round(end).toLocaleString('es-CR');
            clearInterval(timer);
        } else {
            element.textContent = '₡' + Math.round(current).toLocaleString('es-CR');
        }
    }, 16);
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
                this.style.background = 'linear-gradient(90deg, rgba(143, 142, 106, 0.05) 0%, transparent 100%)';
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
        background: linear-gradient(135deg, var(--dark-green), var(--olive-green));
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
// MODAL PAGAR PLANILLA
// ============================================
function initializeModals() {
    const botonesPagar = document.querySelectorAll('.btn-pagar');

    botonesPagar.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const empleado = this.getAttribute('data-empleado');
            const periodo = this.getAttribute('data-periodo');
            const neto = this.getAttribute('data-monto');

            const elId = document.getElementById('pagar-id');
            const elEmpleado = document.getElementById('pagar-empleado');
            const elPeriodo = document.getElementById('pagar-periodo');
            const elNeto = document.getElementById('pagar-neto');

            if (elId) elId.value = id;
            if (elEmpleado) elEmpleado.textContent = empleado;
            if (elPeriodo) elPeriodo.textContent = periodo;
            if (elNeto) elNeto.textContent = '₡' +  neto;

            const form = document.getElementById('formPagarPlanilla');
            if (form) {
                form.action = '/Planilla/MarcarComoPagado/' + id;
            }
        });
    });
}

// ============================================
// VALIDACIÓN DE FORMULARIOS
// ============================================
function initializeFormValidation() {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const inputs = this.querySelectorAll('input[required], select[required]');
            let isValid = true;

            inputs.forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;
                    input.style.borderColor = 'var(--red)';
                } else {
                    input.style.borderColor = '';
                }
            });

            // Validar fechas en el formulario de cálculo
            const periodoInicio = this.querySelector('input[name="periodoInicio"]');
            const periodoFin = this.querySelector('input[name="periodoFin"]');

            if (periodoInicio && periodoFin) {
                const inicio = new Date(periodoInicio.value);
                const fin = new Date(periodoFin.value);

                if (inicio >= fin) {
                    e.preventDefault();
                    mostrarNotificacion('La fecha de inicio debe ser anterior a la fecha de fin', 'error');
                    periodoInicio.style.borderColor = 'var(--red)';
                    periodoFin.style.borderColor = 'var(--red)';
                    return;
                }
            }

            if (!isValid) {
                e.preventDefault();
                mostrarNotificacion('Por favor, completa todos los campos obligatorios', 'error');
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
if (!document.getElementById('planilla-styles')) {
    const planillaStyles = document.createElement('style');
    planillaStyles.id = 'planilla-styles';
    planillaStyles.innerHTML = `
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
    document.head.appendChild(planillaStyles);
}