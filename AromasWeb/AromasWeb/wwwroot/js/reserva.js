// reserva.js - Script para gestión de reservas

document.addEventListener('DOMContentLoaded', function () {

    // =============================
    // MODAL DETALLES RESERVA
    // =============================
    document.addEventListener('DOMContentLoaded', function () {
        const modalDetalles = document.getElementById('modalDetallesReserva');

        if (modalDetalles) {
            modalDetalles.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget;

                // Obtener datos del botón
                const id = button.getAttribute('data-id');
                const cliente = button.getAttribute('data-cliente');
                const telefono = button.getAttribute('data-telefono');
                const personas = button.getAttribute('data-personas');
                const fecha = button.getAttribute('data-fecha');
                const hora = button.getAttribute('data-hora');
                const estado = button.getAttribute('data-estado');
                const observaciones = button.getAttribute('data-observaciones');

                // Actualizar el contenido del modal
                document.getElementById('detalles-id-reserva').textContent = '#' + id;
                document.getElementById('detalles-cliente-reserva').textContent = cliente;
                document.querySelector('#detalles-telefono-reserva span').textContent = telefono;
                document.getElementById('detalles-fecha-reserva').textContent = fecha;
                document.getElementById('detalles-hora-reserva').textContent = hora;
                document.getElementById('detalles-personas-reserva').textContent = personas + ' personas';
                document.getElementById('detalles-estado-texto-reserva').textContent = estado;

                // Configurar el badge de estado con color e icono
                const badgeEstado = document.getElementById('detalles-estado-badge-reserva');
                const iconoEstado = document.getElementById('detalles-estado-icono-reserva');

                // Resetear clases
                badgeEstado.className = 'badge';
                iconoEstado.className = 'fas';

                switch (estado.toLowerCase()) {
                    case 'pendiente':
                        badgeEstado.style.background = '#f39c12';
                        iconoEstado.classList.add('fa-clock');
                        break;
                    case 'confirmada':
                        badgeEstado.style.background = '#27ae60';
                        iconoEstado.classList.add('fa-check-circle');
                        break;
                    case 'completada':
                        badgeEstado.style.background = '#3498db';
                        iconoEstado.classList.add('fa-check-double');
                        break;
                    case 'cancelada':
                        badgeEstado.style.background = '#e74c3c';
                        iconoEstado.classList.add('fa-times-circle');
                        break;
                    default:
                        badgeEstado.style.background = '#95a5a6';
                        iconoEstado.classList.add('fa-question-circle');
                }

                // Mostrar u ocultar observaciones
                const obsContainer = document.getElementById('detalles-observaciones-container');
                const obsTexto = document.getElementById('detalles-observaciones-reserva');

                if (observaciones && observaciones.trim() !== '') {
                    obsContainer.style.display = 'block';
                    obsTexto.textContent = observaciones;
                } else {
                    obsContainer.style.display = 'none';
                }
            });
        }
    });

    // =============================
    // MODAL CANCELAR RESERVA
    // =============================
    document.addEventListener('DOMContentLoaded', function () {
        const modalCancelar = document.getElementById('modalCancelarReserva');

        if (modalCancelar) {
            modalCancelar.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget;

                // Obtener datos del botón
                const id = button.getAttribute('data-id');
                const fecha = button.getAttribute('data-fecha');
                const hora = button.getAttribute('data-hora');
                const personas = button.getAttribute('data-personas');

                // Actualizar el contenido del modal
                document.getElementById('cancelar-id-reserva').value = id;
                document.getElementById('cancelar-id-display').textContent = '#' + id;
                document.getElementById('cancelar-fecha').textContent = fecha;
                document.getElementById('cancelar-hora').textContent = hora;
                document.getElementById('cancelar-personas').textContent = personas + ' personas';

                // Actualizar la acción del formulario
                const form = document.getElementById('formCancelarReserva');
                form.action = '/Reserva/CancelarReserva/' + id;
            });
        }
    });

    // =============================
    // SISTEMA DE NOTIFICACIONES
    // =============================
    window.mostrarNotificacion = function (mensaje, tipo = 'info') {
        // Crear contenedor si no existe
        let contenedor = document.getElementById('notificaciones-container');
        if (!contenedor) {
            contenedor = document.createElement('div');
            contenedor.id = 'notificaciones-container';
            contenedor.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                display: flex;
                flex-direction: column;
                gap: 10px;
            `;
            document.body.appendChild(contenedor);
        }

        // Crear notificación
        const notificacion = document.createElement('div');
        notificacion.style.cssText = `
            background: white;
            padding: 1rem 1.5rem;
            border-radius: 15px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            display: flex;
            align-items: center;
            gap: 1rem;
            min-width: 300px;
            animation: slideIn 0.3s ease;
        `;

        // Configurar según tipo
        let icono = '';
        let color = '';
        switch (tipo) {
            case 'success':
                icono = 'fa-check-circle';
                color = '#27ae60';
                break;
            case 'error':
                icono = 'fa-times-circle';
                color = '#e74c3c';
                break;
            case 'warning':
                icono = 'fa-exclamation-triangle';
                color = '#f39c12';
                break;
            default:
                icono = 'fa-info-circle';
                color = '#3498db';
        }

        notificacion.innerHTML = `
            <i class="fas ${icono}" style="color: ${color}; font-size: 1.5rem;"></i>
            <span style="flex: 1; color: #2c3e50; font-weight: 500;">${mensaje}</span>
            <button onclick="this.parentElement.remove()" style="background: none; border: none; color: #95a5a6; cursor: pointer; font-size: 1.2rem;">
                <i class="fas fa-times"></i>
            </button>
        `;

        contenedor.appendChild(notificacion);

        // Auto-remover después de 5 segundos
        setTimeout(() => {
            notificacion.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => notificacion.remove(), 300);
        }, 5000);
    };

    // =============================
    // VALIDACIÓN DE FORMULARIOS
    // =============================
    const formularios = document.querySelectorAll('form');
    formularios.forEach(form => {
        form.addEventListener('submit', function (e) {
            // Validar campos requeridos
            const camposRequeridos = form.querySelectorAll('[required]');
            let formularioValido = true;

            camposRequeridos.forEach(campo => {
                if (!campo.value.trim()) {
                    formularioValido = false;
                    campo.style.borderColor = '#e74c3c';
                } else {
                    campo.style.borderColor = '';
                }
            });

            if (!formularioValido) {
                e.preventDefault();
                mostrarNotificacion('Por favor complete todos los campos requeridos', 'warning');
            }
        });
    });

    // =============================
    // PAGINACIÓN (si aplica)
    // =============================
    window.paginaAnterior = function () {
        // Implementar lógica de paginación
        console.log('Página anterior');
    };

    window.paginaSiguiente = function () {
        // Implementar lógica de paginación
        console.log('Página siguiente');
    };

    // =============================
    // BÚSQUEDA EN TIEMPO REAL (opcional)
    // =============================
    const campoBusqueda = document.querySelector('input[name="buscar"]');
    if (campoBusqueda) {
        let timeoutBusqueda;
        campoBusqueda.addEventListener('input', function () {
            clearTimeout(timeoutBusqueda);
            timeoutBusqueda = setTimeout(() => {
                // Implementar búsqueda en tiempo real si es necesario
                console.log('Buscando:', this.value);
            }, 500);
        });
    }

    // =============================
    // ANIMACIONES DE TABLA
    // =============================
    const filasTabla = document.querySelectorAll('tbody tr');
    filasTabla.forEach((fila, index) => {
        fila.style.animation = `fadeIn 0.5s ease ${index * 0.05}s both`;
    });

    // =============================
    // CONFIRMACIÓN AL SALIR
    // =============================
    const formulariosEdicion = document.querySelectorAll('form[action*="Editar"], form[action*="Crear"]');
    formulariosEdicion.forEach(form => {
        let formularioModificado = false;

        form.addEventListener('change', () => {
            formularioModificado = true;
        });

        window.addEventListener('beforeunload', (e) => {
            if (formularioModificado) {
                e.preventDefault();
                e.returnValue = '';
            }
        });

        form.addEventListener('submit', () => {
            formularioModificado = false;
        });
    });
});

// =============================
// ESTILOS DE ANIMACIÓN
// =============================
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
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
            transform: translateX(100%);
            opacity: 0;
        }
    }

    @keyframes fadeIn {
        from {
            opacity: 0;
            transform: translateY(10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
`;
document.head.appendChild(style);