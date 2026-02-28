// bitacora.js - Gestión de bitácora
document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN
    // ============================================
    if (document.getElementById('laTablaDeBitacoras')) {
        initTablePagination({
            tableId: 'laTablaDeBitacoras',
            recordsPerPage: 15,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // ============================================
    // HOVER EFFECTS EN TABLA
    // ============================================
    const tabla = document.getElementById('laTablaDeBitacoras');
    if (tabla) {
        tabla.querySelectorAll('tbody tr').forEach(fila => {
            fila.addEventListener('mouseenter', function () {
                this.style.background = 'linear-gradient(90deg, rgba(32, 116, 118, 0.05) 0%, transparent 100%)';
                this.style.transform = 'translateX(5px)';
                this.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.08)';
                this.style.transition = 'all 0.2s ease';
            });
            fila.addEventListener('mouseleave', function () {
                this.style.background = '';
                this.style.transform = '';
                this.style.boxShadow = '';
            });
        });
    }

    // ============================================
    // ANIMACIÓN DE ENTRADA
    // ============================================
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

    document.querySelectorAll('.feature-card, .admin-form-wrapper').forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.6s ease';
        observer.observe(card);
    });

    // ============================================
    // MODAL DETALLES BITÁCORA
    // ============================================
    document.querySelectorAll('.btn-detalles').forEach(btn => {
        btn.addEventListener('click', function () {
            const set = (id, val) => {
                const el = document.getElementById(id);
                if (el) el.textContent = val || '—';
            };

            set('detalles-id-bitacora', '#' + this.dataset.id);
            set('detalles-accion-bitacora', this.dataset.accion);
            set('detalles-descripcion-bitacora', this.dataset.descripcion);
            set('detalles-empleado-bitacora', this.dataset.empleado);
            set('detalles-fecha-bitacora', this.dataset.fecha);
            set('detalles-tabla-bitacora', this.dataset.tabla);
            set('detalles-tipo-accion-bitacora', this.dataset.accion);

            const elModulo = document.getElementById('detalles-modulo-badge-bitacora');
            if (elModulo) elModulo.textContent = this.dataset.modulo || '—';

            const formatearJSON = (texto) => {
                if (!texto || texto.trim() === '') return 'Sin datos';
                try { return JSON.stringify(JSON.parse(texto), null, 2); }
                catch { return texto; }
            };

            const elAntes = document.getElementById('detalles-datos-anteriores-bitacora');
            const elDespues = document.getElementById('detalles-datos-nuevos-bitacora');
            if (elAntes) elAntes.textContent = formatearJSON(this.dataset.datosAnteriores);
            if (elDespues) elDespues.textContent = formatearJSON(this.dataset.datosNuevos);
        });
    });
});

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = { 'success': 'fa-check-circle', 'error': 'fa-exclamation-triangle', 'warning': 'fa-exclamation-triangle', 'info': 'fa-info-circle' };
    const colores = { 'success': 'var(--green)', 'error': 'var(--red)', 'warning': 'var(--yellow)', 'info': 'var(--gold)' };
    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed; top: 100px; right: 20px;
        background: ${colores[tipo]}; color: white;
        padding: 1.5rem 2rem; border-radius: 10px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001; animation: slideIn 0.3s ease;
        font-weight: 600; display: flex; align-items: center; gap: 1rem; min-width: 300px;
    `;
    notification.innerHTML = `<i class="fas ${iconos[tipo]}" style="font-size: 1.5rem;"></i><span>${mensaje}</span>`;
    document.body.appendChild(notification);
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

const notifStyle = document.createElement('style');
notifStyle.innerHTML = `
    @keyframes slideIn { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
    @keyframes slideOut { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
`;
document.head.appendChild(notifStyle);