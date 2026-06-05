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
            }
        ]
    },

    // 4. CAPTURA DE RUTAS INEXISTENTES (Comodín)
    {
        path: '**',
        redirectTo: 'login'
    }
];