import { Injectable, Inject } from "@angular/core";
import { Http, RequestOptions } from "@angular/http";
import * as globals from '../globals';
import { Observable } from "rxjs/Observable";
import 'rxjs/add/operator/map';
import { EmpresaViewModel } from "../model/EmpresaViewModel";

@Injectable()
export class EmpresaProvider {

    private empresa: EmpresaViewModel = new EmpresaViewModel();
    private baseUrl: string;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    public getEmpresa(): EmpresaViewModel {
        return this.empresa;
    }

    load() {
        return new Promise((resolve, reject) => {

            this.http
                .get(this.baseUrl + 'api/Empresa/GetEmpresa?codEmpresa=' + globals.globalData.codEmpresa)
                .map(res => res.json())
                .subscribe(response => {
                    this.empresa = response;
                    globals.globalData.empresa = response;
                    resolve(true);
                })
        })
    }
}