// categoriaReceta.js - Gestión de categorías de recetas

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // MODAL DE DETALLES DE CATEGORÍA RECETA
    // ============================================
    const botonesDetalles = document.querySelectorAll('.btn-detalles-categoria');

    botonesDetalles.forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const descripcion = this.getAttribute('data-descripcion');
            const estado = this.getAttribute('data-estado');

            // Llenar el modal con los datos
            document.getElementById('detalles-id-categoria').textContent = id;
            document.getElementById('detalles-nombre-categoria').textContent = nombre;
            document.getElementById('detalles-descripcion-categoria').textContent = descripcion || 'Sin descripción';

            const badgeEstado = document.getElementById('detalles-estado-badge-categoria');
            if (estado === 'Activa') {
                badgeEstado.textContent = 'Activa';
                badgeEstado.style.background = 'var(--green)';
                badgeEstado.style.color = 'white';
            } else {
                badgeEstado.textContent = 'Inactiva';
                badgeEstado.style.background = 'var(--red)';
                badgeEstado.style.color = 'white';
            }
        });
    });

    // ============================================
    // MODAL DE ELIMINAR CATEGORÍA
    // ============================================
    const botonesEliminar = document.querySelectorAll('.btn-eliminar-categoria');

    botonesEliminar.forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const descripcion = this.getAttribute('data-descripcion');
            const estado = this.getAttribute('data-estado');

            // Llenar el modal con los datos
            document.getElementById('eliminar-id-display-categoria').textContent = id;
            document.getElementById('eliminar-nombre-categoria').textContent = nombre;
            document.getElementById('eliminar-descripcion-categoria').textContent = descripcion || 'Sin descripción';

            const badgeEstado = document.getElementById('eliminar-estado-badge-categoria');
            if (estado === 'Activa') {
                badgeEstado.textContent = 'Activa';
                badgeEstado.style.background = 'var(--green)';
                badgeEstado.style.color = 'white';
            } else {
                badgeEstado.textContent = 'Inactiva';
                badgeEstado.style.background = 'var(--red)';
                badgeEstado.style.color = 'white';
            }

            // Configurar el formulario de eliminación
            const formEliminar = document.getElementById('formEliminarCategoria');
            const actionUrl = '/CategoriaReceta/EliminarCategoriaReceta/' + id;
            formEliminar.setAttribute('action', actionUrl);

            document.getElementById('eliminar-id-categoria').value = id;
        });
    });

    // ============================================
    // VALIDACIÓN DE FORMULARIOS
    // ============================================
    const formsCategoria = document.querySelectorAll('.contact-form');

    formsCategoria.forEach(form => {
        const nombreInput = form.querySelector('input[name="Nombre"]');
        const descripcionTextarea = form.querySelector('textarea[name="Descripcion"]');

        if (descripcionTextarea) {
            // Contador de caracteres para la descripción
            const maxChars = 500;

            descripcionTextarea.addEventListener('input', function () {
                const remaining = maxChars - this.value.length;
                const small = this.parentElement.querySelector('small');

                if (remaining < 0) {
                    this.value = this.value.substring(0, maxChars);
                }
            });
        }

        if (nombreInput) {
            // Validación en tiempo real del nombre
            nombreInput.addEventListener('input', function () {
                const maxLength = 100;
                if (this.value.length > maxLength) {
                    this.value = this.value.substring(0, maxLength);
                }
            });
        }
    });

    // ============================================
    // PAGINACIÓN SIMPLE
    // ============================================
    let paginaActual = 1;
    const registrosPorPagina = 10;

    window.paginaAnterior = function () {
        if (paginaActual > 1) {
            paginaActual--;
            actualizarPaginacion();
        }
    };

    window.paginaSiguiente = function () {
        const tabla = document.getElementById('laTablaDeCategoria');
        if (tabla) {
            const tbody = tabla.querySelector('tbody');
            const totalFilas = tbody.querySelectorAll('tr').length;
            const totalPaginas = Math.ceil(totalFilas / registrosPorPagina);

            if (paginaActual < totalPaginas) {
                paginaActual++;
                actualizarPaginacion();
            }
        }
    };

    function actualizarPaginacion() {
        const tabla = document.getElementById('laTablaDeCategoria');
        if (!tabla) return;

        const tbody = tabla.querySelector('tbody');
        const filas = tbody.querySelectorAll('tr');
        const totalFilas = filas.length;

        const inicio = (paginaActual - 1) * registrosPorPagina;
        const fin = inicio + registrosPorPagina;

        // Ocultar/mostrar filas
        filas.forEach((fila, index) => {
            if (index >= inicio && index < fin) {
                fila.style.display = '';
            } else {
                fila.style.display = 'none';
            }
        });

        // Actualizar contadores
        const startRecord = document.getElementById('startRecord');
        const endRecord = document.getElementById('endRecord');
        const totalRecords = document.getElementById('totalRecords');

        if (startRecord && endRecord && totalRecords) {
            startRecord.textContent = totalFilas > 0 ? inicio + 1 : 0;
            endRecord.textContent = Math.min(fin, totalFilas);
            totalRecords.textContent = totalFilas;
        }

        // Habilitar/deshabilitar botones
        const btnAnterior = document.getElementById('btnAnterior');
        const btnSiguiente = document.getElementById('btnSiguiente');

        if (btnAnterior) {
            btnAnterior.disabled = paginaActual === 1;
            btnAnterior.style.opacity = paginaActual === 1 ? '0.5' : '1';
            btnAnterior.style.cursor = paginaActual === 1 ? 'not-allowed' : 'pointer';
        }

        if (btnSiguiente) {
            const totalPaginas = Math.ceil(totalFilas / registrosPorPagina);
            btnSiguiente.disabled = paginaActual >= totalPaginas;
            btnSiguiente.style.opacity = paginaActual >= totalPaginas ? '0.5' : '1';
            btnSiguiente.style.cursor = paginaActual >= totalPaginas ? 'not-allowed' : 'pointer';
        }
    }

    // Inicializar paginación
    actualizarPaginacion();

    // ============================================
    // HOVER EFFECTS EN TABLA
    // ============================================
    const tabla = document.getElementById('laTablaDeCategoria');
    if (tabla) {
        const filas = tabla.querySelectorAll('tbody tr');

        filas.forEach(fila => {
            fila.addEventListener('mouseenter', function () {
                this.style.background = 'rgba(32, 116, 118, 0.05)';
                this.style.transform = 'translateX(5px)';
            });

            fila.addEventListener('mouseleave', function () {
                this.style.background = '';
                this.style.transform = '';
            });
        });
    }

    // ============================================
    // ANIMACIÓN DE ENTRADA
    // ============================================
    const cards = document.querySelectorAll('.feature-card, .contact-form-wrapper');

    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    cards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.6s ease';
        observer.observe(card);
    });

    // ============================================
    // MENSAJE DE CONFIRMACIÓN
    // ============================================
    const tempDataMensaje = document.querySelector('[data-temp-mensaje]');
    if (tempDataMensaje) {
        const mensaje = tempDataMensaje.getAttribute('data-temp-mensaje');
        if (mensaje) {
            mostrarNotificacion(mensaje, 'success');
        }
    }

    function mostrarNotificacion(mensaje, tipo) {
        const notificacion = document.createElement('div');
        notificacion.style.position = 'fixed';
        notificacion.style.top = '20px';
        notificacion.style.right = '20px';
        notificacion.style.padding = '1rem 1.5rem';
        notificacion.style.borderRadius = '10px';
        notificacion.style.boxShadow = '0 4px 12px rgba(0,0,0,0.15)';
        notificacion.style.zIndex = '9999';
        notificacion.style.animation = 'slideInRight 0.4s ease';
        notificacion.style.fontWeight = '600';

        if (tipo === 'success') {
            notificacion.style.background = '#var(--green)';
            notificacion.style.color = 'white';
            notificacion.innerHTML = '<i class="fas fa-check-circle"></i> ' + mensaje;
        } else {
            notificacion.style.background = 'var(--red)';
            notificacion.style.color = 'white';
            notificacion.innerHTML = '<i class="fas fa-exclamation-circle"></i> ' + mensaje;
        }

        document.body.appendChild(notificacion);

        setTimeout(() => {
            notificacion.style.animation = 'slideOutRight 0.4s ease';
            setTimeout(() => {
                document.body.removeChild(notificacion);
            }, 400);
        }, 3000);
    }
});

// Animaciones CSS
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);