import { Component } from '@angular/core';
import { Comanda } from '../../model/Comanda';
import { GlobalData } from '../../GlobalData';
import { Router } from '@angular/router';
import { Empresa } from '../../model/Empresa';
import { GlobalDataService } from '../../GlobalData.service';
import * as globals from '../../GlobalVariables';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    public comanda: Comanda;
    public empresa: Empresa;
    public cardapio: ClasseItemCardapioViewModel[];

    constructor(
        private router: Router) {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.empresa = globals.globalData.empresa;

        this.data = new Observable<Array<any>>(observer => {
            this.dataObserver = observer;
        });

    }

}