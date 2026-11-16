const videoElement = document.getElementById('videoElement');
const canvasElement = document.getElementById('canvasEsqueleto');
const canvasCtx = canvasElement.getContext('2d');
const alertaPostura = document.getElementById('alertaPostura');

let ultimoEnvio = 0;

// 1. Configuración del modelo Pose de MediaPipe
const pose = new Pose({
    locateFile: (file) => `https://cdn.jsdelivr.net/npm/@mediapipe/pose/${file}`
});

pose.setOptions({
    modelComplexity: 1,
    smoothLandmarks: true,
    minDetectionConfidence: 0.5,
    minTrackingConfidence: 0.5
});

pose.onResults(onResults);

// 2. Inicialización de la webcam
const camera = new Camera(videoElement, {
    onFrame: async () => {
        await pose.send({ image: videoElement });
    },
    width: 640,
    height: 480
});
camera.start();

// 3. Renderizado y extracción
function onResults(results) {
    canvasCtx.save();
    canvasCtx.clearRect(0, 0, canvasElement.width, canvasElement.height);

    // Dibujar fondo de cámara
    canvasCtx.drawImage(results.image, 0, 0, canvasElement.width, canvasElement.height);

    if (results.poseLandmarks) {
        // Dibujar uniones y puntos del cuerpo
        drawConnectors(canvasCtx, results.poseLandmarks, POSE_CONNECTIONS, { color: '#00FF00', lineWidth: 4 });
        drawLandmarks(canvasCtx, results.poseLandmarks, { color: '#FF0000', lineWidth: 2, radius: 4 });

        // Enviar vector numérico al backend
        evaluarPostura(results.poseLandmarks);
    }
    canvasCtx.restore();
}

// 4. Comunicación con la API
async function evaluarPostura(landmarks) {
    const ahora = Date.now();
    // Throttle: Limita las peticiones a ~3 por segundo para proteger el servidor
    if (ahora - ultimoEnvio < 300) return;
    ultimoEnvio = ahora;

    try {
        // URL temporal, se ajustará cuando el backend C# exponga el endpoint
        const response = await fetch('/api/EvaluacionFisica/Clasificar', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ keypoints: landmarks })
        });

        if (!response.ok) return;

        const resultado = await response.json();

        // Actualizar UI
        alertaPostura.innerText = resultado.mensaje;

        if (resultado.estado === "Peligro") {
            alertaPostura.style.color = "#dc3545"; // Rojo
            canvasElement.style.borderColor = "#dc3545";
        } else {
            alertaPostura.style.color = "#198754"; // Verde
            canvasElement.style.borderColor = "#198754";
        }

    } catch (error) {
        console.warn("API de evaluación no conectada aún.", error);
    }
}