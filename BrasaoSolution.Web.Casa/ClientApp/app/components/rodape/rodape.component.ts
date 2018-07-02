import { Component } from '@angular/core';
import { Comanda } from '../../model/Comanda';
import { GlobalData } from '../../GlobalData';
import { EmpresaViewModel } from '../../model/EmpresaViewModel';
import * as globals from '../../GlobalVariables';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';

@Component({
    selector: 'rodape',
    templateUrl: './rodape.component.html',
    styleUrls: ['./rodape.component.css']
})
export class RodapeComponent {
    public comanda: Comanda;
    public empresa: EmpresaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();

    constructor() {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;

    }

}
