import { Injectable, Inject } from "@angular/core";
import { Http, Headers } from "@angular/http";
import { ComandaViewModel } from "../model/ComandaViewModel";

@Injectable()
export class ComandaService {
    private baseUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    registraPedido(comanda: ComandaViewModel) {

        const headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('Accept', 'application/json');

        return this.http.post(this.baseUrl + 'api/Comanda/RegistraPedido', JSON.stringify(comanda), { headers: headers });
    }
}