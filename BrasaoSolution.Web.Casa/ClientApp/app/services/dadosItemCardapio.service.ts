import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { Observable } from 'rxjs/Observable';
import { DadosItemCardapioViewModel } from "../model/DadosItemCardapioViewModel";

@Injectable()
export class DadosItemCardapioService {
    private _baseUrl: string;

    constructor(private http: HttpClient, @Inject('WEBAPI_URL') baseUrl: string) {
        this._baseUrl = baseUrl;
    }

    getDadosItemCardapio(codItemCardapio: number): Observable<DadosItemCardapioViewModel> {
        return this.http.get(this._baseUrl + 'api/Cardapio/GetDadosItemCardapio?codItemCardapio=' + codItemCardapio)
            .do(x => console.log('lugar', x))
            .map(res => (<DadosItemCardapioViewModel>res));
    }

}