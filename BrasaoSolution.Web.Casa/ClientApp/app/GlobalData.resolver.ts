import { GlobalData } from "./GlobalData";
import { Injectable } from "@angular/core";
import { GlobalDataService } from "./GlobalData.service";
import { ActivatedRouteSnapshot, RouterStateSnapshot, Resolve } from "@angular/router";
import { ClasseItemCardapioViewModel } from "./model/ClasseItemCardapioViewModel";
import { Observable } from "rxjs/Observable";
import 'rxjs/add/observable/of';
import * as globals from './GlobalVariables';

@Injectable()
export class CardapioResolver implements Resolve<ClasseItemCardapioViewModel[]> {
    constructor(private globalDataService: GlobalDataService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<ClasseItemCardapioViewModel[]> {

        if (!globals.globalData.initialized) {
            return this.globalDataService.getCardapio().do(cardapio => {
                if (cardapio) {
                    globals.globalData.cardapio = cardapio;
                    globals.globalData.initialized = true;
                    return cardapio;
                } else {
                    return new Array<ClasseItemCardapioViewModel>();
                }
            });
        } else {

            return Observable.of(globals.globalData.cardapio);

        }

        
    }
} 