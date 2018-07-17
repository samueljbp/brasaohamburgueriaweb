import { Injectable } from "@angular/core";
import { EmpresaViewModel } from "../model/EmpresaViewModel";
import { ComandaViewModel } from "../model/ComandaViewModel";
import { ClasseItemCardapioViewModel } from "../model/ClasseItemCardapioViewModel";
import { ItemCardapioViewModel } from "../model/ItemCardapioViewModel";
import { DadosItemCardapioViewModel } from "./DadosItemCardapioViewModel";

@Injectable()
export class GlobalData {

    initialized: boolean = false;
    botaoVoltarVisivel: boolean = true;
    codEmpresa: number;
    numMesa: number;
    empresa: EmpresaViewModel = new EmpresaViewModel();
    comanda: ComandaViewModel = new ComandaViewModel();
    cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    itemSelecionado: ItemCardapioViewModel = new ItemCardapioViewModel();
    dadosItemSelecionado: DadosItemCardapioViewModel = new DadosItemCardapioViewModel();

}