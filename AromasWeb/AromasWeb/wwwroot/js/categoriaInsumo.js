// categoriaInsumo.js - Gestión de categorías de insumos

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN (usando función general de site.js)
    // ============================================
    if (document.getElementById('laTablaDeCategoriaInsumos')) {
        initTablePagination({
            tableId: 'laTablaDeCategoriaInsumos',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // ============================================
    // MODAL DE DETALLES DE CATEGORÍA
    // ============================================
    const botonesDetalles = document.querySelectorAll('.btn-detalles-categoria');

    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const descripcion = this.getAttribute('data-descripcion');
            const estado = this.getAttribute('data-estado');

            document.getElementById('detalles-id-categoria').textContent = id;
            document.getElementById('detalles-nombre-categoria').textContent = nombre;
            document.getElementById('detalles-descripcion-categoria').textContent = descripcion || 'Sin descripción';

            const estadoBadge = document.getElementById('detalles-estado-badge-categoria');
            if (estado === 'Activa') {
                estadoBadge.textContent = 'Activa';
                estadoBadge.style.background = 'var(--green)';
                estadoBadge.style.color = 'white';
            } else {
                estadoBadge.textContent = 'Inactiva';
                estadoBadge.style.background = 'var(--red)';
                estadoBadge.style.color = 'white';
            }
        });
    });

    // ============================================
    // PREVENIR ENVÍO DOBLE EN FORMULARIO ELIMINAR
    // ============================================
    const formEliminar = document.getElementById('formEliminarCategoria');
    if (formEliminar) {
        formEliminar.addEventListener('submit', function () {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Eliminando...';
            }
        });
    }

    // ============================================
    // VALIDACIÓN DE FORMULARIOS
    // ============================================
    const formsCategoria = document.querySelectorAll('.contact-form');

    formsCategoria.forEach(form => {
        const nombreInput = form.querySelector('input[name="NombreCategoria"]');
        const descripcionTextarea = form.querySelector('textarea[name="Descripcion"]');

        if (descripcionTextarea) {
            descripcionTextarea.addEventListener('input', function () {
                if (this.value.length > 500) {
                    this.value = this.value.substring(0, 500);
                }
            });
        }

        if (nombreInput) {
            nombreInput.addEventListener('input', function () {
                if (this.value.length > 100) {
                    this.value = this.value.substring(0, 100);
                }
            });
        }
    });

    // ============================================
    // HOVER EFFECTS EN TABLA
    // ============================================
    const tabla = document.getElementById('laTablaDeCategoriaInsumos');
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
    // ANIMACIÓN DE ENTRADA (feature-cards)
    // ============================================
    const cards = document.querySelectorAll('.feature-card, .admin-form-wrapper');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

    cards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.6s ease';
        observer.observe(card);
    });

});