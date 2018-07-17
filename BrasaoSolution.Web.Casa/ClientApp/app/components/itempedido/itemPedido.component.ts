import { Component, Inject } from '@angular/core';
import { ComandaViewModel } from '../../model/ComandaViewModel';
import * as globals from '../../globals';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import { ItemCardapioViewModel } from '../../model/ItemCardapioViewModel';
import { ItemComandaViewModel, TipoAcaoRegistro } from '../../model/ItemComandaViewModel';
import { GlobalDataService } from '../../services/globalData.service';
import { ActivatedRoute, Router, NavigationStart, NavigationEnd, NavigationCancel, NavigationError, Event } from '@angular/router';
import { DadosItemCardapioViewModel } from '../../model/DadosItemCardapioViewModel';
import { ExtraItemComandaViewModel } from '../../model/ExtraItemComandaViewModel';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ConfirmationDialog } from '../confirmDialog/confirmDialog.component';
import { MatSnackBar } from '@angular/material';

@Component({
    selector: 'itemPedido',
    templateUrl: './itemPedido.component.html',
    styleUrls: ['./itemPedido.component.css']
})
export class ItemPedidoComponent {
    public comanda: ComandaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    public itemSelecionado: ItemCardapioViewModel;
    public dadosItemSelecionado: DadosItemCardapioViewModel;
    public urlBase: string;
    public urlImagemGrande: string;
    private lock: boolean = false;
    public novoItem: ItemComandaViewModel;
    public dialogRef: MatDialogRef<ConfirmationDialog>;
    public quantidadeInvalida: boolean = false;

    constructor(@Inject('BASE_URL') baseUrl: string, private data: GlobalDataService, private router: Router, private route: ActivatedRoute, public dialog: MatDialog, public snackBar: MatSnackBar) {

        if (!globals.globalData.itemSelecionado.codItemCardapio) {
            this.router.navigate(['/pedido']);
        }

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.urlBase = baseUrl;
        this.itemSelecionado = globals.globalData.itemSelecionado;
        this.dadosItemSelecionado = globals.globalData.dadosItemSelecionado;

        this.novoItem = new ItemComandaViewModel();
        this.novoItem.seqItem = this.comanda.getNextSeqItem();
        this.novoItem.codItem = this.itemSelecionado.codItemCardapio;
        this.novoItem.codCombo = this.itemSelecionado.codCombo;
        this.novoItem.acaoRegistro = TipoAcaoRegistro.EmInclusao;
        this.novoItem.codPromocaoVenda = this.itemSelecionado.codPromocaoVenda;
        this.novoItem.descricaoCombo = this.itemSelecionado.nome;
        this.novoItem.descricaoItem = this.itemSelecionado.nome;
        this.novoItem.percentualDesconto = this.itemSelecionado.percentualDesconto;
        this.novoItem.precoCombo = this.itemSelecionado.precoCombo;
        this.novoItem.precoUnitario = this.itemSelecionado.preco;
        this.novoItem.precoUnitarioComDesconto = this.itemSelecionado.precoComDesconto;

        this.comanda.addItem(this.novoItem);
        this.data.changeMessage(globals.globalData);

    }

    incrementaQuantidade() {
        this.novoItem.incrementaQuantidade();
        this.comanda.calculaTotal();
        this.data.changeMessage(globals.globalData);
        this.quantidadeInvalida = this.novoItem.quantidade <= 0;
    }

    decrementaQuantidade() {
        this.novoItem.decrementaQuantidade();
        this.comanda.calculaTotal();
        this.data.changeMessage(globals.globalData);
        this.quantidadeInvalida = this.novoItem.quantidade <= 0;
    }

    atualizaValorTotalItem() {
        this.novoItem.calculaValorTotalItem();
    }

    onNgModelChange(event: any) {
        this.novoItem.calculaValorTotalItem();
        this.comanda.calculaTotal();
        this.data.changeMessage(globals.globalData);
    }

    confirmaAbandonarPedido() {
        this.dialogRef = this.dialog.open(ConfirmationDialog, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Tem certeza que deseja abandonar o pedido?"

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.comanda.removeItem(this.novoItem.seqItem);
                this.router.navigate(['/pedido']);
            }
        });
    }

    confirmaRegistrarPedido() {
        this.dialogRef = this.dialog.open(ConfirmationDialog, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Confirma o pedido?"

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.quantidadeInvalida = this.novoItem.quantidade <= 0;

                if (this.quantidadeInvalida) {
                    this.snackBar.open("Quantidade inválida", "Fechar", {
                        duration: 2000,
                        panelClass: 'snackbarClass'
                    });
                } else {
                    this.novoItem.acaoRegistro = TipoAcaoRegistro.Incluir;
                    this.router.navigate(['/pedido']);
                }
                
            }
        });
    }
}