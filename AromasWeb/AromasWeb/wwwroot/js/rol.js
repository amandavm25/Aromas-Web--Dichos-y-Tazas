// ============================================
// PAGINACIÓN DE TABLA DE ROLES
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    const tabla = document.getElementById('laTablaDeRoles');
    if (!tabla) return;

    const tbody = tabla.querySelector('tbody');
    const filas = Array.from(tbody.querySelectorAll('tr'));
    const btnAnterior = document.getElementById('btnAnterior');
    const btnSiguiente = document.getElementById('btnSiguiente');
    const startRecord = document.getElementById('startRecord');
    const endRecord = document.getElementById('endRecord');
    const totalRecords = document.getElementById('totalRecords');

    const registrosPorPagina = 10;
    let paginaActual = 1;

    // Filtrar solo filas válidas (que no sean el mensaje de "No se encontraron")
    const filasValidas = filas.filter(fila => fila.querySelectorAll('td').length > 1);
    const totalPaginas = Math.ceil(filasValidas.length / registrosPorPagina);

    function mostrarPagina(pagina) {
        const inicio = (pagina - 1) * registrosPorPagina;
        const fin = inicio + registrosPorPagina;

        // Ocultar todas las filas
        filasValidas.forEach(fila => fila.style.display = 'none');

        // Mostrar solo las filas de la página actual
        filasValidas.slice(inicio, fin).forEach(fila => fila.style.display = '');

        // Actualizar contador
        if (filasValidas.length > 0) {
            startRecord.textContent = inicio + 1;
            endRecord.textContent = Math.min(fin, filasValidas.length);
            totalRecords.textContent = filasValidas.length;
        } else {
            startRecord.textContent = '0';
            endRecord.textContent = '0';
            totalRecords.textContent = '0';
        }

        // Actualizar estado de botones
        btnAnterior.disabled = pagina === 1;
        btnSiguiente.disabled = pagina === totalPaginas || totalPaginas === 0;
    }

    // Event listeners
    if (btnAnterior) {
        btnAnterior.addEventListener('click', () => {
            if (paginaActual > 1) {
                paginaActual--;
                mostrarPagina(paginaActual);
            }
        });
    }

    if (btnSiguiente) {
        btnSiguiente.addEventListener('click', () => {
            if (paginaActual < totalPaginas) {
                paginaActual++;
                mostrarPagina(paginaActual);
            }
        });
    }

    // Mostrar primera página al cargar
    mostrarPagina(1);
});

// ============================================
// MODAL DE DETALLES DEL ROL
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.btn-detalles-rol').forEach(btn => {
        btn.addEventListener('click', function () {
            document.getElementById('detalles-id-rol').textContent = '#' + this.dataset.id;
            document.getElementById('detalles-nombre-rol').textContent = this.dataset.nombre;
            document.getElementById('detalles-descripcion-rol').textContent = this.dataset.descripcion || 'Sin descripción';

            const badge = document.getElementById('detalles-estado-badge-rol');
            const estado = this.dataset.estado;
            badge.textContent = estado;
            badge.style.background = estado === 'Activo' ? 'var(--green)' : 'var(--red)';
            badge.style.color = 'white';
        });
    });
});