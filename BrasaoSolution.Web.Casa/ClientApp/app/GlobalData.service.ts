import { Injectable, Inject } from '@angular/core';
import { Http, Response } from '@angular/http';
import { ClasseItemCardapioViewModel } from './model/ClasseItemCardapioViewModel';
import * as globals from './GlobalVariables';
import { Comanda } from './model/Comanda';
import { ItemComanda } from './model/ItemComanda';
import { EmpresaViewModel } from './model/EmpresaViewModel';
import { GlobalData } from './GlobalData';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';

@Injectable()
export class GlobalDataService {
    private _http: Http;
    private _baseUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this._http = http;
        this._baseUrl = baseUrl;
    }

    getCardapio(): Observable<ClasseItemCardapioViewModel[]> {
        return this._http.get(this._baseUrl + 'api/SampleData/GetCardapio')
            .map((response: Response) => <ClasseItemCardapioViewModel[]>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);

        //globals.globalData.cardapio = new Array<ClasseItemCardapioViewModel>();
        //this._http.get(this._baseUrl + 'api/SampleData/GetCardapio').subscribe(result => {
        //    globals.globalData.cardapio = result.json() as ClasseItemCardapioViewModel[];
        //}

        //verifica se já tem comanda aberta, se já tiver carrega para esta variável, se não inicia uma nova
        //globals.globalData.comanda = new Comanda();
        //globals.globalData.comanda.numMesa = 99;
        //globals.globalData.comanda.valorTotal = 0;
        //globals.globalData.comanda.itens = new Array<ItemComanda>();

        //globals.globalData.empresa = new Empresa();
        //globals.globalData.empresa.razaoSocial = "Brasão Hamburgueria";
        //globals.globalData.empresa.urlSite = "www.brasaohamburgueria.com.br";
        //globals.globalData.empresa.logomarca = "Content/img/img_logo1.png";

        //globals.globalData.initialized = true;
    }

    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }

}