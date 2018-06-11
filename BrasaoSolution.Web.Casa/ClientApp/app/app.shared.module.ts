import { NgModule, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Router } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { RodapeComponent } from './components/rodape/rodape.component';
import { Comanda } from './model/Comanda';
import { Empresa } from './model/Empresa';
import { GlobalData } from './GlobalData';
import { ItemComanda } from './model/ItemComanda';
import { GlobalDataService } from './GlobalData.service';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        RodapeComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [GlobalDataService, { provide: GlobalData, useValue: getGlobalDataInstance() }]
})
export class AppModuleShared {

    constructor(
        private globalDataService: GlobalDataService,
        private globalData: GlobalData) {

        globalDataService.getDados();

    }

}
function getGlobalDataInstance() {
    let globalData: GlobalData;

    globalData = new GlobalData();

    //verifica se já tem comanda aberta, se já tiver carrega para esta variável, se não inicia uma nova
    globalData.comanda = new Comanda();
    globalData.comanda.numMesa = 99;
    globalData.comanda.valorTotal = 0;
    globalData.comanda.itens = new Array<ItemComanda>();

    globalData.empresa = new Empresa();
    globalData.empresa.razaoSocial = "Brasão Hamburgueria";
    globalData.empresa.urlSite = "www.brasaohamburgueria.com.br";
    globalData.empresa.logomarca = "Content/img/img_logo1.png";

    return globalData;
}