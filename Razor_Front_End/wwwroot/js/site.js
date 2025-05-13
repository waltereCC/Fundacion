// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById('cedula').addEventListener('input', async function () {
    const cedula = this.value.replace(/\D/g, ''); // Elimina caracteres no numéricos
    if (cedula.length === 9) { // Verifica que la cédula tenga 9 dígitos
        try {
            const response = await fetch(`https://apis.gometa.org/cedulas/${cedula}`);
            if (response.ok) {
                const data = await response.json();

                if (data.results && data.results.length > 0) {
                    const result = data.results[0];
                    document.getElementById('nombre').value = result.firstname1 + ' ' + result.firstname2;
                    document.getElementById('apellidos').value = result.lastname1 + ' ' + result.lastname2;
                } else {
                    console.error('No se encontraron resultados para la cédula proporcionada.');
                    document.getElementById('nombre').value = '';
                    document.getElementById('apellidos').value = '';
                }
            } else {
                console.error('Error en la solicitud a la API:', response.statusText);
            }
        } catch (error) {
            console.error('Error al realizar la solicitud:', error);
        }
    } else {
        // Limpia los campos si la cédula no tiene 9 dígitos
        document.getElementById('nombre').value = '';
        document.getElementById('apellidos').value = '';
    }
});



