export interface LoginRequest {
  username: string;
  password: string;
  isExternal: boolean;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  data: {
    token: string;
    nombre: string;
    rol: string;
  };
}

export interface ApiResponse<T = null> {
  success: boolean;
  message: string;
  data?: T;
  linkDesarrollo?: string;
}

export interface RegistroPostulanteRequest {
  numDocumento: string;
  nombres: string;
  apellidoPaterno: string;
  apellidoMaterno: string;
  correo: string;
  password: string;
}