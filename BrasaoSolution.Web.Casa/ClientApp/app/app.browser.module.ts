import { NgModule, APP_INITIALIZER, LOCALE_ID } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppModuleShared } from './app.shared.module';
import { AppComponent } from './components/app/app.component';
import { ActivatedRoute } from '@angular/router';
import * as globals from './globals';
import { CardapioProvider } from './services/cardapio.provider';
import { URLSearchParams } from "@angular/http";
import { EmpresaProvider } from './services/empresa.provider';
import { registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { DadosItemCardapioService } from './services/dadosItemCardapio.service';
import { DadosItemCardapioResolver } from './resolvers/dadosItemCardapio.resolver';
import { GlobalDataService } from './services/globalData.service';
import { ComandaService } from './services/comanda.service';
import { HTTP_INTERCEPTORS, HttpClient, HttpHandler, HttpClientModule } from '@angular/common/http';
import { CustomHttpInterceptor } from './interceptors/http.interceptor';
import { AutenticacaoService } from './services/autenticacao.service';

registerLocaleData(localePt);

@NgModule({
    bootstrap: [ AppComponent ],
    imports: [
        BrowserModule,
        HttpClientModule,
        AppModuleShared
    ],
    providers: [
        { provide: 'BASE_URL', useFactory: getBaseUrl },
        { provide: 'WEBAPI_URL', useFactory: getWebAPIUrl },
        CardapioProvider, EmpresaProvider, DadosItemCardapioService, DadosItemCardapioResolver, GlobalDataService, ComandaService, AutenticacaoService, HttpClient,
        { provide: APP_INITIALIZER, useFactory: cardapioProviderFactory, deps: [CardapioProvider], multi: true },
        { provide: APP_INITIALIZER, useFactory: empresaProviderFactory, deps: [EmpresaProvider], multi: true },
        { provide: LOCALE_ID, useValue: 'pt-BR' },
        { provide: HTTP_INTERCEPTORS, useClass: CustomHttpInterceptor, multi: true }
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

export function getWebAPIUrl() {
    setGlobalData();

    return "http://localhost:57919/";
}

export function getBaseUrl() {
    setGlobalData();

    return document.getElementsByTagName('base')[0].href;
}

function setGlobalData() {
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
