import { Observable } from 'rxjs/Observable';
import { Location } from '@angular/common';
import { Injectable } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http';
import { HttpRequest } from '@angular/common/http';
import { HttpHandler } from '@angular/common/http';
import { HttpEvent } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import 'rxjs/add/observable/fromPromise';
import { AutenticacaoService } from '../services/autenticacao.service';
import { Router } from '@angular/router';

@Injectable()
export class CustomHttpInterceptor implements HttpInterceptor {
    constructor(private autService: AutenticacaoService, private router: Router) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return Observable.fromPromise(this.handleAccess(request, next));
    }

    private async handleAccess(request: HttpRequest<any>, next: HttpHandler): Promise<HttpEvent<any>> {
        if (request.url.toUpperCase().indexOf("/TOKEN") >= 0) {
            return next.handle(request).toPromise();
        }
       
        const tokenData = await this.autService.login('samuel_jbp@yahoo.com.br', 'r852456t').toPromise();
        const token = tokenData.access_token;
        let changedRequest = request;
        // HttpHeader object immutable - copy values
        let headerSettings: { [name: string]: string; } = {};

        for (const key of request.headers.keys()) {
            headerSettings[key] = <string>request.headers.get(key);
        }
        if (token) {
            headerSettings['Authorization'] = 'Bearer ' + token;
        } else {
            this.router.navigate(['/erroToken']);
        }
        headerSettings['Content-Type'] = 'application/json';
        
        let newHeader = new HttpHeaders(headerSettings);

        changedRequest = request.clone({
            headers: newHeader
        });
        return next.handle(changedRequest).toPromise();
    }

}