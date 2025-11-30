// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    // Animar elementos al cargar
    animateOnLoad();

    // Inicializar tooltips
    initializeTooltips();

    // Efectos hover en filas
    initializeTableHoverEffects();

    // Inicializar modales
    initializeModals();

    // Contador animado para estadísticas
    animateStatistics();
});

// ============================================
// ANIMACIONES AL CARGAR
// ============================================
function animateOnLoad() {
    // Animar cards de estadísticas
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

    // Animar tabla
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

        // Si es un número (sin símbolos de moneda ni porcentaje)
        if (text.match(/^[0-9.]+$/)) {
            const number = parseFloat(text);
            animateNumber(stat, 0, number, 1500);
        }
        // Si es moneda
        else if (text.includes('₡')) {
            const number = parseFloat(text.replace(/[^0-9.]/g, ''));
            if (!isNaN(number)) {
                animateNumberWithCurrency(stat, 0, number, 1500);
            }
        }
        // Si es porcentaje
        else if (text.includes('%')) {
            const number = parseFloat(text.replace(/[^0-9.-]/g, ''));
            if (!isNaN(number)) {
                animateNumberWithPercentage(stat, 0, number, 1500);
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
            element.textContent = '₡' + end.toLocaleString('es-CR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            clearInterval(timer);
        } else {
            element.textContent = '₡' + current.toLocaleString('es-CR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
    }, 16);
}

function animateNumberWithPercentage(element, start, end, duration) {
    const range = end - start;
    const increment = range / (duration / 16);
    let current = start;

    const timer = setInterval(() => {
        current += increment;
        if (current >= end) {
            element.textContent = end.toFixed(1) + '%';
            clearInterval(timer);
        } else {
            element.textContent = current.toFixed(1) + '%';
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

// Agregar animaciones de tooltip
const tooltipStyles = document.createElement('style');
tooltipStyles.innerHTML = `
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
document.head.appendChild(tooltipStyles);

// ============================================
// MODAL DETALLES TARIFA
// ============================================
function initializeModals() {
    const botonesDetalles = document.querySelectorAll('.btn-detalles');

    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const empleado = this.getAttribute('data-empleado');
            const cargo = this.getAttribute('data-cargo');
            const tarifa = this.getAttribute('data-tarifa');
            const salario = this.getAttribute('data-salario');
            const inicio = this.getAttribute('data-inicio');
            const fin = this.getAttribute('data-fin');
            const estado = this.getAttribute('data-estado');
            const motivo = this.getAttribute('data-motivo');
            const dias = this.getAttribute('data-dias');
            const registro = this.getAttribute('data-registro');

            // Llenar el modal con los datos
            document.getElementById('detalles-id-tarifa').textContent = id;
            document.getElementById('detalles-empleado-tarifa').textContent = empleado;
            document.getElementById('detalles-cargo-tarifa').textContent = cargo;
            document.getElementById('detalles-tarifa-hora').textContent = '₡' + tarifa;
            document.getElementById('detalles-salario-mensual').textContent = '₡' + salario;
            document.getElementById('detalles-fecha-inicio').textContent = inicio;
            document.getElementById('detalles-fecha-fin').textContent = fin;
            document.getElementById('detalles-dias-vigencia').textContent = dias + ' días';
            document.getElementById('detalles-motivo-tarifa').textContent = motivo;
            document.getElementById('detalles-fecha-registro').textContent = registro;

            // Estado badge
            const estadoBadge = document.getElementById('detalles-estado-badge-tarifa');

            let colorEstado, iconoEstado;

            switch (estado) {
                case 'Vigente':
                    colorEstado = '#27ae60';
                    iconoEstado = 'fa-check-circle';
                    break;
                case 'Vencida':
                    colorEstado = '#e74c3c';
                    iconoEstado = 'fa-times-circle';
                    break;
                case 'Futura':
                    colorEstado = '#3498db';
                    iconoEstado = 'fa-clock';
                    break;
                default:
                    colorEstado = '#95a5a6';
                    iconoEstado = 'fa-question-circle';
            }

            estadoBadge.innerHTML = `<i class="fas ${iconoEstado}"></i> ${estado}`;
            estadoBadge.style.background = colorEstado;
            estadoBadge.style.color = 'white';
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
        'success': '#27ae60',
        'error': '#e74c3c',
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
        animation: slideIn 0.3s ease;
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
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 3000);
}

// Animaciones para notificaciones
const notifStyle = document.createElement('style');
notifStyle.innerHTML = `
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
document.head.appendChild(notifStyle);

// ============================================
// VALIDACIÓN DE FORMULARIOS
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const inputs = this.querySelectorAll('input[required], select[required], textarea[required]');
            let isValid = true;

            inputs.forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;
                    input.style.borderColor = '#e74c3c';
                } else {
                    input.style.borderColor = 'var(--secondary-color)';
                }
            });

            if (!isValid) {
                e.preventDefault();
                mostrarNotificacion('Por favor, completa todos los campos obligatorios', 'error');
            }
        });
    });
});

console.log('%c✓ Script de historial de tarifas cargado correctamente', 'color: #27ae60; font-weight: bold;');