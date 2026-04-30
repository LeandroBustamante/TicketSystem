const API_URL = "https://localhost:7223/api/v1";

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
                btn.onclick = () => reserveSeat(s.id, s.version, btn);
            }
            map.appendChild(btn);
        });
    } catch (err) {
        showError("Error al cargar asientos.");
    }
}

async function reserveSeat(seatId, version, element) {
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
            showSuccess(res.message);
            element.className = 'seat reserved';
            element.innerText = originalText;
        } else {
            const err = await response.json();
            showError(err.message || "La butaca ya no está disponible.");

            // RECARGAMOS EL MAPA PARA ACTUALIZAR ESTADOS Y VERSIONES
            loadSeats(document.getElementById('sector-select').value);
        }
    } catch (err) {
        showError("Error de comunicación.");
        element.innerText = originalText;
    }
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