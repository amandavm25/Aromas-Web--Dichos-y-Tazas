// permiso.js - Gestión de permisos
document.addEventListener('DOMContentLoaded', function () {

    animarFilas();
    initializeTooltips();
    initializeTableHoverEffects();
    initializeModalDetalles();
    inicializarCheckboxes();

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

});

// ============================================
// ANIMACIÓN DE FILAS AL CARGAR
// ============================================
function animarFilas() {
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
// HOVER EN TABLA
// ============================================
function initializeTableHoverEffects() {
    document.querySelectorAll('table').forEach(table => {
        const tbody = table.querySelector('tbody');
        if (!tbody) return;

        tbody.querySelectorAll('tr').forEach(row => {
            row.addEventListener('mouseenter', function () {
                this.style.background = 'linear-gradient(90deg, rgba(32, 116, 118, 0.05) 0%, transparent 100%)';
                this.style.transform = 'translateX(5px)';
                this.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.08)';
                this.style.transition = 'all 0.2s ease';
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
    let tooltipElement = null;

    document.querySelectorAll('[title]').forEach(element => {
        const title = element.getAttribute('title');
        element.removeAttribute('title');
        element.setAttribute('data-tooltip', title);

        element.addEventListener('mouseenter', function (e) {
            if (tooltipElement) tooltipElement.remove();

            tooltipElement = document.createElement('div');
            tooltipElement.textContent = title;
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
            tooltipElement.style.left = (e.clientX - rect.width / 2) + 'px';
            tooltipElement.style.top = (e.clientY - rect.height - 10) + 'px';
        });

        element.addEventListener('mouseleave', function () {
            if (tooltipElement) {
                tooltipElement.remove();
                tooltipElement = null;
            }
        });
    });
}

// ============================================
// MODAL DETALLES PERMISO — lee todos los campos
// ============================================
function initializeModalDetalles() {
    document.querySelectorAll('.btn-detalles-permiso').forEach(btn => {
        btn.addEventListener('click', function () {
            const elId = document.getElementById('detalles-id-permiso');
            const elNombre = document.getElementById('detalles-nombre-permiso');
            const elModulo = document.getElementById('detalles-modulo-permiso');
            const elDescripcion = document.getElementById('detalles-descripcion-permiso');
            const elEstado = document.getElementById('detalles-estado-badge-permiso');

            if (elId) elId.textContent = '#' + this.dataset.id;
            if (elNombre) elNombre.textContent = this.dataset.nombre;
            if (elModulo) elModulo.textContent = this.dataset.modulo || '—';
            if (elDescripcion) elDescripcion.textContent = this.dataset.descripcion || 'Sin descripción';

            if (elEstado) {
                const activo = this.dataset.estado === 'Activo';
                elEstado.textContent = activo ? 'Activo' : 'Inactivo';
                elEstado.style.background = activo ? 'var(--green)' : 'var(--red)';
                elEstado.style.color = 'white';
            }
        });
    });
}

// ============================================
// CHECKBOXES - ASIGNAR PERMISOS
// ============================================
function inicializarCheckboxes() {
    document.querySelectorAll('.permiso-checkbox').forEach(cb => {
        actualizarEstiloItem(cb);
        cb.addEventListener('change', function () {
            actualizarEstiloItem(this);
        });
    });
}

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
        font-weight: 600; display: flex; align-items: center;
        gap: 1rem; min-width: 300px;
    `;
    notification.innerHTML = `<i class="fas ${iconos[tipo]}" style="font-size:1.5rem;"></i><span>${mensaje}</span>`;
    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}
// slideIn / slideOut ya están definidos en site.js — no se redefinen aquí