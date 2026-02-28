// permiso.js - Gestión de permisos
document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN
    // ============================================
    if (document.getElementById('laTablaDePermisos')) {
        initTablePagination({
            tableId: 'laTablaDePermisos',
            recordsPerPage: 10,
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
    const tabla = document.getElementById('laTablaDePermisos');
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
    // MODAL DETALLES PERMISO
    // ============================================
    document.querySelectorAll('.btn-detalles-permiso').forEach(btn => {
        btn.addEventListener('click', function () {
            const elId = document.getElementById('detalles-id-permiso');
            const elNombre = document.getElementById('detalles-nombre-permiso');
            const elModulo = document.getElementById('detalles-modulo-permiso');
            if (elId) elId.textContent = '#' + this.dataset.id;
            if (elNombre) elNombre.textContent = this.dataset.nombre;
            if (elModulo) elModulo.textContent = this.dataset.modulo;
        });
    });

    // ============================================
    // RESALTAR ITEMS SELECCIONADOS EN ASIGNAR PERMISOS
    // ============================================
    document.querySelectorAll('.permiso-checkbox').forEach(cb => {
        // Aplicar estilo inicial si ya está checked (al cargar la página)
        actualizarEstiloItem(cb);
        cb.addEventListener('change', function () {
            actualizarEstiloItem(this);
        });
    });

    function actualizarEstiloItem(checkbox) {
        const label = checkbox.closest('.permiso-item');
        if (!label) return;
        if (checkbox.checked) {
            label.style.background = 'rgba(32, 116, 118, 0.08)';
            label.style.borderColor = 'var(--olive-green)';
        } else {
            label.style.background = '';
            label.style.borderColor = 'transparent';
        }
    }
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