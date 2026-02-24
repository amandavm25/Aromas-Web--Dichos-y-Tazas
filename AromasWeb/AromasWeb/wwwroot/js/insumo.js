// insumo.js - Gestión de insumos

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN (usando función general de site.js)
    // ============================================
    if (document.getElementById('laTablaDeInsumos')) {
        initTablePagination({
            tableId: 'laTablaDeInsumos',
            recordsPerPage: 5,
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
    const tabla = document.getElementById('laTablaDeInsumos');
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

    // ============================================
    // VALIDACIÓN DE FORMULARIOS
    // ============================================
    const formsInsumo = document.querySelectorAll('.contact-form');

    formsInsumo.forEach(form => {
        const nombreInput = form.querySelector('input[name="NombreInsumo"]');

        if (nombreInput) {
            nombreInput.addEventListener('input', function () {
                if (this.value.length > 100) {
                    this.value = this.value.substring(0, 100);
                }
            });
        }
    });

});