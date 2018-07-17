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
    selector: 'listaPedido',
    templateUrl: './listaPedido.component.html',
    styleUrls: ['./listaPedido.component.css'],
    animations: [
        trigger('detailExpand', [
            state('collapsed', style({ height: '0px', minHeight: '0', display: 'none' })),
            state('expanded', style({ height: '*' })),
            transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
        ]),
    ],
})
export class ListaPedidoComponent {
    public comanda: ComandaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    public urlBase: string;
    public dialogRef: MatDialogRef<ConfirmationDialog>;
    public datasource: any;
    dataChange: BehaviorSubject<ItemComandaViewModel[]>;
    displayedColumns: string[] = ['seqItem', 'nome', 'quantidade', 'precoUnitario', 'valorTotal', 'remover'];
    expandedElement: ItemComandaViewModel | null;
    public valorTotalPedido: number = 0;
    public mensagemErro: string | null = null;

    constructor(@Inject('BASE_URL') baseUrl: string,
        private data: GlobalDataService,
        private comandaService: ComandaService,
        private router: Router,
        private route: ActivatedRoute,
        public dialog: MatDialog,
        public snackBar: MatSnackBar) {

        if (!globals.globalData.comanda.itens.length || globals.globalData.comanda.itens.length <= 0) {
            this.router.navigate(['/home']);
        }

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.valorTotalPedido = this.comanda.getValorTotalPedido();
        this.urlBase = baseUrl;

        this.dataChange = new BehaviorSubject<ItemComandaViewModel[]>(this.comanda.getItensMostrar());
        this.datasource = new ListaPedidoDataSource(this.dataChange);
        this.mensagemErro = null;
    }

    connect() {

    }

    confirmaExclusaoItem(item: ItemComandaViewModel) {
        this.dialogRef = this.dialog.open(ConfirmationDialog, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Confirma a exclusão do item " + item.seqItem + " do pedido?"

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {

                this.comanda.removeItem(item.seqItem);
                this.dataChange.next(this.comanda.itens);
                this.data.changeMessage(globals.globalData);

            }
        });
    }

    confirmaPedido() {
        this.mensagemErro = null;

        this.dialogRef = this.dialog.open(ConfirmationDialog, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Confirma o envio dos itens para preparação?"

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.comandaService.registraPedido(this.comanda).subscribe((response) => {
                    if (response.ok) {
                        this.comanda = jquery.extend(true, globals.globalData.comanda, response.json(), globals.globalData.comanda);
                        globals.globalData.comanda = this.comanda;

                        this.data.changeMessage(globals.globalData);
                        this.snackBar.open("Pedido gravado com sucesso!", "Fechar", {
                            duration: 2000,
                            panelClass: 'snackbarClass'
                        });
                        this.router.navigate(['/home']);
                    } else {
                        this.snackBar.open("Falha na transação", "Fechar", {
                            duration: 2000,
                            panelClass: 'snackbarClass'
                        });
                        this.mensagemErro = response.statusText;
                    }

                });
            }
        });
    }
}