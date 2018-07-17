import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { ComandaViewModel } from '../../model/ComandaViewModel';
import { EmpresaViewModel } from '../../model/EmpresaViewModel';
import * as globals from '../../globals';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import * as jquery from 'jquery';
import { ItemCardapioViewModel } from '../../model/ItemCardapioViewModel';
import { GlobalDataService } from '../../services/globalData.service';

@Component({
    selector: 'pedido',
    templateUrl: './pedido.component.html',
    styleUrls: ['./pedido.component.css']
})
export class PedidoComponent {
    public comanda: ComandaViewModel;
    public empresa: EmpresaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    public urlBase: string;
    public urlImagemGrande: string;
    private lock: boolean = false;
    public loading: boolean = false;
    public itemSelecionado: ItemCardapioViewModel | null = null;

    constructor(@Inject('BASE_URL') baseUrl: string, private router: Router, private data: GlobalDataService) {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.empresa = globals.globalData.empresa;
        this.urlBase = baseUrl;
        this.loading = false;

        globals.globalData.botaoVoltarVisivel = true;
        this.data.changeMessage(globals.globalData);
    }

    mostraImagem(imagem: string) {
        this.urlImagemGrande = imagem;
        jquery('#modalImagem').modal();
    }

    selecionaItem(item: ItemCardapioViewModel) {
        this.itemSelecionado = item;
        this.loading = true;
        globals.globalData.itemSelecionado = item;
        this.router.navigate(['/itemPedido']).catch(error => { console.log(error); this.loading = false; });
    }

}