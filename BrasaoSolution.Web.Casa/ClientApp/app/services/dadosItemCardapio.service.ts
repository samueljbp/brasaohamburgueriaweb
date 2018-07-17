import { Injectable, Inject } from "@angular/core";
import { Http } from "@angular/http";
import { Observable } from 'rxjs/Observable';
import { DadosItemCardapioViewModel } from "../model/DadosItemCardapioViewModel";
import 'rxjs/add/operator/map';

@Injectable()
export class DadosItemCardapioService {
    private _http: Http;
    private _baseUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this._http = http;
        this._baseUrl = baseUrl;
    }

    getDadosItemCardapio(codItemCardapio: number): Observable<DadosItemCardapioViewModel> {
        return this._http.get(this._baseUrl + 'api/Cardapio/GetDadosItemCardapio?codItemCardapio=' + codItemCardapio)
            .map(res => res.json());
    }

}