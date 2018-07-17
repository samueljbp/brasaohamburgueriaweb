import { Component, OnInit } from '@angular/core';
import { ComandaViewModel } from '../../model/ComandaViewModel';
import { ItemComandaViewModel } from '../../model/ItemComandaViewModel';
import * as globals from '../../globals';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import { ItemCardapioViewModel } from '../../model/ItemCardapioViewModel';
import { GlobalDataService } from '../../services/globalData.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    public comanda: ComandaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();

    constructor(private data: GlobalDataService) {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.comanda.removeLixo();
        globals.globalData.botaoVoltarVisivel = false;
        this.data.changeMessage(globals.globalData);

    }

    ngOnInit(): void {


    }

    novoPedido() {
        let item: ItemComandaViewModel = new ItemComandaViewModel();
        item.valorTotal = 10;
        this.comanda.addItem(item);
    }
}
