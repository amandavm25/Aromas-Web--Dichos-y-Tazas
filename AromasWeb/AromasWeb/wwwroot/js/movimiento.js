// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTooltips();
    initializeTableHoverEffects();

    // Paginación de tabla (HistorialMovimientos)
    if (document.getElementById('laTablaDeMovimientos')) {
        initTablePagination({
            tableId: 'laTablaDeMovimientos',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Inicializar formulario de movimiento (RegistrarMovimiento)
    if (document.getElementById('cantidadMovimiento')) {
        initializeMovimientoForm();
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
// FORMULARIO DE MOVIMIENTO
// ============================================
function initializeMovimientoForm() {
    // cantidadActual, costoUnitario y unidad son inyectados por la vista en una etiqueta <script>
    const inputCantidad = document.getElementById('cantidadMovimiento');
    const nuevaCantidadSpan = document.getElementById('nuevaCantidad');
    const alertaStock = document.getElementById('alertaStockInsuficiente');
    const btnRegistrar = document.getElementById('btnRegistrar');
    const resumenCosto = document.getElementById('resumenCosto');
    const cantidadResumen = document.getElementById('cantidadResumen');
    const costoTotalSpan = document.getElementById('costoTotal');

    const radioEntrada = document.querySelector('input[value="E"]');
    const radioSalida = document.querySelector('input[value="S"]');
    const entradaLabel = document.getElementById('entradaLabel');
    const salidaLabel = document.getElementById('salidaLabel');

    if (!inputCantidad || !radioEntrada || !radioSalida) return;

    // Valores del insumo definidos desde la vista
    const stockActual = typeof cantidadActual !== 'undefined' ? cantidadActual : 0;
    const costo = typeof costoUnitario !== 'undefined' ? costoUnitario : 0;
    const ud = typeof unidad !== 'undefined' ? unidad : '';

    function actualizarEstilosRadio() {
        if (radioEntrada.checked) {
            entradaLabel.style.borderColor = 'var(--green)';
            entradaLabel.style.background = 'rgba(39, 174, 96, 0.05)';
            salidaLabel.style.borderColor = 'var(--cream)';
            salidaLabel.style.background = '';
            if (resumenCosto) resumenCosto.style.display = 'block';
        } else {
            salidaLabel.style.borderColor = 'var(--red)';
            salidaLabel.style.background = 'rgba(231, 76, 60, 0.05)';
            entradaLabel.style.borderColor = 'var(--cream)';
            entradaLabel.style.background = '';
            if (resumenCosto) resumenCosto.style.display = 'none';
        }
    }

    function calcularNuevaCantidad() {
        const cantidad = parseFloat(inputCantidad.value) || 0;

        if (radioEntrada.checked) {
            const nueva = stockActual + cantidad;
            if (nuevaCantidadSpan) nuevaCantidadSpan.textContent = nueva.toFixed(2) + ' ' + ud;
            if (alertaStock) alertaStock.style.display = 'none';
            if (btnRegistrar) btnRegistrar.disabled = false;

            // Resumen de costo
            if (cantidadResumen) cantidadResumen.textContent = cantidad.toFixed(2) + ' ' + ud;
            if (costoTotalSpan) {
                const total = costo * cantidad;
                costoTotalSpan.textContent = '₡' + total.toLocaleString('es-CR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }
        } else {
            const nueva = stockActual - cantidad;
            if (nuevaCantidadSpan) nuevaCantidadSpan.textContent = nueva.toFixed(2) + ' ' + ud;

            if (nueva < 0) {
                if (alertaStock) alertaStock.style.display = 'block';
                if (btnRegistrar) btnRegistrar.disabled = true;
            } else {
                if (alertaStock) alertaStock.style.display = 'none';
                if (btnRegistrar) btnRegistrar.disabled = false;
            }
        }
    }

    radioEntrada.addEventListener('change', function () {
        actualizarEstilosRadio();
        calcularNuevaCantidad();
    });

    radioSalida.addEventListener('change', function () {
        actualizarEstilosRadio();
        calcularNuevaCantidad();
    });

    inputCantidad.addEventListener('input', calcularNuevaCantidad);

    // Inicializar estado
    actualizarEstilosRadio();
    calcularNuevaCantidad();
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
if (!document.getElementById('movimiento-styles')) {
    const s = document.createElement('style');
    s.id = 'movimiento-styles';
    s.innerHTML = `
        @keyframes slideInNotif { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
        @keyframes slideOutNotif { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
        @keyframes tooltipFadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes tooltipFadeOut { from { opacity: 1; transform: translateY(0); } to { opacity: 0; transform: translateY(10px); } }
    `;
    document.head.appendChild(s);
}