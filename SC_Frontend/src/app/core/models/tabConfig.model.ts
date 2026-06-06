import { Type } from "@angular/core";

export interface TabConfig {
  id: string;
  titulo: string;
  icono: string;
  componente: Type<any>;
  verificarFlag: (flags: any) => boolean;
  inputsComponente: any;
}