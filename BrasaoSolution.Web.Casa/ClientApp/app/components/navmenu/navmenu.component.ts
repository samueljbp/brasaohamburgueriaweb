import { Component } from '@angular/core';
import { Comanda } from '../../model/Comanda';
import { GlobalData } from '../../GlobalData';
import { EmpresaViewModel } from '../../model/EmpresaViewModel';
import * as globals from '../../GlobalVariables';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    public comanda: Comanda;
    public empresa: EmpresaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();

    constructor() {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.empresa = globals.globalData.empresa;

    }

}