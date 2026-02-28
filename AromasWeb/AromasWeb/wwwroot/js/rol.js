// rol.js - Gestión de roles
document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN
    // ============================================
    if (document.getElementById('laTablaDeRoles')) {
        initTablePagination({
            tableId: 'laTablaDeRoles',
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
    const tabla = document.getElementById('laTablaDeRoles');
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
    // MODAL DETALLES ROL
    // ============================================
    document.querySelectorAll('.btn-detalles-rol').forEach(btn => {
        btn.addEventListener('click', function () {
            const set = (id, val) => {
                const el = document.getElementById(id);
                if (el) el.textContent = val || '—';
            };

            set('detalles-id-rol', '#' + this.dataset.id);
            set('detalles-nombre-rol', this.dataset.nombre);
            set('detalles-descripcion-rol', this.dataset.descripcion);

            const badge = document.getElementById('detalles-estado-badge-rol');
            if (badge) {
                const estado = this.dataset.estado;
                badge.textContent = estado;
                badge.style.background = estado === 'Activo' ? 'var(--green)' : 'var(--red)';
                badge.style.color = 'white';
            }
        });
    });

    // ============================================
    // MODAL ELIMINAR ROL
    // ============================================
    document.querySelectorAll('.btn-eliminar-rol').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.dataset.id;

            const set = (elId, val) => {
                const el = document.getElementById(elId);
                if (el) el.textContent = val || '—';
            };

            set('eliminar-id-display-rol', '#' + id);
            set('eliminar-nombre-rol', this.dataset.nombre);
            set('eliminar-descripcion-rol', this.dataset.descripcion);
            set('eliminar-cantidadempleados-rol', this.dataset.cantidadempleados || '0 empleado(s)');

            // Hidden input del ID para el form
            const inputId = document.getElementById('eliminar-id-rol');
            if (inputId) inputId.value = id;

            // Action del form
            const form = document.getElementById('formEliminarRol');
            if (form) form.action = `/Rol/EliminarRol?id=${id}`;

            // Badge estado
            const badge = document.getElementById('eliminar-estado-badge-rol');
            if (badge) {
                const estado = this.dataset.estado;
                badge.textContent = estado;
                badge.style.background = estado === 'Activo' ? 'var(--green)' : 'var(--red)';
                badge.style.color = 'white';
            }
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