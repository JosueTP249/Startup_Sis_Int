-- =====================================================================
-- Posture AI - Consultas de demostración para el pitch
-- Ejecutar en el editor SQL de Neon / Supabase o en pgAdmin después de
-- usar la cámara unos minutos en la página.
-- =====================================================================

-- 1. Últimas 20 evaluaciones guardadas (historial en vivo)
SELECT ev."FechaHora",
       p."Nombre"            AS paciente,
       ev."ClaseDetectada",
       ev."Estado",
       ROUND((ev."Confianza" * 100)::numeric, 1) AS confianza_pct
FROM "EvaluacionesPosturales" ev
JOIN "Pacientes" p ON p."Id" = ev."UsuarioId"
ORDER BY ev."FechaHora" DESC
LIMIT 20;

-- 2. Alertas de postura de riesgo por paciente
SELECT p."Nombre" AS paciente,
       COUNT(*) FILTER (WHERE ev."Estado" = 'Peligro') AS alertas,
       COUNT(*)                                        AS total,
       ROUND(100.0 * COUNT(*) FILTER (WHERE ev."Estado" = 'Peligro') / NULLIF(COUNT(*), 0), 1) AS pct_riesgo
FROM "Pacientes" p
LEFT JOIN "EvaluacionesPosturales" ev ON ev."UsuarioId" = p."Id"
GROUP BY p."Nombre"
ORDER BY alertas DESC;

-- 3. Métricas B2B por clínica (lo que vería el cliente que paga la suscripción)
SELECT c."Nombre" AS clinica,
       c."Plan",
       COUNT(DISTINCT p."Id")  AS pacientes,
       COUNT(ev."Id")          AS evaluaciones,
       COUNT(ev."Id") FILTER (WHERE ev."Estado" = 'Peligro') AS alertas
FROM "Clinicas" c
LEFT JOIN "Pacientes" p               ON p."ClinicaId" = c."Id"
LEFT JOIN "EvaluacionesPosturales" ev ON ev."UsuarioId" = p."Id"
GROUP BY c."Nombre", c."Plan";

-- 4. Ejercicios más realizados (historial de ejercicios)
SELECT e."Nombre", e."Fase", COUNT(ev."Id") AS repeticiones
FROM "Ejercicios" e
LEFT JOIN "EvaluacionesPosturales" ev ON ev."EjercicioId" = e."Id"
GROUP BY e."Nombre", e."Fase"
ORDER BY repeticiones DESC;

-- 5. Actividad por día
SELECT DATE(ev."FechaHora") AS dia,
       COUNT(*) AS evaluaciones,
       COUNT(*) FILTER (WHERE ev."Estado" = 'Peligro') AS alertas
FROM "EvaluacionesPosturales" ev
GROUP BY DATE(ev."FechaHora")
ORDER BY dia DESC;
