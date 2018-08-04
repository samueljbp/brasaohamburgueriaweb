import { Component, Inject, ChangeDetectorRef } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { ComandaViewModel } from '../../model/ComandaViewModel';
import * as globals from '../../globals';
import { ItemComandaViewModel, TipoAcaoRegistro } from '../../model/ItemComandaViewModel';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ConfirmationDialog } from '../confirmDialog/confirmDialog.component';
import { MatSnackBar } from '@angular/material';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import { GlobalDataService } from '../../services/globalData.service';
import { Observable } from "rxjs/Observable";
import { BehaviorSubject } from 'rxjs';
import { ListaPedidoDataSource } from '../../datasources/listaPedido.datasource';
import { ComandaService } from '../../services/comanda.service';
import * as jquery from 'jquery';


@Component({
    selector: 'listaComanda',
    templateUrl: './listaComanda.component.html',
    styleUrls: ['./listaComanda.component.css'],
    animations: [
        trigger('detailExpand', [
            state('collapsed', style({ height: '0px', minHeight: '0', display: 'none' })),
            state('expanded', style({ height: '*' })),
            transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
        ]),
    ],
})
export class ListaComandaComponent {
    public comanda: ComandaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    public urlBase: string;
    public datasource: any;
    dataChange: BehaviorSubject<ItemComandaViewModel[]>;
    displayedColumns: string[] = ['seqItem', 'nome', 'quantidade', 'precoUnitario', 'valorTotal'];
    expandedElement: ItemComandaViewModel | null;
    public valorTotalComanda: number = 0;
    public mensagemErro: string | null = null;

    constructor(@Inject('BASE_URL') baseUrl: string,
        private data: GlobalDataService,
        private comandaService: ComandaService,
        private router: Router,
        private route: ActivatedRoute) {

        if (!globals.globalData.comanda.itens.length || globals.globalData.comanda.itens.length <= 0) {
            this.router.navigate(['/home']);
        }

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.valorTotalComanda = this.comanda.getValorTotalComanda();
        this.urlBase = baseUrl;

        this.dataChange = new BehaviorSubject<ItemComandaViewModel[]>(this.comanda.getItensComandaMostrar());
        this.datasource = new ListaPedidoDataSource(this.dataChange);
        this.mensagemErro = null;
    }

    connect() {

    }
    
}