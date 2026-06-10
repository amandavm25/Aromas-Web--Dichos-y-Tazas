// ============================================
// INICIALIZACIÓN
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTableHoverEffects();
    initializeModals();

    // Paginación de tabla (ListadoTarifas)
    if (document.getElementById('laTablaDeTarifas')) {
        initTablePagination({
            tableId: 'laTablaDeTarifas',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Paginación de timeline/cards (VerHistorialEmpleado)
    if (document.getElementById('historial-tarifa-timeline')) {
        initCardsPagination({
            containerId: 'historial-tarifa-timeline',
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
const tabla = document.getElementById('laTablaDeTarifas');
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
// MODALES
// ============================================
function initializeModals() {

    // Modal de detalles
    document.querySelectorAll('.btn-detalles-tarifa').forEach(btn => {
        btn.addEventListener('click', function () {
            const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };

            set('detalles-empleado-tarifa', this.dataset.empleado);
            set('detalles-identificacion-tarifa', this.dataset.identificacion);
            set('detalles-cargo-tarifa', this.dataset.cargo);
            set('detalles-tarifa-hora', '₡' + this.dataset.tarifa);
            set('detalles-salario-mensual', '₡' + this.dataset.salario);
            set('detalles-fecha-inicio', this.dataset.fechaInicio);
            set('detalles-fecha-fin', this.dataset.fechaFin);
            set('detalles-fecha-registro', this.dataset.fechaRegistro);
            set('detalles-dias-vigencia', this.dataset.diasVigencia + ' días');
            set('detalles-motivo-tarifa', this.dataset.motivo);

            // Badge de estado
            const estadoBadge = document.getElementById('detalles-estado-badge-tarifa');
            if (estadoBadge) {
                const estado = this.dataset.estado;
                const colores = { 'Vigente': 'var(--green)', 'Vencida': 'var(--red)', 'Futura': 'var(--gold)' };
                const iconos = { 'Vigente': 'fa-check-circle', 'Vencida': 'fa-times-circle', 'Futura': 'fa-calendar' };
                estadoBadge.innerHTML = `<i class="fas ${iconos[estado] || 'fa-question-circle'}"></i> ${estado}`;
                estadoBadge.style.background = colores[estado] || 'var(--gray)';
                estadoBadge.style.color = 'white';
            }

            // Badge cumple mínimo
            const minimoBadge = document.getElementById('detalles-cumple-minimo');
            if (minimoBadge) {
                const cumple = this.dataset.cumpleMinimo;
                minimoBadge.innerHTML = cumple === 'Sí'
                    ? '<i class="fas fa-shield-alt"></i> Cumple mínimo'
                    : '<i class="fas fa-exclamation-triangle"></i> Bajo mínimo';
                minimoBadge.style.background = cumple === 'Sí' ? 'var(--green)' : 'var(--red)';
                minimoBadge.style.color = 'white';
            }
        });
    });

    // Modal de finalizar
    document.querySelectorAll('.btn-finalizar-tarifa').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.dataset.id;

            const set = (elId, val) => { const el = document.getElementById(elId); if (el) el.textContent = val; };
            set('finalizar-empleado', this.dataset.empleado);
            set('finalizar-tarifa', this.dataset.tarifa);

            const inputId = document.getElementById('finalizar-id-tarifa');
            if (inputId) inputId.value = id;

            // ✅ AGREGAR: sincronizar fecha visible → hidden
            const inputVisible = document.getElementById('finalizar-fecha-fin');
            const inputHidden = document.getElementById('finalizar-fecha-fin-hidden');
            if (inputVisible && inputHidden) {
                inputVisible.value = ''; // limpiar fecha anterior
                inputHidden.value = '';
                inputVisible.oninput = function () {
                    inputHidden.value = this.value;
                };
            }

            const form = document.getElementById('formFinalizarTarifa');
            if (form) form.action = '/HistorialTarifa/FinalizarTarifa';
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
if (!document.getElementById('historial-tarifa-styles')) {
    const s = document.createElement('style');
    s.id = 'historial-tarifa-styles';
    s.innerHTML = `
        @keyframes slideInNotif  { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
        @keyframes slideOutNotif { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
        @keyframes tooltipFadeIn  { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes tooltipFadeOut { from { opacity: 1; transform: translateY(0); } to { opacity: 0; transform: translateY(10px); } }
    `;
    document.head.appendChild(s);
}