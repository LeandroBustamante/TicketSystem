const API_URL = "https://localhost:7223/api/v1";

// ==================== NUEVO: VARIABLES GLOBALES ====================
let countdownInterval = null;
let activeReservationId = null;
let activeUserId = 1;
// ==================== FIN NUEVO ====================

document.addEventListener("DOMContentLoaded", () => {
    loadEvents();
});

async function loadEvents() {
    try {
        const response = await fetch(`${API_URL}/events`);
        const events = await response.json();
        const select = document.getElementById('event-select');
        
        events.forEach(e => {
            const option = document.createElement('option');
            option.value = e.id;

            // MAPEADO CON LAS PROPIEDADES DEL DTO EVENTRESPONSE
            option.textContent = `${e.name} - ${e.venue}`;
            select.appendChild(option);
        });
    } catch (err) {
        showError("No se pudo conectar con el servidor.");
    }
}

async function loadSectors(eventId) {
    const select = document.getElementById('sector-select');
    if (!eventId) {
        select.disabled = true;
        return;
    }
    
    try {
        const response = await fetch(`${API_URL}/events/${eventId}/sectors`);
        const sectors = await response.json();
        
        select.disabled = false;
        select.innerHTML = '<option value="">-- Seleccioná un sector --</option>';
        sectors.forEach(s => {

            // MAPEADO CON LAS PROPIEDADES DEL DTO SECTORRESPONSE
            select.innerHTML += `<option value="${s.id}">${s.name} ($${s.price})</option>`;
        });
    } catch (err) {
        showError("Error al cargar sectores.");
    }
}

async function loadSeats(sectorId) {
    if (!sectorId) return;
    try {
        const response = await fetch(`${API_URL}/events/sectors/${sectorId}/seats`);
        const seats = await response.json();
        const map = document.getElementById('seat-map');
        const placeholder = document.getElementById('map-placeholder');

        placeholder.classList.add('d-none');
        map.innerHTML = '';

        seats.forEach(s => {
            const statusClass = s.status.toLowerCase();
            const btn = document.createElement('div');
            btn.className = `seat ${statusClass}`;

            // MAPEADO CON SEATNUMBER DEL SEATRESPONSE
            btn.innerText = s.seatNumber; 

            if (statusClass === 'available') {

                // PASAMOS EL ID Y LA VERSIÓN PARA EL OPTIMISTIC LOCKING
                btn.onclick = () => reserveSeat(s.id, s.version, btn, s.seatNumber);
            }
            map.appendChild(btn);
        });
    } catch (err) {
        showError("Error al cargar asientos.");
    }
}

// CAMBIO: agregamos el parámetro seatNumber a la firma
async function reserveSeat(seatId, version, element, seatNumber) {
    const originalText = element.innerText;
    element.innerText = '...';
    element.onclick = null;

    try {

        // LA URL DEBE COINCIDIR CON EL ROUTE DEL EVENTSCONTROLLER
        const response = await fetch(`${API_URL}/events/seats/reserve`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },

            // ENVIAMOS EL OBJETO TAL CUAL LO ESPERA EL RESERVESEATCOMMAND
            body: JSON.stringify({ 
                userId: 1, 
                seatId: seatId,
                version: version 
            })
        });

        if (response.ok) {
            const res = await response.json();
            // CAMBIO: guardamos el reservationId y mostramos el carrito
            activeReservationId = res.reservationId;
            element.className = 'seat reserved';
            element.innerText = originalText;
            showCart(seatNumber);
            showSuccess(res.message);
        } else {
            element.className = 'seat reserved';
            element.innerText = originalText;

            // CAMBIO: si es 409 mostramos el toast, sino el mensaje de error
            if (response.status === 409) {
                showConflictToast();
            } else {
                const err = await response.json();
                showError(err.message || "La butaca ya no está disponible.");
            }

            // RECARGAMOS EL MAPA PARA ACTUALIZAR ESTADOS Y VERSIONES
            loadSeats(document.getElementById('sector-select').value);
        }
    } catch (err) {
        showError("Error de comunicación.");
        element.innerText = originalText;
    }
}

// ==================== NUEVO: FUNCIONES DE CARRITO Y TEMPORIZADOR ====================

function showCart(seatNumber) {
    document.getElementById('cart-seat-info').innerText = `Butaca ${seatNumber}`;
    document.getElementById('cart-panel').classList.remove('d-none');
    startCountdown(5 * 60);
}

function startCountdown(seconds) {
    if (countdownInterval) clearInterval(countdownInterval);
    const countdownEl = document.getElementById('countdown');
    let remaining = seconds;

    countdownInterval = setInterval(() => {
        const minutes = Math.floor(remaining / 60);
        const secs = remaining % 60;
        countdownEl.innerText = `${String(minutes).padStart(2, '0')}:${String(secs).padStart(2, '0')}`;

        if (remaining <= 0) {
            clearInterval(countdownInterval);
            hideCart();
            showError("⏰ Tu reserva expiró. La butaca fue liberada.");
            loadSeats(document.getElementById('sector-select').value);
        }
        remaining--;
    }, 1000);
}

async function confirmPayment() {
    if (!activeReservationId) return;
    const payBtn = document.getElementById('pay-btn');
    payBtn.disabled = true;
    payBtn.innerText = 'Procesando...';

    try {
        const response = await fetch(`${API_URL}/events/reservations/${activeReservationId}/pay`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId: activeUserId })
        });

        if (response.ok) {
            clearInterval(countdownInterval);
            hideCart();
            showSuccess("✅ ¡Pago confirmado! Disfrutá el evento.");
            loadSeats(document.getElementById('sector-select').value);
        } else {
            const err = await response.json();
            showError(err.message || "Error al procesar el pago.");
            payBtn.disabled = false;
            payBtn.innerText = '💳 Confirmar Pago';
        }
    } catch (err) {
        showError("Error de comunicación al pagar.");
        payBtn.disabled = false;
        payBtn.innerText = '💳 Confirmar Pago';
    }
}

function hideCart() {
    document.getElementById('cart-panel').classList.add('d-none');
    activeReservationId = null;
}

function showConflictToast() {
    const toastEl = document.getElementById('conflict-toast');
    const toast = new bootstrap.Toast(toastEl, { delay: 4000 });
    toast.show();
}

// ==================== FIN NUEVO ====================

function showSuccess(msg) {
    const box = document.getElementById('message-box');
    box.className = 'alert alert-success';
    box.innerText = msg;
    box.classList.remove('d-none');
}

function showError(msg) {
    const box = document.getElementById('message-box');
    box.className = 'alert alert-danger';
    box.innerText = msg;
    box.classList.remove('d-none');
}