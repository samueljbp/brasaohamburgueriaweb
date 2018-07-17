import { Component } from '@angular/core';
import { ComandaViewModel } from '../../model/ComandaViewModel';
import { EmpresaViewModel } from '../../model/EmpresaViewModel';
import * as globals from '../../globals';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import { GlobalDataService } from '../../services/globalData.service';
import { GlobalData } from '../../model/GlobalData';

@Component({
    selector: 'rodape',
    templateUrl: './rodape.component.html',
    styleUrls: ['./rodape.component.css']
})
export class RodapeComponent {
    public comanda: ComandaViewModel;
    public empresa: EmpresaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    public message: string;
    public quantidadeItensPendentes: number;

    constructor(private data: GlobalDataService) {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        
        this.data.currentMessage.subscribe(message =>
            this.atualizaDados(message)
        );

    }

    atualizaDados(globalData: GlobalData) {
        this.comanda = globalData.comanda;
        this.quantidadeItensPendentes = globalData.comanda.quantidadeItensPendentesGravacao();
    }

}
