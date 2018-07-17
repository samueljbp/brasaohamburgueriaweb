import { DataSource } from "@angular/cdk/table";
import { BehaviorSubject } from "rxjs";
import { ItemComandaViewModel } from "../model/ItemComandaViewModel";
import { Observable } from 'rxjs/Observable';

export class ListaPedidoDataSource extends DataSource<any> {
    constructor(private ds: BehaviorSubject<ItemComandaViewModel[]>) {
        super();
    }
    connect(): Observable<ItemComandaViewModel[]> {
        return this.ds;
    }
    disconnect() { }
}