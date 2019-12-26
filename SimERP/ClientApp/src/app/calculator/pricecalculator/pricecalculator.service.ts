import {Inject, Injectable} from '@angular/core';
import {AuthenParams} from 'src/app/common/commomodel/AuthenParams';
import {ReqListSearch} from 'src/app/common/commomodel/ReqListSearch';
import {ReqListDelete} from 'src/app/common/commomodel/ReqListDelete';
import {ReqListAdd} from 'src/app/common/commomodel/ReqListAdd';
import {ReqListUpdateSortOrder} from 'src/app/common/commomodel/ReqListUpdateSortOrder';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {ResponeResult} from 'src/app/common/commomodel/ResponeResult';
import {PriceSpreadsheets} from './model/pricespreadsheets';
import { ROOT_URL } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PricecalculatorService {

  AuthenParams = new AuthenParams();
  SearchParams = new ReqListSearch();
  DelParams = new ReqListDelete();
  InsertParams = new ReqListAdd();
  ReqListUpdateSortOrder = new ReqListUpdateSortOrder();

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private readonly baseURL: string) {
    this.baseURL = ROOT_URL;
  }

  getData() {

    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;

    const jsonString = JSON.stringify(this.SearchParams);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/cal/pricecalculator', jsonString, {headers});
  }

  Insert(obj: PriceSpreadsheets, isNewModel: boolean) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.InsertParams.AuthenParams = this.AuthenParams;
    this.InsertParams.RowData = obj;
    this.InsertParams.IsNew = isNewModel;

    const jsonString = JSON.stringify(this.InsertParams);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/cal/saveshippedprice', jsonString, {headers});
  }

  Delete(Id: any) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.DelParams.AuthenParams = this.AuthenParams;
    this.DelParams.ID = Id;
    const jsonString = JSON.stringify(this.DelParams);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/cal/deleteshippedprice', jsonString, {headers});
  }

  GetProductCache() {
    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;

    const jsonString = JSON.stringify(this.SearchParams);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/cal/productcache', jsonString, {headers});
  }

  GetCustomerCache() {
    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;

    const jsonString = JSON.stringify(this.SearchParams);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/cal/customercache', jsonString, {headers});
  }

  GetCustomerDeliveryAddress(CusId: number) {
    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;
    this.SearchParams.SearchString = CusId.toString();

    const jsonString = JSON.stringify(this.SearchParams);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/cal/arealistcache', jsonString, {headers});
  }

  GetAmountDefine() {
    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;

    const jsonString = JSON.stringify(this.SearchParams);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/cal/amountdefine', jsonString, {headers});
  }

  GetPriceNow(CusId: number, ProductId: number) {
    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;

    const jsonString = JSON.stringify(this.SearchParams);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/cal/amountdefine', jsonString, {headers});
  }
}
