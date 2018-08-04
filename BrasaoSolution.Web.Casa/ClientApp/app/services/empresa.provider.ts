import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import * as globals from '../globals';
import { Observable } from "rxjs/Observable";
import 'rxjs/add/operator/map';
import { EmpresaViewModel } from "../model/EmpresaViewModel";

@Injectable()
export class EmpresaProvider {

    private empresa: EmpresaViewModel = new EmpresaViewModel();
    private baseUrl: string;

    constructor(private http: HttpClient, @Inject('WEBAPI_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    public getEmpresa(): EmpresaViewModel {
        return this.empresa;
    }

    load() {
        return new Promise((resolve, reject) => {

            this.http
                .get(this.baseUrl + 'api/Empresa/GetEmpresa?codEmpresa=' + globals.globalData.codEmpresa)
                .map(res => <EmpresaViewModel>res)
                .subscribe(response => {
                    this.empresa = response;
                    globals.globalData.empresa = response;
                    resolve(true);
                })
        })
    }
}