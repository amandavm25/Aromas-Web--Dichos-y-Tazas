// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    // Animar elementos al cargar
    animateOnLoad();

    // Inicializar tooltips
    initializeTooltips();

    // Efectos hover en filas
    initializeTableHoverEffects();

    // Búsqueda en tiempo real
    inicializarBusquedaHistorial();

    // Inicializar paginación
    mostrarPagina();

    // Inicializar modales
    initializeModals();

    // Contador animado para estadísticas
    animateStatistics();
});

// ============================================
// ANIMACIONES AL CARGAR
// ============================================
function animateOnLoad() {
    // Animar cards de estadísticas
    const statCards = document.querySelectorAll('[style*="linear-gradient(135deg, #"]');
    statCards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px) scale(0.9)';

        setTimeout(() => {
            card.style.transition = 'all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0) scale(1)';
        }, index * 100);
    });

    // Animar tabla
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
// CONTADOR ANIMADO PARA ESTADÍSTICAS
// ============================================
function animateStatistics() {
    const statNumbers = document.querySelectorAll('[style*="font-size: 2.5rem"]');

    statNumbers.forEach(stat => {
        const text = stat.textContent;
        const number = parseFloat(text.replace(/[^0-9.]/g, ''));

        if (!isNaN(number)) {
            animateNumber(stat, 0, number, 1500);
        }
    });
}

function animateNumber(element, start, end, duration) {
    const range = end - start;
    const increment = range / (duration / 16);
    let current = start;

    const timer = setInterval(() => {
        current += increment;
        if (current >= end) {
            element.textContent = Math.round(end);
            clearInterval(timer);
        } else {
            element.textContent = Math.round(current);
        }
    }, 16);
}

// ============================================
// EFECTOS HOVER MEJORADOS
// ============================================
function initializeTableHoverEffects() {
    const tables = document.querySelectorAll('table');

    tables.forEach(table => {
        const tbody = table.querySelector('tbody');
        if (!tbody) return;

        const rows = tbody.querySelectorAll('tr');

        rows.forEach(row => {
            row.addEventListener('mouseenter', function () {
                this.style.background = 'linear-gradient(90deg, rgba(32, 116, 118, 0.05) 0%, transparent 100%)';
                this.style.transform = 'translateX(5px)';
                this.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.08)';
            });

            row.addEventListener('mouseleave', function () {
                if (!this.style.borderLeft.includes('4px solid var(--red)')) {
                    this.style.background = '';
                    this.style.transform = '';
                    this.style.boxShadow = '';
                }
            });
        });
    });
}

// ============================================
// TOOLTIPS PERSONALIZADOS
// ============================================
function initializeTooltips() {
    const elementsWithTitle = document.querySelectorAll('[title]');

    elementsWithTitle.forEach(element => {
        const title = element.getAttribute('title');
        element.removeAttribute('title');
        element.setAttribute('data-tooltip', title);

        element.addEventListener('mouseenter', function (e) {
            showTooltip(e, title);
        });

        element.addEventListener('mouseleave', function () {
            hideTooltip();
        });
    });
}

let tooltipElement = null;

