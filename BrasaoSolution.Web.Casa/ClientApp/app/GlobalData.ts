import { Injectable } from "@angular/core";
import { Empresa } from "./model/Empresa";
import { Comanda } from "./model/Comanda";
import { ClasseItemCardapioViewModel } from "./model/ClasseItemCardapioViewModel";

@Injectable()
export class GlobalData {

    empresa: Empresa;
    comanda: Comanda;
    cardapio: ClasseItemCardapioViewModel[];

}