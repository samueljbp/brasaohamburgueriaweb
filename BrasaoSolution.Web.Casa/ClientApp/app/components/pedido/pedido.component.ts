import { Component, Inject } from '@angular/core';
import { ViewChild } from '@angular/core';
import { ElementRef } from '@angular/core';
import { Comanda } from '../../model/Comanda';
import { GlobalData } from '../../GlobalData';
import { EmpresaViewModel } from '../../model/EmpresaViewModel';
import * as globals from '../../GlobalVariables';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import * as jquery from 'jquery';
import { ItemCardapioViewModel } from '../../model/ItemCardapioViewModel';

@Component({
    selector: 'pedido',
    templateUrl: './pedido.component.html',
    styleUrls: ['./pedido.component.css']
})
export class PedidoComponent {
    public comanda: Comanda;
    public empresa: EmpresaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    public urlBase: string;
    @ViewChild('modalImagem') modalImagem: ElementRef;
    public urlImagemGrande: string;
    private lock: boolean = false;

    constructor(@Inject('BASE_URL') baseUrl: string) {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.empresa = globals.globalData.empresa;
        this.urlBase = baseUrl;

    }

    mostraImagem(imagem: string) {
        this.urlImagemGrande = imagem;
        jquery('#modalImagem').modal();
    }

    selecionaItem(item: ItemCardapioViewModel) {
        alert(item.nome);
    }

}