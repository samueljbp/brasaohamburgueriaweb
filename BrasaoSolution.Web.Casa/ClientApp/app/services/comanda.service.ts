import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ComandaViewModel } from "../model/ComandaViewModel";

@Injectable()
export class ComandaService {
    private baseUrl: string;

    constructor(private http: HttpClient, @Inject('WEBAPI_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    registraPedido(comanda: ComandaViewModel) {
        return this.http
            .post(this.baseUrl + 'api/Comanda/RegistraPedido', JSON.stringify(comanda));
    }
}