import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';

/** This class implements some features that should be tested. */
@Injectable()
export class AutenticacaoService {

    constructor(
        private http: HttpClient
    ) { }

    login(user: string, password: string): Observable<TokenData> {
        const body = new HttpParams()
            .set('username', user)
            .set('password', password)
            .set('grant_type', 'password');
        let headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

        return this.http.post('http://localhost:57919/token', body.toString(), { headers: headers, observe: 'response' })
            .map(res => <TokenData>res.body)
            .catch((err: any) => Observable.of(new TokenData()));
    }

}

export class TokenData {
    public access_token: string;
    token_type: string;
    expires_in: number;
    userName: string;
}