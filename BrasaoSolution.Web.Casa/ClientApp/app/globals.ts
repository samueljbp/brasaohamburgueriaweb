import { GlobalData } from "./model/GlobalData";
import * as jquery from 'jquery';
import * as Models from './model/lib';

export const globalData: GlobalData = new GlobalData();

export function ajustaPrototipoDeep(objeto: any) {
    if (Array.isArray(objeto)) {
        for (const element of objeto) {
            if (element && element.className) {
                let className: string = element.className;
                jquery.extend(true, element, Object.create((<any>Models)[className].prototype));
                ajustaPrototipoDeep(element);
            }
        }
    } else {
        for (let key in objeto) {
            if (Array.isArray(objeto[key])) {
                ajustaPrototipoDeep(objeto[key]);
            } else if (typeof objeto[key] === 'object') {
                if (objeto[key] && objeto[key].className) {
                    let className: string = objeto[key].className;
                    jquery.extend(true, objeto[key], Object.create((<any>Models)[className].prototype));
                }
            }
        }
    }
}