import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    },
    {
        path: 'login',
        loadComponent: () => import('./pages/login/login').then(m => m.Login)
    },
    {
        path: '',
        loadComponent: () => import('./pages/layout/layout').then(m => m.Layout),
        canActivate: [authGuard], // Protege todas las rutas hijas automáticamente
        children: [
            {
                path: 'dashboard',
                loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
            }
        ]
    },
    {
        path: '**',
        redirectTo: 'login'
    }
];
