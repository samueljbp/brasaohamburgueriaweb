import { Component } from "@angular/core";
import * as globals from '../../globals';
import { GlobalDataService } from "../../services/globalData.service";

@Component({
    selector: 'erroToken',
    templateUrl: './erroToken.component.html',
    styleUrls: ['./erroToken.component.css']
})
export class ErroTokenComponent {

    constructor(private data: GlobalDataService) {
        globals.globalData.botaoVoltarVisivel = false;
        this.data.changeMessage(globals.globalData);
    }

    reload() {
        window.location.reload(false);
    }
}