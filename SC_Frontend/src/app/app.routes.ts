import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    // 1. REDIRECCIÓN INICIAL
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    },
    
    // 2. RUTAS PÚBLICAS DE AUTENTICACIÓN (Sin Layout, accesibles para ciudadanos externos)
    {
        path: 'login',
        loadComponent: () => import('./pages/login/login').then(m => m.Login)
    },
    {
        path: 'auth/registro-postulante',
        loadComponent: () => import('./pages/auth/registro-postulante/registro-postulante').then(m => m.RegistroPostulante)
    },
    {
        path: 'auth/recuperar-clave',
        loadComponent: () => import('./pages/auth/solicitar-recuperacion/solicitar-recuperacion').then(m => m.SolicitarRecuperacion)
    },
    { 
        path: 'auth/restablecer-password', 
        loadComponent: () => import('./pages/auth/recuperar-clave/recuperar-clave').then(m => m.RecuperarClave) 
    },

    // 3. RUTAS PRIVADAS / INTRANET (Protegidas por Guard y envueltas en el Layout de navegación)
    {
        path: '',
        loadComponent: () => import('./pages/layout/layout').then(m => m.Layout),
        canActivate: [authGuard],
        children: [
            {
                path: 'dashboard',
                loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
            },
            {
                path: 'admin/mantenedores',
                loadComponent: () => import('./pages/mantenedores/mantenedores').then(m => m.Mantenedores)
            },
            {
                path: 'admin/config-sistema',
                loadComponent: () => import('./pages/parametros/parametros').then(m => m.Parametros)
            },
            {
                path: 'postulante/datos-personales',
                loadComponent: () => import('./pages/postulante/datos-personales/datos-personales').then(m => m.DatosPersonales)
            },
            {
                path: 'postulante/formacion',
                loadComponent: () => import('./pages/postulante/formacion/formacion').then(m => m.Formacion)
            },
            {
                path: 'postulante/certificaciones',
                loadComponent: () => import('./pages/postulante/certificacion/certificacion').then(m => m.Certificacion)
            },
            {
                path: 'postulante/experiencia',
                loadComponent: () => import('./pages/postulante/experiencia/experiencia').then(m => m.Experiencia)
            },
            {
                path: 'postulante/colegiatura',
                loadComponent: () => import('./pages/postulante/colegiatura/colegiatura').then(m => m.Colegiatura)
            },
            {
                path: 'postulante/idiomas',
                loadComponent: () => import('./pages/postulante/idioma/idioma').then(m => m.Idiomas)
            },
            {
                path: 'postulante/ofimatica',
                loadComponent: () => import('./pages/postulante/ofimatica/ofimatica').then(m => m.Ofimatica)
            },
            {
                path: 'postulante/otros-requisitos',
                loadComponent: () => import('./pages/postulante/otros-requisitos/otros-requisitos').then(m => m.OtrosRequisitos)
            },
            {
                path: 'postulante/informacion-adicional',
                loadComponent: () => import('./pages/postulante/info-adicional/info-adicional').then(m => m.InformacionAdicional)
            },
            {
                path: 'postulante/firma',
                loadComponent: () => import('./pages/postulante/firma-digitalizada/firma-digitalizada').then(m => m.FirmaDigitalizada)
            },
            {
                path: 'postulante/ficha-resumen',
                loadComponent: () => import('./pages/postulante/ficha-resumen/ficha-resumen').then(m => m.FichaResumen)
            },
            {
                path: 'postulante/declaraciones',
                loadComponent: () => import('./pages/postulante/declaraciones-juradas/declaraciones-juradas').then(m => m.DeclaracionesJuradas)
            },
            {
                path: 'postulante/buscar-plazas',
                loadComponent: () => import('./pages/buscar-plazas/buscar-plazas').then(m => m.BuscarPlazas)
            },
            {
                path: 'postulante/resumen',
                loadComponent: () => import('./pages/resumen-postulaciones/resumen-postulaciones').then(m => m.ResumenPostulaciones)
            },
            {
                path: 'postulante/notificaciones',
                loadComponent: () => import('./pages/postulante/alertas-notificaciones/alertas-notificaciones').then(m => m.AlertasNotificaciones)
            },
            {
                path: 'postulante/consultas-reclamos',
                loadComponent: () => import('./pages/postulante/consultas-reclamos/consultas-reclamos').then(m => m.ConsultasReclamos)
            },
            {
                path: 'admin/bandeja-consultas',
                loadComponent: () => import('./pages/admin/bandeja-consultas/bandeja-consultas').then(m => m.BandejaConsultas)
            },
            {
                path: 'admin/log-procesos',
                loadComponent: () => import('./pages/admin/trazabilidad-postulaciones/trazabilidad-postulaciones').then(m => m.TrazabilidadPostulaciones)
            },
            {
                path: 'admin/logs-auditoria',
                loadComponent: () => import('./pages/admin/logs-auditoria/logs-auditoria').then(m => m.LogsAuditoria)
            }
        ]
    },

    // 4. CAPTURA DE RUTAS INEXISTENTES (Comodín)
    {
        path: '**',
        redirectTo: 'login'
    }
];