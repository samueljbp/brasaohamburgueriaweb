import { NgModule, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { RodapeComponent } from './components/rodape/rodape.component';
import { PedidoComponent } from './components/pedido/pedido.component';
import { ItemPedidoComponent } from './components/itempedido/itemPedido.component';
import { FilterPipe } from './pipes/filter.pipe';
import { BrowserModule } from '@angular/platform-browser';
import { DadosItemCardapioResolver } from './resolvers/dadosItemCardapio.resolver';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { MatSliderModule } from '@angular/material/slider';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { MatDialogModule } from '@angular/material/dialog';
import { ConfirmationDialog } from './components/confirmDialog/confirmDialog.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ListaPedidoComponent } from './components/listapedido/listaPedido.component';
import { ErroTokenComponent } from './components/erroToken/erroToken.component';
import { ListaComandaComponent } from './components/listacomanda/listaComanda.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        RodapeComponent,
        PedidoComponent,
        ItemPedidoComponent,
        ListaPedidoComponent,
        ListaComandaComponent,
        ErroTokenComponent,
        FilterPipe,
        ConfirmationDialog
    ],
    imports: [
        CommonModule,
        HttpModule,
        BrowserModule,
        BrowserAnimationsModule,
        FormsModule,
        ReactiveFormsModule,
        MatSliderModule,
        MatButtonModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        MatProgressSpinnerModule,
        MatListModule,
        MatDialogModule,
        MatSnackBarModule,
        MatExpansionModule,
        MatTableModule,
        MatTooltipModule,
        RouterModule.forRoot([
            {
                path: '', redirectTo: 'home', pathMatch: 'full'
            },
            { path: 'home', component: HomeComponent },
            { path: 'pedido', component: PedidoComponent },
            { path: 'erroToken', component: ErroTokenComponent },
            { path: 'listaPedido', component: ListaPedidoComponent },
            { path: 'listaComanda', component: ListaComandaComponent },
            { path: 'itemPedido', component: ItemPedidoComponent, resolve: { dadosItemCardapio: DadosItemCardapioResolver } },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    entryComponents: [ConfirmationDialog]
})
export class AppModuleShared {


}