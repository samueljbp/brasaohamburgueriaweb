import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { Observable } from "rxjs/Observable";
import 'rxjs/add/observable/of';
import * as globals from '../globals';
import { GlobalData } from '../model/GlobalData';

@Injectable()
export class GlobalDataService {

    private messageSource = new BehaviorSubject(globals.globalData);
    currentMessage = this.messageSource.asObservable();

    constructor() { }

    changeMessage(message: GlobalData) {
        this.messageSource.next(message)
    }

}