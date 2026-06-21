import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-layout',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, MatSidenavModule, MatToolbarModule, MatListModule, MatButtonModule, MatIconModule, MatMenuModule],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);

  // Signals reactivos para la UI
  public nombreUsuario = signal<string>('Usuario');
  public rolUsuario = signal<string>('Postulante');
  public menuItems = signal<any[]>([]); // Almacena el árbol estructurado final

  ngOnInit(): void {
    this.cargarPerfilYMenus();
  }

  cargarPerfilYMenus(): void {
    try {
      const rawProfile = sessionStorage.getItem('user_profile');
      if (!rawProfile) {
        this.authService.logout(); // ← delegar al servicio
        return;
      }

      const profile = JSON.parse(rawProfile); // ← ahora protegido por try/catch
      this.nombreUsuario.set(profile.nombreCompleto);
      this.rolUsuario.set(profile.rol);

      if (profile.menus && Array.isArray(profile.menus)) {
        this.menuItems.set(this.estructurarMenuSasi(profile.menus));
      }
    } catch {
      this.authService.logout(); // ← JSON corrupto → sesión inválida → logout
    }
  }

  private estructurarMenuSasi(flatMenus: any[]): any[] {
    // 1. Filtramos los menús principales (los que tienen tipo 'Menu' o idPadre nulo)
    const principales = flatMenus.filter(m => 
      m.idPadre === null || m.tipo === 'Menu' || m.tipo === 'M'
    );
    
    // 2. Filtramos los submenús
    const submenus = flatMenus.filter(m => 
      m.idPadre !== null && (m.tipo === 'Submenu' || m.tipo === 'S')
    );

    // 3. Relacionamos de forma matemática estricta por ID
    return principales.map(parent => {
      // Buscamos todos los submenús cuyo idPadre coincida exactamente con el idObjeto de este menú
      const hijos = submenus.filter(sub => sub.idPadre === parent.idObjeto);

      return {
        ...parent,
        hijos: hijos.length > 0 ? hijos : null
      };
    });
  }

  confirmarCerrarSesion(): void {
    Swal.fire({
      title: '¿Cerrar Sesión?',
      text: '¿Está seguro de que desea salir del Sistema de Convocatorias?',
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#1e3c72',
      cancelButtonColor: '#7f8c8d',
      confirmButtonText: 'Sí, Salir',
      cancelButtonText: 'Cancelar',
      heightAuto: false
    }).then((result) => {
      if (result.isConfirmed) {
        this.authService.logout();
      }
    });
  }
}
