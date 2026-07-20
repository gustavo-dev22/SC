import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink, RouterLinkActive, RouterOutlet, NavigationEnd } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../../services/auth.service';
import { PostulacionService } from '../../services/postulacion.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { filter } from 'rxjs';
import { SessionTimeoutService } from '../../core/services/session-timeout.service';

@Component({
  selector: 'app-layout',
  imports: [CommonModule, 
            RouterOutlet, 
            RouterLink, 
            RouterLinkActive, 
            MatSidenavModule, 
            MatToolbarModule, 
            MatListModule, 
            MatButtonModule, 
            MatIconModule, 
            MatMenuModule,
            MatFormFieldModule,
            MatSelectModule],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout implements OnInit, OnDestroy {
  private router = inject(Router);
  private _postulacionService = inject(PostulacionService);
  private authService = inject(AuthService);
  private sessionTimeoutService = inject(SessionTimeoutService);

  public nombreUsuario = signal<string>('Usuario');
  public rolUsuario = signal<string>('Postulante');
  public menuItems = signal<any[]>([]);

  public misConvocatorias = this._postulacionService.misPostulacionesActive;
  public plazaSeleccionada = this._postulacionService.plazaContextoSeleccionada;

  mostrarSelectorCV = signal<boolean>(false);

  constructor() {
    // Escuchar los cambios de ruta en el sistema
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      const urlActual = event.urlAfterRedirects || event.url;
      
      // Lista de las rutas exactas donde SÍ debe aparecer el selector
      const rutasCurriculum = [
        '/postulante/formacion',
        '/postulante/colegiatura',
        '/postulante/idiomas',
        '/postulante/ofimatica',
        '/postulante/certificaciones',
        '/postulante/experiencia',
        '/postulante/otros-requisitos'
      ];

      // Si la URL actual empieza con alguna de tus rutas, se vuelve true
      const esRutaCV = rutasCurriculum.some(ruta => urlActual.includes(ruta));
      this.mostrarSelectorCV.set(esRutaCV);
    });
  }

  ngOnInit(): void {
    this.sessionTimeoutService.iniciarMonitoreo();
    this.cargarPerfilYMenus();

    const profileStr = sessionStorage.getItem('user_profile');
    if (profileStr) {
      try {
        const profile = JSON.parse(profileStr);
        
        if (profile && profile.token) {
          // DETECCIÓN: Si el token contiene puntos ".", es un JWT de Admin/Comité.
          // No es un token simple de Postulante, por lo que NO debemos usar tu lógica de guiones.
          if (profile.token.includes('.')) {
            // Es un rol administrativo. Si en el futuro necesitas el id del Admin, 
            // podrías decodificar el payload del JWT aquí. Por ahora, lo ignoramos limpiamente.
            return; 
          }

          // Si no tiene puntos, asumimos que es el token codificado del Postulante
          try {
            const tokenDecodificado = atob(profile.token);
            const tokenParts = tokenDecodificado.split('-');
            
            if (tokenParts.length > 1) {
              const idPostulante = Number(tokenParts[1]);
              if (idPostulante && !isNaN(idPostulante)) {
                this._postulacionService.cargarContextoPostulaciones(idPostulante).subscribe();
              }
            }
          } catch (atobError) {
            // Este catch solo saltará si un token de postulante real viniera corrupto
            console.error('Error al decodificar un token de postulante:', atobError);
          }
        }
      } catch (jsonError) {
        console.error('Error al parsear el "user_profile":', jsonError);
      }
    }
  }

  ngOnDestroy(): void {
    // 🚀 Se destruyen los escuchadores si el usuario sale manualmente del Layout
    this.sessionTimeoutService.detenerMonitoreo();
  }

  public onCambioGlobalPlaza(idPlaza: number): void {
    this._postulacionService.cambiarContextoPlaza(idPlaza);
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
