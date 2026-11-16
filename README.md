# Posture AI
### Sistema inteligente de análisis y evaluación de la postura corporal mediante visión por computadora e inteligencia artificial

## Descripción del proyecto

**Posture AI** es una propuesta de solución tecnológica orientada al análisis, reconocimiento y evaluación de la postura corporal mediante técnicas de inteligencia artificial (*Machine Learning*) y visión por computadora (*Computer Vision*).

El proyecto busca desarrollar una plataforma web capaz de utilizar la cámara de un dispositivo para capturar información sobre la posición y el movimiento del cuerpo humano, identificar puntos anatómicos de referencia y analizar patrones posturales mediante modelos matemáticos de aprendizaje automático.

A partir del procesamiento de estos datos, el sistema pretende proporcionar información sobre la postura del usuario, identificar posibles desviaciones respecto a patrones de referencia y generar retroalimentación visual en tiempo real.

La iniciativa contempla una arquitectura tecnológica integral que conecta la captura de información desde el navegador, el procesamiento de características corporales, la inferencia mediante modelos de inteligencia artificial y el almacenamiento estructurado de información.

## Problemática

Los problemas relacionados con la postura corporal pueden presentarse durante actividades cotidianas, laborales, deportivas o de rehabilitación física. Sin embargo, su identificación y seguimiento suelen depender de evaluaciones presenciales, observación especializada o herramientas que requieren dispositivos adicionales.

Ante esta situación, surge la oportunidad de aprovechar tecnologías accesibles, como las cámaras convencionales, los modelos de aprendizaje automático y los sistemas de visión por computadora, para desarrollar herramientas digitales que faciliten el análisis postural.

Posture AI plantea una alternativa tecnológica que busca automatizar parte del proceso de observación y evaluación postural, proporcionando información de manera accesible mediante una aplicación web.

## Objetivo general

Desarrollar una plataforma web inteligente que permita analizar la postura corporal mediante técnicas de visión por computadora y modelos de aprendizaje automático, utilizando información espacial obtenida de los puntos de referencia del cuerpo humano para identificar patrones posturales y proporcionar retroalimentación al usuario.

## Fundamentos tecnológicos

El proyecto contempla la integración de diferentes áreas de la computación:

### 1. Inteligencia artificial y Machine Learning

Se contempla el desarrollo de un modelo de aprendizaje automático basado en un **Perceptrón Multicapa (MLP)**, una arquitectura de red neuronal artificial capaz de aprender relaciones no lineales entre las características de entrada y las categorías o patrones posturales.

El entrenamiento considera el procesamiento, limpieza y normalización de datos provenientes del conjunto `fitness_poses_csvs_out_full_list.csv`.

Se utilizarán herramientas como Python y TensorFlow/Keras para el diseño, entrenamiento y evaluación del modelo.

### 2. Visión por computadora

La captura y extracción de información corporal se realizará mediante tecnologías de visión por computadora integradas en el navegador.

Se contempla el uso de **MediaPipe** para la detección de puntos de referencia corporales (*landmarks*), obteniendo coordenadas espaciales como:

- Coordenada X: posición horizontal.
- Coordenada Y: posición vertical.
- Coordenada Z: información de profundidad relativa.

Estos valores permitirán construir representaciones matemáticas del cuerpo humano y utilizarlas como características para el análisis postural.

### 3. Desarrollo web

La plataforma tendrá una interfaz web diseñada para facilitar la interacción con el usuario y la visualización de los resultados.

Se consideran tecnologías como:

- HTML5.
- CSS3.
- JavaScript.
- API `getUserMedia` para acceder a la cámara del dispositivo.
- Fetch API para la comunicación con los servicios del sistema.

La interfaz contempla la visualización del video capturado, la representación del esqueleto corporal y la presentación de indicadores o alertas relacionadas con la evaluación postural.

### 4. Backend e integración de inteligencia artificial

Se plantea una arquitectura de servicios mediante una API REST, encargada de establecer la comunicación entre la aplicación web y el modelo de inteligencia artificial.

El procesamiento contempla la recepción de coordenadas corporales, la preparación de los datos de entrada, la ejecución de inferencias y la devolución de resultados estructurados al frontend.

Entre las tecnologías consideradas se encuentran C# con .NET o Node.js, dependiendo de las decisiones de implementación y compatibilidad del sistema.

### 5. Bases de datos e infraestructura en la nube

La solución contempla una arquitectura de persistencia mediante bases de datos relacionales, como MySQL, PostgreSQL o SQL Server.

La estructura de información podrá contemplar entidades como:

- Clínicas o instituciones.
- Usuarios y pacientes.
- Evaluaciones posturales.
- Historial de ejercicios.
- Resultados y registros de seguimiento.

Asimismo, se considera el uso de Docker para la contenerización de los servicios y plataformas de infraestructura en la nube, como Google Cloud Run, para facilitar el despliegue, la disponibilidad y el acceso remoto a la aplicación.

