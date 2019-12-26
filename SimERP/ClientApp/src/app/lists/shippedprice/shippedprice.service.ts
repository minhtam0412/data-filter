import {Inject, Injectable} from '@angular/core';
import {AuthenParams} from 'src/app/common/commomodel/AuthenParams';
import {ReqListSearch} from 'src/app/common/commomodel/ReqListSearch';
import {ReqListDelete} from 'src/app/common/commomodel/ReqListDelete';
import {ReqListAdd} from 'src/app/common/commomodel/ReqListAdd';
import {ReqListUpdateSortOrder} from 'src/app/common/commomodel/ReqListUpdateSortOrder';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {ResponeResult} from 'src/app/common/commomodel/ResponeResult';
import {ShippedPrice} from './model/shippedprice';
import { ROOT_URL } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ShippedpriceService {

  AuthenParams = new AuthenParams();
  SearchParams = new ReqListSearch();
  DelParams = new ReqListDelete();
  InsertParams = new ReqListAdd();
  ReqListUpdateSortOrder = new ReqListUpdateSortOrder();

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private readonly baseURL: string,) {
    this.baseURL = ROOT_URL;
   }

  getData(searchString?: string, startRow?: number, maxRow?: number) {

    this.AuthenParams.Sign = 'tai.ngo';
    this.SearchParams.AuthenParams = this.AuthenParams;
    this.SearchParams.MaxRow = maxRow;
    this.SearchParams.StartRow = startRow;
    this.SearchParams.SearchString = searchString;

    const jsonString = JSON.stringify(this.SearchParams);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>( this.baseURL +  'api/list/shippedprice', jsonString, { headers });
  }

  Insert(obj: ShippedPrice, isNew: boolean) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.InsertParams.AuthenParams = this.AuthenParams;
    this.InsertParams.RowData = obj;
    this.InsertParams.IsNew = isNew;

    const jsonString = JSON.stringify(this.InsertParams);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/list/saveshippedprice', jsonString, { headers });
  }

  Delete(Id: any) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.DelParams.AuthenParams = this.AuthenParams;
    this.DelParams.ID = Id;
    const jsonString = JSON.stringify(this.DelParams);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/list/deleteshippedprice', jsonString, { headers });
  }

  Sort(UpID: number, DowID: number) {

    this.AuthenParams.Sign = 'tai.ngo';
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.ReqListUpdateSortOrder.AuthenParams = this.AuthenParams;
    this.ReqListUpdateSortOrder.UpID = UpID;
    this.ReqListUpdateSortOrder.DownID = DowID;

    const jsonString = JSON.stringify(this.ReqListUpdateSortOrder);
    return this.httpClient.post<ResponeResult>(this.baseURL + 'api/list/updateSortOrderShippedPrice', jsonString, { headers });
  }
}
