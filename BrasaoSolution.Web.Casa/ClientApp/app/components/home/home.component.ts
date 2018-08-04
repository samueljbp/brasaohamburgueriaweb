import { Component, OnInit } from '@angular/core';
import { ComandaViewModel } from '../../model/ComandaViewModel';
import { ItemComandaViewModel } from '../../model/ItemComandaViewModel';
import * as globals from '../../globals';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import { ItemCardapioViewModel } from '../../model/ItemCardapioViewModel';
import { GlobalDataService } from '../../services/globalData.service';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ConfirmationDialog } from '../confirmDialog/confirmDialog.component';
import { Observable } from "rxjs/Observable";

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    public comanda: ComandaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    public dialogRef: MatDialogRef<ConfirmationDialog>;

    constructor(private data: GlobalDataService,
                public dialog: MatDialog) {

        this.cardapio = globals.globalData.cardapio;
        globals.globalData.comanda.removeLixo();
        this.comanda = globals.globalData.comanda;
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

    pedeConta() {
        this.dialogRef = this.dialog.open(ConfirmationDialog, {
            disableClose: false
        });
        this.dialogRef.componentInstance.confirmMessage = "Ao pedir a conta, você não poderá mais consumir. Deseja continuar?";

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {

                this.comanda.bloqueiaComanda();
                globals.globalData.comanda = this.comanda;
                this.data.changeMessage(globals.globalData);

            }
        });
    }
}
