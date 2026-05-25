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
        canActivate: [authGuard],
        children: [
            {
                path: 'dashboard',
                loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
            },
            {
                path: 'admin/mantenedores',
                loadComponent: () => import('./pages/mantenedores/mantenedores').then(m => m.Mantenedores)
            }
        ]
    },
    {
        path: '**',
        redirectTo: 'login'
    }
];