## Arquitectura conceptual

El funcionamiento general de Posture AI contempla el siguiente flujo de procesamiento:

1. **Captura:** el usuario accede a la aplicación web y habilita la cámara de su dispositivo.
2. **Detección corporal:** MediaPipe identifica los puntos de referencia del cuerpo humano.
3. **Extracción de características:** se obtienen y organizan las coordenadas espaciales para construir un vector de características.
4. **Procesamiento:** los datos se envían mediante una API REST para su evaluación.
5. **Inferencia:** el modelo MLP procesa las características y genera una predicción de acuerdo con los patrones aprendidos durante su entrenamiento.
6. **Retroalimentación:** la aplicación presenta los resultados mediante indicadores visuales y alertas posturales.
7. **Persistencia:** cuando la funcionalidad esté habilitada, se almacenarán los resultados relevantes para su consulta y seguimiento.

## Alcance del proyecto

Posture AI contempla el desarrollo de un prototipo funcional que integre inteligencia artificial, visión por computadora y desarrollo web dentro de una misma solución tecnológica.

Su alcance considera:

- Captura de video mediante cámaras convencionales.
- Detección y representación de puntos corporales en tiempo real.
- Extracción y procesamiento de características espaciales.
- Entrenamiento y evaluación de un modelo de aprendizaje automático.
- Clasificación o identificación de patrones posturales según las categorías disponibles en los datos de entrenamiento.
- Comunicación entre frontend, backend y modelo de inteligencia artificial.
- Presentación de resultados y retroalimentación visual.
- Diseño de una estructura de almacenamiento para evaluaciones e historial.
- Preparación de una arquitectura susceptible de desplegarse en infraestructura de nube.

Como proyección, la solución contempla su posible adaptación a escenarios de fisioterapia remota, seguimiento de ejercicios y evaluación ergonómica en entornos laborales.

Es importante señalar que el sistema se plantea como una herramienta de apoyo tecnológico y no como un sustituto del diagnóstico, valoración o tratamiento realizado por profesionales de la salud.

## Enfoque de aplicación y modelo de negocio

Además de su componente tecnológico, Posture AI contempla posibilidades de aplicación bajo un enfoque **B2B (Business to Business)**.

Entre sus posibles escenarios de aplicación se encuentran:

- **Clínicas de fisioterapia:** herramientas complementarias para el seguimiento de ejercicios y la observación de patrones posturales.
- **Centros de rehabilitación:** apoyo tecnológico para registrar y consultar evaluaciones.
- **Entornos corporativos:** herramientas orientadas a la observación ergonómica y la concientización sobre hábitos posturales.
- **Instituciones deportivas:** seguimiento complementario de patrones de movimiento y ejecución de ejercicios.

Como alternativa comercial, se considera un modelo de servicio mediante suscripción (SaaS), dirigido a instituciones que requieran herramientas digitales para apoyar sus procesos de seguimiento y evaluación.

## Tecnologías contempladas

| Área | Tecnologías |
|---|---|
| Lenguaje para Machine Learning | Python |
| Inteligencia artificial | TensorFlow / Keras |
| Arquitectura del modelo | Perceptrón Multicapa (MLP) |
| Visión por computadora | MediaPipe |
| Desarrollo frontend | HTML5, CSS3 y JavaScript |
| Comunicación | Fetch API y REST API |
| Backend | C# .NET o Node.js |
| Base de datos | MySQL, PostgreSQL o SQL Server |
| Contenerización | Docker |
| Infraestructura cloud | Google Cloud Run |
| Formato de exportación del modelo | ONNX o TensorFlow.js |

*Las tecnologías indicadas representan las herramientas contempladas para la implementación y pueden ajustarse durante el desarrollo.*

## Consideraciones y limitaciones

El desempeño del sistema dependerá de la calidad y representatividad de los datos de entrenamiento, las condiciones de iluminación, la posición de la cámara, la visibilidad del cuerpo y la precisión de los puntos de referencia detectados.

Asimismo, las coordenadas espaciales obtenidas mediante visión por computadora representan estimaciones y no mediciones biomecánicas clínicas.

La identificación de patrones posturales estará limitada por las categorías, características y capacidad de generalización del modelo entrenado.

Por lo tanto, los resultados deberán interpretarse como información orientativa y no como una evaluación médica definitiva.

## Visión del proyecto

**Posture AI busca demostrar cómo la integración de inteligencia artificial, visión por computadora y servicios web puede transformar información visual del cuerpo humano en datos estructurados y útiles para el análisis postural.**

El proyecto propone una solución tecnológica escalable, accesible y con posibilidades de aplicación en los ámbitos de la salud, la rehabilitación y la ergonomía, integrando modelos de aprendizaje automático con una experiencia de usuario basada en procesamiento visual en tiempo real.