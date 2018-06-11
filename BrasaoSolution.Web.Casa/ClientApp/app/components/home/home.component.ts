import { Component, OnInit } from '@angular/core';
import { Comanda } from '../../model/Comanda';
import { GlobalData } from '../../GlobalData';
import { Router } from '@angular/router';
import { ItemComanda } from '../../model/ItemComanda';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    public comanda: Comanda;

    constructor(
        private router: Router,
        private globalData: GlobalData) {

        this.comanda = globalData.comanda;

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
