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

@Component({
  selector: 'app-layout',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, MatSidenavModule, MatToolbarModule, MatListModule, MatButtonModule, MatIconModule, MatMenuModule],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout implements OnInit {
  private router = inject(Router);

  // Signals reactivos para la UI
  public nombreUsuario = signal<string>('Usuario');
  public rolUsuario = signal<string>('Postulante');
  public menuItems = signal<any[]>([]); // Almacena el árbol estructurado final

  ngOnInit(): void {
    this.cargarPerfilYMenus();
  }

  cargarPerfilYMenus(): void {
    const rawProfile = sessionStorage.getItem('user_profile');
    if (!rawProfile) {
      this.logoutSencillo();
      return;
    }

    const profile = JSON.parse(rawProfile);
    this.nombreUsuario.set(profile.nombreCompleto);
    this.rolUsuario.set(profile.rol);

    // ANALISIS SENIOR: Agrupar submenús debajo de sus respectivos menús padres
    // La estructura de SASI nos da los objetos planos. Los mapearemos eficientemente:
    if (profile.menus && Array.isArray(profile.menus)) {
      this.menuItems.set(this.estructurarMenuSasi(profile.menus));
    }
  }

  private estructurarMenuSasi(flatMenus: any[]): any[] {
    // Filtramos los elementos principales de tipo "Menu"
    const principales = flatMenus.filter(m => m.tipo === 'Menu');
    
    // Filtramos los de tipo "Submenu"
    const submenus = flatMenus.filter(m => m.tipo === 'Submenu');

    // Mapeamos cada menú principal y le asignamos sus hijos correspondientes
    // En el JSON que nos diste, las URLs como "Admin/TiposDocumentos" se asocian lógicamente.
    return principales.map(parent => {
      // Como patrón de coincidencia estándar para SASI, buscamos si el submenú comparte la raíz de la URL o estructura
      // Para hacerlo flexible, si SASI no te da un IdPadre explícito en este nodo, los agrupamos de forma predictiva:
      const hijos = submenus.filter(sub => {
        if (parent.nombre === 'Administración' && sub.url.startsWith('Admin/')) return true;
        if (parent.nombre === 'Reportes' && sub.url.startsWith('Reporte/')) return true;
        if (parent.nombre === 'Bandejas' && sub.url.startsWith('Bandeja/')) return true;
        return false;
      });

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
        this.logoutSencillo();
      }
    });
  }

  private logoutSencillo(): void {
    // Limpieza absoluta de la sesión de información
    sessionStorage.clear();
    // Redirección forzada al Login
    this.router.navigate(['/login']);
  }
}
