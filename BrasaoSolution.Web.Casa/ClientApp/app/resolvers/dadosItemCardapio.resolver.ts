import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { DadosItemCardapioViewModel } from "../model/DadosItemCardapioViewModel";
import { DadosItemCardapioService } from "../services/dadosItemCardapio.service";
import { Observable } from "rxjs/Observable";
import * as globals from '../globals';
import 'rxjs/add/operator/do';

@Injectable()
export class DadosItemCardapioResolver implements Resolve<DadosItemCardapioViewModel> {
    constructor(private service: DadosItemCardapioService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<DadosItemCardapioViewModel> {

        return this.service.getDadosItemCardapio(globals.globalData.itemSelecionado.codItemCardapio).do(dados => {
            if (dados) {
                globals.globalData.dadosItemSelecionado = dados;
                return dados;
            } else {
                return new DadosItemCardapioViewModel();
            }
        });



    }
} 