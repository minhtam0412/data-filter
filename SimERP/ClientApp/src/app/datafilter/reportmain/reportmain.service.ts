import {Inject, Injectable} from '@angular/core';
import {AuthenParams} from '../../common/commomodel/AuthenParams';
import {ReqListSearch} from '../../common/commomodel/ReqListSearch';
import {ReqListAdd} from '../../common/commomodel/ReqListAdd';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {ROOT_URL} from '../../../environments/environment';
import {ResponeResult} from '../../common/commomodel/ResponeResult';

@Injectable({
  providedIn: 'root'
})
export class ReportmainService {
  authenParams = new AuthenParams();
  reqListSearch = new ReqListSearch();
  reqListAdd = new ReqListAdd();

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.baseUrl = ROOT_URL;
  }

  getData(searchString: string) {
    this.reqListSearch.AuthenParams = this.authenParams;
    this.reqListSearch.MaxRow = 10;
    this.reqListSearch.IsActive = true;
    this.reqListSearch.StartRow = 0;
    this.reqListSearch.SearchString = searchString;

    const jsonString = JSON.stringify(this.reqListSearch);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseUrl + 'api/list/reporttotal', jsonString, {headers});
  }


}
