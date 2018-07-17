import { Injectable, Inject } from "@angular/core";
import { Http } from "@angular/http";
import * as globals from '../globals';
import { ComandaViewModel } from "../model/ComandaViewModel";
import { ItemComandaViewModel } from "../model/ItemComandaViewModel";
import { Observable } from "rxjs/Observable";
import 'rxjs/add/operator/map';
import { ClasseItemCardapioViewModel } from "../model/ClasseItemCardapioViewModel";

@Injectable()
export class CardapioProvider {

    private cardapio: ClasseItemCardapioViewModel[] = new Array<ClasseItemCardapioViewModel>();
    private baseUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    public getCardapio(): ClasseItemCardapioViewModel[] {
        return this.cardapio;
    }

    load() {
        return new Promise((resolve, reject) => {

            globals.globalData.comanda = new ComandaViewModel();
            globals.globalData.comanda.numMesa = globals.globalData.numMesa;
            globals.globalData.comanda.valorTotal = 0;
            globals.globalData.comanda.itens = new Array<ItemComandaViewModel>();

            this.http
                .get(this.baseUrl + 'api/Cardapio/GetCardapio?codEmpresa=' + globals.globalData.codEmpresa)
                .map(res => res.json())
                .subscribe(response => {
                    this.cardapio = response;
                    globals.globalData.initialized = true;
                    globals.globalData.cardapio = response;
                    resolve(true);
                })
        })
    }
}