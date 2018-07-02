import { NgModule, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppModuleShared } from './app.shared.module';
import { AppComponent } from './components/app/app.component';
import { ActivatedRoute } from '@angular/router';
import * as globals from './GlobalVariables';
import { CardapioProvider } from './cardapio.provider';
import { URLSearchParams } from "@angular/http";
import { EmpresaProvider } from './empresa.provider';

@NgModule({
    bootstrap: [ AppComponent ],
    imports: [
        BrowserModule,
        AppModuleShared
    ],
    providers: [
        { provide: 'BASE_URL', useFactory: getBaseUrl },
        CardapioProvider, EmpresaProvider,
        { provide: APP_INITIALIZER, useFactory: cardapioProviderFactory, deps: [CardapioProvider], multi: true },
        { provide: APP_INITIALIZER, useFactory: empresaProviderFactory, deps: [EmpresaProvider], multi: true },
        { provide: LOCALE_ID, useValue: 'pt-BR' }
    ]
})
export class AppModule {

}

export function cardapioProviderFactory(provider: CardapioProvider) {
    return () => provider.load();
}

export function empresaProviderFactory(provider: EmpresaProvider) {
    return () => provider.load();
}

export function getBaseUrl() {
    let codEmpresa = getParameterByName("codEmpresa");
    if (codEmpresa != null) {
        globals.globalData.codEmpresa = Number(codEmpresa);
    } else {
        globals.globalData.codEmpresa = 1;
    }

    let numMesa = getParameterByName("numMesa");
    if (numMesa != null) {
        globals.globalData.numMesa = Number(numMesa);
    } else {
        globals.globalData.numMesa = 2;
    }

    return document.getElementsByTagName('base')[0].href;
}

export function getParameterByName(name: string) {
    const url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    const regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}
