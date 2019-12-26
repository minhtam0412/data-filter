import {Inject, Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {ResponeResult} from '../../common/commomodel/ResponeResult';
import {AuthenParams} from '../../common/commomodel/AuthenParams';
import {ReqListAdd} from '../../common/commomodel/ReqListAdd';
import { ROOT_URL } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ImportdataService {
  authenParams = new AuthenParams();
  reqListAdd = new ReqListAdd();

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.baseUrl = ROOT_URL;
  }

  importCIFPrice() {
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.reqListAdd.AuthenParams = this.authenParams;
    return this.httpClient.post<ResponeResult>(this.baseUrl + 'api/import/cifprice', '', {headers});
  }

}
