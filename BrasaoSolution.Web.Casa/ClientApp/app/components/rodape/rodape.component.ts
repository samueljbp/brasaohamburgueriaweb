import { Component } from '@angular/core';
import { Comanda } from '../../model/Comanda';
import { GlobalData } from '../../GlobalData';
import { Router } from '@angular/router';
import { Empresa } from '../../model/Empresa';

@Component({
    selector: 'rodape',
    templateUrl: './rodape.component.html',
    styleUrls: ['./rodape.component.css']
})
export class RodapeComponent {
    public comanda: Comanda;
    public empresa: Empresa;

    constructor(
        private router: Router,
        private globalData: GlobalData) {

        this.comanda = globalData.comanda;
        this.empresa = globalData.empresa;

    }

}
