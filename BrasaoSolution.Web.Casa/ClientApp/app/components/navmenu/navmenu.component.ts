import { Component } from '@angular/core';
import { ComandaViewModel } from '../../model/ComandaViewModel';
import { EmpresaViewModel } from '../../model/EmpresaViewModel';
import * as globals from '../../globals';
import { ClasseItemCardapioViewModel } from '../../model/ClasseItemCardapioViewModel';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { GlobalDataService } from '../../services/globalData.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    public comanda: ComandaViewModel;
    public empresa: EmpresaViewModel;
    public cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    public botaoVoltarVisivel: boolean;

    constructor(private location: Location, private router: Router, private data: GlobalDataService) {

        this.cardapio = globals.globalData.cardapio;
        this.comanda = globals.globalData.comanda;
        this.empresa = globals.globalData.empresa;

        this.data.currentMessage.subscribe(message =>
            this.botaoVoltarVisivel = message.botaoVoltarVisivel
        );
    }

    voltar() {
        this.location.back();
    }

}