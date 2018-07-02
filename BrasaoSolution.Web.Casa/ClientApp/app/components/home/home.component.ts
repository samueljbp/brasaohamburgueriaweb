import { Component, OnInit } from '@angular/core';
import { Comanda } from '../../model/Comanda';
import { GlobalData } from '../../GlobalData';
import { ItemComanda } from '../../model/ItemComanda';
import * as globals from '../../GlobalVariables';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import { ItemCardapioViewModel } from '../../model/ItemCardapioViewModel';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    public comanda: Comanda;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();

    constructor() {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;

    }

    ngOnInit(): void {


    }

    novoPedido() {
        console.log(this.comanda.itens.length);
        let item: ItemComanda = new ItemComanda();
        item.valorTotalItem = 10;
        this.comanda.addItem(item);
    }
}