function showTooltip(event, text) {
    hideTooltip();

    tooltipElement = document.createElement('div');
    tooltipElement.textContent = text;
    tooltipElement.style.cssText = `
        position: fixed;
        background: linear-gradient(135deg, var(--dark-teal), var(--teal));
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
    tooltipElement.style.left = (event.clientX - rect.width / 2) + 'px';
    tooltipElement.style.top = (event.clientY - rect.height - 10) + 'px';
}

function hideTooltip() {
    if (tooltipElement) {
        tooltipElement.style.animation = 'tooltipFadeOut 0.2s ease';
        setTimeout(() => {
            if (tooltipElement && tooltipElement.parentNode) {
                tooltipElement.remove();
            }
            tooltipElement = null;
        }, 200);
    }
}

// Agregar animaciones de tooltip
const tooltipStyles = document.createElement('style');
tooltipStyles.innerHTML = `
    @keyframes tooltipFadeIn {
        from {
            opacity: 0;
            transform: translateY(10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
    @keyframes tooltipFadeOut {
        from {
            opacity: 1;
            transform: translateY(0);
        }
        to {
            opacity: 0;
            transform: translateY(10px);
        }
    }
`;
document.head.appendChild(tooltipStyles);

// ============================================
// PAGINACIÓN UNIVERSAL
// ============================================
let paginaActual = 1;
const registrosPorPagina = 10;

function paginaAnterior() {
    if (paginaActual > 1) {
        paginaActual--;
        mostrarPagina();
    }
}

function paginaSiguiente() {
    // Detectar qué tabla usar
    const tabla = document.getElementById('laTablaDeInsumos') || document.getElementById('tablaHistorial');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const filas = Array.from(tbody.querySelectorAll('tr')).filter(fila => fila.style.display !== 'none');
    const totalPaginas = Math.ceil(filas.length / registrosPorPagina);

    if (paginaActual < totalPaginas) {
        paginaActual++;
        mostrarPagina();
    }
}

function mostrarPagina() {
    // Detectar qué tabla usar
    const tabla = document.getElementById('laTablaDeInsumos') || document.getElementById('tablaHistorial');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const todasLasFilas = tbody.querySelectorAll('tr');
    const filasVisibles = Array.from(todasLasFilas).filter(fila => fila.style.display !== 'none');

    const inicio = (paginaActual - 1) * registrosPorPagina;
    const fin = inicio + registrosPorPagina;

    // Si estamos en historial, usar clases
    const esHistorial = !!document.getElementById('tablaHistorial');

    if (esHistorial) {
        todasLasFilas.forEach(fila => {
            if (fila.style.display !== 'none') {
                fila.classList.remove('visible-row');
            }
        });

        filasVisibles.forEach((fila, index) => {
            if (index >= inicio && index < fin) {
                fila.classList.add('visible-row');
            }
        });
    } else {
        // Para listado de insumos
        todasLasFilas.forEach((fila, index) => {
            if (index >= inicio && index < fin) {
                fila.style.display = '';
            } else {
                fila.style.display = 'none';
            }
        });
    }

    // Actualizar información de paginación
    const totalRegistros = filasVisibles.length;
    const totalPaginas = Math.ceil(totalRegistros / registrosPorPagina);

    const startRecord = document.getElementById('startRecord');
    const endRecord = document.getElementById('endRecord');
    const totalRecordsEl = document.getElementById('totalRecords');

    if (startRecord) startRecord.textContent = totalRegistros > 0 ? inicio + 1 : 0;
    if (endRecord) endRecord.textContent = Math.min(fin, totalRegistros);
    if (totalRecordsEl) totalRecordsEl.textContent = totalRegistros;

    // Deshabilitar botones según corresponda
    const btnAnterior = document.getElementById('btnAnterior');
    const btnSiguiente = document.getElementById('btnSiguiente');

    if (btnAnterior) {
        btnAnterior.disabled = paginaActual === 1;
        btnAnterior.style.opacity = paginaActual === 1 ? '0.5' : '1';
        btnAnterior.style.cursor = paginaActual === 1 ? 'not-allowed' : 'pointer';
    }

    if (btnSiguiente) {
        btnSiguiente.disabled = paginaActual === totalPaginas || totalRegistros === 0;
        btnSiguiente.style.opacity = (paginaActual === totalPaginas || totalRegistros === 0) ? '0.5' : '1';
        btnSiguiente.style.cursor = (paginaActual === totalPaginas || totalRegistros === 0) ? 'not-allowed' : 'pointer';
    }
}

// CSS para filas visibles (historial)
const stylePaginacion = document.createElement('style');
stylePaginacion.innerHTML = `
    .fila-movimiento {
        display: none;
    }
    .fila-movimiento.visible-row {
        display: table-row;
    }
`;
document.head.appendChild(stylePaginacion);

// ============================================
// MODAL DETALLES INSUMO
// ============================================
    const botonesDetalles = document.querySelectorAll('.btn-detalles');

    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            // Obtener datos del botón
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const unidad = this.getAttribute('data-unidad');
            const categoria = this.getAttribute('data-categoria');
            const costo = this.getAttribute('data-costo');
            const cantidad = this.getAttribute('data-cantidad');
            const stockMinimo = this.getAttribute('data-stockminimo');
            const estado = this.getAttribute('data-estado');

            // Llenar el modal con los datos
            document.getElementById('detalles-id-insumo').textContent = id;
            document.getElementById('detalles-nombre-insumo').textContent = nombre;
            document.getElementById('detalles-unidad-insumo').textContent = unidad;
            document.getElementById('detalles-categoria-insumo').textContent = categoria;
            document.getElementById('detalles-costo-insumo').textContent = costo;
            document.getElementById('detalles-cantidad-insumo').textContent = cantidad;
            document.getElementById('detalles-stockminimo-insumo').textContent = stockMinimo;

            // Calcular valor total
            const costoNumerico = parseFloat(costo.replace(/[^0-9.-]+/g, ''));
            const cantidadNumerica = parseFloat(cantidad.replace(/,/g, ''));
            const valorTotal = costoNumerico * cantidadNumerica;
            document.getElementById('detalles-valor-total-insumo').textContent = '₡' + valorTotal.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');

            // Configurar el badge de estado
            const estadoBadge = document.getElementById('detalles-estado-badge-insumo');
            if (estado === 'Activo') {
                estadoBadge.textContent = 'Activo';
                estadoBadge.style.background = 'var(--green)';
                estadoBadge.style.color = 'white';
            } else {
                estadoBadge.textContent = 'Inactivo';
                estadoBadge.style.background = '#95a5a6';
                estadoBadge.style.color = 'white';
            }

            // Nota: La categoría se puede obtener del DOM o pasar como data-attribute
            // Por ahora dejamos un placeholder
            const categoriaElement = document.getElementById('detalles-categoria-insumo');
            if (categoriaElement) {
                // Buscar el nombre de la categoría en la fila correspondiente de la tabla
                const fila = this.closest('tr');
                if (fila) {
                    const celdaCategoria = fila.querySelector('td:nth-child(4)'); // Ajustar según la posición
                    if (celdaCategoria) {
                        categoriaElement.textContent = celdaCategoria.textContent.trim();
                    }
                }
            }
        });
    });

    // ============================================
    // MODAL ELIMINAR INSUMO
    // ============================================
    const botonesEliminar = document.querySelectorAll('.btn-eliminar');

    botonesEliminar.forEach(boton => {
        boton.addEventListener('click', function () {
            // Obtener datos del botón
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const unidad = this.getAttribute('data-unidad');
            const cantidad = this.getAttribute('data-cantidad');
            const estado = this.getAttribute('data-estado');

            // Llenar el modal con los datos
            document.getElementById('eliminar-id-display-insumo').textContent = id;
            document.getElementById('eliminar-nombre-insumo').textContent = nombre;
            document.getElementById('eliminar-unidad-insumo').textContent = unidad;
            document.getElementById('eliminar-cantidad-insumo').textContent = cantidad;

            // Establecer el ID en el campo oculto del formulario
            document.getElementById('eliminar-id-insumo').value = id;

            // Configurar el badge de estado
            const estadoBadge = document.getElementById('eliminar-estado-badge-insumo');
            if (estado === 'Activo') {
                estadoBadge.textContent = 'Activo';
                estadoBadge.style.background = 'var(--green)';
                estadoBadge.style.color = 'white';
            } else {
                estadoBadge.textContent = 'Inactivo';
                estadoBadge.style.background = '#95a5a6';
                estadoBadge.style.color = 'white';
            }

            // Obtener la categoría de la fila
            const fila = this.closest('tr');
            if (fila) {
                const celdaCategoria = fila.querySelector('td:nth-child(4)'); // Ajustar según la posición
                const categoriaElement = document.getElementById('eliminar-categoria-insumo');
                if (celdaCategoria && categoriaElement) {
                    categoriaElement.textContent = celdaCategoria.textContent.trim();
                }
            }

            // Configurar la acción del formulario
            const form = document.getElementById('formEliminarInsumo');
            form.action = '/Insumo/EliminarInsumo/' + id;
        });
    });

    // ==========================================
    // VALIDACIÓN Y MENSAJES
    // ==========================================
    // Prevenir envío doble del formulario
    const formEliminar = document.getElementById('formEliminarInsumo');
    if (formEliminar) {
        formEliminar.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Eliminando...';
            }
        });
    }

    // ==========================================
    // ACTUALIZACIÓN AUTOMÁTICA DE ESTADÍSTICAS
    // ==========================================
    function actualizarEstadisticas() {
        // Esta función se puede expandir para actualizar las estadísticas en tiempo real
        // si se implementa con AJAX
        console.log('Estadísticas actualizadas');
    }

    // Inicializar paginación
    mostrarPagina();
});

// ============================================
// FILTROS DEL HISTORIAL
// ============================================
function aplicarFiltros() {
    const buscarInsumo = document.getElementById('buscarInsumo').value.toLowerCase();
    const filtroTipo = document.getElementById('filtroTipo').value;
    const fechaDesde = document.getElementById('fechaDesde').value;
    const fechaHasta = document.getElementById('fechaHasta').value;

    const tabla = document.getElementById('tablaHistorial');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const filas = tbody.querySelectorAll('.fila-movimiento');
    let contadorVisibles = 0;

    filas.forEach(fila => {
        const insumo = fila.getAttribute('data-insumo');
        const tipo = fila.getAttribute('data-tipo');
        const fecha = fila.getAttribute('data-fecha');
        const fechaSolo = fecha.split(' ')[0]; // Obtener solo la fecha sin hora

        let mostrar = true;

        // Filtro por insumo
        if (buscarInsumo && !insumo.includes(buscarInsumo)) {
            mostrar = false;
        }

        // Filtro por tipo
        if (filtroTipo && tipo !== filtroTipo) {
            mostrar = false;
        }

        // Filtro por fecha desde
        if (fechaDesde && mostrar) {
            const fechaMovimiento = convertirFecha(fechaSolo);
            const fechaDesdeObj = new Date(fechaDesde);
            if (fechaMovimiento < fechaDesdeObj) {
                mostrar = false;
            }
        }

        // Filtro por fecha hasta
        if (fechaHasta && mostrar) {
            const fechaMovimiento = convertirFecha(fechaSolo);
            const fechaHastaObj = new Date(fechaHasta);
            if (fechaMovimiento > fechaHastaObj) {
                mostrar = false;
            }
        }

        if (mostrar) {
            fila.style.display = '';
            contadorVisibles++;
        } else {
            fila.style.display = 'none';
            fila.classList.remove('visible-row');
        }
    });

    // Resetear a página 1 después de filtrar
    paginaActual = 1;
    mostrarPagina();

    // Mostrar notificación
    mostrarNotificacion(`Se encontraron ${contadorVisibles} movimiento(s)`, 'success');
}

function limpiarFiltros() {
    document.getElementById('buscarInsumo').value = '';
    document.getElementById('filtroTipo').value = '';
    document.getElementById('fechaDesde').value = '';
    document.getElementById('fechaHasta').value = '';

    const tabla = document.getElementById('tablaHistorial');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const filas = tbody.querySelectorAll('.fila-movimiento');
    filas.forEach(fila => {
        fila.style.display = '';
    });

    paginaActual = 1;
    mostrarPagina();

    mostrarNotificacion('Filtros limpiados', 'success');
}

function convertirFecha(fechaStr) {
    // Convierte fecha de formato DD/MM/YYYY a objeto Date
    const partes = fechaStr.split('/');
    return new Date(partes[2], partes[1] - 1, partes[0]);
}

// ============================================
// EXPORTAR A EXCEL
// ============================================
function exportarExcel() {
    mostrarNotificacion('Exportando a Excel...', 'success');

    // Simulación de exportación
    setTimeout(() => {
        mostrarNotificacion('Archivo descargado: historial_movimientos.xlsx', 'success');
    }, 1500);

    // Aquí iría la lógica real de exportación
}

// ============================================
// ANIMACIÓN DE ESTADÍSTICAS (HISTORIAL)
// ============================================
function animarNumero(elemento, inicio, fin, duracion, esDecimal = false) {
    const pasos = 60;
    const incremento = (fin - inicio) / pasos;
    const tiempoPaso = duracion / pasos;
    let actual = inicio;
    let paso = 0;

    const intervalo = setInterval(() => {
        paso++;
        actual += incremento;

        if (paso >= pasos) {
            actual = fin;
            clearInterval(intervalo);
        }

        elemento.textContent = esDecimal ? actual.toFixed(1) : Math.floor(actual);
    }, tiempoPaso);
}

// ============================================
// BÚSQUEDA EN TIEMPO REAL
// ============================================
function inicializarBusquedaHistorial() {
    const inputBusqueda = document.getElementById('buscarInsumo');

    if (inputBusqueda) {
        inputBusqueda.addEventListener('input', function () {
            const valorBusqueda = this.value.toLowerCase();
            const tabla = document.getElementById('tablaHistorial');
            const tbody = tabla ? tabla.querySelector('tbody') : null;

            if (!tbody) return;

            const filas = tbody.querySelectorAll('.fila-movimiento');

            filas.forEach(fila => {
                // Solo buscar si no hay otros filtros activos
                const filtroTipo = document.getElementById('filtroTipo').value;
                const fechaDesde = document.getElementById('fechaDesde').value;
                const fechaHasta = document.getElementById('fechaHasta').value;

                if (!filtroTipo && !fechaDesde && !fechaHasta) {
                    const insumo = fila.getAttribute('data-insumo');
                    if (insumo.includes(valorBusqueda)) {
                        fila.style.display = '';
                    } else {
                        fila.style.display = 'none';
                        fila.classList.remove('visible-row');
                    }
                }
            });

            // Resetear a página 1
            paginaActual = 1;
            mostrarPagina();
        });
    }

    // Tecla Enter en filtros para aplicar
    const filtros = [
        inputBusqueda,
        document.getElementById('filtroTipo'),
        document.getElementById('fechaDesde'),
        document.getElementById('fechaHasta')
    ];

    filtros.forEach(filtro => {
        if (filtro) {
            filtro.addEventListener('keypress', function (e) {
                if (e.key === 'Enter') {
                    aplicarFiltros();
                }
            });
        }
    });
}

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = {
        'success': 'fa-check-circle',
        'error': 'fa-exclamation-triangle',
        'warning': 'fa-exclamation-triangle'
    };

    const colores = {
        'success': 'var(--green)',
        'error': 'var(--red)',
        'warning': '#f39c12'
    };

    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 100px;
        right: 20px;
        background: ${colores[tipo]};
        color: white;
        padding: 1.5rem 2rem;
        border-radius: 10px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001;
        animation: slideIn 0.3s ease;
        font-weight: 600;
        display: flex;
        align-items: center;
        gap: 1rem;
        min-width: 300px;
    `;

    notification.innerHTML = `
        <i class="fas ${iconos[tipo]}" style="font-size: 1.5rem;"></i>
        <span>${mensaje}</span>
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 3000);
}

// Animaciones para notificaciones
const notifStyle = document.createElement('style');
notifStyle.innerHTML = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(notifStyle);

// ============================================
// VALIDACIÓN DE FORMULARIOS
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const inputs = this.querySelectorAll('input[required], select[required], textarea[required]');
            let isValid = true;

            inputs.forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;
                    input.style.borderColor = 'var(--red)';
                } else {
                    input.style.borderColor = 'var(--secondary-color)';
                }
            });

            if (!isValid) {
                e.preventDefault();
                mostrarNotificacion('Por favor, completa todos los campos obligatorios', 'error');
            }
        });
    });
});

// ============================================
// EFECTOS HOVER EN FILAS DE TABLA
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    const tabla = document.getElementById('laTablaDeInsumos');
    if (!tabla) return;

    const tbody = tabla.querySelector('tbody');
    if (!tbody) return;

    const filas = tbody.querySelectorAll('tr');

    filas.forEach(fila => {
        fila.addEventListener('mouseenter', function () {
            if (!this.style.borderLeft.includes('4px solid var(--red)')) {
                this.style.background = 'var(--light-color)';
            }
        });

        fila.addEventListener('mouseleave', function () {
            if (!this.style.borderLeft.includes('4px solid var(--red)')) {
                this.style.background = '';
            }
        });
    });
});

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = {
        'success': 'fa-check-circle',
        'error': 'fa-exclamation-triangle',
        'warning': 'fa-exclamation-triangle',
        'info': 'fa-info-circle'
    };

    const colores = {
        'success': 'var(--green)',
        'error': 'var(--red)',
        'warning': '#f39c12',
        'info': '#3498db'
    };

    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 100px;
        right: 20px;
        background: ${colores[tipo]};
        color: white;
        padding: 1.5rem 2rem;
        border-radius: 10px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001;
        animation: slideIn 0.3s ease;
        font-weight: 600;
        display: flex;
        align-items: center;
        gap: 1rem;
        min-width: 300px;
    `;

    notification.innerHTML = `
        <i class="fas ${iconos[tipo]}" style="font-size: 1.5rem;"></i>
        <span>${mensaje}</span>
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 3000);
}

// Animaciones para notificaciones
const notifStyle = document.createElement('style');
notifStyle.innerHTML = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(notifStyle);

console.log('%c✓ Script de insumos cargado correctamente', 'color: var(--green); font-weight: bold;');