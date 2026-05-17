const API_URL = "https://localhost:7223/api/v1";

let countdownInterval = null;
let activeReservationId = null;
let activeUserId = 1;

document.addEventListener("DOMContentLoaded", () => {
    loadEvents();
});

async function loadEvents() {
    const grid = document.getElementById('events-grid');
    const loading = document.getElementById('events-loading');

    // Mostramos el spinner mientras esperamos la respuesta
    loading.classList.remove('d-none');
    grid.innerHTML = '';

    try {
        const response = await fetch(`${API_URL}/events`);
        const result = await response.json();

        loading.classList.add('d-none');

        if (!result.data || result.data.length === 0) {
            grid.innerHTML = '<p class="text-muted">No hay eventos disponibles.</p>';
            return;
        }

        result.data.forEach(e => {
            const col = document.createElement('div');
            col.className = 'col';
            col.innerHTML = `
                <div class="card h-100 shadow-sm event-card" onclick="selectEvent(${e.id}, '${e.name} - ${e.venue}')" style="cursor:pointer;">
                    <div class="card-body">
                        <span class="badge bg-success mb-2">${e.status === 'Active' ? 'Activo' : e.status}</span>
                        <h5 class="card-title">${e.name}</h5>
                        <p class="card-text text-muted mb-1">📍 ${e.venue}</p>
                        <p class="card-text text-muted small">📅 ${new Date(e.eventDate).toLocaleDateString('es-AR', { dateStyle: 'long' })}</p>
                    </div>
                    <div class="card-footer bg-white">
                        <button class="btn btn-primary w-100">Ver butacas</button>
                    </div>
                </div>
            `;
            grid.appendChild(col);
        });
    } catch (err) {
        loading.classList.add('d-none');
        showError("No se pudo conectar con el servidor.");
    }
}

async function loadSectors(eventId) {
    const select = document.getElementById('sector-select');

    if (!eventId) {
        select.disabled = true;
        return;
    }

    // Deshabilitamos el select y mostramos estado de carga mientras esperamos
    select.disabled = true;
    select.innerHTML = '<option value="">Cargando sectores...</option>';

    try {
        const response = await fetch(`${API_URL}/events/${eventId}/sectors`);
        const sectors = await response.json();

        select.innerHTML = '<option value="">-- Seleccioná un sector --</option>';
        sectors.forEach(s => {
            select.innerHTML += `<option value="${s.id}">${s.name} ($${s.price})</option>`;
        });
        select.disabled = false;
    } catch (err) {
        select.innerHTML = '<option value="">Error al cargar sectores</option>';
        showError("Error al cargar sectores.");
    }
}

    async function loadSeats(sectorId) {
        if (!sectorId) return;
        try {
            const response = await fetch(`${API_URL}/sectors/${sectorId}/seats`);
            const seats = await response.json();
            const map = document.getElementById('seat-map');
            const placeholder = document.getElementById('map-placeholder');

            placeholder.classList.add('d-none');
            map.innerHTML = '';

            seats.forEach(s => {
                const statusClass = s.status.toLowerCase();
                const btn = document.createElement('div');
                btn.className = `seat ${statusClass}`;

                // El número de butaca es lo que ve el usuario en el mapa, no el ID interno.
                btn.innerText = s.seatNumber;

                if (statusClass === 'available') {

                    // Pasamos version para que el backend detecte conflictos si otro usuario modificó la butaca entre que se cargó el mapa y se hizo click.
                    btn.onclick = () => reserveSeat(s.id, s.version, btn, s.seatNumber, sectorId);
                }
                map.appendChild(btn);
            });
        } catch (err) {
            showError("Error al cargar asientos.");
        }
    }

    async function reserveSeat(seatId, version, element, seatNumber, sectorId) {
        const originalText = element.innerText;
        element.innerText = '...';
        element.onclick = null;

        try {
            const response = await fetch(`${API_URL}/sectors/${sectorId}/seats/reserve`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    userId: 1,
                    seatId: seatId,
                    version: version
                })
            });

            if (response.ok) {
                const res = await response.json();
                activeReservationId = res.reservationId;
                element.className = 'seat reserved';
                element.innerText = originalText;
                showCart(seatNumber);
                showSuccess(res.message);
            } else {
                // Rollback visual puntual según el tipo de error
                if (response.status === 409) {
                    // Otro usuario tomó la butaca — la marcamos reservada
                    element.className = 'seat reserved';
                    element.innerText = originalText;
                    element.onclick = null;
                    showConflictToast();
                } else if (response.status === 404) {
                    // La butaca no existe — la deshabilitamos
                    element.className = 'seat sold';
                    element.innerText = originalText;
                    element.onclick = null;
                    const err = await response.json();
                    showError(err.message || "La butaca no existe.");
                } else {
                    // Otro error — restauramos la butaca a disponible
                    element.className = 'seat available';
                    element.innerText = originalText;
                    element.onclick = () => reserveSeat(seatId, version, element, seatNumber, sectorId);
                    const err = await response.json();
                    showError(err.message || "La butaca ya no está disponible.");
                }
            }
        } catch (err) {
            // Error de red — restauramos la butaca a disponible
            element.className = 'seat available';
            element.innerText = originalText;
            element.onclick = () => reserveSeat(seatId, version, element, seatNumber, sectorId);
            showError("Error de comunicación.");
        }
    }

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
            const response = await fetch(`${API_URL}/reservations/${activeReservationId}/pay`, {
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

    function selectEvent(eventId, eventName) {
        document.getElementById('catalog-view').classList.add('d-none');
        document.getElementById('seats-view').classList.remove('d-none');
        document.getElementById('seats-view-title').innerText = eventName;
        loadSectors(eventId);
    }

    function goBackToCatalog() {
        document.getElementById('seats-view').classList.add('d-none');
        document.getElementById('catalog-view').classList.remove('d-none');
        document.getElementById('seat-map').innerHTML = '';
        document.getElementById('map-placeholder').classList.remove('d-none');
    }

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
