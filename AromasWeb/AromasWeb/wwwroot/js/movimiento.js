// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initTableHoverEffects();

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
const tabla = document.getElementById('laTablaDeMovimientos');
if (tabla) {
    const filas = tabla.querySelectorAll('tbody tr');

    filas.forEach(fila => {
        fila.addEventListener('mouseenter', function () {
            this.style.background = 'linear-gradient(90deg, rgba(32, 116, 118, 0.05) 0%, transparent 100%)';
            this.style.transform = 'translateX(5px)';
            this.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.08)';
        });

        fila.addEventListener('mouseleave', function () {
            this.style.background = '';
            this.style.transform = '';
            this.style.boxShadow = '';
        });
    });
}

// ============================================
// FORMULARIO DE MOVIMIENTO
// ============================================
function initializeMovimientoForm() {
    const resumenCosto = document.getElementById('resumenCosto');
    const radioEntrada = document.querySelector('input[value="E"]');
    const radioSalida = document.querySelector('input[value="S"]');
    const entradaLabel = document.getElementById('entradaLabel');
    const salidaLabel = document.getElementById('salidaLabel');

    if (!radioEntrada || !radioSalida) return;

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

    radioEntrada.addEventListener('change', actualizarEstilosRadio);
    radioSalida.addEventListener('change', actualizarEstilosRadio);

    // Solo estilos iniciales, sin tocar el cálculo
    actualizarEstilosRadio();
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

function initTableHoverEffects() {
    const tabla = document.getElementById('laTablaDeMovimientos');
    if (!tabla) return;
    tabla.querySelectorAll('tbody tr').forEach(fila => {
        fila.addEventListener('mouseenter', function () {
            this.style.background = 'linear-gradient(90deg, rgba(32,116,118,0.05) 0%, transparent 100%)';
            this.style.transform = 'translateX(5px)';
        });
        fila.addEventListener('mouseleave', function () {
            this.style.background = '';
            this.style.transform = '';
        });
    });
}