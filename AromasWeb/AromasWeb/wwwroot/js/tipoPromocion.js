// ============================================
// SCRIPT PARA TIPOS DE PROMOCIONES
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    console.log('%c✓ Script de tipos de promociones cargado', 'color: var(--green); font-weight: bold;');

    // Inicializar eventos de detalles
    inicializarModales();
});

// ============================================
// MODALES
// ============================================
function inicializarModales() {
    // Modal de detalles
    const botonesDetalles = document.querySelectorAll('.btn-detalles-tipo');
    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const descripcion = this.getAttribute('data-descripcion');
            const estado = this.getAttribute('data-estado');

            document.getElementById('detalles-id-tipo').textContent = id;
            document.getElementById('detalles-nombre-tipo').textContent = nombre;
            document.getElementById('detalles-descripcion-tipo').textContent = descripcion || 'Sin descripción';

            const estadoBadge = document.getElementById('detalles-estado-badge-tipo');
            estadoBadge.textContent = estado;
            estadoBadge.style.background = estado === 'Activo' ? 'var(--green)' : 'var(--red)';
        });
    });

    // Modal de eliminar
    const botonesEliminar = document.querySelectorAll('.btn-eliminar-tipo');
    botonesEliminar.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const descripcion = this.getAttribute('data-descripcion');
            const estado = this.getAttribute('data-estado');

            document.getElementById('eliminar-id-display-tipo').textContent = id;
            document.getElementById('eliminar-id-tipo').value = id;
            document.getElementById('eliminar-nombre-tipo').textContent = nombre;
            document.getElementById('eliminar-descripcion-tipo').textContent = descripcion || 'Sin descripción';

            const estadoBadge = document.getElementById('eliminar-estado-badge-tipo');
            estadoBadge.textContent = estado;
            estadoBadge.style.background = estado === 'Activo' ? 'var(--green)' : 'var(--red)';

            const form = document.getElementById('formEliminarTipo');
            form.action = '/TipoPromocion/EliminarTipoPromocion/' + id;
        });
    });
}

// ============================================
// PAGINACIÓN
// ============================================
function paginaAnterior() {
    console.log('Página anterior');
}

function paginaSiguiente() {
    console.log('Página siguiente');
}