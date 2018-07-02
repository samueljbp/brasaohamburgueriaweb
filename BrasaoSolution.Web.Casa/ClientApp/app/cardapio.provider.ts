import { Injectable, Inject } from "@angular/core";
import { ClasseItemCardapioViewModel } from "./model/ClasseItemCardapioViewModel";
import { Http } from "@angular/http";
import * as globals from './GlobalVariables';
import { Comanda } from "./model/Comanda";
import { ItemComanda } from "./model/ItemComanda";
import { EmpresaViewModel } from "./model/EmpresaViewModel";
import { Observable } from "rxjs/Observable";
import 'rxjs/add/operator/map';

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

            globals.globalData.comanda = new Comanda();
            globals.globalData.comanda.numMesa = globals.globalData.numMesa;
            globals.globalData.comanda.valorTotal = 0;
            globals.globalData.comanda.itens = new Array<ItemComanda>();

            //globals.globalData.empresa = new EmpresaViewModel();
            //globals.globalData.empresa.razaoSocial = "Brasão Hamburgueria";
            //globals.globalData.empresa.urlSite = "www.brasaohamburgueria.com.br";
            //globals.globalData.empresa.logomarca = "Content/img/img_logo1.png";

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