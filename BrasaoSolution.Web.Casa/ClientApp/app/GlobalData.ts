import { Injectable } from "@angular/core";
import { EmpresaViewModel } from "./model/EmpresaViewModel";
import { Comanda } from "./model/Comanda";
import { ClasseItemCardapioViewModel } from "./model/ClasseItemCardapioViewModel";

@Injectable()
export class GlobalData {

    initialized: boolean = false;
    codEmpresa: number;
    numMesa: number;
    empresa: EmpresaViewModel = new EmpresaViewModel();
    comanda: Comanda = new Comanda();
    cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();

}